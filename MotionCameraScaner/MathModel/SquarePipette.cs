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
		public override void DrawPipette(Mat image, int x, int y)
		{
			byte[] color = new byte[] { 0, 0, 0 };
			for (int i = 0; i < Size; ++i) {
				image.SetPixel(y, x + i, ref color);
				image.SetPixel(y + Size, x + i, ref color);
				image.SetPixel(y + i, x, ref color);
				image.SetPixel(y + i, x + Size, ref color);
			}
		}

		public override IntensivityColorFilter GetColorFilter(Mat image, int x, int y)
		{
			IntensivityColorFilter res = new IntensivityColorFilter();
			int n = 0;
			for (int i=0;i<Size;++i){
				for(int j = 0; j < Size; ++j) {
					// получаем цвет
					if (x + i >= image.Width || y + j >= image.Height) continue;
					var color = image.GetPixel(y + j, x + i);
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
