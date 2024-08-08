using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPIUdemy.Controllers;

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
        }

        [Fact]
        public async Task PutProduct_Update_Return_BadRequest()
        {
        }
    
    }
}
