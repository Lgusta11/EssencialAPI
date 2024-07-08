﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIUdemy.Context;
using WebAPIUdemy.Model;

namespace WebAPIUdemy.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly CatalogoContext? _context;

        public CategoriesController(CatalogoContext? context)
        {
            _context = context;
        }

       
        [HttpGet("produtos")]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategoriasProdutos()
        {
             return await _context!.Categories.Include(p=> p.Products).Where(c=> c.CategoryId <= 5).ToListAsync();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> Get()
        {
            try
            {
                var category = await _context!.Categories.AsNoTracking().ToListAsync();
                if (category is null)
                {
                    return NotFound("Nenhuma categoria cadastrada!");
                }
                return Ok(category);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu umproblema ao tratar a sua solicitação.");
            }
        }

        [HttpGet("{id:int:min(1)}", Name = "ObterCategoria")]
        public async Task<ActionResult<Category>> Get(int id)
        {
            try
            {
                var category = await _context!.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.CategoryId == id);
                if (category is null)
                {
                    return NotFound($"Categoria com id={id} não encontrada");
                }
                return Ok(category);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao tratar a sua solicitação.");
            }
        }



    

    [HttpPost]
        public ActionResult Post(Category category)
        {
            if (category is null)
            {
                return BadRequest();
            }
            _context!.Categories.Add(category);
            _context.SaveChanges();

            return new CreatedAtRouteResult("ObterCategoria",
                new { id = category.CategoryId }, category);
        }

        [HttpPut("{id:int:min(1)}")]
        public ActionResult Put(Category category, int id)
        {
            if (id != category.CategoryId)
            {
                return BadRequest("Informe um id valido");
            }

            _context!.Entry(category).State = EntityState.Modified;
            _context!.SaveChanges();

            return Ok(category);
        }

        [HttpDelete("{id:int:min(1)}")]
        public ActionResult Delete(int id)
        {
            var category = _context!.Categories.FirstOrDefault(c => c.CategoryId == id);

            if (category is null)
            {
                return NotFound("Produto não localizado");
            }

            _context!.Categories.Remove(category);
            _context!.SaveChanges();

            return Ok(category);
        }
    }
}
