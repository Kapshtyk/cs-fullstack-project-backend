using System.ComponentModel.DataAnnotations.Schema;
using Ecommerce.Domain.Common;

namespace Ecommerce.Domain.Models
{
    public class FrontPage : BaseEntity
    {
        [Column(TypeName = "varchar(256)")]
        public required string HeroBanner { get; set; }

        [Column(TypeName = "varchar(256)")]
        public required string HeroBannerText { get; set; }
        public int SelectedProductId { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsPublished { get; set; }

        public Product? SelectedProduct { get; set; }
    }
}