using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LeagueHelper
{
    public static class Extensions
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindow(string strClassName, string strWindowName);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);

        public struct Rect
        {
            public int Left { get; set; }
            public int Top { get; set; }
            public int Right { get; set; }
            public int Bottom { get; set; }
        }

        public static Point GetWindowLocation(this IntPtr main)
        {
            Rect MyRect = new Rect();
            GetWindowRect(main, ref MyRect);
            return new Point(MyRect.Left, MyRect.Top);
        }

        public static Point Center(this Image main, Point origin)
        {
            return new Point(origin.X + p2p(main.Width / 2), origin.Y + p2p(main.Height / 2));
        }

        public static Point AlmostCenter(this Image main, Point origin, int diffusion)
        {
            Random r = new Random(Environment.TickCount);
            Point _base = Center(main, origin);
            _base.X = r.Next(_base.X - diffusion, _base.X + diffusion);
            _base.Y = r.Next(_base.Y - diffusion, _base.Y - diffusion);
            return _base;
        }

        public static int p2p(int pixels)
        {
            return (pixels * 72 / 96);
        }

        private const int SW_SHOWNORMAL = 1;
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetForegroundWindow(IntPtr hwnd);
        public static void SwitchTo(this Process main)
        {
            ShowWindow(main.MainWindowHandle, SW_SHOWNORMAL);
            SetForegroundWindow(main.MainWindowHandle);
        }
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        public static string GetActiveWindowTitle()
        {
            const int nChars = 256;
            StringBuilder Buff = new StringBuilder(nChars);
            IntPtr handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }
            return null;
        }
    }
}
