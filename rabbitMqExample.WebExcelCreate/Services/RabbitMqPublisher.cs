using RabbitMQ.Client;
using rabbitMqExample.Shared;
using System.Text;
using System.Text.Json;

namespace rabbitMqExample.WebExcelCreate.Services {
    public class RabbitMqPublisher {
        private readonly RabbitMqClientServices _rabbitMqClientServices;

        public RabbitMqPublisher(RabbitMqClientServices rabbitMqClientServices) {
            _rabbitMqClientServices = rabbitMqClientServices;
        }

        public void Publish(CreateExcelMessage createExcelMessage) {
            var channel = _rabbitMqClientServices.Connect();
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            var data = JsonSerializer.Serialize(createExcelMessage);
            var byteBody = Encoding.UTF8.GetBytes(data);

            channel.BasicPublish(RabbitMqClientServices.ExchangeName, RabbitMqClientServices.RouteName, false,properties,byteBody);
        }
 
    }
}
