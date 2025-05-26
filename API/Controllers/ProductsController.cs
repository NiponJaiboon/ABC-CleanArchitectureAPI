using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ABC.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _repo;

        public ProductsController(IProductRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Product>> Get()
        {
            var products = _repo.GetAllProductsAsync();
            return Ok(products);
        }
    }
}
