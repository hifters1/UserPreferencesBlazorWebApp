using Microsoft.EntityFrameworkCore;
using UserPreferencesBlazorWebApp.Models;

namespace UserPreferencesBlazorWebApp.Data
{
	public class AppDbContext : DbContext
	{
		public DbSet<Preference> Preferences { get; set; }
		public DbSet<User> Users { get; set; }
		public DbSet<UserPreference> UserPreferences { get; set; }

		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<UserPreference>(entity =>
			{
				entity.HasOne(e => e.User)
				.WithMany(e => e.PreferenceList)
				.HasForeignKey(e => e.UserId);
			});
		}
	}
}
