using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunJammer.WP.Model.Interface
{
    public interface IRunSession<TDateTime>
    {
        int ID { get; set; }
        TDateTime StartTime { get; set; }

        TDateTime LastUpdateTime { get; set; }

        TDateTime EndTime { get; set; }

        double TotalDistance { get; set; }

        bool IsSessionActive { get; set; }
        bool UserEndedSession { get; set; }
        string Pace { get; set; }
        double AverageSpeed { get; set; }
        double CurrentSpeed { get; set; }
        double TopSpeed { get; set; }

    }
}
