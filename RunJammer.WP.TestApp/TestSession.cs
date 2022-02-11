using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using RunJammer.WP.DataAccess;
using RunJammer.WP.Model;
using RunJammer.WP.Model.Implementation;

namespace RunJammer.WP.TestApp
{
    [TestClass]
    public class TestSession
    {
        [TestMethod]
        public void TestDataUpdates()
        {
            var session = new RunSession();
            var timer = new DispatcherTimer();
            var dc = new RunJammerDataContext();
            if (!dc.DatabaseExists())
            {
                dc.CreateDatabase();
            }

            dc.GetTable<RunSession>().InsertOnSubmit(session);
            try
            {

                dc.SubmitChanges();
            }
            catch (ChangeConflictException ex)
            {
                var v = ex;
            }
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (o, args) =>
            {
                session.LastUpdateTime = DateTime.Now;
                session.TotalDistance += 0.1;
                session.Pace = TimeSpan.FromMinutes(new Random().NextDouble()*12).ToString();
                try
                {

                    dc.SubmitChanges();
                }
                catch (ChangeConflictException ex)
                {
                    var v = ex;
                }
            };
            
            timer.Start();

            

        }

    }
}
