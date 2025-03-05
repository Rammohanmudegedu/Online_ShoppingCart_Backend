using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;


namespace Online_ShoppingCart_API.Models
{
    //This service used to store Product information
    public class Product
    {
   
       

        [Key]
        public int ProductId { get; set; }     //  unique product id 

        [Required(AllowEmptyStrings = false, ErrorMessage = "Product Name is Required")]
        [RegularExpression(@"^[a-zA-Z0-9\s]+$", ErrorMessage = "Enter alphanumeric characters only")]
        public string Product_Name { get; set; }   // product name

        [Required(AllowEmptyStrings = false, ErrorMessage = "Description is Required")]
        public string Description { get; set; }   // Description for product
      
        [Required(ErrorMessage = "Price is Required")]
        [Range(1.00, double.MaxValue, ErrorMessage = "Price must be a non-negative value.")]
        public decimal UnitPrice { get; set; }    // price for product

       
        public Byte[]? Product_Image { get; set; }   // to store product image into bytes

        [Required(AllowEmptyStrings = false, ErrorMessage = "Category is Required")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Enter Characters only")]
        public string Category { get; set;}    // category

        [Required(AllowEmptyStrings = false, ErrorMessage = "Brand is Required")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Enter Characters only")]
        public string Brand_Name { get; set; }   // brandname of product

        [Required(AllowEmptyStrings = false, ErrorMessage = "Quantity is Required")]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a non-negative value.")]
        public int QuantityInStock { get; set; }   // stock quantity

        
    }
}
