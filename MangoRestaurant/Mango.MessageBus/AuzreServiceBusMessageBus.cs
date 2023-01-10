using Azure.Messaging.ServiceBus;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mango.MessageBus
{
    public class AuzreServiceBusMessageBus : IMessageBus
    {
        private string connectionString = "Endpoint=sb://mangorestaurantwe.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=CelrixoRPLsTlYXz7x+g/Y3iFdkzwD1acOkwBPU3oyI=";
        public async Task PublishMessage(BaseMessage message, string topicName)
        {
            await using var Client = new ServiceBusClient(connectionString);
            ServiceBusSender sender = Client.CreateSender(topicName);
            var jsonMessage= JsonConvert.SerializeObject(message);

            ServiceBusMessage finalMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonMessage))
            {
                CorrelationId = Guid.NewGuid().ToString()
            };
            await sender.SendMessageAsync(finalMessage);
            await Client.DisposeAsync();

        }
    }
}
