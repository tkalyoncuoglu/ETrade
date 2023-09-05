#nullable disable

using AppCore.Records.Bases;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities
{
    public class Store : EntityBase // Mağaza
    {
        [Required]
        [StringLength(150)]
        [DisplayName("Store Name")]
        public string Name { get; set; }

        [DisplayName("Virtual")]
        public bool IsVirtual { get; set; }

        public List<ProductStore> ProductStores { get; set; } // many to many ilişki için ürün mağaza kolleksiyon referansı
    }
}
