using System.Runtime.InteropServices;

namespace Win32;

public struct RECT
{
    public int Left { get; set; }
    public int Top { get; set; }
    public int Right { get; set; }
    public int Bottom { get; set; }
}

public struct WINDOWINFO
{
    public int cbSize;
    public RECT rcWindow;
    public RECT rcClient;
    public int dwStyle;
    public int dwExStyle;
    public int dwWindowStatus;
    public uint cxWindowBorders;
    public uint cyWindowBorders;
    public ushort atomWindowType;
    public ushort wCreatorVersion;
}

public static class Interop
{
    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool GetWindowInfo(IntPtr hWnd, ref WINDOWINFO wi);

    //https://msdn.microsoft.com/en-us/library/windows/desktop/ms633545%28v=vs.85%29.aspx
    [DllImport("user32.dll")]
    public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
    public static IntPtr HWND_TOPMOST = new IntPtr(-1);
    public static IntPtr HWND_TOP = new IntPtr(0);
    public static IntPtr HWND_NOTOPMOST = new IntPtr(-1);
    public static IntPtr HWND_BOTTOM = new IntPtr(2);
    public const uint SWP_NOACTIVATE = 0x0010;
    public const uint SWP_NOMOVE = 0x0002;
    public const uint SWP_NOSIZE = 0x0001;
    public const uint SWP_NOZORDER = 0x0004;
    public const uint SWP_FRAMECHANGED = 0x0020;


    [DllImport("User32.dll")]
    public static extern int SetWindowLongPtr(IntPtr hWnd, int nIndex, int dwNewLong);

    public const int GWL_STYLE = (-16);
    public const int GWL_EXSTYLE = -0x14;
    public const int WS_EX_APPWINDOW = 0x40000;
    public const int WS_EX_TOOLWINDOW = 0x00000080;

    public const int WS_BORDER = 0x00800000;
    public const int WS_CAPTION = 0x00C00000;
    

    public const int WS_SYSMENU = 0x00080000;
    public const int WS_MINIMIZEBOX = 0x00020000;
    public const int WS_MAXIMIZEBOX = 0x00010000;

    public const int WS_OVERLAPPED = 0x00000000;
    public const int WS_THICKFRAME = 0x00040000;

    public const int WS_TILEDWINDOW  = (WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX);
}