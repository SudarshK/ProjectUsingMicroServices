
using Microsoft.EntityFrameworkCore.Metadata;
using Newtonsoft.Json;
using RabbitMQ.Client;
using ServiceMesh.Services.RewardAPI.Dto;
using ServiceMesh.Services.RewardAPI.Services;
using System.Threading.Channels;

namespace ServiceMesh.Services.RewardAPI.Messaging
{
    public class RabbitMQOrderConsumer : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly RewardService _rewardService;
        private IConnection _connection;
        private RabbitMQ.Client.IModel _channel;
        private const string OrderCreated_RewardsUpdateQueue = "RewardsUpdateQueue";
        private string ExchangeName = "";

        public RabbitMQOrderConsumer(IConfiguration configuration, RewardService rewardService)
        {
            _configuration = configuration;
            _rewardService = rewardService;
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
            _channel.QueueDeclare(OrderCreated_RewardsUpdateQueue,false,false,false,null);
            _channel.QueueBind(OrderCreated_RewardsUpdateQueue, ExchangeName, "RewardsUpdate");

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
            _channel.BasicConsume(OrderCreated_RewardsUpdateQueue, false, consumer);
            return Task.CompletedTask;
        }
        private async Task HandleMessage(RewardMessage rewardsMessage)
        {
            _rewardService.UpdateRewards(rewardsMessage).GetAwaiter().GetResult();
        }
    }
}
