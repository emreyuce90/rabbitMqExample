using RabbitMQ.Client;

namespace rabbitMqExample.Watermark.Web.Services {
    public class RabbitMqService : IDisposable {
        private readonly ConnectionFactory _connectionFactory;
        private readonly ILogger<RabbitMqService> _logger;
        private IConnection _connection;
        private IModel _channel;
        public static string ExchangeName = "WatermarkDirectExchange";
        public static string QueueName = "queue-watermark-image";
        public static string RouteName = "watermark-route-image";
        public RabbitMqService(ConnectionFactory connectionFactory, ILogger<RabbitMqService> logger) {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        //geriye channel döner
        public IModel Connect() {
            //connection un elde edilmesi
            _connection = _connectionFactory.CreateConnection();
            
            if(_channel is { IsOpen: true }) {
                return _channel;
            }
            //kanalın elde edilmesi
            _channel = _connection.CreateModel();
            //declaring exchange
            _channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct, true, false);
            _channel.QueueDeclare(QueueName,true,false,false);
            _channel.QueueBind(QueueName, ExchangeName, RouteName, null);
            _logger.LogInformation("RabbitMQ ile bağlantı kuruldu");
            return _channel;
        }

        public void Dispose() {
            _channel?.Close();
            _channel?.Dispose();

            _connection?.Close();
            _connection?.Dispose();
            _logger.LogInformation("Rabbit MQ ile bağlantı koptu");
        }

    }
}
