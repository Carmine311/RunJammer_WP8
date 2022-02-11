using System.Windows;
using Common.DataAccess.Interface;
using RunJammer.WP.DataAccess.Implementation;
using RunJammer.WP.Model;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace RunJammer.WP.ViewModel
{
    public class RunSessionHistoryViewModel : ViewModelBase
    {
        private ObservableCollection<RunSessionViewModel> _runSessions;
        public ObservableCollection<RunSessionViewModel> RunSessions
        {
            get { return _runSessions; }
            set
            {
                if (value != _runSessions)
                {
                    _runSessions = value;
                    OnPropertyChanged("RunSessions");
                }
            }
        }


        private RunSessionViewModel _selectedRunSessionHistoryItem;
        public RunSessionViewModel SelectedRunSessionHistoryItem
        {
            get { return _selectedRunSessionHistoryItem; }
            set
            {
                if (value != _selectedRunSessionHistoryItem)
                {
                    _selectedRunSessionHistoryItem = value;
                    if (_selectedRunSessionHistoryItem != null)
                    {
                        try
                        {
                            foreach (var runJammerSongViewModel in _selectedRunSessionHistoryItem.Songs)
                            {
                                runJammerSongViewModel.GetHiResDisplayImage();
                                runJammerSongViewModel.CalculateAverageSpeed(_selectedRunSessionHistoryItem.Waypoints);
                            }
                        }
                        catch (System.Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }

                    OnPropertyChanged("SelectedRunSessionHistoryItem");
                }
            }
        }




        public RunSessionHistoryViewModel()
        {
            RunSessions = new ObservableCollection<RunSessionViewModel>();
            RunSessions.CollectionChanged += ExecuteRunSessionCollectionChanged;
        }

        private void ExecuteRunSessionCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems.Count > 0)
            {
                //foreach (RunSessionViewModel runSessionHistoryItem in e.NewItems)
                //{
                //	runSessionHistoryItem.ItemSelected += HandleRunSessionHistoryItemSelected;
                //}
            }
        }


        public RunSessionHistoryViewModel(RunJammerApplicationDataProvider dataProvider)
            : this()
        {
            InitializeRunSessionViewModels(dataProvider);
        }

        private async void InitializeRunSessionViewModels(RunJammerApplicationDataProvider dataProvider)
        {
            var runSessionHistory = dataProvider.GetRunSessionHistory();


            var runSessionViewModels = await Task.Run(() => runSessionHistory.Select(rs => new RunSessionViewModel(rs)));
            RunSessions = new ObservableCollection<RunSessionViewModel>(runSessionViewModels);
        }
    }

}
