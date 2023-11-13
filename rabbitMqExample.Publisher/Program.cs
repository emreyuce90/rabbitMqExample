using RabbitMQ.Client;
using System.Runtime.ConstrainedExecution;
using System.Text;


//Rabbit MQ url & connection
var factory = new ConnectionFactory();
factory.Uri = new Uri("amqps://epytrzjx:V1fSdtcKv36kwPqdaa1dgMw9MkPFlsPz@fish.rmq.cloudamqp.com/epytrzjx");
using var connection = factory.CreateConnection();
//Creating channel and sending message directly to queue
var channel = connection.CreateModel();
//durable, exchange down olsa bile bu exchange tutulmaya devam eder
//exchange: kuyruğa farklı kaynaklardan bağlanılsın mı? 
//autoDelete:publisher down olduğunda exchange silinsin mi
channel.ExchangeDeclare(exchange: "logs-direct",durable:true,type:ExchangeType.Direct); //Exchange in oluşturulmasıu

//4 farklı queue nin oluşturulması ve bu queuelerin mevcut exchange bind edilmesi
Enum.GetNames(typeof(LogTypes)).ToList().ForEach(x => {
    var routingKey = $"route-{x}"; //route-Critical route-Error...
    var queueName = $"direct-queue-{x}"; //direct-queue-Critical //direct-queue-Error...
    channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false); //kuyruk oluşturulması
    channel.QueueBind(queueName, "logs-direct", routingKey, null);//bu kuyrukların exchange e bind edilmesi
});



//Kuyruklara mesaj gönderme işlemi
for (int i = 0; i <= 50; i++) {

    //Rasgele bir rota oluşturulur
    LogTypes randomTypes = (LogTypes)new Random().Next(1,5);  
    
    string message = $"LogType:{randomTypes}";
    var messageBody = Encoding.UTF8.GetBytes(message);
    var routeKey = $"route-{randomTypes}";
    channel.BasicPublish(exchange: "logs-direct",routeKey, null, messageBody);
    Console.WriteLine($"Log gönderildi:{message}");

}

Console.ReadLine();


public enum LogTypes {
    Critical = 1,
    Error = 2,
    Warning = 3,
    Info = 4
}

