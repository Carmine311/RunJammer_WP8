using Microsoft.WindowsAzure.MobileServices;
using RunJammer.WP.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunJammer.WP.ViewModel
{
	public class GreetingViewModel : ViewModelBase
	{

		
		public DelegateCommand ViewRunSessionHistoryCommand { get; protected set; }
		public DelegateCommand<string> AuthenticateCommand { get; protected set; }

		public event EventHandler StartNewRunSession;
		public event EventHandler ViewRunSessionHistory;

		public GreetingViewModel()
		{
			ViewRunSessionHistoryCommand = new DelegateCommand(RaiseViewRunSessionHistory);
			AuthenticateCommand = new DelegateCommand<string>(ExecuteAuthenticateCommand);
		}

		private void ExecuteAuthenticateCommand(string authProvider)
		{
			if (authProvider == "Facebook")
			{
				_authProvider = MobileServiceAuthenticationProvider.Facebook;
			}
			Authenticate();
		}

		private async void Authenticate()
		{
			MobileServiceUser user = null;
			while (user == null)
			{
				try
				{
					//user = await AzureProvider.Login(_authProvider);
					//RaiseLoginSuccessful(user);
				}
				catch
				{
				}
			}
		}

		private void RaiseStartNewRunSession()
		{
			if (StartNewRunSession != null)
			{
				StartNewRunSession(this, EventArgs.Empty);
			}
		}

		private void RaiseViewRunSessionHistory()
		{
			if (ViewRunSessionHistory != null)
			{
				ViewRunSessionHistory(this, EventArgs.Empty);
			}
		}

		//private void RaiseLoginSuccessful(MobileServiceUser user)
		//{
		//	var runJammerUser = new RunJammerUser();
		//	switch (_authProvider)
		//	{
		//		case MobileServiceAuthenticationProvider.Facebook:
		//			runJammerUser.FacebookID = user.UserId;
		//			break;
		//		case MobileServiceAuthenticationProvider.Google:
		//			runJammerUser.GoogleID = user.UserId;
		//			break;
		//		case MobileServiceAuthenticationProvider.MicrosoftAccount:
		//			runJammerUser.MicrosoftID = user.UserId;
		//			break;
		//		case MobileServiceAuthenticationProvider.Twitter:
		//			runJammerUser.TwitterID = user.UserId;
		//			break;
		//		default:
		//			break;
		//	}
		//}

		private MobileServiceAuthenticationProvider _authProvider;
	}
}
