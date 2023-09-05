using AppCore.Business.Services.Bases;
using AppCore.DataAccess.EntityFramework.Bases;
using AppCore.Results.Bases;
using Business.Models;
using DataAccess.Entities;
using DataAccess.Repositories;

namespace Business.Services
{
    public interface ICountryService 
	{
		List<CountryModel> GetList(); 
		CountryModel GetItem(int id); // id parametresine göre ülkeyi dönen method tanımı
    }

    public class CountryService : ICountryService
	{
		private readonly ICountryRepository _countryRepository;

		public CountryService(ICountryRepository countryRepo)
		{
			_countryRepository = countryRepo;
		}


		public List<CountryModel> GetList()
		{
			return _countryRepository.OrderBy(x => x.Name).GetList().Select(ToCountryModel).ToList();
		}

		public CountryModel GetItem(int id) 
		{
			var country = _countryRepository.Get(c => c.Id == id);

			if(country == null) 
			{
				return null;
			}

			return ToCountryModel(country);
		}
		
		private CountryModel ToCountryModel(Country c) 
		{
			return new CountryModel
			{
				Name = c.Name,
				Guid = c.Guid,
				Id = c.Id
			};

        }
		
	}
}
