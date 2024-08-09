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
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class ProductsController : ControllerBase
    {
        private readonly IUnitOfWork? _unitOfWork;

        public ProductsController(IUnitOfWork? unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Retorna os produtos relacionados ao id da categoria.
        /// </summary>
        /// <param name="id">Id da categoria.</param>
        /// <returns>Uma lista de produtos relacionados à categoria escolhida.</returns>
        /// <remarks>
        /// Exemplo de request:
        /// 
        /// GET /products/productsbycategory/{id}
        /// </remarks>
        [HttpGet("productsbycategory/{id}")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProductByCategory(int id)
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

        /// <summary>
        /// Retorna uma lista paginada de produtos.
        /// </summary>
        /// <param name="productsParameters">Parâmetros de paginação.</param>
        /// <returns>Uma lista paginada de produtos.</returns>
        /// <remarks>
        /// Exemplo de request:
        /// 
        /// GET /products/pagination?PageNumber=1&amp;PageSize=10
        /// </remarks>
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

        /// <summary>
        /// Retorna uma lista paginada de produtos filtrados por preço.
        /// </summary>
        /// <param name="productsFilterParameters">Parâmetros de filtro por preço.</param>
        /// <returns>Uma lista paginada de produtos filtrados por preço.</returns>
        /// <remarks>
        /// Exemplo de request:
        /// 
        /// GET /products/filter/price/pagination?MinPrice=10&amp;MaxPrice=100
        /// </remarks>
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


        /// <summary>
        /// Exibe todos os produtos.
        /// </summary>
        /// <returns>Retorna uma lista de produtos.</returns>
        /// <remarks>
        /// Exemplo de request:
        /// 
        /// GET /products
        /// </remarks>
        [HttpGet]
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

        /// <summary>
        /// Retorna um produto pelo seu Id.
        /// </summary>
        /// <param name="id">Id do produto.</param>
        /// <returns>Objeto Produto.</returns>
        /// <remarks>
        /// Exemplo de request:
        /// 
        /// GET /products/{id}
        /// </remarks>
        [HttpGet("{id:int:min(1)}", Name = "ObterProduto")]
        public async Task<ActionResult<ProductDTO>> Get(int id)
        {

            if (id == null || id <= 0)
            {
                return BadRequest("ID de produto invalido");
            }
            var product = await _unitOfWork!.ProductRepository.GetAsync(p => p.ProductId == id);
            if (product is null)
            {
                return NotFound("Produto não encontrado");
            }

            var productDto = product.ToProductDTO();

            return Ok(productDto);
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
                return BadRequest("Informe um id válido");
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

            return Ok(deletedProduct);
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
    }
}
