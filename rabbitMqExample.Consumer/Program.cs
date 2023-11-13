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

            Console.WriteLine("Loglar dinleniyor....");
            consumer.Received += (object sender, BasicDeliverEventArgs e) => {
                //mesaj kuyruktan alınıp stringe çevrilir
                var message = Encoding.UTF8.GetString(e.Body.ToArray());
                Console.WriteLine("Log:" + message);
                //multiple:false değeri şunu ifade eder,eğer susbcribe instance ında diyelim 5 tane mesaj olsun ilk mesaj düzgün şekilde işlendiğinde diğer 4 mesaj memory de işlenmeyi bekler. multiple false dediğimizde ,diğer dört mesajı tek tek işleme alıp işlem başarılı ise rabbit mq ya bilgi verir 
                //multiple true ise: memory de olan mesajlar da sanki başarılı bir şekilde işlenmişçesine rabbit mq ya 
                Thread.Sleep(1000);
                channel.BasicAck(e.DeliveryTag, multiple:false);
            };
            channel.BasicConsume("direct-queue-Critical", autoAck:false, consumer);

            Console.ReadLine();
        }
    }
}