using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceMesh.MessageBus
{
    public class MessageBus : IMessageBus
    {
        private readonly string connectionString =
            Environment.GetEnvironmentVariable("SERVICEMESH_SERVICEBUS__ConnectionString")
            ?? throw new InvalidOperationException("Missing environment variable: SERVICEMESH_SERVICEBUS__ConnectionString");

        public async Task PublishMessage(object message, string topic_queue_Name)
        {

            await using var client = new ServiceBusClient(connectionString);
            ServiceBusSender sender = client.CreateSender(topic_queue_Name);
            var jsonMessage = JsonConvert.SerializeObject(message);
            ServiceBusMessage finalMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonMessage))
            {
                CorrelationId = Guid.NewGuid().ToString(),
            };

            await sender.SendMessageAsync(finalMessage);
            await client.DisposeAsync();
        }
    }
}
