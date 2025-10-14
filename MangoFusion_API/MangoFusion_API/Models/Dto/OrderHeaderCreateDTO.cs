using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MangoFusion_API.Models.Dto
{
    public class OrderHeaderCreateDTO
    {
        // saummry for the order

     

        [Required]
        public string PickUpName { get; set; } = string.Empty;// foreign key to the user who placed the order
        [Required]
        public string PickUpPhoneNumber { get; set; } = string.Empty;
        [Required]
        public string PickUpEmail { get; set; } = string.Empty; // email of the user who placed the order
        


        public string ApplicationUserId { get; set; } = string.Empty; // foreign key to the user who placed the order
      

        public Double OrderTotal { get; set; } // total amount of the order 


        public string OrderStatus { get; set; } = string.Empty; // status of the order (e.g. pending, completed, cancelled)


        public int TotalItems { get; set; } // total number of items in the order


        public List<OrderDetailsCreateDTO> OrderDetailsDTO { get; set; } = new(); // navigation property to the order details (list of items in the order)
    }
}
