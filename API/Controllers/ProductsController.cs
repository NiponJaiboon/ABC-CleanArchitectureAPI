using Application.Services;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ABC.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _service;

        public ProductsController(ProductService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<Product>>> Get()
        {
            var products = await _service.GetAllProductsAsync();
            return Ok(products);
        }

        [HttpPost]
        public async Task<ActionResult<Product>> AddProduct([FromBody] Product product)
        {
            if (product == null)
            {
                return BadRequest();
            }

            await _service.AddProductAsync(product);
            return Ok(product);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductAsync(int id)
        {
            var product = await _service.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            await _service.DeleteProductAsync(id);
            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProductByIdAsync(int id)
        {
            var product = await _service.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProductAsync(int id, [FromBody] Product updatedProduct)
        {
            if (updatedProduct == null || updatedProduct.Id != id)
            {
                return BadRequest();
            }

            var existingProduct = await _service.GetProductByIdAsync(id);
            if (existingProduct == null)
            {
                return NotFound();
            }

            await _service.UpdateProductAsync(updatedProduct);
            return NoContent();
        }
    }
}
