using Castle.Components.DictionaryAdapter.Xml;
using Castle.Core.Resource;
using System.ComponentModel.DataAnnotations.Schema;

namespace Casestudy.DAL.DomainClasses
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }

        [Column(TypeName = "money")]
        public decimal OrderAmount { get; set; }
        public int CustomerId { get; set; } 

        [ForeignKey("CustomerId")]
        public virtual Customer? Customer { get; set; }

    }
}


