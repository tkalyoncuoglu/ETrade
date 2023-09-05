using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class ReportFilter // view'da filtreleme kısmında kullanıcıdan alacağımız input'lar
    {
        public int? CategoryId { get; set; } // view'daki tüm kategoriler işlemi için null geleceğinden nullable tanımladık
        public string ProductName { get; set; }
        public double? UnitPriceBegin { get; set; }
        public double? UnitPriceEnd { get; set; }
        public int? StockAmountBegin { get; set; }
        public int? StockAmountEnd { get; set; }
        public DateTime? ExpirationDateBegin { get; set; }
        public DateTime? ExpirationDateEnd { get; set; }
        public List<int> StoreIds { get; set; }
    }
}
