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
		// GetAllPreferencesAsync - get all available preferences
		// GetPreferenceByIdAsync - get a specific preference by their id 
		// UpdatePreferenceAsync - update preference name
		// DeletePreferenceAsync - delete a preference 
		// AddPreferenceAsync - add a new preference 
		//

		public async Task<IEnumerable<Preference>> GetAllPreferencesAsyc()
		{
			using var context = await _dbContextFactory.CreateDbContextAsync();
			return await context.Preferences.ToListAsync();
		}

		public async Task<Preference?> GetPreferenceByIdAsync(int id)
		{
			using var context = await _dbContextFactory.CreateDbContextAsync();
			return await context.Preferences.FirstOrDefaultAsync(p => p.Id == id);
		}

		public async Task<Preference?> UpdatePreferenceAsync(int id,Preference pref)
		{
			using var context = await _dbContextFactory.CreateDbContextAsync();
			var existingPreference = await context.Preferences.FirstOrDefaultAsync(p => p.Id == id);

			if (existingPreference is not null)
			{
				existingPreference.Name = pref.Name;
				await context.SaveChangesAsync();
				return existingPreference;
			}
			return null;
		}

		public async Task DeletePreferenceAsync(int id)
		{
			using var context = await _dbContextFactory.CreateDbContextAsync();
			var existingPreference = await context.Preferences.FirstOrDefaultAsync(p => p.Id == id);

			if (existingPreference is not null)
			{
				context.Preferences.Remove(existingPreference);
				await context.SaveChangesAsync();
			}
		}
		public async Task<Preference> AddPreferenceAsync(Preference preference)
		{
			using var context = await _dbContextFactory.CreateDbContextAsync();
			context.Preferences.Add(preference);
			await context.SaveChangesAsync();
			return preference;
		}



		//User methods
		// GetAllUsersAsyc - get all users from db to display on the UserList page
		// GetUserByIdAsync - get a specific user by their id
		// UpdateUserAsync - update a users data from overlay
		// DeleteUserAsync - remove the user from the db
		// AddUserAsync - add a new user with null preferencelist

		public async Task<IEnumerable<User>> GetAllUsersAsyc()
		{
			using var context = await _dbContextFactory.CreateDbContextAsync();
			return await context.Users.ToListAsync();
		}

		public async Task<User?> GetUserByIdAsync(int id)
		{
			using var context = await _dbContextFactory.CreateDbContextAsync();
			return await context.Users.Include(_ => _.PreferenceList).FirstOrDefaultAsync(u => u.Id == id);
		}

		public async Task<User?> UpdateUserAsync(User updatedUser)
		{
			using var context = await _dbContextFactory.CreateDbContextAsync();
			var existingUser = await context.Users.FirstOrDefaultAsync(p => p.Id == updatedUser.Id);

			if (existingUser is not null)
			{
				existingUser.FirstName = updatedUser.FirstName;
				existingUser.LastName = updatedUser.LastName;
				existingUser.Address= updatedUser.Address;
				existingUser.Email = updatedUser.Email;
				existingUser.Phone = updatedUser.Phone;

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
		// GetAllUserPreferencesAsyc - get all one to many records from user preferences
		// AddNewUPUserAsync - add a new user from the user preference page
		// DeleteUserPreferenceByIdAsync - delete a specific preference from a user
		// UpdateUserPreferenceAsync - update a preference name for a spcific user preference id
		// AddPreferenceUserPreferences - add a preference to user in user preferences

		public async Task<ICollection<User>> GetAllUserPreferencesAsyc()
		{
			using var context = await _dbContextFactory.CreateDbContextAsync();
			return await context.Users.Include(_ => _.PreferenceList).ToListAsync();
		}
		public async Task<User> AddNewUPUserAsync(User user, List<string> prefs)
		{
			using var context = await _dbContextFactory.CreateDbContextAsync();
			//Load new user info into user table
			context.Users.Add(user);
			await context.SaveChangesAsync();
			if ((prefs == null) || (prefs.Count == 0))  // user is being added from users page, set PreferenceName to Null
			{                                           // or user was added from UserPreferences page and no preferences selected
				var newUserPref = new UserPreference();
				newUserPref.PreferenceName = null;
				newUserPref.UserId = user.Id;
				context.UserPreferences.Add(newUserPref);
				await context.SaveChangesAsync();
				return user;
			}
			else  // new user was added from UserPreferences page and selected preferences were passed
			{
				for (int i = 0; i < prefs.Count; i++)
				{
					var newUserPref = new UserPreference();
					newUserPref.PreferenceName = prefs[i].ToString();
					newUserPref.UserId = user.Id;
					context.UserPreferences.Add(newUserPref);
					await context.SaveChangesAsync();
				}
				return user;
			}
		}
		public async Task<UserPreference> DeleteUserPreferenceByIdAsync(int id)
		{
			using var context = await _dbContextFactory.CreateDbContextAsync();
			var existingPreference = await context.UserPreferences.FirstOrDefaultAsync(p => p.Id == id);

			if (existingPreference is not null)
			{
				context.UserPreferences.Remove(existingPreference);
				await context.SaveChangesAsync();
			}
			return null;
		}
		public async Task<UserPreference?> UpdateUserPreferenceAsync(UserPreference updatedUserPref)
		{
			using var context = await _dbContextFactory.CreateDbContextAsync();
			var existingUser = await context.UserPreferences.FirstOrDefaultAsync(p => p.Id == updatedUserPref.UserId);

			if (existingUser is not null)
			{
				existingUser.PreferenceName = updatedUserPref.PreferenceName;
				existingUser.Id = updatedUserPref.UserId;

				await context.SaveChangesAsync();
				return existingUser;
			}
			return null;
		}

		public async Task<UserPreference> AddPreferenceUserPreferences(UserPreference newPref)
		{
			using var context = await _dbContextFactory.CreateDbContextAsync();
			context.UserPreferences.Add(newPref);
			await context.SaveChangesAsync();
			return null;
		}
	}
}
