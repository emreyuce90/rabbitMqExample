using RabbitMQ.Client;

namespace rabbitMqExample.WebExcelCreate.Services {
    public class RabbitMqClientServices : IDisposable {
       
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
