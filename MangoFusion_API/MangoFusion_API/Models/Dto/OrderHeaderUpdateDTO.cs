using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MangoFusion_API.Models.Dto
{
    public class OrderHeaderUpdateDTO
    {
        // saummry for the order

        
        [Required]
        public int OrderHeaderId { get; set; }

      
        public string PickUpName { get; set; } = string.Empty;// foreign key to the user who placed the order
       
        public string PickUpPhoneNumber { get; set; } = string.Empty;
        
        public string PickUpEmail { get; set; } = string.Empty; // email of the user who placed the order
     


        public string OrderStatus { get; set; } = string.Empty; // status of the order (e.g. pending, completed, cancelled)


     
    }
}
