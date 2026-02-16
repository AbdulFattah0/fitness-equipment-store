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

    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _ctx;
        public ProductController(AppDbContext context)
        {
            _ctx = context;
        }
        [HttpGet]
        [Route("{BrandId}")]
        public async Task<ActionResult<List<Product>>> Index(int BrandId)
        {
            ProductDAO dao = new(_ctx!);
            List<Product> itemsForBrand = await dao.GetAllByBrand(BrandId);
            return itemsForBrand;
        }

    }
}
