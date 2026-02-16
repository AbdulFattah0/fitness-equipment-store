using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Casestudy.DAL.DomainClasses;
using Casestudy.DAL;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace Casestudy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class DataController : ControllerBase
    {
        private readonly AppDbContext _ctx;

        public DataController(AppDbContext context)
        {
            _ctx = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {

                var brands = await (_ctx.Brands?.ToListAsync() ?? Task.FromResult(new List<Brand>()));
                var products = await (_ctx.Products?.ToListAsync() ?? Task.FromResult(new List<Product>()));

                Console.WriteLine("Brands:");
                Console.WriteLine(JsonSerializer.Serialize(brands));
                Console.WriteLine("Products:");
                Console.WriteLine(JsonSerializer.Serialize(products));


                return Ok(new { message = "Tables loaded" });

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }
    }
}
