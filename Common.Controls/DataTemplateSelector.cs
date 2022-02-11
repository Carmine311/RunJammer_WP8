using System.Windows;

namespace Common.Controls
{
    public abstract class DataTemplateSelector
    {
        public abstract DataTemplate SelectDataTemplate(object item);
    }
}
