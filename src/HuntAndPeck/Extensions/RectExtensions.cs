using System;
using System.Windows;
using HuntAndPeck.NativeMethods;

namespace HuntAndPeck.Extensions
{
    public static class RectExtensions
    {

        public static Rect PhysicalToLogicalRect(this Rect source, IntPtr hWnd)
        {
            POINT tl = source.TopLeft;
            POINT br = source.BottomRight;
            if (User32.PhysicalToLogicalPoint(hWnd, out tl) &&
                User32.PhysicalToLogicalPoint(hWnd, out br))
            {
                return new Rect(tl, br);
            }

            return Rect.Empty;
        }

        public static Rect ScreenToWindowCoordinates(this Rect source, Rect windowRect)
        {
            var result = new Rect(source.TopLeft, source.BottomRight);
            result.X -= windowRect.X;
            result.Y -= windowRect.Y;

            return result;
        }
    }
}
