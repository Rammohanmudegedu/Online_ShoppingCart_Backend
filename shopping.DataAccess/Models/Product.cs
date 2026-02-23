using System.ComponentModel.DataAnnotations;

namespace Shopping.DataAccess.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Product Name is Required")]
        [RegularExpression(@"^[a-zA-Z0-9\s]+$", ErrorMessage = "Enter alphanumeric characters only")]
        public string Product_Name { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Description is Required")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Price is Required")]
        [Range(1.00, double.MaxValue, ErrorMessage = "Price must be a non-negative value.")]
        public decimal UnitPrice { get; set; }

        public byte[]? Product_Image { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Category is Required")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Enter Characters only")]
        public string Category { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Brand is Required")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Enter Characters only")]
        public string Brand_Name { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Quantity is Required")]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a non-negative value.")]
        public int QuantityInStock { get; set; }
    }
}
