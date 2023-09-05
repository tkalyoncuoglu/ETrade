using Microsoft.AspNetCore.Mvc;
using Services.Abstract;

namespace MvcWebUI.Controllers
{
    [Route("[controller]")] // bu route tanımı bu controller'a controller adı üzerinden ~/Cities olarak ulaşmamızı sağlar
    public class CitiesController : Controller
	{
		private readonly ICityService _cityService;

		public CitiesController(ICityService cityService)
		{
			_cityService = cityService;
		}

		[Route("[action]/{countryId?}")] // bu route tanımı bu action'a bu action adı üzerinden örneğin ~/Cities/GetCities/1 şeklinde ulaşmamızı sağlar,
									     // eğer burada olduğu gibi Program.cs'teki controller/action/id? route tanımı
									     // ihtiyacımızı karşılamıyorsa controller ve action üzerinde Route attribute'u ile
									     // kendi route tanımımızı oluşturup action çağrımı için oluşturduğumuz route tanımını kullanabiliriz,
		public IActionResult GetCities(int? countryId) // gönderilen ülke id parametresine göre o ülke id'ye sahip şehirleri dönmek için,
												       // Account -> Controllers -> Users -> Register action'ının view'ında
													   // AJAX (Asynchronous Javascript and XML) isteği üzerinden view'daki
													   // şehir drop down list'ini ülke drop down list'i üzerinden seçilen ülke id'ye göre
													   // doldurmak için kullanacağız, parametrenin ? olarak gönderilmesi önemlidir
		{
			if (!countryId.HasValue)
				return NotFound();
			var cities = _cityService.GetList(countryId.Value);
			return Ok(cities); 
		}
	}
}
