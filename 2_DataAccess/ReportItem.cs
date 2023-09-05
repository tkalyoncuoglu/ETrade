using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class ReportItem // bu modeli sadece rapor için kullanacağımızdan ve üzerinden CRUD işlemleri yapmayacağımızdan
                                 // RecordBase'den miras almamıza gerek yok
    {
        #region Rapor view'ında gösterim için kullanacağımız özellikler
        public string ProductName { get; set; }

        // view'da ProductDescription'ı HTML title attribute'u üzerinden göstereceğimizden DisplayName'e gerek yok
        public string ProductDescription { get; set; }

        public string UnitPrice { get; set; }

        public string StockAmount { get; set; }

        public string ExpirationDate { get; set; }

        public string CategoryName { get; set; }

        // view'da CategoryDescription'ı HTML title attribute'u üzerinden göstereceğimizden DisplayName'e gerek yok
        public string CategoryDescription { get; set; }

        public string StoreName { get; set; }
        #endregion

        #region Rapor view'ında sorgu üzerinden filtreleme için kullanacağımız özellikler
        public int? CategoryId { get; set; } // tüm kategoriler için null geleceğinden int? tipini kullandık

        public int? StoreId { get; set; } // tüm mağazalar için null geleceğinden int? tipini kullandık

        public double UnitPriceValue { get; set; }

        public int StockAmountValue { get; set; }

        public DateTime? ExpirationDateValue { get; set; }
        #endregion
    }
}
