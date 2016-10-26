using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Emgu.CV;
using Emgu.CV.CvEnum;

namespace MathModel
{
	/// <summary>
	/// расширения класса Mat
	/// </summary>
	public static class MatExtentions
	{
		public static int GetPixelBytesCount(this Mat mat)
		{
			switch (mat.Depth) {
				case DepthType.Default: return 4;
				case DepthType.Cv8U: return 1;
				case DepthType.Cv8S: return 1;
				case DepthType.Cv16U: return 2;
				case DepthType.Cv16S: return 2;
				case DepthType.Cv32S: return 4;
				case DepthType.Cv32F: return 4;
				case DepthType.Cv64F: return 8;
				default: return 4;
			}
		}
		public static byte[] GetValue(this Mat mat, int row, int col)
		{
			// создаем данные нужной длины
			var bytes = new byte[mat.GetPixelBytesCount()];
			// копируем из адреса данные нужной длины
			Marshal.Copy(new IntPtr(mat.DataPointer.ToInt32() + ((row * mat.Cols + col) * mat.ElementSize)), bytes, 0, bytes.Length);
			// выводим данные
			return bytes;
		}
		public static void SetValue(this Mat mat, int row, int col, byte[] value)
		{
			Marshal.Copy(value, 0, new IntPtr(mat.DataPointer.ToInt32() + (row * mat.Cols + col) * mat.ElementSize), value.Length);
		}

		/*private static object CreateElement(DepthType depthType, object value)
		{
			var element = CreateElement(depthType);
			element[0] = value;
			return element;
		}*/
		public static void SetPixel(this Mat mat, int row, int col, byte r, byte g, byte b)
		{
			byte[] target = new byte[] { b, g, r };
			Marshal.Copy(target, 0, new IntPtr(mat.DataPointer.ToInt32() + (row * mat.Cols + col) * mat.ElementSize), 3);
		}
		public static void SetPixel(this Mat mat, int row, int col, System.Drawing.Color color)
		{
			byte[] target = new byte[] { color.B, color.G, color.R };
			Marshal.Copy(target, 0, new IntPtr(mat.DataPointer.ToInt32() + (row * mat.Cols + col) * mat.ElementSize), 3);
		}
		public static System.Drawing.Color GetPixel(this Mat mat, int row, int col)
		{
			byte[] value = new byte[3];
			Marshal.Copy(new IntPtr(mat.DataPointer.ToInt32() + (row * mat.Cols + col) * mat.ElementSize), value, 0, 3);
			return System.Drawing.Color.FromArgb(value[2], value[1], value[0]);
		}

		public static void SetPixel(this Mat mat, int row, int col, ref byte[] pixel)
		{
			if (row >= mat.Height || col >= mat.Width || col < 0 || row < 0) return;
			Marshal.Copy(pixel, 0, new IntPtr(mat.DataPointer.ToInt32() + (row * mat.Cols + col) * mat.ElementSize), 3);
		}
		public static void GetPixel(this Mat mat, int row, int col, ref byte[] pixel)
		{
			Marshal.Copy(new IntPtr(mat.DataPointer.ToInt32() + (row * mat.Cols + col) * mat.ElementSize), pixel, 0, 3);
		}

		/*private static object CreateElement(DepthType depthType)
		{
			switch (depthType) {
				case DepthType.Default: return new float[1];
				case DepthType.Cv8U: return new byte[1];
				case DepthType.Cv8S: return new sbyte[1];
				case DepthType.Cv16U: return new ushort[1];
				case DepthType.Cv16S: return new short[1];
				case DepthType.Cv32S: return new int[1];
				case DepthType.Cv32F: return new float[1];
				case DepthType.Cv64F: return new double[1];
				default: return new float[1];
			}
		}*/
		public static double GetDoubleValue(this Mat mat, int row, int col)
		{
			var value = new double[1];
			Marshal.Copy(new IntPtr(mat.DataPointer.ToInt32() + (row * mat.Cols + col) * mat.ElementSize), value, 0, 1);
			return value[0];
		}
		public static void SetDoubleValue(this Mat mat, int row, int col, double value)
		{
			var target = new[] { value };
			Marshal.Copy(target, 0, new IntPtr(mat.DataPointer.ToInt32() + (row * mat.Cols + col) * mat.ElementSize), 1);
		}
	}
}
