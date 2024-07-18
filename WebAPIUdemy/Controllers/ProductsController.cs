using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using WebAPIUdemy.DTOs;
using WebAPIUdemy.DTOs.Mappings;
using WebAPIUdemy.Pagination;
using WebAPIUdemy.Repositories;
using System.Text.Json;

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
        public ActionResult<IEnumerable<ProductDTO>> GetProductByCategory(int id)
        {
            if (!_unitOfWork!.CategoryRepository.CategoryExists(id))
                return NotFound("Categoria não encontrada");

            var products = _unitOfWork.ProductRepository.GetProductsByCategory(id);

            if (products == null || !products.Any())
                return NotFound("Nenhum produto encontrado para esta categoria");

            var productsDto = products.ToProductDtoList();

            return Ok(productsDto);
        }


        [HttpGet("pagination")]
        public ActionResult<IEnumerable<ProductDTO>> Get([FromQuery] ProductsParameters productsParameters)
        {
            var products = _unitOfWork!.ProductRepository.GetProducts(productsParameters);
            if (products is null)
            {
                return NotFound("Produtos não encontrados");
            }

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
        public ActionResult<IEnumerable<ProductDTO>> Get()
        {
            var products = _unitOfWork!.ProductRepository.GetAll();
            if (products is null)
            {
                return NotFound("Produto não encontrado");
            }
            var productsDto = products.ToProductDtoList();
            return Ok(productsDto);
        }

        [HttpGet("{id:int:min(1)}", Name = "ObterProduto")]
        public ActionResult<ProductDTO> Get(int id)
        {
            var product = _unitOfWork!.ProductRepository.Get(p => p.ProductId == id);
            if (product is null)
            {
                return NotFound("Produto não encontrado");
            }

            var productDto = product.ToProductDTO();

            return Ok(product);
        }

        [HttpPost]
        public ActionResult<ProductDTO> Post(ProductDTO productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = productDto.ToProduct();

            var productCreated = _unitOfWork!.ProductRepository.Create(product);
            _unitOfWork.Commit();

            var newProductDto = productCreated!.ToProductDTO();

            return new CreatedAtRouteResult("ObterProduto", new { id = newProductDto!.ProductId }, newProductDto);
        }

        [HttpPatch("{id}/UpdatePartial")]
        public ActionResult<ProductDTOUpdateResponse> Patch(int id, JsonPatchDocument<ProductDTOUpdateRequest> patchProductDTO)
        {
            if (patchProductDTO == null || id <= 0)
                return BadRequest();

            var product = _unitOfWork.ProductRepository.Get(p => p.ProductId == id);

            if (product == null)
                return NotFound();

            var productUpdateRequest = product.ProductUpdateDtoRequest();

            patchProductDTO.ApplyTo(productUpdateRequest, ModelState);

            if (!ModelState.IsValid || !TryValidateModel(productUpdateRequest))
                return BadRequest(ModelState);

            product.Stock = productUpdateRequest.Stock;
            product.RegistrationDate = productUpdateRequest.RegistrationDate;

            _unitOfWork.ProductRepository.Update(product);
            _unitOfWork.Commit();

            var productResponse = product.ToProductUpdateResponse();
            return Ok(productResponse);
        }

        [HttpPut("{id:int:min(1)}")]
        public ActionResult<ProductDTO> Put(ProductDTO productDto, int id)
        {
            if (id != productDto.ProductId)
            {
                return BadRequest("Informe um id valido");
            }

            var product = productDto.ToProduct();

            var productsUpdated = _unitOfWork!.ProductRepository.Update(product);
            _unitOfWork.Commit();

            var newUpdatedProductDto = productsUpdated!.ToProductDTO();

            return Ok(newUpdatedProductDto);
        }

        [HttpDelete("{id:int:min(1)}")]
        public ActionResult<ProductDTO> Delete(int id)
        {
            var product = _unitOfWork!.ProductRepository.Get(p => p.ProductId == id);

            if (product is null)
            {
                return NotFound("Produto não localizado");
            }

            var productDelete = _unitOfWork!.ProductRepository.Delete(product);
            _unitOfWork.Commit();

            var deletedProduct = productDelete!.ToProductDTO();

            return Ok(productDelete);
        }
    }
}
