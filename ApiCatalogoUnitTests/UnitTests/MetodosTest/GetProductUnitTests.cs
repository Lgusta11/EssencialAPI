using WebAPIUdemy.Controllers;

namespace ApiCatalogoUnitTests.UnitTests.MetodosTest;

public class GetProductUnitTests : IClassFixture<ProductsUnitTestController>
{
    private readonly ProductsController _controller;

    public GetProductUnitTests(ProductsUnitTestController fixture)
    {
        _controller = new ProductsController(fixture.repository);
    }

    [Fact]
    public async Task GetProductById_Return_OkResult()
    {

    }

    [Fact]
    public async Task GetProductById_Return_NotFound()
    {
    }

    [Fact]
    public async Task GetProductById_Return_BadRequest()
    {
    }

    [Fact]
    public async Task GetProducts_Return_ListOfProductDTO()
    {
    }

    [Fact]
    public async Task GetProducts_Return_BadRequestResult()
    {
    }
}
