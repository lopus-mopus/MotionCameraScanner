using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace MathModel
{
	/// <summary>
	/// описывает анализатор камеры, который выдает все параметры объекта в камере
	/// </summary>
	public class CameraScanner
	{
		/// <summary>
		/// захватчик изображения
		/// </summary>
		public Capture capture { get; set; } = new Capture();
		/// <summary>
		/// цветовой фильтр пикселей
		/// </summary>
		public IntensivityColorFilter IntensivityColorFilter { get; set; } = IntensivityColorFilter.All;
		/// <summary>
		/// составной фильтр пикселей
		/// </summary>
		public ColorFilterList colorFilter { get; set; } = new ColorFilterList();
		/// <summary>
		/// пипетка для выбора цвета
		/// </summary>
		public SquarePipette pipette = new SquarePipette { Size = 35 };
		/// <summary>
		/// параметры алгоритма поиска контуров
		/// </summary>
		public CannyParameters cannuParameters = new CannyParameters { Tresh = 100, TreshLinking = 1 };
		/// <summary>
		/// текущее изображение в оперативной памяти
		/// </summary>
		public Mat image { get; private set; }
		/// <summary>
		/// нужно ли суммировать фильтр
		/// </summary>
		public bool SummFilter { get; set; }

		bool getFilter;	// нужно взять фильтр с пипетки
		int mX = -1;	// 
		int mY = -1;		

		/// <summary>
		/// создает сканнер камеры
		/// </summary>
		public CameraScanner()
		{
			colorFilter.Add(IntensivityColorFilter);
		}

		/// <summary>
		/// производит один аналитический цикл
		/// </summary>
		public void Update()
		{
			// Берем кадр
			image = capture.QuerySmallFrame();

			// убираем шумы (размываем)
			CvInvoke.GaussianBlur(image, image, new Size(7, 7), 100);

			// берем фильтр с пипетки
			if (getFilter) {
				if (!SummFilter || IntensivityColorFilter == null) IntensivityColorFilter = pipette.GetColorFilter(image, mX, mY);
				else IntensivityColorFilter.Extend(pipette.GetColorFilter(image, mX, mY));
				getFilter = false;
			}

			// определяем цвет
			byte[] pixel0 = new byte[3];
			byte[] pixel1 = new byte[3];
			byte[] pixel2 = new byte[3];
			byte[] color = new byte[3];
			image.GetPixel(0, 0, ref pixel0);
			image.GetPixel(0, 1, ref pixel1);
			for (int i = 0; i < image.Width; ++i)
				for (int j = 0; j < image.Height; ++j) {
					//image.SetPixel(j, i, Color.FromArgb(128, 0, 0));
					//image.SetPixel(j, i, image.GetPixel(j, i));

					image.GetPixel(j, i, ref pixel2);

					// проверяем границу


					if (colorFilter.Check(pixel2)) {
						color[0] = pixel2[0];
						color[1] = pixel2[1];
						color[2] = pixel2[2];
					}
					else {
						color[0] = 255;
						color[1] = 0;
						color[2] = 255;
					}

					image.SetPixel(j, i, ref color);
					pixel0[0] = pixel1[0];
					pixel0[1] = pixel1[1];
					pixel0[2] = pixel1[2];
					pixel1[0] = pixel2[0];
					pixel1[1] = pixel2[1];
					pixel1[2] = pixel2[2];
				}

			// превращаем изображение в монохром
			//CvInvoke.CvtColor(image, image, ColorConversion.Rgb2Gray);

			// получаем двоичные данные картинки
			var grayImage = new Image<Gray, byte>(image.Bitmap);
			grayImage = grayImage.Canny(cannuParameters.Tresh, cannuParameters.TreshLinking);
			//grayImage = grayImage.ThresholdBinary(new Gray(128), new Gray(255));

			// ищем контуры
			VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
			CvInvoke.FindContours(grayImage, contours, null, RetrType.List, ChainApproxMethod.ChainApproxNone);//,new Point { X = 0, Y = 0 });

			// поиск макс контура
			double largest_area = 0;
			int largest_contour_index = -1;
			for (int i = 0; i < contours.Size; ++i) {
				double a = CvInvoke.ContourArea(contours[i], false);  //  Find the area of contour
				if (a > largest_area) {
					largest_area = a;
					largest_contour_index = i;                //Store the index of largest contour
				}
			}

			// вывод макс контура
			if (largest_contour_index >= 0) CvInvoke.DrawContours(image, contours, largest_contour_index, new MCvScalar(255, 255, 0));

			// вывод всех контуров
			//CvInvoke.DrawContours(image, contours, -1, new MCvScalar(255, 255, 0));

			// определяем центр контуров (вначале нужно отсеить самый крупный контур)

			// проводим линию через 2 контура поворот x

			// определяем относительные размеры контуров для поворота вдоль y

			// даем поправку на знак угла поворота y

			// рисуем контур пипетки
			pipette.DrawPipette(image, mX, mY);

			//Вставляем в imageBox
			OnDrawImage(image);
			OnDrawGrayImage(grayImage);
		}

		double PixelSum(byte[] pixel)
		{
			return pixel[0] + pixel[1] + pixel[2];
		}

		private bool CheckGran(byte[] pixel0, byte[] pixel1, byte[] pixel2, double param4, double param5)
		{
			var a = PixelSum(pixel0);
			var b = PixelSum(pixel1);
			var c = PixelSum(pixel2);
			return Math.Abs(a - b) > b * 0.09;// && Math.Abs(b - a) > a * 0.2;
		}

		/// <summary>
		/// ввызывается, когда нужно вывести изображение
		/// </summary>
		public event EventHandler<Mat> DrawImage;
		/// <summary>
		/// вывод серого изображения
		/// </summary>
		public event EventHandler<Image<Gray, byte>> DrawGrayImage;

		public void OnDrawImage(Mat image)
		{
			if (DrawImage != null) DrawImage(this, image);
		}
		public void OnDrawGrayImage(Image<Gray, byte> image)
		{
			if (DrawGrayImage != null) DrawGrayImage(this, image);
		}
	}
}
