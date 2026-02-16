using Casestudy.DAL;
using Casestudy.DAL.DomainClasses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Casestudy.DAL.DAO;
using Microsoft.AspNetCore.Authorization;

namespace Casestudy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class BrandController : ControllerBase
    {

        private readonly AppDbContext _ctx;

        public BrandController(AppDbContext context)
        {
            _ctx = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Brand>>> Index()
        {
            BrandDAO dao = new(_ctx!);
            List<Brand> allCategories = await dao.GetAll();
            return allCategories;
        }

    }
}
