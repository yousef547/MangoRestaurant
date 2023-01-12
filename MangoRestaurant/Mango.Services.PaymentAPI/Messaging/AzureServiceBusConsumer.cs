using Azure.Messaging.ServiceBus;
using Mango.MessageBus;

using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PaymentProcessor;
using System.Text;

namespace Mango.Services.PaymentAPI.Messaging
{
    public class AzureServiceBusConsumer: IAzureServiceBusConsumer
    {
        private readonly string ServiceBusConectionString;
        private readonly string OrderPaymentProcessSubscription;
        private readonly string OrderPaymentProcessorTopic;
        private readonly string OrderUpdatePaymentResultTpoic;
        private readonly IConfiguration _configuration; 
        private readonly IMessageBus _messageBus;
        private readonly IProcessPayment _processPayment;

        private ServiceBusProcessor orderPaymentProcessor;

        public AzureServiceBusConsumer(IProcessPayment processPayment, IConfiguration configuration, IMessageBus messageBus)
        {
            _configuration = configuration;
            _messageBus = messageBus;
            _processPayment = processPayment;

            ServiceBusConectionString = _configuration.GetValue<string>("ServiceBusConectionString");
            OrderPaymentProcessSubscription = _configuration.GetValue<string>("OrderPaymentProcessSubscription");
            OrderUpdatePaymentResultTpoic = _configuration.GetValue<string>("OrderUpdatePaymentResultTpoic");

            OrderPaymentProcessorTopic = _configuration.GetValue<string>("OrderPaymentProcessorTopic");


            var client = new ServiceBusClient(ServiceBusConectionString);
            orderPaymentProcessor = client.CreateProcessor(OrderPaymentProcessorTopic, OrderPaymentProcessSubscription);

        }
        public async Task Start()
        {
            orderPaymentProcessor.ProcessMessageAsync += ProcessPayments;
            orderPaymentProcessor.ProcessErrorAsync += ErrorHandler;
            await orderPaymentProcessor.StartProcessingAsync();
        }
        public async Task Stop()
        {
            await orderPaymentProcessor.StopProcessingAsync();
            await orderPaymentProcessor.DisposeAsync();
        }
        Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
        private async Task ProcessPayments(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            PaymentRequestMessage paymentRequestMessage = JsonConvert.DeserializeObject<PaymentRequestMessage>(body);
            var result = _processPayment.PaymentProcessor();
            UpdatePaymentResultMessage updatePaymentResultMessage = new()
            {
                Status = result,
                OrderId = paymentRequestMessage.OrderId,
            };

          
            try
            {
                await _messageBus.PublishMessage(updatePaymentResultMessage, OrderUpdatePaymentResultTpoic);
                await args.CompleteMessageAsync(args.Message);
            }catch (Exception ex)
            {
                throw;
            }
        }
    }
}
