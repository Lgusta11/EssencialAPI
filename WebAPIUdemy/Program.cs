using Microsoft.EntityFrameworkCore;
using WebAPIUdemy.Context;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string mysqlConnection = builder.Configuration.GetConnectionString("DefaultConnection")!;

builder.Services.AddDbContext<CatalogoContext>(options =>
options.UseMySql(mysqlConnection, ServerVersion.AutoDetect(mysqlConnection)));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
