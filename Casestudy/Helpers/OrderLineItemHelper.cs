using Casestudy.DAL.DomainClasses;


namespace Casestudy.Helpers
{
    public class OrderLineItemHelper
    {
        public int Qty { get; set; }
        public Product? Product { get; set; } 
        public decimal SellingPrice { get; set; } 
    }
}
