﻿#nullable disable

using Business.Records.Bases;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities
{
	public class Role : EntityBase // Rol
	{
		[Required]
		[StringLength(20)]
		public string Name { get; set; }

		public List<User> Users { get; set; } // 1 to many ilişkili başka entity kolleksiyonuna referans, 1 rolün 0 veya daha çok kullanıcısı olabilir
	}
}
