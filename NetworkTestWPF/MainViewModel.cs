using OxyPlot.Wpf;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Echevil;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using LineSeries = OxyPlot.Wpf.LineSeries;

namespace NetworkTestWPF
{
	public class MainViewModel
	{
		public PlotModel MyModel { get; private set; }

		private readonly NetworkAdapter[] adapters;
		private readonly NetworkMonitor monitor;
		private double _xValue = 1;

		public MainViewModel()
		{
			monitor = new NetworkMonitor();
			adapters = monitor.Adapters;

			if (adapters.Length == 0)
			{
				MessageBox.Show("No network adapters found on this computer.");
				return;
			}

			monitor.StopMonitoring();
			monitor.StartMonitoring(adapters[1]);

			MyModel = new PlotModel { Title = "Up / Down (kbps)" };
			MyModel.Series.Add(new OxyPlot.Series.LineSeries());
			MyModel.Series.Add(new OxyPlot.Series.LineSeries());

			var dispatcherTimer = new DispatcherTimer();
			dispatcherTimer.Tick += dispatcherTimer_Tick;
			dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
			dispatcherTimer.Start();
		}

		void dispatcherTimer_Tick(object sender, EventArgs e)
		{
			Dispatcher.CurrentDispatcher.Invoke(() =>
			{
				var adapter = adapters[1];
				var upload = MyModel.Series[0] as OxyPlot.Series.LineSeries;
				var download = MyModel.Series[1] as OxyPlot.Series.LineSeries;

				if (upload.Points.Count > 10)
					upload.Points.RemoveAt(0);
				upload.Points.Add(new DataPoint(_xValue, adapter.UploadSpeedKbps));

				if (download.Points.Count > 10)
					download.Points.RemoveAt(0);
				download.Points.Add(new DataPoint(_xValue, adapter.DownloadSpeedKbps));

				MyModel.InvalidatePlot(true);
				_xValue++;
			});
		}
	}
}
