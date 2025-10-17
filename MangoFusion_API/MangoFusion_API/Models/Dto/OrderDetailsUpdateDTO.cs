using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MangoFusion_API.Models.Dto
{
    public class OrderDetailsUpdateDTO
    {

        [Required]
        public int OrderDetailId { get; set; } // primary key

        //[Required]
        //public int MenuItemId { get; set; } = new(); // navigation property to the order header
        [Required]
        [Range(1,5)]


        public int Rating { get; set; } // rating given by the user for this item in this order detail (1-5 stars)
    }
}
