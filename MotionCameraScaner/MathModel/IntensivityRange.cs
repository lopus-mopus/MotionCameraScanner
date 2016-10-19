using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MathModel
{
	/// <summary>
	/// диапазон интенсивности
	/// </summary>
	[Serializable, XmlRoot]
	public class IntensivityRange
	{
		[XmlAttribute]
		public byte Min { get; set; }
		[XmlAttribute]
		public byte Max { get; set; }

		public IntensivityRange()
		{

		}
		public IntensivityRange(byte min, byte max)
		{
			this.Min = min;
			this.Max = max;
		}
		public IntensivityRange(byte intensivity)
		{
			this.Min = intensivity;
			this.Max = intensivity;
		}

		public static implicit operator IntensivityRange(byte intensivity)
		{
			return new IntensivityRange { Min = intensivity, Max = intensivity };
		}

		/// <summary>
		/// возвращает диапазон всех цветов
		/// </summary>
		public static IntensivityRange All
		{
			get
			{
				return new IntensivityRange(0, 255);
			}
		}

        /// <summary>
        /// расширяет спектр, чтобы был включен другой спектр
        /// </summary>
        /// <param name="other">другой спектр</param>
        public void Extend(IntensivityRange other)
        {
            if (Min > other.Min) Min = other.Min;
            if (Max < other.Max) Max = other.Max;
        }

        /// <summary>
        /// включает в текущую интенсивность вказанную интенсивность по минимуму интервала
        /// </summary>
        /// <param name="intensivity">включаемая интенсивность</param>
        public void Include(byte intensivity)
		{
			if (intensivity > Max) Max = intensivity;
			if (intensivity < Min) Min = intensivity;
		}
	}
}
