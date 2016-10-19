using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
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
		/// <param name="point">координата пипетки</param>
		/// <returns>фильтр цвета</returns>
		public abstract IntensivityColorFilter GetColorFilter(Mat image, Point point);

		/// <summary>
		/// рисует пипетку в указанном месте на изображении
		/// </summary>
		/// <param name="image">изображение</param>
		/// <param name="point">координата пипетки</param>
		public abstract void DrawPipette(Mat image, Point point);
	}
}
