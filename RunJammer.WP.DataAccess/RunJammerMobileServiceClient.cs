
using System.Diagnostics;
using Common.Model.Logging;
using Microsoft.WindowsAzure.MobileServices;
using RunJammer.WP.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunJammer.WP.Model;
using RunJammer.WP.Model.Implementation;

namespace RunJammer.WP.DataAccess
{
    public class RunJammerMobileServiceClient
    {
        private MobileServiceClient _mobileServiceClient = new MobileServiceClient("https://runjammer.azure-mobile.net/", "nHfHZmPuLpLQeJPiUhrtdOEQqxrJEV95");
        private ILogger _logger;

        public async Task<MobileServiceUser> LoginAsync(MobileServiceAuthenticationProvider authenticationProvider)
        {
            try
            {
                return await _mobileServiceClient.LoginAsync(authenticationProvider);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task CreateRunJammerSongsAsync(IEnumerable<RunJammerSong> runJammerSongs)
        {
            foreach (var runJammerSong in runJammerSongs)
            {
                try
                {
                    await _mobileServiceClient.GetTable<RunJammerSong>().InsertAsync(runJammerSong);
                }
                catch (Exception ex)
                {
                    
                }
            }
        }

        public async Task CreateRunJammerSongAsync(RunJammerSong runJammerSong)
        {
            await _mobileServiceClient.GetTable<RunJammerSong>().InsertAsync(runJammerSong);
        }

        public async Task CreateRunJammerUser(RunJammerUser runJammerUser)
        {
            try
            {
                await _mobileServiceClient.GetTable<RunJammerUser>().InsertAsync(runJammerUser);
            }
            catch (Exception ex)
            {
                _logger.Log(new Exception("Error creating user.", ex));
            }
        }

        public async void CreateRunSession(RunSession runSession)
        {
            await _mobileServiceClient.GetTable<RunSession>().InsertAsync(runSession);
        }

        public async Task UpdateAsync<T>(T item)
        {
            await _mobileServiceClient.GetTable<T>().UpdateAsync(item);
        }

        public async void CreateUserSongRating(UserSongRating userSongRating)
        {
            await _mobileServiceClient.GetTable<UserSongRating>().InsertAsync(userSongRating);
        }

        public RunJammerMobileServiceClient(ILogger logger)
        {
            _logger = logger;
        }
    }
}
