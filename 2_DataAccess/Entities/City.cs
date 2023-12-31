﻿#nullable disable

using Business.Records.Bases;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities
{
	public class City : EntityBase
	{
		[Required]
		[StringLength(150)]
		public string Name { get; set; } // zorunlu

		public int CountryId { get; set; } // zorunlu, 1 to many ilişki, 1 şehrin mutlaka 1 ülkesi olmalı

		public Country Country { get; set; }

		public List<UserDetail> UserDetails { get; set; } // 1 şehrin 0 veya daha çok kullanıcı detayı (1'e 1 ilişki olduğu için kullanıcısı) olabilir
	}
}
