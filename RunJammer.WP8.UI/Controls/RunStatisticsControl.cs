using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RunJammer.WP.UI.Controls
{
	public class RunStatisticsControl : Control
	{
		#region Distance (Dependency Property)

		public static readonly DependencyProperty DistanceProperty =
		DependencyProperty.Register("Distance", typeof(string), typeof(RunStatisticsControl), new PropertyMetadata(string.Empty));

		public string Distance
		{
			get { return (string)GetValue(DistanceProperty); }
			set { SetValue(DistanceProperty, value); }
		}

		#endregion

		#region DistanceUnit (Dependency Property)
		public static readonly DependencyProperty DistanceUnitProperty =
		  DependencyProperty.Register("DistanceUnit", typeof(string), typeof(RunStatisticsControl),
			new PropertyMetadata(String.Empty));

		public string DistanceUnit
		{
			get { return (string)GetValue(DistanceUnitProperty); }
			set { SetValue(DistanceUnitProperty, value); }
		}
		#endregion 

		#region Speed (Dependency Property)

		public static readonly DependencyProperty SpeedProperty =
		DependencyProperty.Register("Speed", typeof(string), typeof(RunStatisticsControl), new PropertyMetadata(string.Empty));

		public string Speed
		{
			get { return (string)GetValue(SpeedProperty); }
			set { SetValue(SpeedProperty, value); }
		}

		#endregion

		#region ElapsedTime (Dependency Property)

		public static readonly DependencyProperty ElapsedTimeProperty =
		DependencyProperty.Register("ElapsedTime", typeof(string), typeof(RunStatisticsControl), new PropertyMetadata(string.Empty));

		public string ElapsedTime
		{
			get { return (string)GetValue(ElapsedTimeProperty); }
			set { SetValue(ElapsedTimeProperty, value); }
		}

		#endregion

		#region Pace (Dependency Property)

		public static readonly DependencyProperty PaceProperty =
		DependencyProperty.Register("Pace", typeof(string), typeof(RunStatisticsControl), new PropertyMetadata(string.Empty));

		public string Pace
		{
			get { return (string)GetValue(PaceProperty); }
			set { SetValue(PaceProperty, value); }
		}

		#endregion

		#region PrimaryLabelSize (Dependency Property)

		public static readonly DependencyProperty PrimaryLabelSizeProperty =
		DependencyProperty.Register("PrimaryLabelSize", typeof(double), typeof(RunStatisticsControl), new PropertyMetadata(26d));

		public double PrimaryLabelSize
		{
			get { return (double)GetValue(PrimaryLabelSizeProperty); }
			set { SetValue(PrimaryLabelSizeProperty, value); }
		}

		#endregion

		#region SecondaryLabelSize (Dependency Property)
		public static readonly DependencyProperty SecondaryLabelSizeProperty =
		  DependencyProperty.Register("SecondaryLabelSize", typeof(double), typeof(RunStatisticsControl),
			new PropertyMetadata(20d));

		public double SecondaryLabelSize
		{
			get { return (double)GetValue(SecondaryLabelSizeProperty); }
			set { SetValue(SecondaryLabelSizeProperty, value); }
		}
		#endregion 

		#region InfoLabelSize (Dependency Property)
		public static readonly DependencyProperty InfoLabelSizeProperty =
		  DependencyProperty.Register("InfoLabelSize", typeof(double), typeof(RunStatisticsControl),
			new PropertyMetadata(18.667));

		public double InfoLabelSize
		{
			get { return (double)GetValue(InfoLabelSizeProperty); }
			set { SetValue(InfoLabelSizeProperty, value); }
		}
		#endregion 

		#region DistanceHeaderBrush (Dependency Property)
		public static readonly DependencyProperty DistanceHeaderBrushProperty =
		  DependencyProperty.Register("DistanceHeaderBrush", typeof(Brush), typeof(RunStatisticsControl),
			new PropertyMetadata(null));

		public Brush DistanceHeaderBrush
		{
			get { return (Brush)GetValue(DistanceHeaderBrushProperty); }
			set { SetValue(DistanceHeaderBrushProperty, value); }
		}
		#endregion 

		#region ElapsedTimeHeaderBrush (Dependency Property)
		public static readonly DependencyProperty ElapsedTimeHeaderBrushProperty =
		  DependencyProperty.Register("ElapsedTimeHeaderBrush", typeof(Brush), typeof(RunStatisticsControl),
			new PropertyMetadata(null));

		public Brush ElapsedTimeHeaderBrush
		{
			get { return (Brush)GetValue(ElapsedTimeHeaderBrushProperty); }
			set { SetValue(ElapsedTimeHeaderBrushProperty, value); }
		}
		#endregion 

		#region SpeedHeaderBrush (Dependency Property)
		public static readonly DependencyProperty SpeedHeaderBrushProperty =
		  DependencyProperty.Register("SpeedHeaderBrush", typeof(Brush), typeof(RunStatisticsControl),
			new PropertyMetadata(null));

		public Brush SpeedHeaderBrush
		{
			get { return (Brush)GetValue(SpeedHeaderBrushProperty); }
			set { SetValue(SpeedHeaderBrushProperty, value); }
		}
		#endregion 

		#region PaceHeaderBrush (Dependency Property)
		public static readonly DependencyProperty PaceHeaderBrushProperty =
		  DependencyProperty.Register("PaceHeaderBrush", typeof(Brush), typeof(RunStatisticsControl),
			new PropertyMetadata(null));

		public Brush PaceHeaderBrush
		{
			get { return (Brush)GetValue(PaceHeaderBrushProperty); }
			set { SetValue(PaceHeaderBrushProperty, value); }
		}
		#endregion 


		public RunStatisticsControl()
		{
			DefaultStyleKey = typeof(RunStatisticsControl);
		}
	}
}
