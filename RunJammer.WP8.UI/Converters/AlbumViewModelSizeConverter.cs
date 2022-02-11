using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace RunJammer.WP.UI.Converters
{
	public class AlbumViewModelSizeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var number = new Random().Next(100);
			number = new Random().Next(100);
			number = new Random().Next(100);

			if (number >= 0 && number <= 32)
			{
				return 35;
			}
			else if (number >= 33 && number <= 65)
			{
				return 70;
			}
			else
			{
				return 140;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
