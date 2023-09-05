using AppCore.Results.Bases;
using Business.Models;
using DataAccess.Entities;
using DataAccess.Repositories;

namespace Business.Services
{
    public interface ICityService 
	{
		List<CityModel> GetList(int countryId); // gönderilen ülke id parametresine göre şehirleri ülkelere göre filtreleyerek liste olarak dönen method tanımı
		List<CityModel> GetList(); // tüm şehirleri liste olarak dönen method tanımı
		CityModel GetItem(int id); // id üzerinden şehri dönen method tanımı
	}

	public class CityService : ICityService
	{
		private readonly ICityRepository _cityRepo;

		public CityService(ICityRepository cityRepo)
		{
			_cityRepo = cityRepo;
		}
		
		public CityModel GetItem(int id)
		{
			var city = _cityRepo.Get(c => c.Id == id);

			return ToCityModel(city);
		}

		public List<CityModel> GetList(int countryId)
		{
			return _cityRepo.OrderBy(x => x.Name).GetList(c => c.CountryId == countryId).Select(ToCityModel).ToList();
		}

		public List<CityModel> GetList()
		{
			return _cityRepo.OrderBy(x => x.Name).GetList().Select(ToCityModel).ToList();
		}

		private CityModel ToCityModel(City c) 
		{
			return new CityModel()
			{
				CountryId = c.CountryId,
				Name = c.Name,
				Guid = c.Guid,
				Id = c.Id
			};

        }
	}
}
