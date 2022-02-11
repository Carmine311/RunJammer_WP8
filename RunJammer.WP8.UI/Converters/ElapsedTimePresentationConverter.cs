using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace RunJammer.WP.UI.Converters
{
	public class ElapsedTimePresentationConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var elapsedTime = (TimeSpan)value;
			if (elapsedTime.Minutes == 0)
			{
				return string.Format("{0}.{1} {2}", elapsedTime.Seconds, elapsedTime.Milliseconds / 100, "s");
			}
			if (elapsedTime.Hours == 0)
			{
				return string.Format("{0} {1} {2}.{3} {4}", elapsedTime.Minutes, "m", elapsedTime.Seconds, elapsedTime.Milliseconds / 100, "s");
			}
			return elapsedTime;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
