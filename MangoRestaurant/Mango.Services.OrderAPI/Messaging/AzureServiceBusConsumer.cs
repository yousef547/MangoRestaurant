using Azure.Messaging.ServiceBus;
using Mango.MessageBus;
using Mango.Services.OrderAPI.Messages;
using Mango.Services.OrderAPI.Model;
using Mango.Services.OrderAPI.Respository;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.OrderAPI.Messaging
{
    public class AzureServiceBusConsumer: IAzureServiceBusConsumer
    {
        private readonly string ServiceBusConectionString;
        private readonly string SubscriptionCheckout;
        private readonly string CheckoutMessageTopic;
        private readonly string OrderPaymentProcessorTopic;
        private readonly string OrderUpdatePaymentResultTpoic;

        private readonly OrderRepository _orderRepository;
        private readonly IConfiguration _configuration;
        private readonly IMessageBus _messageBus;

        private ServiceBusProcessor checkoutProcessor;
        private ServiceBusProcessor orderUpdatePaymentResultTpoic;


        public AzureServiceBusConsumer(OrderRepository orderRepository, IConfiguration configuration, IMessageBus messageBus)
        {
            _orderRepository = orderRepository;
            _configuration = configuration;
            _messageBus = messageBus;
            ServiceBusConectionString = _configuration.GetValue<string>("ServiceBusConectionString");
            CheckoutMessageTopic = _configuration.GetValue<string>("CheckoutMessageTopic");
            SubscriptionCheckout = _configuration.GetValue<string>("SubscriptionCheckout");
            OrderPaymentProcessorTopic = _configuration.GetValue<string>("OrderPaymentProcessorTopic");
            OrderUpdatePaymentResultTpoic = _configuration.GetValue<string>("OrderUpdatePaymentResultTpoic");


            var client = new ServiceBusClient(ServiceBusConectionString);
            checkoutProcessor = client.CreateProcessor(CheckoutMessageTopic);
            orderUpdatePaymentResultTpoic= client.CreateProcessor(OrderUpdatePaymentResultTpoic, SubscriptionCheckout);

        }
        public async Task Start()
        {
            checkoutProcessor.ProcessMessageAsync += OnCheckoutMessageReceived;
            checkoutProcessor.ProcessErrorAsync += ErrorHandler;
            await checkoutProcessor.StartProcessingAsync();

            orderUpdatePaymentResultTpoic.ProcessMessageAsync += OnOrderUpdateReceived;
            orderUpdatePaymentResultTpoic.ProcessErrorAsync += ErrorHandler;
            await orderUpdatePaymentResultTpoic.StartProcessingAsync();
        }
        public async Task Stop()
        {
            await checkoutProcessor.StopProcessingAsync();
            await checkoutProcessor.DisposeAsync();

            await orderUpdatePaymentResultTpoic.StopProcessingAsync();
            await orderUpdatePaymentResultTpoic.DisposeAsync();
        }
        Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
        private async Task OnCheckoutMessageReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            CheckoutHeaderDto checkoutHeaderDto = JsonConvert.DeserializeObject<CheckoutHeaderDto>(body);
            OrderHeader orderHeader = new()
            {
                UserId = checkoutHeaderDto.UserId,
                FirstName = checkoutHeaderDto.FirstName,
                LastName = checkoutHeaderDto.LastName,
                OrderDetails = new List<OrderDetails>(),
                CardNumber = checkoutHeaderDto.CardNumber,
                CouponCode = checkoutHeaderDto.CouponCode,
                CVV = checkoutHeaderDto.CVV,
                DiscountTotal = checkoutHeaderDto.DiscountTotal,
                Email = checkoutHeaderDto.Email,
                ExpiryMonthYear = checkoutHeaderDto.ExpiryMonthYear,
                OrderTime = DateTime.Now,
                OrderTotle = checkoutHeaderDto.OrderTotal,
                PaymentStatus = false,
                Phone = checkoutHeaderDto.Phone,
                PickupDateTime = checkoutHeaderDto.PickupDateTime
            };
            foreach (var detailList in checkoutHeaderDto.CartDetails)
            {
                OrderDetails orderDetails = new()
                {
                    ProductId = detailList.ProductId,
                    ProductName = detailList.Product.Name,
                    Price = detailList.Product.Price,
                    Count = detailList.Count
                };
                orderHeader.CartTotleItem += detailList.Count;
                orderHeader.OrderDetails.Add(orderDetails);
            }
            await _orderRepository.AddOrder(orderHeader);

            PaymentRequestMessage paymentRequestMessage = new()
            {
                Name = orderHeader.FirstName + " " + orderHeader.LastName,
                CardNumber = orderHeader.CardNumber,
                CVV = orderHeader.CVV,
                ExpiryMothYear = orderHeader.ExpiryMonthYear,
                OrderId = orderHeader.OrderHeaderId,
                OrderTotle = orderHeader.OrderTotle,
                Email = orderHeader.Email,
            };

            try
            {
                await _messageBus.PublishMessage(paymentRequestMessage, OrderPaymentProcessorTopic);
                await args.CompleteMessageAsync(args.Message);
            }catch (Exception ex)
            {
                throw;
            }
        }
        private async Task OnOrderUpdateReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            UpdatePaymentResultMessage paymentResultMessage = JsonConvert.DeserializeObject<UpdatePaymentResultMessage>(body);
            await _orderRepository.updateOrderPaymentStatus(paymentResultMessage.OrderId, paymentResultMessage.Status);
            await args.CompleteMessageAsync(args.Message);
        }
    }
}
