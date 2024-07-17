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
        private readonly IUnitOfWork? _unitOfWork;

        public ProductsController(IUnitOfWork? unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("productsbycategory/{id}")]
        public ActionResult<IEnumerable<Product>> GetProductByCategory(int id)
        {
            var products = _unitOfWork!.ProductRepository.GetProductsByCategory(id);

            if (products is null)
                return NotFound("Produto não encontrado");

            return Ok(products);
        }



        [HttpGet]
        public ActionResult<IEnumerable<Product>> Get() 
        {
            var products = _unitOfWork!.ProductRepository.GetAll();
            if (products is null)
            {
                return NotFound("Produto não encontrado");  
            }
            return Ok(products);
        }

        [HttpGet("{id:int:min(1)}", Name = "ObterProduto")]
        public ActionResult<Product> Get(int id)
        {
            var products = _unitOfWork!.ProductRepository.Get(p => p.ProductId == id);
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

            var productCreate = _unitOfWork!.ProductRepository.Create(product);
            _unitOfWork.Commit();

            return new CreatedAtRouteResult("ObterProduto", new { id = productCreate!.ProductId} , productCreate);            
        }

        [HttpPut("{id:int:min(1)}")]
        public ActionResult Put(Product product, int id)
        {
            if (id != product.ProductId)
            {
                return BadRequest("Informe um id valido");
            }

            var products = _unitOfWork!.ProductRepository.Update(product);
            _unitOfWork.Commit();

            return Ok(products);
        }

        [HttpDelete("{id:int:min(1)}")]
        public ActionResult Delete(int id)
        {
            var product = _unitOfWork!.ProductRepository.Get(p => p.ProductId == id);

            if (product is null)
            {
                return NotFound("Produto não localizado");
            }

            var productDelete = _unitOfWork!.ProductRepository.Delete(product);
            _unitOfWork.Commit();

            return Ok(productDelete);
        }
    }
}
