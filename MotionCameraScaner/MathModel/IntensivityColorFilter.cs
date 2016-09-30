using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathModel
{
	/// <summary>
	/// фильтр цвета по интенсивностям
	/// </summary>
	public class IntensivityColorFilter: IColorFilter
	{
        /// <summary>
        /// возвращает фильтр полнго спектра
        /// </summary>
        public static IntensivityColorFilter All
        {
            get
            {
                return new IntensivityColorFilter
                {
                    R = IntensivityRange.All,
                    G = IntensivityRange.All,
                    B = IntensivityRange.All
                };
            }
        }
		public IntensivityRange R { get; set; } = new IntensivityRange { Min = 0, Max = 255 };
		public IntensivityRange G { get; set; } = new IntensivityRange { Min = 0, Max = 255 };
		public IntensivityRange B { get; set; } = new IntensivityRange { Min = 0, Max = 255 };

		public bool Check(byte[] color)
		{
			return 
				R.Min <= color[2] && color[2] <= R.Max &&
				G.Min <= color[1] && color[1] <= G.Max &&
                B.Min <= color[0] && color[0] <= B.Max;
		}

        /// <summary>
        /// расширяет фильтр, чтобы был включен спектр другого фильтра
        /// </summary>
        /// <param name="other">другой фильтр</param>
        public void Extend(IntensivityColorFilter other)
        {
            R.Extend(other.R);
            G.Extend(other.G);
            B.Extend(other.B);
        }
    }
}
