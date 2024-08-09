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
    public class PostProductUnitTest : IClassFixture<ProductsUnitTestController>
    {
        private readonly ProductsController _controller;

        public PostProductUnitTest(ProductsUnitTestController fixture)
        {
            _controller = new ProductsController(fixture.repository);
        }



        [Fact]
        public async Task PostProduct_Return_CreatedStatusCode()
        {
            var newProductDto = new ProductDTO
            {
                Name = "Novo Produto",
                Description = "Descrição do novo produto",
                Price = 10.99m,
                ImageUrl = "imagemfake1.jpg",
                CategoryId = 1
            };

            var data =  _controller.Post(newProductDto);

            var createdResult = data.Result.Should().BeOfType<CreatedAtRouteResult>();
            createdResult.Subject.StatusCode.Should().Be(201);
        }

        //[Fact]
        //public async Task PutProduct_Update_Return_BadRequest()
        //{
        //    var prodId = 1000;

        //    var updatedProductDto = new ProductDTO
        //    {
        //        ProductId = 1,  // Isso deve causar o BadRequest porque o id não coincide
        //        Name = "Produto Atualizado",
        //        Description = "Descrição do novo produto",
        //        ImageUrl = "imagemfake1.jpg",
        //        CategoryId = 1
        //    };

        //    var result = _controller.Put(updatedProductDto, prodId);

        //    // Verifica se o resultado é um BadRequestObjectResult
        //    result.Result.Should().BeOfType<BadRequestObjectResult>()
        //        .Which.StatusCode.Should().Be(400);
        //}


    }
}
