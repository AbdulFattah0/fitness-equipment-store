using System.ComponentModel.DataAnnotations.Schema;

namespace Casestudy.DAL.DomainClasses
{
    public class OrderLineitem
    {

        public int Id { get; set; }
        public int OrderId { get; set; } 
        [ForeignKey("OrderId")]
        public virtual Order? Order { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string? ProductId { get; set; } 
        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }
        public int QtyOrdered { get; set; }
        public int QtySold { get; set; }
        public int QtyBackOrdered { get; set; }
        [Column(TypeName = "money")]
        public decimal SellingPrice { get; set; }

    }
}
