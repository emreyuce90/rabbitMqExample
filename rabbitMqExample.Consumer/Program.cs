using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using rabbitMqExample.Shared;
using System.Text;
using System.Text.Json;

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

            //prefetchCount => Subscriberlara gönderilecek verinin adedini belirler
            //5 verirsek 5-5-5 şeklinde kaç tane subscriber var ise toplu şekilde gönderir
            //global => true ise göndereceği veriyi kaç tane subscriber var ise bölerek gönderir.
            //Diyelim ki 25 tane mesajımız,prefetchCount:5 ve global true; ve beş tane subscriber instance ımız var. O zaman her birine tek seferde 5-5-5-5-5 şeklinde dağılım yaparak gönderir.
            //Diyelim ki 10 tane mesajımız,prefetchCount:1 ,global:false olsun 2 tane de subscribe instance ımız olsun o zaman her bir instance a tek seferde 1 er 1 er datayı gönderir
            //yani global true ise prefetchCount u tüm instancelara eşit şekilde bölerken,global false ise yazılan prefetchCount değerini direkt olarak her bir instance a gönderir
            //Önemli not:Eğer subscriberlar hızlı bir şekilde işlem yapıyorlar ise prefetchCount yüksek verilir,tam tersi durumda düşük verilir
            channel.BasicQos(0,prefetchCount:1, global: false);

            //bir consumer eventi oluşturulur
            var consumer = new EventingBasicConsumer(channel);

            //dictionary tipinde propertylerin oluşturulması
            Dictionary<string, object> headers = new();
            headers.Add("format", "pdf");
            headers.Add("type", "a4");
            headers.Add("x-match", "all");
            //property oluşturduk
            var properties = channel.CreateBasicProperties();
            //mesaj içeriğinin kalıcı hale getirilmesi
            properties.Persistent = true;
            properties.Headers = headers;

            //queuelerin chanela bind edilmesi
            var queueName = channel.QueueDeclare().QueueName;
            channel.QueueBind(queueName, "headers-exchange", string.Empty, headers);

            

            consumer.Received += (object sender, BasicDeliverEventArgs e) => {
                //mesaj kuyruktan alınıp stringe çevrilir
                var message = Encoding.UTF8.GetString(e.Body.ToArray());
                Product product = JsonSerializer.Deserialize<Product>(message);

                Console.WriteLine($"Product Id:{product.Id} \n Product Name:{product.Name}\n Product Price:{product.Price}");
                channel.BasicAck(e.DeliveryTag, multiple:false);
            };
            channel.BasicConsume(queueName,false,consumer);

            Console.ReadLine();
        }
    }
}