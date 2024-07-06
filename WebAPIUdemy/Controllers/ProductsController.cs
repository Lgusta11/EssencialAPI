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
        public async Task<ActionResult<IEnumerable<Product>>> Get() 
        {
            var products = await _context!.Products.AsNoTracking().ToListAsync();
            if (products is null)
            {
                return NotFound("Produto não encontrado");  
            }
            return Ok(products);
        }

        [HttpGet("{id:int:min(1)}", Name ="ObterProduto")]
        public async Task<ActionResult<Product>> Get(int id) 
        {
            var products = await _context!.Products.AsNoTracking().FirstOrDefaultAsync(p => p.ProductId == id);
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

        [HttpPut("{id:int:min(1)}")]
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

        [HttpDelete("{id:int:min(1)}")]
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
