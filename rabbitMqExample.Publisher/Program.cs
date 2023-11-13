using RabbitMQ.Client;
using System.Text;


//Rabbit MQ url & connection
var factory = new ConnectionFactory();
factory.Uri = new Uri("amqps://epytrzjx:V1fSdtcKv36kwPqdaa1dgMw9MkPFlsPz@fish.rmq.cloudamqp.com/epytrzjx");
using var connection = factory.CreateConnection();
//Creating channel and sending message directly to queue
var channel = connection.CreateModel();
//durable, exchange down olsa bile bu exchange tutulmaya devam eder
channel.ExchangeDeclare(exchange:"logs-fanout",type:ExchangeType.Fanout,durable:true,false,null);


for (int i = 0; i <= 50; i++) {
    string message = "Log"+i;
    var messageBody = Encoding.UTF8.GetBytes(message);
    channel.BasicPublish(exchange:"logs-fanout","", null, messageBody);
    Console.WriteLine("Log gönderildi");

}

Console.ReadLine();

