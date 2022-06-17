using System;
using System.Runtime.InteropServices;
using System.Text;

namespace TCATimerWPF
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Rect
    {
        public int X1;
        public int Y1;
        public int X2;
        public int Y2;

        public uint Width { get { return (uint)(X2 - X1); } }
        public uint Height { get { return (uint)(Y2 - Y1); } }
    }

    public enum GetWindowCmd : uint
    {
        GW_HWNDFIRST = 0,
        GW_HWNDLAST = 1,
        GW_HWNDNEXT = 2,
        GW_HWNDPREV = 3,
        GW_OWNER = 4,
        GW_CHILD = 5,
        GW_ENABLEDPOPUP = 6
    }

    public enum GetWindowLong : int
    {
        GWL_WNDPROC = -4,
        GWL_HINSTANCE = -6,
        GWL_ID = -12,
        GWL_STYLE = -16,
        GWL_EXSTYLE = -20,
        GWL_USERDATA = -21
    }

    public static class StaticPInvoke
    {
        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);


        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool EnumChildWindows(IntPtr hWndParent, EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetWindow(IntPtr hWnd, GetWindowCmd uCmd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetClientRect(IntPtr hWnd, out Rect lpRect);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetWindowRect(IntPtr hWnd, out Rect lpRect);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowLong(IntPtr hWnd, GetWindowLong nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint SetWindowLong(IntPtr hWnd, GetWindowLong nIndex, uint dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, uint Width, uint Height, bool Repaint);
    }

    public static class Win32Utils
    {

        const int WS_EX_TRANSPARENT = 0x00000020;

        public static void SetWindowExTransparent(IntPtr hwnd)
        {
            var extendedStyle = StaticPInvoke.GetWindowLong(hwnd, GetWindowLong.GWL_EXSTYLE);
            StaticPInvoke.SetWindowLong(hwnd, GetWindowLong.GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
        }

    }
}
