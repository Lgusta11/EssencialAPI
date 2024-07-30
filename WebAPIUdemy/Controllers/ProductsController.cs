using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using WebAPIUdemy.DTOs;
using WebAPIUdemy.DTOs.Mappings;
using WebAPIUdemy.Pagination;
using WebAPIUdemy.Repositories;
using System.Text.Json;
using WebAPIUdemy.Pagination.Filters;
using WebAPIUdemy.Model;
using Microsoft.AspNetCore.Authorization;

namespace WebAPIUdemy.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IUnitOfWork? _unitOfWork;

        public ProductsController(IUnitOfWork? unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("productsbycategory/{id}")]
        public async  Task<ActionResult<IEnumerable<ProductDTO>>> GetProductByCategory(int id)
        {
            bool categoryExists = await _unitOfWork!.CategoryRepository.CategoryExistsAsync(id);

            if (!categoryExists)
             return NotFound("Categoria não encontrada");

            var products = await _unitOfWork.ProductRepository.GetProductsByCategoryAsync(id);

            if (products == null || !products.Any())
                return NotFound("Nenhum produto encontrado para esta categoria");

            var productsDto = products.ToProductDtoList();

            return Ok(productsDto);
        }


        [HttpGet("pagination")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> Get([FromQuery] ProductsParameters productsParameters)
        {
            var products = await _unitOfWork!.ProductRepository.GetProductsAsync(productsParameters);
            if (products is null)
            {
                return NotFound("Produtos não encontrados");
            }

            return GetProducts(products);
        }

        [HttpGet("filter/price/pagination")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProductsPrice([FromQuery] ProductsFilterPrice productsFilterParameters)
        {
            var products = await _unitOfWork.ProductRepository.GetProductsFilterPriceAsync(productsFilterParameters);
            if (products is null)
            {
                return NotFound("Produtos não encontrados");
            }

            return GetProducts(products);
        }

        private ActionResult<IEnumerable<ProductDTO>> GetProducts(PagedList<Product>? products)
        {
            var metaData = new
            {
                products.TotalCount,
                products.PageSize,
                products.CurrentPage,
                products.TotalPages,
                products.HasNext,
                products.HasPrevious
            };

            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metaData));

            var productsDto = products.ToProductDtoList();
            return Ok(productsDto);
        }


        [HttpGet]
        [Authorize(Policy = "UserOnly")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> Get()
        {
            var products = await _unitOfWork!.ProductRepository.GetAllAsync();
            if (products is null)
            {
                return NotFound("Produto não encontrado");
            }
            var productsDto = products.ToProductDtoList();
            return Ok(productsDto);
        }

        [HttpGet("{id:int:min(1)}", Name = "ObterProduto")]
        public async Task<ActionResult<ProductDTO>> Get(int id)
        {
            var product = await _unitOfWork!.ProductRepository.GetAsync(p => p.ProductId == id);
            if (product is null)
            {
                return NotFound("Produto não encontrado");
            }

            var productDto = product.ToProductDTO();

            return Ok(product);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public ActionResult<ProductDTO> Post(ProductDTO productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = productDto.ToProduct();

            var productCreated = _unitOfWork!.ProductRepository.Create(product);
            _unitOfWork.CommitAsync();

            var newProductDto = productCreated!.ToProductDTO();

            return new CreatedAtRouteResult("ObterProduto", new { id = newProductDto!.ProductId }, newProductDto);
        }

        [HttpPatch("{id}/UpdatePartial")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<ProductDTOUpdateResponse>> Patch(int id, JsonPatchDocument<ProductDTOUpdateRequest> patchProductDTO)
        {
            if (patchProductDTO == null || id <= 0)
                return BadRequest();

            var product = await _unitOfWork.ProductRepository.GetAsync(p => p.ProductId == id);

            if (product == null)
                return NotFound();

            var productUpdateRequest = product.ProductUpdateDtoRequest();

            patchProductDTO.ApplyTo(productUpdateRequest, ModelState);

            if (!ModelState.IsValid || !TryValidateModel(productUpdateRequest))
                return BadRequest(ModelState);

            product.Stock = productUpdateRequest.Stock;
            product.RegistrationDate = productUpdateRequest.RegistrationDate;

            _unitOfWork.ProductRepository.Update(product);
            _unitOfWork.CommitAsync();

            var productResponse = product.ToProductUpdateResponse();
            return Ok(productResponse);
        }

        [HttpPut("{id:int:min(1)}")]
        [Authorize(Policy = "AdminOnly")]
        public ActionResult<ProductDTO> Put(ProductDTO productDto, int id)
        {
            if (id != productDto.ProductId)
            {
                return BadRequest("Informe um id valido");
            }

            var product = productDto.ToProduct();

            var productsUpdated = _unitOfWork!.ProductRepository.Update(product);
            _unitOfWork.CommitAsync();

            var newUpdatedProductDto = productsUpdated!.ToProductDTO();

            return Ok(newUpdatedProductDto);
        }

        [HttpDelete("{id:int:min(1)}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<ProductDTO>> Delete(int id)
        {
            var product = await _unitOfWork!.ProductRepository.GetAsync(p => p.ProductId == id);

            if (product is null)
            {
                return NotFound("Produto não localizado");
            }

            var productDelete = _unitOfWork!.ProductRepository.Delete(product);
            _unitOfWork.CommitAsync();

            var deletedProduct = productDelete!.ToProductDTO();

            return Ok(productDelete);
        }
    }
}
