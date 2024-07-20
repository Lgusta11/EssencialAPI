using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WebAPIUdemy.DTOs;
using WebAPIUdemy.DTOs.Mappings;
using WebAPIUdemy.Filters;
using WebAPIUdemy.Model;
using WebAPIUdemy.Pagination;
using WebAPIUdemy.Pagination.Filters;
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
            var categoriesDto = categories.ToCategoryDtoList();

            return Ok(categoriesDto);

        }

        [HttpGet("pagination")]
        public ActionResult<IEnumerable<CategoryDTO>> Get([FromQuery] CategoriesParameters categoriesParameters)
        {
            var categories = _unitOfWork?.CategoryRepository.GetCategories(categoriesParameters);

            return GetCategories(categories);
        }

        [HttpGet("filter/name/pagination")]
        public ActionResult<IEnumerable<CategoryDTO>> GetCategoriesFiltered([FromQuery] CategoriesFilterName categoriesFilter)
        {
            var categoriesFiltered = _unitOfWork.CategoryRepository.GetCategoriesFilterName(categoriesFilter);

            return Ok(categoriesFiltered);
        }

        [HttpGet("{id:int:min(1)}", Name = "ObterCategoria")]
        public ActionResult<CategoryDTO> Get(int id)
        {

            var category = _unitOfWork!.CategoryRepository.Get(c => c.CategoryId == id);
            if (category is null)
            {
                return NotFound($"Categoria com id={id} não encontrada");
            }
            var categoryDto = category.ToCategoryDTO();
            return Ok(categoryDto);

        }

        private ActionResult<IEnumerable<CategoryDTO>> GetCategories(PagedList<Category>? categories)
        {
            var metaData = new
            {
                categories.TotalCount,
                categories.PageSize,
                categories.CurrentPage,
                categories.TotalPages,
                categories.HasNext,
                categories.HasPrevious
            };

            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metaData));

            var categoriesDto = categories.ToCategoryDtoList();
            return Ok(categoriesDto);
        }
        [HttpPost]
        public ActionResult<CategoryDTO> Post(CategoryDTO categoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var category = categoryDto.ToCartegory();

            var categoryCreated = _unitOfWork!.CategoryRepository.Create(category);
            _unitOfWork.Commit();

            var newCategoryDto = categoryCreated!.ToCategoryDTO();

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

            var category = categoryDto.ToCartegory();

            var categoryUpdated = _unitOfWork!.CategoryRepository.Update(category!);
            _unitOfWork.Commit();

            var newUpdatedCategoryDto = categoryUpdated!.ToCategoryDTO();

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

            var deletedCategory = categoryDelete!.ToCategoryDTO();
            return Ok(deletedCategory);
        }
    }
}
