using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MangoFusion_API.Models.Dto
{
    public class OrderDetailsCreateDTO
    {
        [Required]
        public int MenuItemId { get; set; } = new(); // navigation property to the order header


        [Required]
        public int Quantity { get; set; } // number of items ordered

        [Required]
        public string ItemName { get; set; } = string.Empty; // name of the item ordered (denormalized for convenience)

        [Required]
        public double Price { get; set; } // price of the item ordered (denormalized for convenience)
    }
}
