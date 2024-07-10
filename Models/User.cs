using System.ComponentModel.DataAnnotations;

namespace UserPreferencesBlazorWebApp.Models
{
	public class User
	{
		[Key]
		public int Id { get; set; }
		
		[Required]
		[StringLength(60, MinimumLength = 3)]
		public string FirstName { get; set; }
		[StringLength(60, MinimumLength = 3)]
		public string? LastName { get; set; }
		public string? Address { get; set; }
		public string? Phone { get; set; }
		public string? Email { get; set; }
		public List<UserPreference> PreferenceList { get; set; }
	}
}
