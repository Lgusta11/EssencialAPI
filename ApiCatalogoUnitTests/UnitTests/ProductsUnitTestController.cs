using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using WebAPIUdemy.Context;
using WebAPIUdemy.DTOs;
using WebAPIUdemy.Models;
using WebAPIUdemy.Repositories;

namespace ApiCatalogoUnitTests.UnitTests;

public class ProductsUnitTestController
{
    public IUnitOfWork repository;
    public static DbContextOptions<CatalogoContext> dbContextOptions { get; }

    public static string connectionString;

    static ProductsUnitTestController()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddUserSecrets<ProductsUnitTestController>()
            .Build();

        connectionString = configuration.GetConnectionString("DefaultConnection")!;

        dbContextOptions = new DbContextOptionsBuilder<CatalogoContext>()
            .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
            .Options;
    }

    public ProductsUnitTestController()
    {
        var context = new CatalogoContext(dbContextOptions);

        var userStore = new Mock<IUserStore<ApplicationUser>>();
        var userManager = new Mock<UserManager<ApplicationUser>>(userStore.Object, null, null, null, null, null, null, null, null);

        var roleStore = new Mock<IRoleStore<IdentityRole>>();
        var roleManager = new Mock<RoleManager<IdentityRole>>(roleStore.Object, null, null, null, null);

        repository = new UnitOfWork(context, userManager.Object, roleManager.Object);
    }

}
