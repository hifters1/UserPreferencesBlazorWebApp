using System.ComponentModel.DataAnnotations;

namespace UserPreferencesBlazorWebApp.Models
{
	public class Preference
	{
		[Key]
		public int Id { get; set; }
		[Required]
		[RegularExpression(@"^[A-Z]+[a-zA-Z\s]*$"), StringLength(30)]
		public string Name { get; set; }
	}
}
