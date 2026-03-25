
using Microsoft.EntityFrameworkCore.Metadata;
using Newtonsoft.Json;
using RabbitMQ.Client;
using ServiceMesh.Services.EmailAPI.Model.DTO;
using ServiceMesh.Services.EmailAPI.Services;
using System.Threading.Channels;

namespace ServiceMesh.Services.EmailAPI.Messaging
{
    public class RabbitMQOrderConsumer : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;
        private IConnection _connection;
        private RabbitMQ.Client.IModel _channel;
        string queueName="";
        private const string OrderCreated_EmailUpdateQueue = "EmailUpdateQueue";
        private string ExchangeName = "";

        public RabbitMQOrderConsumer(IConfiguration configuration, EmailService emailService)
        {
            _configuration = configuration;
            _emailService = emailService;
            ExchangeName = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct);
            _channel.QueueDeclare(OrderCreated_EmailUpdateQueue,false,false,false,null);
            _channel.QueueBind(OrderCreated_EmailUpdateQueue, ExchangeName, "EmailUpdate");
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new RabbitMQ.Client.Events.EventingBasicConsumer(_channel);
            consumer.Received += (ch, ea) =>
            {
                var content = System.Text.Encoding.UTF8.GetString(ea.Body.ToArray());
                RewardMessage rewardsMessage = JsonConvert.DeserializeObject<RewardMessage>(content);
                HandleMessage(rewardsMessage).GetAwaiter().GetResult();
                _channel.BasicAck(ea.DeliveryTag, false);
            };
            _channel.BasicConsume(OrderCreated_EmailUpdateQueue, false, consumer);
            return Task.CompletedTask;
        }
        private async Task HandleMessage(RewardMessage rewardsMessage)
        {
            _emailService.LogOrderPlaced(rewardsMessage).GetAwaiter().GetResult();
        }
    }
}
