using RabbitMQ.Client;

namespace rabbitMqExample.WebExcelCreate.Services {
    public class RabbitMqClientServices : IDisposable {
        public static string QueueName = "direct-queue-excel";
        public static string RouteName = "direct-route-excel";
        public static string ExchangeName = "DirectExcelExchange";
        private IConnection _connection;
        private IModel _channel;
        private readonly ConnectionFactory _connectionFactory;
        public RabbitMqClientServices(ConnectionFactory connectionFactory) {
            _connectionFactory = connectionFactory;
        }


        public IModel Connect() {
            //1-Connection
            _connection = _connectionFactory.CreateConnection();
            if (_channel is { IsOpen: true }) return _channel;
            //2-Channel
            _channel = _connection.CreateModel();
            //3-Exchange
            _channel.ExchangeDeclare(RabbitMqClientServices.ExchangeName, ExchangeType.Direct, true, false, null);
            //4-Queue
            _channel.QueueDeclare(RabbitMqClientServices.QueueName,true,false,false,null);
            //5-QueueBind
            _channel.QueueBind(RabbitMqClientServices.QueueName, RabbitMqClientServices.ExchangeName, RabbitMqClientServices.RouteName, null);
            return _channel;
        }

        public void Dispose() {
            _channel?.Dispose();
            _channel?.Close();
            _connection?.Dispose();
            _connection?.Close();
        }
    }
}
