namespace Win32;

public static class Operations
{
	public static void EnableWindowTitleBar(IntPtr windowHandle, bool apply, out bool success)
	{
		success = false;
		if (apply && windowHandle != null && windowHandle != IntPtr.Zero)
		{
        	WINDOWINFO wi = new WINDOWINFO();
        	if (Interop.GetWindowInfo(windowHandle, ref wi))
        	{
        		var result = Interop.SetWindowLongPtr(windowHandle, Interop.GWL_STYLE, wi.dwStyle | Interop.WS_TILEDWINDOW);
        		Interop.SetWindowPos(windowHandle, Interop.HWND_TOP, 0, 0, 0, 0, Interop.SWP_NOACTIVATE | Interop.SWP_NOZORDER | Interop.SWP_NOSIZE | Interop.SWP_NOMOVE | Interop.SWP_FRAMECHANGED);
        		success = result != 0;
        	}
		}
	}

    public static void HasBorder(IntPtr windowHandle, bool check, out bool result)
    {
        result = false;
        if (check && windowHandle != null && windowHandle != IntPtr.Zero)
        {
            WINDOWINFO wi = new WINDOWINFO();
            if (Interop.GetWindowInfo(windowHandle, ref wi))
            {
                result = (wi.dwStyle & Interop.WS_BORDER) != 0;
            }
        }
    }

}