using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;

namespace Common.Controls
{
    public class MultiTemplatePivot : Pivot
    {
        public static readonly DependencyProperty ItemTemplateSelectorProperty =
            DependencyProperty.Register("ItemTemplateSelector", typeof(DataTemplateSelector), typeof(MultiTemplatePivot), new PropertyMetadata(default(DataTemplateSelector)));

        public DataTemplateSelector ItemTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(ItemTemplateSelectorProperty); }
            set { SetValue(ItemTemplateSelectorProperty, value); }
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            var pivotItem = element as PivotItem;
            if (pivotItem != null)
            {
                pivotItem.ContentTemplate = this.ItemTemplateSelector.SelectDataTemplate(
                    item);
            }
        }
    }
}
