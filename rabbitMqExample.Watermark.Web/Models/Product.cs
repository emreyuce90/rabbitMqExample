using System.ComponentModel.DataAnnotations;

namespace rabbitMqExample.Watermark.Web.Models {
    public class Product {
        [Key]
        public int Id { get; set; }
        [StringLength(100)]
        public string Name { get; set; }
        public decimal Price { get; set; }
        [Range(1,100)]
        public int Stock { get; set; }
        public string? PictureUrl { get; set; }
    }
}
