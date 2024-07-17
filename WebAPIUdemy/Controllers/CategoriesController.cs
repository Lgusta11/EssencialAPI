using Microsoft.AspNetCore.Mvc;
using WebAPIUdemy.DTOs;
using WebAPIUdemy.Filters;
using WebAPIUdemy.Model;
using WebAPIUdemy.Repositories;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace WebAPIUdemy.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IUnitOfWork? _unitOfWork;

        public CategoriesController(IUnitOfWork? unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public ActionResult<IEnumerable<CategoryDTO>> Get()
        {
            var categories = _unitOfWork!.CategoryRepository.GetAll();

            var categoriesDto = new List<CategoryDTO>();
            foreach (var category in categories)
            {
                var categoryDto = new CategoryDTO()
                {
                    CategoryId = category.CategoryId,
                    Name = category.Name!,
                    ImageUrl = category.ImageUrl!
                };
                categoriesDto.Add(categoryDto);
            }

            return Ok(categoriesDto);

        }
        

        [HttpGet("{id:int:min(1)}", Name = "ObterCategoria")]
        public ActionResult<CategoryDTO> Get(int id)
        {
         
                var category = _unitOfWork!.CategoryRepository.Get(c => c.CategoryId == id);
                if (category is null)
                {
                    return NotFound($"Categoria com id={id} não encontrada");
                }


            var categoryDto = new CategoryDTO()
            {
                CategoryId = category.CategoryId,
                Name = category.Name!,
                ImageUrl = category.ImageUrl!
            };
                return Ok(categoryDto);
           
        }


        [HttpPost]
        public ActionResult<CategoryDTO> Post(CategoryDTO categoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var category = new Category()
            {
                CategoryId = categoryDto.CategoryId,
                Name = categoryDto.Name!,
                ImageUrl = categoryDto.ImageUrl!
            };

            var categoryCreate = _unitOfWork!.CategoryRepository.Create(category);
            _unitOfWork.Commit();

            var newCategoryDto = new CategoryDTO()
            {
                CategoryId = categoryCreate.CategoryId,
                Name = categoryCreate.Name!,
                ImageUrl = categoryCreate.ImageUrl!
            };

            return new CreatedAtRouteResult("ObterCategoria",
                new { id = newCategoryDto!.CategoryId }, newCategoryDto);
        }

        [HttpPut("{id:int:min(1)}")]
        public ActionResult<CategoryDTO> Put(CategoryDTO categoryDto, int id)
        {
            if (id != categoryDto.CategoryId)
            {
                return BadRequest("Informe um id valido");
            }

            var category = new Category()
            {
                CategoryId = categoryDto.CategoryId,
                Name = categoryDto.Name!,
                ImageUrl = categoryDto.ImageUrl!
            };

            var categoryUpdated = _unitOfWork!.CategoryRepository.Update(category);
            _unitOfWork.Commit();

            var newUpdatedCategoryDto = new CategoryDTO()
            {
                CategoryId = categoryUpdated.CategoryId,
                Name = categoryUpdated.Name!,
                ImageUrl = categoryUpdated.ImageUrl!
            };

            return Ok(newUpdatedCategoryDto);

        }

        [HttpDelete("{id:int:min(1)}")]
        public ActionResult<CategoryDTO> Delete(int id)
        {
            var category = _unitOfWork!.CategoryRepository.Get(c => c.CategoryId == id);

            if (category is null)
            {
                return NotFound("Produto não localizado");
            }

            var categoryDelete = _unitOfWork.CategoryRepository.Delete(category);
            _unitOfWork.Commit();

            var deletedCategory = new CategoryDTO()
            {
                CategoryId = categoryDelete.CategoryId,
                Name = categoryDelete.Name!,
                ImageUrl = categoryDelete.ImageUrl!
            };
            return Ok(deletedCategory);
        }
    }
}
