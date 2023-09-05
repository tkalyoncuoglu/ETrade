#nullable disable

using AppCore.Records.Bases;
using DataAccess.Enums;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities
{
	public class UserDetail : EntityBase
	{
		public int UserId { get; set; }
		public Sex Sex { get; set; }

		[Required]
		[StringLength(250)]
		public string Email { get; set; } // zorunlu

		[StringLength(25)]
		public string Phone { get; set; } // zorunlu değil

		[Required]
		[StringLength(750)]
		public string Address { get; set; } // zorunlu

		public int CountryId { get; set; } // ülke id, zorunlu

		public Country Country { get; set; } // 1 to many ilişki, 1 kullanıcı detayının (1'e 1 ilişki olduğu için kullanıcının) mutlaka 1 ülkesi olmalı

		public int CityId { get; set; } // şehir id, zorunlu

		public City City { get; set; } // 1 to many ilişki, 1 kullanıcı detayının (1'e 1 ilişki olduğu için kullanıcının) mutlaka 1 şehri olmalı
	
		public User User { get; set; }
	}
}
