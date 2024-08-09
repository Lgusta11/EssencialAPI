using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPIUdemy.Controllers;
using WebAPIUdemy.DTOs;

namespace ApiCatalogoUnitTests.UnitTests.MetodosTest
{
    public class PutProductUnitTest : IClassFixture<ProductsUnitTestController>
    {
        private readonly ProductsController _controller;

        public PutProductUnitTest(ProductsUnitTestController fixture)
        {
            _controller = new ProductsController(fixture.repository);
        }

        [Fact]
        public async Task PutProduct_Update_Return_OkResult()
        {
            var prodId = 1;

                var updatedProductDto = new ProductDTO
                {
                    ProductId = prodId,
                    Name = "Produto Atualizado",
                    Description = "Descrição do novo produto",
                    ImageUrl = "imagemfake1.jpg",
                    CategoryId = 1
                };

            var result =  _controller.Put(updatedProductDto, prodId) as ActionResult<ProductDTO>;

            result.Should().NotBeNull();
            result.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task PutProduct_Update_Return_BadRequest()
        {
            var prodId = 1000;

            var updatedProductDto = new ProductDTO
            {
                ProductId = 1,  
                Name = "Produto Atualizado",
                Description = "Descrição do novo produto",
                ImageUrl = "imagemfake1.jpg",
                CategoryId = 1
            };

            var result = _controller.Put(updatedProductDto, prodId);

            result.Result.Should().BeOfType<BadRequestObjectResult>()
                .Which.StatusCode.Should().Be(400);
        }


    }
}
