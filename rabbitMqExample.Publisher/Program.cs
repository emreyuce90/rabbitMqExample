﻿using RabbitMQ.Client;
using System.Text;


//Rabbit MQ url & connection
var factory = new ConnectionFactory();
factory.Uri = new Uri("amqps://epytrzjx:V1fSdtcKv36kwPqdaa1dgMw9MkPFlsPz@fish.rmq.cloudamqp.com/epytrzjx");
using var connection = factory.CreateConnection();
//Creating channel and sending message directly to queue
var channel = connection.CreateModel();
//durable:kuyruk memory de mi tutulsun yoksa kayıt edilsin mi
//excluslive:publisher ımız farklı,subscriber ımız farklı olduğu için uygulama farklı processlerde çalıştığı için false geçeriz
//autoDelete: herhangi bir sebepten ötürü consumerımız(subscriber) düşerse kuyruğu kaybetmemek için autoDelete i false a çektik.
channel.QueueDeclare("hello-world-queue",durable:true,exclusive:false,autoDelete:false);

//kuyruğa göndereceğimiz mesaj
string message = "Hello World!";
var messageBody = Encoding.UTF8.GetBytes(message);

//Kuyruğa bu mesajı ekleme
//exchange: şu an mesajımızı exchange üzerinden değil direkt olarak kuyruğa verdik
channel.BasicPublish(String.Empty,"hello-world-queue", null,messageBody);
Console.WriteLine("Mesaj gönderildi");
Console.ReadLine();
