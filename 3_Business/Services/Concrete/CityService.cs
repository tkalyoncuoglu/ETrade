using Business.Models;
using DataAccess.Entities;
using Repositories.Abstract;
using Services.Abstract;

namespace Services.Concrete
{
    public class CityService : ICityService
    {
        private readonly ICityRepository _cityRepo;

        public CityService(ICityRepository cityRepo)
        {
            _cityRepo = cityRepo;
        }

        public CityModel Get(int id)
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
                Id = c.Id
            };

        }
    }
}
