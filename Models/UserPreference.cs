using System.ComponentModel.DataAnnotations.Schema;

namespace UserPreferencesBlazorWebApp.Models
{
	public class UserPreference
	{
		public int Id { get; set; }

		public string? PreferenceName { get; set; }
		[ForeignKey(nameof(UserId))]
		public int UserId { get; set; }
		public User? User { get; set; }
	}
}
