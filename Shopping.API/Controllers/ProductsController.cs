using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Shopping.DataAccess.Models;


namespace Online_ShoppingCart_API.Controllers
{
    [Route("api/v1/products")]
    [ApiController]
    
    public class ProductsController : ControllerBase
    {
        private readonly StoreContext _storecontext;


        public ProductsController(StoreContext storeContext)
        {
            _storecontext = storeContext;

        }


        [Authorize(Roles = "User,Admin,Supplier")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            try
            {
                if (_storecontext.Products == null)
                {
                    return NotFound();
                }
                return Ok(await _storecontext.Products.ToListAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }

        [Authorize(Roles = "User,Admin,Supplier")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> Getproduct(int id)
        {
            try
            {
                if (_storecontext.Products == null)
                {
                    return NotFound();
                }

                var product = await _storecontext.Products.FindAsync(id);
                if (product == null)
                {
                    return NotFound();
                }
                return Ok(product);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    
        [HttpGet("getproductimage/{productId}")]
        public async Task<IActionResult> GetProductImage(int productId)
        {
            try
            {
                var product = await _storecontext.Products.FindAsync(productId);


                if (product == null || product.Product_Image == null)
                {
                    return NotFound();
                    
                }

                return File(new MemoryStream(product.Product_Image), "image/jpeg");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin,Supplier")]
        [HttpPost]
        public async Task<ActionResult> AddProduct([FromForm] Product product, IFormFile ProductImage)
        {
            
            try
            {
                var existingProduct = await _storecontext.Products.FirstOrDefaultAsync(p => p.Product_Name == product.Product_Name);
                if (existingProduct != null)
                {
                    
                    return Conflict("Product with the same name already exists");
                }

                if (ProductImage != null && ProductImage.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await ProductImage.CopyToAsync(memoryStream);
                        product.Product_Image = memoryStream.ToArray();
                    }
                }

            

                _storecontext.Products.Add(product);
                await _storecontext.SaveChangesAsync();

                return Ok("Product Added Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin,Supplier")]
        [HttpPut("{productId}")]
        public async Task<ActionResult> UpdateProduct(int productId, [FromForm] Product newProductData, IFormFile ProductImage)
        {
            try
            {
                var existingProduct = await _storecontext.Products.FindAsync(productId);

                if (existingProduct == null)
                    return NotFound("Product not found");


                
                    existingProduct.Product_Name = newProductData.Product_Name;
                    existingProduct.Description = newProductData.Description;
                    existingProduct.UnitPrice = newProductData.UnitPrice;
                    existingProduct.Category = newProductData.Category;
                    existingProduct.Brand_Name = newProductData.Brand_Name;
                    existingProduct.QuantityInStock = newProductData.QuantityInStock;

                if (ProductImage != null && ProductImage.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await ProductImage.CopyToAsync(memoryStream);
                        existingProduct.Product_Image = memoryStream.ToArray();
                    }
                }

             
                _storecontext.Products.Update(existingProduct);
                await _storecontext.SaveChangesAsync();

                return Ok("Product Updated Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Authorize(Roles = "Admin,Supplier")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> deleteProduct(int Id)
        {
            try
            {
                Product? source = _storecontext.Products.Find(Id);
                if (source == null)
                {
                    return BadRequest("product id not found");
                }
                _storecontext.Products.Remove(source);
                await _storecontext.SaveChangesAsync();
                return Ok("Product Deleted Successfully");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
