using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using rabbitMqExample.Watermark.Web.Services;
using System.Drawing;
using System.Text;
using System.Text.Json;

namespace rabbitMqExample.Watermark.Web.BackgroundServices {
    public class ImageWatermarkBackgroundService : BackgroundService {
        private readonly RabbitMqService _rabbitMqService;
        private IModel _channel;
        private readonly ILogger<ImageWatermarkBackgroundService> _logger;
        public ImageWatermarkBackgroundService(RabbitMqService rabbitMqService, ILogger<ImageWatermarkBackgroundService> logger) {
            _rabbitMqService = rabbitMqService;
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken) {
            //Uygulama çalıştığında kanal aktif hale getirilir
            _channel = _rabbitMqService.Connect();
            //kuyruktan verileri teker teker alır
            _channel.BasicQos(0, 1, false);
            return base.StartAsync(cancellationToken);
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken) {
            var consumer = new AsyncEventingBasicConsumer(_channel);
            //autoAck false vermemizin sebebi ,biz işlemi yaptıktan sonra producer ı kanal aracılığıyla haberdar edeceğiz ,sonrasında kuyruk silinebilirr
            _channel.BasicConsume(RabbitMqService.QueueName,false,consumer);
            //eventin yakalanıp gerekli işlemin yapılması
            consumer.Received += Consumer_Received;
            return Task.CompletedTask;
        }

        private Task Consumer_Received(object sender, BasicDeliverEventArgs @event) {

            try {
                //Bize bir byte dizisi gelecek bu byte dizini önce stringe sonra da class a deserilize etmeliyiz
                var product = JsonSerializer.Deserialize<productImageCreatedEvent>(Encoding.UTF8.GetString(@event.Body.ToArray()));

                #region WaterMarkProcess
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwRoot/images", product.PictureUrl);
                using var img = Image.FromFile(path);
                using var graphic = Graphics.FromImage(img);
                var siteName = "www.zettech.com.tr";
                var font = new Font(FontFamily.GenericMonospace, 32, FontStyle.Bold, GraphicsUnit.Pixel);
                var textSize = graphic.MeasureString(siteName, font);
                var color = Color.FromArgb(128, 255, 255, 255);
                var brush = new SolidBrush(color);
                var position = new Point(img.Width - ((int)textSize.Width + 30), img.Height - ((int)textSize.Height + 30));
                graphic.DrawString(siteName, font, brush, position);
                img.Save("wwwroot/images/watermarkImages/" + product.PictureUrl);
                img.Dispose();
                graphic.Dispose();
                #endregion
                //işlemler düzgün yapıldığını channel aracılığıyla producer a bildiririz
                _channel.BasicAck(@event.DeliveryTag, false);
            } catch (Exception ex) {
                _logger.LogError(ex.Message);
                throw;
            }
            return Task.CompletedTask;
        }
    }
}
