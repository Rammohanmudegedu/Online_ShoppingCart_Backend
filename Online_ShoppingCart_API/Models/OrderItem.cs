using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Online_ShoppingCart_API.Models
{
    [Keyless]
    public class OrderItem 
    {



        [RegularExpression(@"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$", ErrorMessage = "Invalid Email Address.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public int ProductId { get;  set; }

        [Required]
        public string Product_Name { get; set; }   // product name

        [Required]
        public Byte[]? Product_Image { get; set; } 

        [Required]
        
        public int Quantity { get; set; }

        [Required]
        
        public decimal UnitPrice { get; set; }
        
        public int OrderId { get; set; }

    }
}
