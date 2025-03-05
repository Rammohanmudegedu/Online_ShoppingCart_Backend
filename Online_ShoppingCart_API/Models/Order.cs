using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Online_ShoppingCart_API.Models
{
    // this service used to store order details
    public class Order
    {
        public Order()
        {
            OrderedDate = DateTime.Now;
            
            
        }
        [Key]
        public int OrderId { get; set; }   // unique order id

        [Required(AllowEmptyStrings = false, ErrorMessage = "Street is Required")]
        public string Address { get; set; }  // address

        [Required(AllowEmptyStrings = false, ErrorMessage = "ZipCode is Required")]

        [RegularExpression(@"^\d{6}$", ErrorMessage = "Enter valid ZipCode")]
        public string ZipCode { get; set; }  // zipcode

        [Required(AllowEmptyStrings = false, ErrorMessage = "City is Required")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Enter Characters only")]
        public string City { get; set; }   // city
        public List<OrderItem> OrderItems { get; set; }   // items getting from cartItem model

        [Required]
        public decimal OrderPrice { get; set; }   // Order Price
        public DateTime OrderedDate { get; set; }   // order created date

        public string OrderStatus { get; set; }

       


    }
}
