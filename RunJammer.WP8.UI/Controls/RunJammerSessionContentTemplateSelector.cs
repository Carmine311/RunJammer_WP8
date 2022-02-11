using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Common.Controls;
using RunJammer.WP.ViewModel;

namespace RunJammer.WP.UI.Controls
{
    public class RunJammerSessionContentTemplateSelector : DataTemplateSelector
    {
        public DataTemplate JukeboxDataTemplate { get; set; }
        public DataTemplate MapDataTemplate { get; set; }
        public DataTemplate SplitsDataTemplate { get; set; }

        public override DataTemplate SelectDataTemplate(object item)
        {
            if (item is RunJammerJukeBoxViewModel)
            {
                return JukeboxDataTemplate;
            }
            return null;
        }
    }
}
