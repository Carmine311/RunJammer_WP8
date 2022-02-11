using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace RunJammer.WP.UI.Converters
{
	public class MediaStatePlayingToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var mediaState = (MediaState)value;
			var button = parameter != null ? parameter.ToString() : string.Empty;

			if (mediaState == MediaState.Paused || mediaState == MediaState.Stopped)
			{
				if (button == "Play")
				{

					return Visibility.Visible;
				}
				return Visibility.Collapsed;
			}

			if (button == "Play")
			{
				return Visibility.Collapsed;
			}
			return Visibility.Visible;

		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
