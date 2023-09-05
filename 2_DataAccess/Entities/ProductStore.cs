#nullable disable

using AppCore.Records.Bases;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities
{
    public class ProductStore : EntityBase
    {
        public int ProductId { get; set; } // Ürün entity'sinden ürün id'yi taşıyoruz ve mağaza id ile birlikte primary key olarak ilk sırada set ediyoruz

        public Product Product { get; set; }

        public int StoreId { get; set; } // Mağaza entity'sinden mağaza id'yi taşıyoruz ve ürün id ile birlikte primary key olarak ikinci sırada set ediyoruz

        public Store Store { get; set; }
    }
}
