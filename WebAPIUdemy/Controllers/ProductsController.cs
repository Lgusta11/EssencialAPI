using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIUdemy.Context;
using WebAPIUdemy.Model;
using WebAPIUdemy.Repositories;

namespace WebAPIUdemy.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IRepository<Product> _repository;
        private readonly IProductRepository _productRepository;

        public ProductsController(IProductRepository? repository, IProductRepository productRepository)
        {
            _repository = repository;
            _productRepository = productRepository;
        }

        [HttpGet("products/{id}")]
        public ActionResult<IEnumerable<Product>> GetProductByCategory(int id)
        {
            var products = _productRepository.GetProductsByCategory(id);
            if (products is null)
            {
                return StatusCode(404, $"Não encontrado!");
            }

            return Ok(products);
        }



        [HttpGet]
        public ActionResult<IEnumerable<Product>> Get() 
        {
            var products = _repository!.GetAll();
            if (products is null)
            {
                return NotFound("Produto não encontrado");  
            }
            return Ok(products);
        }

        [HttpGet("{id:int:min(1)}", Name = "ObterProduto")]
        public ActionResult<Product> Get(int id)
        {
            var products = _repository?.Get(p => p.ProductId == id);
            if (products is null)
            {
                return NotFound("Produto não encontrado");
            }
            return products;
        }

        [HttpPost]
        public ActionResult Post(Product product) 
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var productCreate = _repository!.Create(product);

            return new CreatedAtRouteResult("ObterProduto", new { id = productCreate.ProductId} , productCreate);            
        }

        [HttpPut("{id:int:min(1)}")]
        public ActionResult Put(Product product, int id)
        {
            if (id != product.ProductId)
            {
                return BadRequest("Informe um id valido");
            }

            var products = _repository!.Update(product);

            return Ok(products);
        }

        [HttpDelete("{id:int:min(1)}")]
        public ActionResult Delete(int id)
        {
            var product = _repository?.Get(p => p.ProductId == id);

            if (product is null)
            {
                return NotFound("Produto não localizado");
            }

            var productDelete = _repository!.Delete(product);
          
            return Ok(productDelete);
        }
    }
}
