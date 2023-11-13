using RabbitMQ.Client;
using rabbitMqExample.Shared;
using System.Net.Http.Headers;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Text.Json;


//Rabbit MQ url & connection
var factory = new ConnectionFactory();
factory.Uri = new Uri("amqps://epytrzjx:V1fSdtcKv36kwPqdaa1dgMw9MkPFlsPz@fish.rmq.cloudamqp.com/epytrzjx");
using var connection = factory.CreateConnection();
//Creating channel and sending message directly to queue
var channel = connection.CreateModel();
//durable, exchange down olsa bile bu exchange tutulmaya devam eder
//exchange: kuyruğa farklı kaynaklardan bağlanılsın mı? 
//autoDelete:publisher down olduğunda exchange silinsin mi
channel.ExchangeDeclare(exchange: "headers-exchange",durable:true,type:ExchangeType.Headers); //Exchange in oluşturulmasıu


//dictionary tipinde propertylerin oluşturulması
Dictionary<string, object> headers = new();
headers.Add("format", "pdf");
headers.Add("type", "a4");

//property oluşturduk
var properties = channel.CreateBasicProperties();
//mesaj içeriğinin kalıcı hale getirilmesi
properties.Persistent = true;
properties.Headers = headers;

var product = new Product {
    Id=1,
    Name="Test Product",
    Price=456,
    Stock=45
};

//json a çevir
var jsonProduct=JsonSerializer.Serialize(product);
//byteArraye çevir
var message = Encoding.UTF8.GetBytes(jsonProduct);

channel.BasicPublish(exchange: "headers-exchange", String.Empty, properties, message);

Console.WriteLine("Mesaj gönderildi");
Console.ReadLine();


public enum LogTypes {
    Critical = 1,
    Error = 2,
    Warning = 3,
    Info = 4
}

