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

        public ProductsController(ProductService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProductDto>>> Get()
        {
            var products = await _service.GetAllProductsAsync();
            var result = _mapper.Map<List<ProductDto>>(products);
            if (result == null || !result.Any())
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<ProductDto>> AddProduct([FromBody] ProductDto productDto)
        {
            if (productDto == null)
            {
                return BadRequest();
            }

            var product = _mapper.Map<Product>(productDto);
            await _service.AddProductAsync(product);
            return Ok(_mapper.Map<ProductDto>(product));
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
        public async Task<ActionResult<ProductDto>> GetProductByIdAsync(int id)
        {
            var product = await _service.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<ProductDto>(product));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProductAsync(int id, [FromBody] ProductDto updatedProductDto)
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

            var updatedProduct = _mapper.Map<Product>(updatedProductDto);
            await _service.UpdateProductAsync(updatedProduct);
            return NoContent();
        }
    }
}
