using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;

namespace MathModel
{
	public class SquarePipette : Pipette
	{
		public override void DrawPipette(Mat image, Point point)
		{
			byte[] color = new byte[] { 0, 0, 0 };
			for (int i = 0; i < Size; ++i) {
				image.SetPixel(point.Y, point.X + i, ref color);
				image.SetPixel(point.Y + Size, point.X + i, ref color);
				image.SetPixel(point.Y + i, point.X, ref color);
				image.SetPixel(point.Y + i, point.X + Size, ref color);
			}
		}

		public override IntensivityColorFilter GetColorFilter(Mat image, Point point)
		{
			IntensivityColorFilter res = new IntensivityColorFilter();
			int n = 0;
			for (int i=0;i<Size;++i){
				for(int j = 0; j < Size; ++j) {
					// получаем цвет
					if (point.X + i >= image.Width || point.Y + j >= image.Height) continue;
					var color = image.GetPixel(point.Y + j, point.X + i);
					// получаем номер пикселя
					++n;
					// анализируем первый пиксель
					if (n == 1) {
						res.R = new IntensivityRange(color.R);
						res.G = new IntensivityRange(color.G);
						res.B = new IntensivityRange(color.B);
						continue;
					}
					// анализируем остальные пиксели
					res.R.Include(color.R);
					res.G.Include(color.G);
					res.B.Include(color.B);
				}
			}

			// вывод результата
			return res;
		}
	}
}
