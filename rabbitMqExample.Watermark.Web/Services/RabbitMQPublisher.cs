using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace rabbitMqExample.Watermark.Web.Services {
    public class RabbitMQPublisher {
        private readonly RabbitMqService _rabbitMqService;

        public RabbitMQPublisher(RabbitMqService rabbitMqService) {
            _rabbitMqService = rabbitMqService;
        }

        public void Publish(productImageCreatedEvent productImageCreatedEvent) {
            
            var channel = _rabbitMqService.Connect();
            //class'ı string a çevir
            var body = JsonSerializer.Serialize(productImageCreatedEvent);
            //string'i byte array e çevir
            byte[] bodyByte = Encoding.UTF8.GetBytes(body);
            //properties ile veri kalıcılığını sağla
            var properties = channel.CreateBasicProperties();
            properties.Persistent=true;

            channel.BasicPublish(exchange:RabbitMqService.ExchangeName, routingKey:RabbitMqService.RouteName, basicProperties: properties, body: bodyByte);
            
        }
    }
}
