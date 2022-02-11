using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RunJammer.WP.Model;
using RunJammer.WP.Model.Implementation;

namespace RunJammer.WP.DataAccess.Implementation
{
    public class RunJammerApplicationDataProvider
    {
        #region Interface

        public async void CreateRunJammerSong(RunJammerSong runJammerSong)
        {
            try
            {
                if (runJammerSong.LocalID == 0)
                {
                    _localDataProvider.Create(runJammerSong);
                    SubmitChanges();
                }

                if (runJammerSong.ID == 0)
                {
                    await _mobileServiceClient.CreateRunJammerSongAsync(runJammerSong);
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        public async Task CreateRunJammerUserAsync(RunJammerUser runJammerUser)
        {
            await _mobileServiceClient.CreateRunJammerUser(runJammerUser);
            if (_localDataProvider.GetData<RunJammerUser>().All(u => u.UserID != runJammerUser.UserID))
            {
                _localDataProvider.Create(runJammerUser);
                _localDataProvider.SubmitChanges();
            }
        }

        public void DeleteRunSession(RunSession runSession)
        {
            _localDataProvider.Delete(runSession);
        }


        public void CreateRunJammerPlaylist(RunJammerPlaylist runJammerPlaylist)
        {
            _localDataProvider.Create(runJammerPlaylist);
        }

        public IEnumerable<RunJammerSong> GetRunJammerSongs()
        {
            return _localDataProvider.GetData<RunJammerSong>();
        }
        public IEnumerable<RunJammerPlaylist> GetRunJammerPlaylists()
        {
            return _localDataProvider.GetData<RunJammerPlaylist>();
        }

        public IEnumerable<RunSession> GetRunSessionHistory()
        {
            try
            {
                return _localDataProvider.GetData<RunSession>();
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting Run Session History.", ex);
            }
        }

        public void CreateRunSession(RunSession runSession, bool isUserLoggedIn = false)
        {
            if (runSession.LocalID == 0)
            {
                _localDataProvider.Create(runSession);
            }

            if (!isUserLoggedIn) return;

            if (runSession.ID == 0)
            {
                try
                {
                    _mobileServiceClient.CreateRunSession(runSession);
                }
                catch
                {
                }
            }
        }

        public void CreateUserSongRating(UserSongRating userSongRating)
        {
            _mobileServiceClient.CreateUserSongRating(userSongRating);
        }

        //public void CreateRunSession(AzureRunSession azureRunSession)
        //{
        //    if (azureRunSession.ID == 0)
        //    {
        //        _mobileServiceClient.CreateRunSession(azureRunSession);
        //    }

        //    //if (runSession.ID == 0)
        //    //{
        //    //    _mobileServiceClient.CreateRunSession(runSession);
        //    //}
        //}


        public void CreateRunSessionWaypoint(RunSessionWaypoint waypoint)
        {
            _localDataProvider.Create(waypoint);
        }


        public Task<T> GetItemByIdAsync<T>(object id) where T : class
        {
            throw new NotImplementedException();
        }

        public void SubmitChanges()
        {

            _localDataProvider.SubmitChanges();
        }

        public void CreatePlaylistSongMapping(RunJammerPlaylistRunJammerSong mapping)
        {
            _localDataProvider.Create(mapping);
        }

        public void Delete<T>(T item) where T : class
        {
            _localDataProvider.Delete(item);
        }

        public async Task Update<T>(T item, bool isUserLoggedIn = false)
        {
            if (item is RunJammerSong)
            {
                CreateRunJammerSong(item as RunJammerSong);
            }
            else
            {
                if (item is RunSession && isUserLoggedIn)
                {
                    var session = item as RunSession;
                    if (session.ID > 0)
                    {

                        try
                        {
                            _mobileServiceClient.UpdateAsync(item);
                        }
                        catch
                        {
                        }
                    }
                }
                _localDataProvider.SubmitChanges();
            }
        }

        #endregion

        #region Construction

        public RunJammerApplicationDataProvider()
        {
            Initialize();
        }

        public RunJammerApplicationDataProvider(LocalDbDataProvider localDataProvider)
            : this()
        {
            _localDataProvider = localDataProvider;
        }

        public RunJammerApplicationDataProvider(LocalDbDataProvider localDataProvider, RunJammerMobileServiceClient mobileServiceClient)
            : this()
        {
            _localDataProvider = localDataProvider;
            _mobileServiceClient = mobileServiceClient;
        }

        #endregion

        #region Fields

        private readonly LocalDbDataProvider _localDataProvider;
        private RunJammerMobileServiceClient _mobileServiceClient;

        #endregion

        private void Initialize()
        {

        }
    }
}
