using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace MathModel
{
	public interface IColorFilter
	{
		bool Check(byte[] color);
	}
}
