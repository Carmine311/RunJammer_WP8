using RunJammer.WP.Model;

namespace RunJammer.WP.ViewModel
{
	public class RunSessionWaypointHistoryItemViewModel : ViewModelBase
	{
		private string _currentSongTitle;
		public string CurrentSongTitle
		{
			get { return _currentSongTitle; }
			set
			{
				if (value != _currentSongTitle)
				{
					_currentSongTitle = value;
					OnPropertyChanged("CurrentSongTitle");
				}
			}
		}

		private RunSessionWaypoint _runSessionWaypoint;

		public RunSessionWaypointHistoryItemViewModel(RunSessionWaypoint runSessionWaypoint)
		{
			_runSessionWaypoint = runSessionWaypoint;

            if (_runSessionWaypoint.CurrentSong != null)
            {
                CurrentSongTitle = _runSessionWaypoint.CurrentSong.Name;
            }
		}
	}
}
