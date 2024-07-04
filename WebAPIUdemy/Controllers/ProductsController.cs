using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIUdemy.Context;
using WebAPIUdemy.Model;

namespace WebAPIUdemy.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly CatalogoContext? _context;

        public ProductsController(CatalogoContext? context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Product>> Get() 
        {
            var products = _context!.Products.AsNoTracking().ToList();
            if (products is null)
            {
                return NotFound("Produto não encontrado");  
            }
            return Ok(products);
        }

        [HttpGet("{id:int}", Name ="ObterProduto")]
        public ActionResult<Product> Get(int id) 
        {
            var products = _context!.Products.AsNoTracking().FirstOrDefault(p => p.ProductId == id);
            if (products is null)
            {
                return NotFound("Produto não encontrado");
            }
            return products;
        }

        [HttpPost]
        public ActionResult Post(Product product) 
        {
            if (product is null)
            {
                return BadRequest();
            }


           _context!.Products.Add(product);
           _context.SaveChanges();

           return new CreatedAtRouteResult("ObterProduto", new { id = product.ProductId} , product);            
        }

        [HttpPut("{id:int}")]
        public ActionResult Put(Product product, int id)
        {
            if(id != product.ProductId)
            {
                return BadRequest("Informe um id valido");
            }

            _context!.Entry(product).State = EntityState.Modified;
            _context!.SaveChanges();

            return Ok(product);
        }

        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id) 
        {
            var product = _context!.Products.FirstOrDefault(p => p.ProductId == id);

            if (product is null)
            {
                return NotFound("Produto não localizado");
            }

            _context!.Products.Remove(product);
            _context!.SaveChanges();

            return Ok(product);
        }


    }
}
