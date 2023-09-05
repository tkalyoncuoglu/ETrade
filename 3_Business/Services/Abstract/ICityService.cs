using Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstract
{
    public interface ICityService
    {
        List<CityModel> GetList(int countryId); // gönderilen ülke id parametresine göre şehirleri ülkelere göre filtreleyerek liste olarak dönen method tanımı
        List<CityModel> GetList(); // tüm şehirleri liste olarak dönen method tanımı
        CityModel Get(int id); // id üzerinden şehri dönen method tanımı
    }

}
