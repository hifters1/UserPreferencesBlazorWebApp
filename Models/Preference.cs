using System.ComponentModel.DataAnnotations;

namespace UserPreferencesBlazorWebApp.Models
{
	public class Preference
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public string Name { get; set; }
	}
}
