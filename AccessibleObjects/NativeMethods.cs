using System;
using System.Runtime.InteropServices;

namespace AccessibleObjects
{
	static class NativeMethods
	{
		[DllImport("user32.dll")]
		public static extern IntPtr WindowFromPoint(POINT Point);

		[DllImport("user32.dll")]
		public static extern bool GetCursorPos(out NativeMethods.POINT lpPoint);


		[StructLayout(LayoutKind.Sequential)]
		public struct POINT
		{
			public int X;
			public int Y;

			public POINT(int x, int y)
			{
				this.X = x;
				this.Y = y;
			}

		}
	}
}
