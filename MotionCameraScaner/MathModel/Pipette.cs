using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;

namespace MathModel
{
	public abstract class Pipette
	{
		/// <summary>
		/// размер в пикселях
		/// </summary>
		public virtual int Size { get; set; }

		/// <summary>
		/// получить фильтр цвета из диапазона
		/// </summary>
		/// <param name="image">изображение, с которого получается цвет</param>
		/// <param name="x">координата x</param>
		/// <param name="y">координата у</param>
		/// <returns>фильтр цвета</returns>
		public abstract IntensivityColorFilter GetColorFilter(Mat image, int x, int y);

		/// <summary>
		/// рисует пипетку в указанном месте на изображении
		/// </summary>
		/// <param name="image">изображение</param>
		/// <param name="x">координата x</param>
		/// <param name="y">координата у</param>
		public abstract void DrawPipette(Mat image, int x, int y);
	}
}
