using Casestudy.DAL;
using Casestudy.DAL.DomainClasses;
using Casestudy.Helpers;
using Castle.Components.DictionaryAdapter.Xml;
using Microsoft.EntityFrameworkCore;

namespace Casestudy.DAL.DAO
{
    public class OrderDAO
    {
        private readonly AppDbContext _db;

        public OrderDAO(AppDbContext ctx)
        {
            _db = ctx;
        }

        public async Task<(int OrderId, string Message)> AddOrder(int customerId, OrderLineItemHelper[] products)
        {
            int orderId = -1;
            string message = "";

            using var trans = await _db.Database.BeginTransactionAsync();

            try
            {
                Order order = new()
                {
                    CustomerId = customerId,
                    OrderDate = DateTime.Now,
                    OrderAmount = products.Sum(p => p.SellingPrice * p.Qty)
                };

                await _db.Orders!.AddAsync(order);
                await _db.SaveChangesAsync();

                foreach (var productHelper in products)
                {
                    var product = await _db.Products!
                        .FirstOrDefaultAsync(p => p.Id == productHelper.Product!.Id);

                    if (product == null)
                    {
                        throw new Exception($"Product {productHelper.Product.Id} not found.");
                    }

                    int qtyOrdered = productHelper.Qty;
                    int qtySold = 0;
                    int qtyBackOrdered = 0;

                    if (qtyOrdered <= product.QtyOnHand)
                    {
                        
                        qtySold = qtyOrdered;
                        qtyBackOrdered = 0;
                        product.QtyOnHand -= qtyOrdered;
                    }
                    else
                    {
                        
                        qtySold = product.QtyOnHand;
                        qtyBackOrdered = qtyOrdered - product.QtyOnHand;

                        product.QtyOnBackOrder += qtyBackOrdered;
                        product.QtyOnHand = 0;

                        message += $"Not enough stock for product {product.Id}. {qtySold} sold, {qtyBackOrdered} backordered.\n";
                    }

                    OrderLineitem lineItem = new()
                    {
                        OrderId = order.Id,
                        ProductId = product.Id,
                        QtyOrdered = qtyOrdered,
                        QtySold = qtySold,
                        QtyBackOrdered = qtyBackOrdered,
                        SellingPrice = productHelper.SellingPrice
                    };

                    await _db.OrderLineitems!.AddAsync(lineItem);
                }

                await _db.SaveChangesAsync();
                await trans.CommitAsync();
                orderId = order.Id;

                if (string.IsNullOrWhiteSpace(message))
                {
                    message = $"Order {orderId} saved successfully.";
                }
                else
                {
                    message = $"Order {orderId} saved with warnings:\n" + message;
                }
            }
            catch (Exception ex)
            {
                await trans.RollbackAsync();
                message = $"Order not saved. Error: {ex.Message}";
            }

            return (orderId, message);
        }

        public async Task<List<Order>> GetAll(int customerId)
        {
            return await _db.Orders!
                .Where(order => order.CustomerId == customerId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }


        public async Task<List<OrderDetailsHelper>> GetOrderDetails(int orderId, string email)
        {
            List<OrderDetailsHelper> details = new();

            var customer = await _db.Customers!.FirstOrDefaultAsync(c => c.Email == email);
            if (customer == null) return details;

            var results = from order in _db.Orders
                          join item in _db.OrderLineitems on order.Id equals item.OrderId
                          join product in _db.Products on item.ProductId equals product.Id
                          where order.CustomerId == customer.Id && order.Id == orderId
                          select new OrderDetailsHelper
                          {
                              OrderId = order.Id,
                              CustomerId = customer.Id,
                              ProductId = product.Id,
                              ProductName = product.ProductName,
                              QtyOrdered = item.QtyOrdered,
                              QtySold = item.QtySold,
                              QtyBackOrdered = item.QtyBackOrdered,
                              SellingPrice = item.SellingPrice,
                              OrderDate = order.OrderDate.ToString("yyyy/MM/dd")
                          };

            details = await results.ToListAsync();
            return details;
        }

    }
}
