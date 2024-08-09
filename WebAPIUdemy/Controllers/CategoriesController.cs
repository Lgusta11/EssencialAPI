using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Text.Json;
using WebAPIUdemy.DTOs;
using WebAPIUdemy.DTOs.Mappings;
using WebAPIUdemy.Filters;
using WebAPIUdemy.Model;
using WebAPIUdemy.Pagination;
using WebAPIUdemy.Pagination.Filters;
using WebAPIUdemy.Repositories;

namespace WebAPIUdemy.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    [ApiConventionType(typeof(DefaultApiConventions))]

    public class CategoriesController : ControllerBase
    {
        private readonly IUnitOfWork? _unitOfWork;

        public CategoriesController(IUnitOfWork? unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        
        [HttpGet]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> Get()
        {
            var categories = await _unitOfWork!.CategoryRepository.GetAllAsync();
            if (categories is null)
                return NotFound();

            var categoriesDto = categories.ToCategoryDtoList();
            return Ok(categoriesDto);
        }

        
        [HttpGet("pagination")]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> Get([FromQuery] CategoriesParameters categoriesParameters)
        {
            var categories = await _unitOfWork?.CategoryRepository.GetCategoriesAsync(categoriesParameters);
            return GetCategories(categories);
        }


        [HttpGet("filter/name/pagination")]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategoriesFiltered([FromQuery] CategoriesFilterName categoriesFilter)
        {
            var categoriesFiltered = await _unitOfWork.CategoryRepository.GetCategoriesFilterNameAsync(categoriesFilter);
            return Ok(categoriesFiltered);
        }

        [HttpGet("{id:int:min(1)}", Name = "ObterCategoria")]
        public async Task<ActionResult<CategoryDTO>> Get(int id)
        {
            var category = await _unitOfWork!.CategoryRepository.GetAsync(c => c.CategoryId == id);
            if (category is null)
            {
                return NotFound($"Categoria com id={id} não encontrada");
            }

            var categoryDto = category.ToCategoryDTO();
            return Ok(categoryDto);
        }


        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public ActionResult<CategoryDTO> Post(CategoryDTO categoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var category = categoryDto.ToCartegory();

            var categoryCreated = _unitOfWork!.CategoryRepository.Create(category);
            _unitOfWork.CommitAsync();

            var newCategoryDto = categoryCreated!.ToCategoryDTO();

            return new CreatedAtRouteResult("ObterCategoria",
                new { id = newCategoryDto!.CategoryId }, newCategoryDto);
        }


        [HttpPut("{id:int:min(1)}")]
        [Authorize(Policy = "AdminOnly")]
        public ActionResult<CategoryDTO> Put(CategoryDTO categoryDto, int id)
        {
            if (id != categoryDto.CategoryId)
            {
                return BadRequest("Informe um id válido");
            }

            var category = categoryDto.ToCartegory();

            var categoryUpdated = _unitOfWork!.CategoryRepository.Update(category!);
            _unitOfWork.CommitAsync();

            var newUpdatedCategoryDto = categoryUpdated!.ToCategoryDTO();

            return Ok(newUpdatedCategoryDto);
        }


        [HttpDelete("{id:int:min(1)}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<CategoryDTO>> Delete(int id)
        {
            var category = await _unitOfWork!.CategoryRepository.GetAsync(c => c.CategoryId == id);

            if (category is null)
            {
                return NotFound("Categoria não localizada");
            }

            var categoryDelete = _unitOfWork.CategoryRepository.Delete(category);
            _unitOfWork.CommitAsync();

            var deletedCategory = categoryDelete!.ToCategoryDTO();
            return Ok(deletedCategory);
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
    }
}
