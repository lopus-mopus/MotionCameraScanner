﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenCVTest2.MathModel
{
	public interface IColorFilter
	{
		bool Check(byte[] color);
	}
}
