using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIUdemy.Context;
using WebAPIUdemy.DTOs;
using WebAPIUdemy.DTOs.Mappings;
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
        public ActionResult<IEnumerable<ProductDTO>> GetProductByCategory(int id)
        {
            var products = _unitOfWork!.ProductRepository.GetProductsByCategory(id);

            if (products is null)
                return NotFound("Produto não encontrado");

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
            if (patchProductDTO is null || id <= 0)
                return BadRequest();

            var product = _unitOfWork!.ProductRepository.Get(p => p.ProductId == id);

            if (product is null)
                return NotFound();

            var productUpdateRequest = product.ProductUpdateDtoRequest();

            if (productUpdateRequest is null)
                return BadRequest("Product update request could not be created.");

            patchProductDTO.ApplyTo(productUpdateRequest, ModelState);

            if (!ModelState.IsValid || !TryValidateModel(productUpdateRequest))
                return BadRequest(ModelState);

            var productUpdated = productUpdateRequest!.ProductUpdateDtoRequestToProduct();
            _unitOfWork!.ProductRepository.Update(productUpdated!);
            _unitOfWork!.Commit();

            var ProductResponse = productUpdated!.ToProductUpdateResponse();
            return Ok(ProductResponse);
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
