using Casestudy.DAL;
using Casestudy.DAL.DAO;
using Casestudy.DAL.DomainClasses;
using Casestudy.Helpers;
using Castle.Components.DictionaryAdapter.Xml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;



namespace Casestudy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        readonly AppDbContext? _ctx;
        public OrderController(AppDbContext context) 
        {
            _ctx = context;
        }

        [HttpPost]
        [Produces("application/json")]
        public async Task<ActionResult<string>> AddOrder([FromBody] OrderHelper helper)
        {
            string retVal;

            try
            {
                var customer = await _ctx!.Customers!
                    .FirstOrDefaultAsync(c => c.Email == helper.Email);

                if (customer == null)
                {
                    return BadRequest("Customer not found.");
                }

                OrderDAO dao = new(_ctx);
                var (orderId, message) = await dao.AddOrder(customer.Id, helper.Products!);

                retVal = orderId > 0 ? message : "Order not saved.";
            }
            catch (DbUpdateException dbEx)
            {

                var innerMsg = dbEx.InnerException?.Message ?? dbEx.Message;
                retVal = "Order not saved. DB error: " + innerMsg;
            }
            catch (Exception ex)
            {
                retVal = "Order not saved. " + ex.Message;
            }

            return Ok(retVal);
        }


        [Route("{email}")]
        [HttpGet]
        public async Task<ActionResult<List<Order>>> List(string email)
        {
            List<Order> trays;
            CustomerDAO uDao = new(_ctx!);
            Customer? orderOwner = await uDao.GetByEmail(email);
            OrderDAO tDao = new(_ctx!);
            trays = await tDao.GetAll(orderOwner!.Id);
            return trays;
        }

        [Route("{orderid}/{email}")]
        [HttpGet]
        public async Task<ActionResult<List<OrderDetailsHelper>>> GetOrderDetails(int orderid, string email)
        {
            OrderDAO dao = new(_ctx!);
            return await dao.GetOrderDetails(orderid, email);
        }

    }
}
