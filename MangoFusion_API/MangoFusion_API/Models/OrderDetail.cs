using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace MangoFusion_API.Models
{
    public class OrderDetail
    {
        [Key]
        public int OrderDetailId { get; set; } // primary key
        [Required]
        public int OrderHeaderId { get; set; } // foreign key to the order header

        [Required]
        public int MenuItemId { get; set; } = new(); // navigation property to the order header
        [ForeignKey("MenuItemId")]
        public MenuItem? MenuItem { get; set; } = new(); // navigation property to the menu item

        [Required]
        public int Quantity { get; set; } // number of items ordered

        [Required]
        public string ItemName { get; set; } = string.Empty; // name of the item ordered (denormalized for convenience)

        [Required]
        public double Price { get; set; } // price of the item ordered (denormalized for convenience)

        public int? Rating { get; set; } = null;// rating given by the user for this item in this order detail (1-5 stars)


    }
}
