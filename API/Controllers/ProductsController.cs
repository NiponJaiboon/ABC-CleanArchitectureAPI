using Application.DTOs;
using Application.Services;
using AutoMapper;
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
        private readonly IMapper _mapper;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(ProductService service, IMapper mapper, ILogger<ProductsController> logger)
        {
            _service = service;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<ProductDto>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<ProductDto>>> Get()
        {
            try
            {
                var products = await _service.GetAllProductsAsync();
                var result = _mapper.Map<List<ProductDto>>(products);
                if (result == null || !result.Any())
                {
                    return NotFound();
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in Get()");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(ProductDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ProductDto>> AddProduct([FromBody] ProductDto productDto)
        {
            try
            {
                if (productDto == null)
                {
                    return BadRequest();
                }

                var product = _mapper.Map<Product>(productDto);
                await _service.AddProductAsync(product);
                return Ok(_mapper.Map<ProductDto>(product));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in AddProduct()");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteProductAsync(int id)
        {
            try
            {
                var product = await _service.GetProductByIdAsync(id);
                if (product == null)
                {
                    return NotFound();
                }

                await _service.DeleteProductAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in DeleteProductAsync()");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProductDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ProductDto>> GetProductByIdAsync(int id)
        {
            try
            {
                var product = await _service.GetProductByIdAsync(id);
                if (product == null)
                {
                    return NotFound();
                }
                return Ok(_mapper.Map<ProductDto>(product));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in GetProductByIdAsync()");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateProductAsync(int id, [FromBody] ProductDto updatedProductDto)
        {
            try
            {
                if (updatedProductDto == null || updatedProductDto.Id != id)
                {
                    return BadRequest();
                }

                var existingProduct = await _service.GetProductByIdAsync(id);
                if (existingProduct == null)
                {
                    return NotFound();
                }

                _mapper.Map(updatedProductDto, existingProduct);
                await _service.UpdateProductAsync(existingProduct);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in UpdateProductAsync()");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("category/{categoryId}")]
        [ProducesResponseType(typeof(List<ProductDto>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<ProductDto>>> GetProductsByCategoryAsync(int categoryId)
        {
            try
            {
                var products = await _service.GetProductsByCategoryAsync(categoryId);
                if (products == null || !products.Any())
                {
                    return NotFound();
                }
                var result = _mapper.Map<List<ProductDto>>(products);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in GetProductsByCategoryAsync()");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
