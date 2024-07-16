using Microsoft.AspNetCore.Mvc;
using WebAPIUdemy.Filters;
using WebAPIUdemy.Model;
using WebAPIUdemy.Repositories;

namespace WebAPIUdemy.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IRepository<Category> _repository;

        public CategoriesController(ICategoryRepository? repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public ActionResult<IEnumerable<Category>> Get()
        {
            var categories = _repository!.GetAll();
            return Ok(categories);

        }
        

        [HttpGet("{id:int:min(1)}", Name = "ObterCategoria")]
        public async Task<ActionResult<Category>> Get(int id)
        {
         
                var category = _repository!.Get(c => c.CategoryId == id);
                if (category is null)
                {
                    return NotFound($"Categoria com id={id} não encontrada");
                }
                return Ok(category);
           
        }


        [HttpPost]
        public ActionResult Post(Category category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var categoryCreate = _repository!.Create(category);

            return new CreatedAtRouteResult("ObterCategoria",
                new { id = categoryCreate.CategoryId }, categoryCreate);
        }

        [HttpPut("{id:int:min(1)}")]
        public ActionResult Put(Category category, int id)
        {
            if (id != category.CategoryId)
            {
                return BadRequest("Informe um id valido");
            }

           _repository!.Update(category);
            return Ok(category);

        }

        [HttpDelete("{id:int:min(1)}")]
        public ActionResult Delete(int id)
        {
            var category = _repository!.Get(c => c.CategoryId == id);

            if (category is null)
            {
                return NotFound("Produto não localizado");
            }

            var categoryDelete = _repository.Delete(category);
            return Ok(categoryDelete);
        }
    }
}
