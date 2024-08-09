using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPIUdemy.Controllers;

namespace ApiCatalogoUnitTests.UnitTests.MetodosTest
{
    public class DeleteProductUnitTests : IClassFixture<ProductsUnitTestController>
    {
        private readonly ProductsController _controller;

        public DeleteProductUnitTests(ProductsUnitTestController fixture)
        {
            _controller = new ProductsController(fixture.repository);
        }

        [Fact]
        public async Task DeleteProductById_Return_OkResult()
        {
            var prodId = 2;

            var result = await _controller.Delete(prodId);

            result.Should().NotBeNull();
            result.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task DeleteProductById_Return_NotFound()
        {
            var prodId = 999;

            var result = await _controller.Delete(prodId);

            result.Should().NotBeNull();
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }
    }
}
