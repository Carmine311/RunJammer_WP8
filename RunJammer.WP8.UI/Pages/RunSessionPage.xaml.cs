using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using RunJammer.WP.ViewModel;

namespace RunJammer.WP.UI
{
    public partial class RunSessionPage : PhoneApplicationPage
    {

        public RunSessionPage()
        {
            InitializeComponent();

        }


        private void DebugClick(object sender, EventArgs e)
        {
            if (Exceptions.Visibility == Visibility.Collapsed)
            {
                Exceptions.Visibility = Visibility.Visible;
            }
            else
            {
                Exceptions.Visibility = Visibility.Collapsed;
            }
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            var vm = LayoutRoot.DataContext as RunSessionViewModel;
            if (vm.IsSessionActive)
            {
                var result = MessageBox.Show("End your Run Session?", string.Empty,
                                MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
                else
                {
                    vm.EndRunSessionCommand.Execute(null);
                    NavigationService.Navigate(new Uri("/Pages/RunSessionBreakDownPage.xaml", UriKind.RelativeOrAbsolute));
                }
            }
            base.OnBackKeyPress(e);
        }

        private void HandleExceptionItemTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var control = sender as FrameworkElement;
            if (control != null)
            {
                var exception = control.DataContext as Exception;
                if (exception != null)
                {
                    var displayText = new StringBuilder(exception.Message);

                    displayText.AppendLine(exception.StackTrace);
                    if (exception.InnerException != null)
                    {
                        displayText.AppendLine(exception.InnerException.Message);
                        displayText.AppendLine(exception.InnerException.StackTrace);
                    }
                    MessageBox.Show(displayText.ToString());
                }
            }
        }
    }
}