using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//Подключаем библиотеки
using Emgu.CV;
using Emgu.CV.UI;
using Emgu.Util;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Runtime.InteropServices;
using Emgu.CV.Util;
using OpenCVTest2.MathModel;

namespace OpenCVTest2
{
	public partial class Form1 : Form
	{
		Capture capture = new Capture();
        IntensivityColorFilter intensivityColorFilter = IntensivityColorFilter.All;
		ColorFilterList colorFilter = new ColorFilterList();
		SquarePipette pipette = new SquarePipette { Size = 35 };
		CannyParameters cannuParameters = new CannyParameters { Tresh = 100, TreshLinking = 1 };
        Mat image;

        bool getFilter;
		int mX = -1;
		int mY = -1;

		IntensivityColorFilter IntensivityColorFilter
		{
			get
			{
				return intensivityColorFilterBindingSource.DataSource as IntensivityColorFilter;
			}
			set
			{
				colorFilter.Remove(intensivityColorFilter);
				intensivityColorFilter = value;
				colorFilter.Add(intensivityColorFilter);
				intensivityColorFilterBindingSource.DataSource = intensivityColorFilter;
			}
		}

		public Form1()
		{
			InitializeComponent();
			colorFilter.Add(intensivityColorFilter);
			intensivityColorFilterBindingSource.DataSource = intensivityColorFilter;
			cannyParametersBindingSource.DataSource = cannuParameters;
			Application.Idle += GetCameRaImage;
            numericUpDown7.Value = pipette.Size;

        }

		void GetCameRaImage(object sender, EventArgs e)
		{
			// Берем кадр
			image = capture.QuerySmallFrame ();

            
            // убираем шумы (размываем)
            CvInvoke.GaussianBlur(image, image, new Size(7, 7), 100);

			// берем фильтр с пипетки
			if (getFilter) {
                if (!checkBox1.Checked || IntensivityColorFilter == null) IntensivityColorFilter = pipette.GetColorFilter(image, mX, mY);
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
			pictureBox1.Image = image.Bitmap;
			pictureBox1.Width = image.Width;
			pictureBox1.Height = image.Height;

			pictureBox2.Image = grayImage.Bitmap;
			pictureBox2.Width = grayImage.Bitmap.Width;
			pictureBox2.Height = grayImage.Bitmap.Height;
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

		private void numericUpDown2_ValueChanged(object sender, EventArgs e)
		{
			intensivityColorFilterBindingSource.EndEdit();
		}

		private void numericUpDown1_ValueChanged(object sender, EventArgs e)
		{
			intensivityColorFilterBindingSource.EndEdit();
			Validate();
		}

		private void numericUpDown2_Click(object sender, EventArgs e)
		{
			intensivityColorFilterBindingSource.EndEdit();
			Validate();
		}

		private void numericUpDown1_Click(object sender, EventArgs e)
		{
			intensivityColorFilterBindingSource.EndEdit();
		}

		private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
		{
			//Bitmap bmp = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
			//this.DrawToBitmap(bmp, new Rectangle(this.ClientRectangle.Location, this.ClientSize));
			//MessageBox.Show(string.Format("Цвет пиксела: {0}", bmp.GetPixel(e.X, e.Y).ToString()));
			//var pixel = bmp.GetPixel(e.X, e.Y);

			//IntensivityColorFilter = new IntensivityColorFilter { R = pixel.R, G = pixel.G, B = pixel.B };
			//IntensivityColorFilter = pipette.GetColorFilter(bmp, e.X, e.Y);
			getFilter = true;

			this.Refresh();
		}

		private void numericUpDown3_ValueChanged(object sender, EventArgs e)
		{
			intensivityColorFilterBindingSource.EndEdit();
			Validate();
		}

		private void numericUpDown4_ValueChanged(object sender, EventArgs e)
		{
			intensivityColorFilterBindingSource.EndEdit();
			Validate();
		}

		private void numericUpDown5_ValueChanged(object sender, EventArgs e)
		{
			intensivityColorFilterBindingSource.EndEdit();
			Validate();
		}

		private void numericUpDown6_ValueChanged(object sender, EventArgs e)
		{
			intensivityColorFilterBindingSource.EndEdit();
			Validate();
		}

		private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
		{
			mX = e.X;
			mY = e.Y;
		}

        private void button1_Click(object sender, EventArgs e)
        {
            IntensivityColorFilter = IntensivityColorFilter.All;
        }

        private void numericUpDown7_ValueChanged(object sender, EventArgs e)
        {
            pipette.Size = (int)numericUpDown7.Value;
        }
    }
}
