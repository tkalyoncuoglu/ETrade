using AppCore.Results;
using AppCore.Results.Bases;
using Business.Models;
using DataAccess.Entities;
using DataAccess.Repositories;
using System.Linq.Expressions;

namespace Business.Services
{
    public interface IUserService  // hem kullanıcılar için login ve register işlemlerinin AccountService üzerinden hem de
														// adminlerin kullanıcı listeleme, detay görüntüleme, ekleme, güncelleme ve silme (CRUD)
														// işlemlerini yapacağı UserService için bu servisi IService'ten implemente ediyoruz
	{
        Result Add(UserModel model); // Create işlemleri

		UserModel? Get(Expression<Func<User, bool>> expression);
    }

    public class UserService : IUserService
	{
		private readonly IUserRepository _userRepo; // kullanıcı servisi enjekte edilen kullanıcı repository'si üzerinden veritabanında işlemleri gerçekleştirecek

		public UserService(IUserRepository userRepo)
		{
			_userRepo = userRepo;
		}

		public Result Add(UserModel model)
		{
			if (_userRepo.Get(u => u.UserName == model.UserName) is not null) 
				return new ErrorResult("User can't be added because user with the same user name exists!");

			var entity = new User()
			{
				IsActive = model.IsActive,

				UserName = model.UserName, // kullanıcı adı hassas veri olduğundan trim'lemiyoruz yani model üzerinden nasıl geliyorsa onu kullanıyoruz

				Password = model.Password, // şifre hassas veri olduğundan trim'lemiyoruz yani model üzerinden nasıl geliyorsa onu kullanıyoruz,
										   // eğer istenirse şifre verisini açık olarak veritabanına kaydetmek yerine hash'leyerek şifreli olarak kaydedebiliriz

				RoleId = model.RoleId,

				UserDetail = new UserDetail() // entity içerisindeki ilişkili UserDetail entity'sini de new'leyip özelliklerini model'deki
											  // UserDetail özelliğine göre dolduruyoruz
				{
					Address = model.UserDetail.Address.Trim(),
					CityId = model.UserDetail.CityId.Value, // UserDetailModel'de Required olduğu için Value kullanabiliriz
					CountryId = model.UserDetail.CountryId.Value, // UserDetailModel'de Required olduğu için Value kullanabiliriz
					Email = model.UserDetail.Email.Trim(),
					Phone = model.UserDetail.Phone?.Trim(), // UserDetailModel'da zorunlu olmadığı yani null gelebileceği için ? kullandık
					Sex = model.UserDetail.Sex
				}
			};

			_userRepo.Add(entity);
			return new SuccessResult("User added successfully.");
		}

        public UserModel? Get(Expression<Func<User, bool>> expression)
		{
			var u = _userRepo.Include(new List<string> { "UserDetail.Country", "UserDetail.City", "Role" }).Get(expression);

			if(u is null)
			{
				return null;
			}
			return new UserModel()
			{
				Guid = u.Guid,
				Id = u.Id,
				IsActive = u.IsActive,
				Password = u.Password,
				RoleId = u.RoleId,
				UserName = u.UserName,
				Role = new RoleModel() // user entity'sindeki ilişkili Role entity'si üzerinden RoleModel tipindeki referans özelliğimizin
									   // Name özelliğini new'leyerek atıyoruz
				{
					Name = u.Role.Name
				},
				UserDetail = new UserDetailModel() // user entity'sindeki ilişkili UserDetail entity'si üzerinden UserDetailModel tipindeki
												   // referans özelliğimizi new'leyerek içerisindeki özellikleri dolduruyoruz
				{
					Address = u.UserDetail.Address,
					CityId = u.UserDetail.CityId,
					CountryId = u.UserDetail.CountryId,
					Email = u.UserDetail.Email,
					Phone = u.UserDetail.Phone,
					Sex = u.UserDetail.Sex,
					Country = new CountryModel() // user entity'sindeki ilişkili UserDetail entity'si içerisindeki Country referans özelliğini
												 // new'leyerek Name özelliğini atıyoruz
					{
						Name = u.UserDetail.Country.Name
					},
					City = new CityModel() // user entity'sindeki ilişkili UserDetail entity'si içerisindeki City referans özelliğini
										   // new'leyerek Name özelliğini atıyoruz
					{
						Name = u.UserDetail.City.Name
					}
				}
			};
		}

		
	}
}
