using System.ComponentModel.DataAnnotations;

namespace Shopping.DataAccess.Models
{
    public class Order
    {
        public Order()
        {
            OrderedDate = DateTime.Now;
        }

        [Key]
        public int OrderId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Street is Required")]
        public string? Address { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "ZipCode is Required")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "Enter valid ZipCode")]
        public string ZipCode { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "City is Required")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Enter Characters only")]
        public string City { get; set; }

        public List<OrderItem> OrderItems { get; set; }

        [Required]
        public decimal OrderPrice { get; set; }
        public DateTime OrderedDate { get; set; }
        public string OrderStatus { get; set; }
    }
}
