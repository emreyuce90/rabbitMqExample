using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace rabbitMqExample.Consumer {
    internal class Program {
        static void Main(string[] args) {
            //Connection oluşturulur
            var factory = new ConnectionFactory();
            //Connection url i verilir
            factory.Uri = new Uri("amqps://epytrzjx:V1fSdtcKv36kwPqdaa1dgMw9MkPFlsPz@fish.rmq.cloudamqp.com/epytrzjx");
            using var connection = factory.CreateConnection();
            //Dinlenilecek kanal oluşturulur
            var channel = connection.CreateModel();

            //bir consumer eventi oluşturulur
            var consumer = new EventingBasicConsumer(channel);
            //Tüketilecek kuyruk adı ve consumer verilir
            channel.BasicConsume("hello-world-queue", true, consumer);
            consumer.Received += (object sender, BasicDeliverEventArgs e) => {
                //mesaj kuyruktan alınıp stringe çevrilir
                var message = Encoding.UTF8.GetString(e.Body.ToArray());
                Console.WriteLine("Gelen mesaj:" + message);
            };

            Console.ReadLine();
        }
    }
}