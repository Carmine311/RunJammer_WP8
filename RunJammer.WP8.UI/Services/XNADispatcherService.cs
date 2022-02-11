using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Xna.Framework;

namespace RunJammer.WP.UI.Services
{
    public class XNADispatcherService : IApplicationService, IApplicationLifetimeAware
    {
        private DispatcherTimer _frameworkTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };

        public XNADispatcherService()
        {
            _frameworkTimer.Tick += HandleFrameworkTimerTick;
        }

        private void HandleFrameworkTimerTick(object sender, EventArgs e)
        {
            FrameworkDispatcher.Update();
        }

        public void StartService(ApplicationServiceContext context)
        {
            
        }

        public void StopService()
        {
            
        }

		public void Exited()
		{
		}

		public void Exiting()
		{
			_frameworkTimer.Stop();
		}

		public void Started()
		{
		}

		public void Starting()
		{
			_frameworkTimer.Start();
		}
	}
}
