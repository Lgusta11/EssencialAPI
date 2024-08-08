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
        }

        [Fact]
        public async Task DeleteProductById_Return_NotFound()
        {
        }
    }
}
