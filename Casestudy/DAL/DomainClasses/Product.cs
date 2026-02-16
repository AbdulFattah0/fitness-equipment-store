using System.ComponentModel.DataAnnotations.Schema;

namespace Casestudy.DAL.DomainClasses
{
    public class Product
    {
        public string? Id { get; set; }

        // Foreign key to Brand
        public int BrandId { get; set; }

        // Navigation property (MISSING before)
        public virtual Brand? Brand { get; set; }

        public string? ProductName { get; set; }
        public string? GraphicName { get; set; }
        public decimal CostPrice { get; set; }
        public decimal MSRP { get; set; }
        public int QtyOnHand { get; set; }
        public int QtyOnBackOrder { get; set; }
        public string? Description { get; set; }

        [NotMapped]
        public string DummyFlag { get; set; } = "";

    }
}
