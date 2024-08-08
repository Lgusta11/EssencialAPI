using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPIUdemy.Controllers;

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
        }

        [Fact]
        public async Task PostProduct_Return_BadRequest()
        {
        }

    }
}
