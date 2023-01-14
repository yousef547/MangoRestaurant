using Azure.Messaging.ServiceBus;
using Mango.Services.Email.Messages;
using Mango.Services.Email.Respository;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.Email.Messaging
{
    public class AzureServiceBusConsumer: IAzureServiceBusConsumer
    {
        private readonly string ServiceBusConectionString;
        private readonly string SubscriptionEmail;

        private readonly string OrderUpdatePaymentResultTpoic;

        private readonly EmailRepository _emailRepo;
        private readonly IConfiguration _configuration;

        private ServiceBusProcessor orderUpdatePaymentResultTpoic;


        public AzureServiceBusConsumer(EmailRepository emailRepo, IConfiguration configuration)
        {
            _emailRepo = emailRepo;
            _configuration = configuration;
            ServiceBusConectionString = _configuration.GetValue<string>("ServiceBusConectionString");
            SubscriptionEmail = _configuration.GetValue<string>("SubscriptionName");
            OrderUpdatePaymentResultTpoic = _configuration.GetValue<string>("OrderUpdatePaymentResultTpoic");


            var client = new ServiceBusClient(ServiceBusConectionString);
            orderUpdatePaymentResultTpoic= client.CreateProcessor(OrderUpdatePaymentResultTpoic, SubscriptionEmail);

        }
        public async Task Start()
        {
        

            orderUpdatePaymentResultTpoic.ProcessMessageAsync += OnOrderUpdateReceived;
            orderUpdatePaymentResultTpoic.ProcessErrorAsync += ErrorHandler;
            await orderUpdatePaymentResultTpoic.StartProcessingAsync();
        }
        public async Task Stop()
        {
  
            await orderUpdatePaymentResultTpoic.StopProcessingAsync();
            await orderUpdatePaymentResultTpoic.DisposeAsync();
        }
        Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
      
        private async Task OnOrderUpdateReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            UpdatePaymentResultMessage updatePaymentResultMessage = JsonConvert.DeserializeObject<UpdatePaymentResultMessage>(body);

            try
            {
                await _emailRepo.SendAndLogEmail(updatePaymentResultMessage);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
