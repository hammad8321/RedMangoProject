using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MangoFusion_API.Models
{
    public class OrderHeader
    // summry for the order
    {
        // saummry for the order

        [Key]
        public int OrderHeaderId { get; set; }

        [Required]
        public string PickUpName { get; set; } = string.Empty;// foreign key to the user who placed the order
        [Required]
        public string PickUpPhoneNumber {get;set; } = string.Empty;
        [Required]
        public string PickUpEmail { get; set; } = string.Empty; // email of the user who placed the order
        public DateTime OrderDate { get; set; } // date when the order was placed


        public string ApplicationUserId { get; set; } = string.Empty; // foreign key to the user who placed the order
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; } = new(); // navigation property to the user who placed the order   


        public Double OrderTotal { get; set; } // total amount of the order 


        public string OrderStatus { get; set; } = string.Empty; // status of the order (e.g. pending, completed, cancelled)


        public int  TotalItems { get; set; } // total number of items in the order


        public List<OrderDetail> OrderDetails { get; set; } = new(); // navigation property to the order details (list of items in the order)


    }
}
