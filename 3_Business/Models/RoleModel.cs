#nullable disable

using AppCore.Records.Bases;
using System.ComponentModel;

namespace Business.Models
{
	public class RoleModel : EntityBase
	{
		#region Entity'den Kopyalanan Özellikler
		[DisplayName("Role")]
		public string Name{ get; set; }
		#endregion
	}
}
