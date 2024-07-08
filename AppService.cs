using Microsoft.EntityFrameworkCore;
using UserPreferencesBlazorWebApp.Data;
using UserPreferencesBlazorWebApp.Models;


namespace UserPreferencesBlazorWebApp
{
	public class AppService
	{
		private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

		public AppService(IDbContextFactory<AppDbContext> dbContextFactory)
		{
			_dbContextFactory = dbContextFactory;
		}

		//Preference methods
		//GetAllPreferences
		//
		public async Task<IQueryable<User>> GetAllUserPreferencesAsyc()
		{
			using var context = await _dbContextFactory.CreateDbContextAsync();
			return (IQueryable<User>)await context.Users.Include(_ => _.PreferenceList).ToListAsync();
		}


		//User methods
		// GetAllUsersAsyc - get all users from db to display on the UserList page
		// GetUserByIdAsync - get a specific user by their id
		// UpdateUserAsync - update a users data from overlay
		// DeleteUserAsync - remove the user from the db

		public async Task<IEnumerable<User>> GetAllUsersAsyc()
		{
			using var context = await _dbContextFactory.CreateDbContextAsync();
			return await context.Users.ToListAsync();
		}

		public async Task<User?> GetUserByIdAsync(int id)
		{
			using var context = await _dbContextFactory.CreateDbContextAsync();
			return await context.Users.FirstOrDefaultAsync(u => u.Id == id);
		}

		public async Task<User?> UpdateUserAsync(User updatedUser)
		{
			using var context = await _dbContextFactory.CreateDbContextAsync();
			var existingUser = await context.Users.FirstOrDefaultAsync(p => p.Id == updatedUser.Id);

			if (existingUser is not null)
			{
				existingUser.FirstName = updatedUser.FirstName;
				await context.SaveChangesAsync();
				return existingUser;
			}
			return null;
		}

		public async Task DeleteUserAsync(int id)
		{
			using var context = await _dbContextFactory.CreateDbContextAsync();
			var existingUser = await context.Users.FirstOrDefaultAsync(p => p.Id == id);

			if (existingUser is not null)
			{
				context.Users.Remove(existingUser);
				await context.SaveChangesAsync();
			}
		}

		public async Task<User> AddUserAsync(User user)
		{
			//Load new user info into user table
			using var context = await _dbContextFactory.CreateDbContextAsync();
			context.Users.Add(user);
			await context.SaveChangesAsync();

			//Update UserPreference table with the just loaded user id
			var newUserPref = new UserPreference();
			newUserPref.PreferenceName = null;
			newUserPref.UserId = user.Id;
			context.UserPreferences.Add(newUserPref);
			await context.SaveChangesAsync();
			return user;
		}

		//UserPreference methods
		//
		//
	}
}
