using RabbitMQ.Client;
using rabbitMqExample.WebExcelCreate.Services;

namespace ExcelCreatorWorkerService {
    public class Program {
        public static void Main(string[] args) {
            IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureServices(services => {
                    services.AddHostedService<Worker>();
                    services.AddSingleton(sp => new ConnectionFactory { Uri = new Uri("amqps://epytrzjx:V1fSdtcKv36kwPqdaa1dgMw9MkPFlsPz@fish.rmq.cloudamqp.com/epytrzjx"), DispatchConsumersAsync = true });
                    services.AddSingleton<RabbitMqClientServices>();
                })
                .Build();

            host.Run();
        }
    }
}