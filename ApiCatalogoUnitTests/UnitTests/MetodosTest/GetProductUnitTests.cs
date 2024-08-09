using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using WebAPIUdemy.Controllers;
using WebAPIUdemy.DTOs;

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
        var prodId = 2;

        var data = await _controller.Get(prodId);

        data.Result.Should().BeOfType<OkObjectResult>()
            .Which.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task GetProductById_Return_NotFound()
    {
        var prodId = 999;

        var data = await _controller.Get(prodId);

        data.Result.Should().BeOfType<NotFoundObjectResult>()
            .Which.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task GetProductById_Return_BadRequest()
    {
        var prodId = -1;

        var data = await _controller.Get(prodId);

        data.Result.Should().BeOfType<BadRequestObjectResult>()
            .Which.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task GetProducts_Return_ListOfProductDTO()
    {
        var data = await _controller.Get();

        data.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeAssignableTo<IEnumerable<ProductDTO>>()
            .And.NotBeNull();
    }

    //[Fact]
    //public async Task GetProducts_Return_BadRequestResult()
    //{
    //    var data = await _controller.Get();

    //    data.Result.Should().BeOfType<BadRequestResult>();
    //}
}
