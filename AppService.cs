﻿using Microsoft.EntityFrameworkCore;
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
		// GetAllPreferencesAsync - 
		// GetPreferenceByIdAs - 
		// UpdatePreferenceAsync - 
		// DeletePreferenceA - 
		// AddPreferenceAsyns - 
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

		public async Task<Preference?> UpdatePreferenceAsync(Preference updatedPreference)
		{
			using var context = await _dbContextFactory.CreateDbContextAsync();
			var existingPreference = await context.Preferences.FirstOrDefaultAsync(p => p.Id == updatedPreference.Id);

			if (existingPreference is not null)
			{
				existingPreference.Name = updatedPreference.ToString();
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
		// GetAllUserPreferencesAsyc - 
		// AddUserAsync - 
		// DeleteUserPreferenceAsync - 
		// DeleteNullUserPreferenceAsync - 
		//

		public async Task<ICollection<User>> GetAllUserPreferencesAsyc()
		{
			using var context = await _dbContextFactory.CreateDbContextAsync();
			return await context.Users.Include(_ => _.PreferenceList).ToListAsync();
		}
		public async Task<User> AddUserAsync(User user, List<string> prefs)
		{
			using var context = await _dbContextFactory.CreateDbContextAsync();

			//Load new user info into user table
			if (user.Id == 0)  //this is a new user, skip for an existing user
			{
				context.Users.Add(user);
				await context.SaveChangesAsync();
			}

			//Update UserPreference table with the just loaded user id
			//Check null prefs
			if (prefs == null)
			{
				var newUserPref = new UserPreference();
				newUserPref.PreferenceName = null;
				newUserPref.UserId = user.Id;
				context.UserPreferences.Add(newUserPref);
				await context.SaveChangesAsync();
				return user;
			}
			else
			{
				for (int i = 0; i < prefs.Count; i++)
				{
					var newUserPref = new UserPreference();
					newUserPref.PreferenceName = prefs[i].ToString();
					newUserPref.UserId = user.Id;
					if (((user.PreferenceList.Count == 1) && (user.PreferenceList[0].PreferenceName == null) && (i == 0))||(user.PreferenceList == null))
					{
						DeleteNullUserPreferenceAsync(user.Id);       //having to delete the null preference name record as
																	  //the update is inserting a new record leaving the null record in place
						context.UserPreferences.Update(newUserPref);
					}
					else
					{
						context.UserPreferences.Add(newUserPref);
					}
					await context.SaveChangesAsync();
				}
				return user;
			}

		}
		public async Task<UserPreference> DeleteUserPreferenceAsync(int id, List<string> pref)
		{
			using var context = await _dbContextFactory.CreateDbContextAsync();
			var existingPreference = await context.UserPreferences.FirstOrDefaultAsync(p => p.UserId == id);

			if (existingPreference is not null)
			{
				context.UserPreferences.Remove(existingPreference);
				await context.SaveChangesAsync();
			}
			return null;
		}
		public async Task<UserPreference> DeleteNullUserPreferenceAsync(int id)
		{
			using var context = await _dbContextFactory.CreateDbContextAsync();
			var existingPreference = await context.UserPreferences.FirstOrDefaultAsync(p => p.UserId == id);

			if (existingPreference is not null)
			{
				context.UserPreferences.Remove(existingPreference);
				await context.SaveChangesAsync();
			}
			return null;
		}

	}
}
