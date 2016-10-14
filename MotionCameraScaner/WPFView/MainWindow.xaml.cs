using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MathModel;
using System.Windows.Threading;

namespace WPFView
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		/// <summary>
		/// логика сканнера кмеры
		/// </summary>
		CameraScanner Scanner { get; set; } = new CameraScanner();
		/// <summary>
		/// таймер обновления
		/// </summary>
		DispatcherTimer Timer { get; set; } = new DispatcherTimer();

		public MainWindow()
		{
			InitializeComponent();
			Scanner.DrawImage += Scanner_DrawImage;
			Scanner.DrawGrayImage += Scanner_DrawGrayImage;
			Scanner.GetPoint += Scanner_GetPoint;
			Scanner.SummFilter = false;
			Timer.Interval = new System.TimeSpan(50000);
			Timer.Tick += Timer_Tick;
			Timer.Start();
		}

		private void Scanner_GetPoint(object sender, System.Drawing.Point e)
		{
			
		}

		private void Timer_Tick(object sender, EventArgs e)
		{
			Scanner.Update();
		}

		private void Scanner_DrawGrayImage(object sender, Emgu.CV.Image<Emgu.CV.Structure.Gray, byte> e)
		{
			image1.Width = e.Width;
			image1.Height = e.Height;
			image1.Source = e.Bitmap.ToBitmapSource();
		}

		private void Scanner_DrawImage(object sender, global::Emgu.CV.Mat e)
		{
			image.Width = e.Width;
			image.Height = e.Height;
			image.Source = e.Bitmap.ToBitmapSource();
		}

		private void image_MouseMove(object sender, MouseEventArgs e)
		{
			Point p = e.GetPosition(image);
			Scanner.PipettePosition = new System.Drawing.Point((int)p.X, (int)p.Y);
		}

		private void image_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton != MouseButtonState.Pressed) return;
			Point p = e.GetPosition(image);
			Scanner.PipettePosition = new System.Drawing.Point((int)p.X, (int)p.Y);
			Scanner.PipetteClick = true;
		}

		private void MenuItem_Click(object sender, RoutedEventArgs e)
		{
			// получаем имя файла
			var dialog = new System.Windows.Forms.SaveFileDialog();
			dialog.Filter = "Файлы XML|*.xml";
			if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
			// сохраняем в файл
			string str = Scanner.IntensivityColorFilter.Serialize();
			System.IO.StreamWriter file = new System.IO.StreamWriter(dialog.FileName);
			file.WriteLine(str);
			file.Close();
		}
	}
}
