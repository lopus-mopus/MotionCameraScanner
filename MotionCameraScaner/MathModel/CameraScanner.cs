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
		/// пипетка для выбора цвета
		/// </summary>
		public SquarePipette Pipette = new SquarePipette { Size = 15 };
		/// <summary>
		/// параметры алгоритма поиска контуров
		/// </summary>
		public CannyParameters CannyParameters = new CannyParameters { Tresh = 100, TreshLinking = 1 };
		/// <summary>
		/// текущее изображение в оперативной памяти
		/// </summary>
		public Mat Image { get; private set; }
		/// <summary>
		/// нужно ли суммировать фильтр
		/// </summary>
		public bool SummFilter { get; set; }
	
		/// <summary>
		/// кликнули на пипетку
		/// </summary>
		public bool PipetteClick { get; set; }
		/// <summary>
		/// где сейчас находится пипетка
		/// </summary>
		public Point? PipettePosition { get; set; }

		/// <summary>
		/// производит один аналитический цикл
		/// </summary>
		public void Update()
		{
			// Берем кадр
			Image = capture.QuerySmallFrame();

			// обработка ошибок кадра
			if (Image == null) throw new Exception("Не удалось получить изображение - возможно отсутствует камера");

			// убираем шумы (размываем)
			CvInvoke.GaussianBlur(Image, Image, new Size(7, 7), 100);

			// берем фильтр с пипетки
			if (PipetteClick && PipettePosition != null) {
				if (!SummFilter || IntensivityColorFilter == null) {
					IntensivityColorFilter = Pipette.GetColorFilter(Image, PipettePosition.Value);
					SummFilter = true;
				}
				else IntensivityColorFilter.Extend(Pipette.GetColorFilter(Image, PipettePosition.Value));
				PipetteClick = false;
			}

			// определяем цвет
			byte[] pixel0 = new byte[3];
			byte[] pixel1 = new byte[3];
			byte[] pixel2 = new byte[3];
			byte[] color = new byte[3];
			Image.GetPixel(0, 0, ref pixel0);
			Image.GetPixel(0, 1, ref pixel1);
			for (int i = 0; i < Image.Width; ++i)
				for (int j = 0; j < Image.Height; ++j) {
					//image.SetPixel(j, i, Color.FromArgb(128, 0, 0));
					//image.SetPixel(j, i, image.GetPixel(j, i));

					Image.GetPixel(j, i, ref pixel2);

					// проверяем границу


					if (IntensivityColorFilter.Check(pixel2)) {
						color[0] = pixel2[0];
						color[1] = pixel2[1];
						color[2] = pixel2[2];
					}
					else {
						color[0] = 255;
						color[1] = 0;
						color[2] = 255;
					}

					Image.SetPixel(j, i, ref color);
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
			var grayImage = new Image<Gray, byte>(Image.Bitmap);
			grayImage = grayImage.Canny(CannyParameters.Tresh, CannyParameters.TreshLinking);
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

			// обработка макс контура
			if (largest_contour_index >= 0) {
				// вывод контура
				CvInvoke.DrawContours(Image, contours, largest_contour_index, new MCvScalar(255, 255, 0));
				// поиск позиции контура
				Point[] pts = contours[largest_contour_index].ToArray();
				Point average = new Point(0, 0);
				int pointCount = 0;
				foreach (Point p in pts) {
					average.X += p.X;
					average.Y += p.Y;
					++pointCount;
				}
				average.X /= pointCount;
				average.Y /= pointCount;
				// вывод полученой точки
				OnGetPoint(average);
				Pipette.DrawPipette(Image, average);
			}

			// вывод всех контуров
			//CvInvoke.DrawContours(image, contours, -1, new MCvScalar(255, 255, 0));

			// определяем центр контуров (вначале нужно отсеить самый крупный контур)

			// проводим линию через 2 контура поворот x

			// определяем относительные размеры контуров для поворота вдоль y

			// даем поправку на знак угла поворота y

			// рисуем контур пипетки
			if (PipettePosition != null) Pipette.DrawPipette(Image, PipettePosition.Value);

			// Вставляем в imageBox
			OnDrawImage(Image);
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
		/// <summary>
		/// когда обнаружена позиция контура
		/// </summary>
		public event EventHandler<Point> GetPoint;

		void OnDrawImage(Mat image)
		{
			if (DrawImage != null) DrawImage(this, image);
		}
		void OnDrawGrayImage(Image<Gray, byte> image)
		{
			if (DrawGrayImage != null) DrawGrayImage(this, image);
		}
		void OnGetPoint(Point point)
		{
			if (GetPoint != null) GetPoint(this, point);
		}
	}
}
