using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;

namespace WPFView
{
	public static class BitmapExtensions
	{
		public static BitmapSource ToBitmapSource(this System.Drawing.Bitmap source)
		{
			return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
					source.GetHbitmap(),
					IntPtr.Zero,
					Int32Rect.Empty,
					BitmapSizeOptions.FromEmptyOptions());
		}
	}
}
