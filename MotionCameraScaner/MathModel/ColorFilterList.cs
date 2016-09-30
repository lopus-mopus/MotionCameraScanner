using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathModel
{
	public class ColorFilterList : List<IColorFilter>, IColorFilter
	{
		public bool Check(byte[] color)
		{
			foreach (var filter in this){
				if (!filter.Check(color)) return false;
			}
			return true;
		}
	}
}
