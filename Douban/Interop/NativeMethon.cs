using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;

namespace Douban.Interop
{
    public static partial class NativeMethon
    {
        private const int DWM_BB_ENABLE = 0x00000001;
        private const int DWM_BB_BLURREGION = 0x00000002;
        private const int DWM_BB_TRANSITIONONMAXIMIZED = 0x00000004;

        struct DWM_BLURBEHIND
        {
            public int dwFlags;
            public bool fEnable;
            public IntPtr hRgnBlur;
            public bool fTransitionOnMaximized;
        };

        [DllImport("Gdi32.dll")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect,
            int nWidthEllipse,
            int nHeightEllipse);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        private static extern IntPtr DwmIsCompositionEnabled(ref bool pfEnabled);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        private static extern IntPtr DwmEnableBlurBehindWindow(IntPtr Hwnd, ref DWM_BLURBEHIND pBlurBehind);

        public static bool ExtendAeroGlass(Window window, bool Enable)
        {
            if (Environment.OSVersion.Version.Major < 6)
                return false;
            bool IsEnableAero = false;

            DwmIsCompositionEnabled(ref IsEnableAero);
            if (!IsEnableAero) return IsEnableAero;

            IntPtr hwnd = new WindowInteropHelper(window).Handle;
            if (hwnd == IntPtr.Zero)
                return false;


            DWM_BLURBEHIND blurBehind = new DWM_BLURBEHIND();
            blurBehind.dwFlags = DWM_BB_ENABLE | DWM_BB_TRANSITIONONMAXIMIZED;
            blurBehind.fEnable = Enable;
            blurBehind.fTransitionOnMaximized = false;

            IntPtr region = CreateRoundRectRgn((int)window.Left, (int)window.Top, (int)window.Width, (int)window.Height, 4, 4);
            if (region != IntPtr.Zero && Enable)
            {
                blurBehind.dwFlags |= DWM_BB_BLURREGION;
                blurBehind.hRgnBlur = region;
                
            }
            IntPtr result = DwmEnableBlurBehindWindow(hwnd, ref blurBehind);
            if (result == IntPtr.Zero)
            {
                return false;
            }
            return true;
        }
    }
}
