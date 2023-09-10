#nullable disable

using Business.Records.Bases;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Business.Models
{
    public class StoreModel : EntityBase
    {
        #region Entity'den Kopyalanan Özellikler
        [Required(ErrorMessage = "{0} is required!")]
        [StringLength(150, ErrorMessage = "{0} must be maximum {1} characters!")]
        [DisplayName("Store Name")]
        public string Name { get; set; }

        [DisplayName("Virtual")]
        public bool IsVirtual { get; set; }
        #endregion



        #region View'larda Gösterim veya Veri Girişi için Kullanacağımız Özellikler
        [DisplayName("Virtual")]
        public string VirtualDisplay { get; set; }
        #endregion
    }
}
