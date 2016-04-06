using System;
using System.Runtime.InteropServices;

namespace Winterdom.Viasfora.Design {
  internal static class NativeMethods {
    public const int WM_GETDLGCODE = 0x0087;
    public const int WM_SETFOCUS = 0x0007;

    public const int DLGC_WANTARROWS = 0x0001;
    public const int DLGC_WANTTAB = 0x0002;
    public const int DLGC_WANTCHARS = 0x0080;
    public const int DLGC_WANTALLKEYS = 0x0004;

    public const int GA_ROOT = 2;

    [DllImport("user32.dll", ExactSpelling = true)]
    internal static extern IntPtr GetAncestor(IntPtr hWnd, int flags);

    [DllImport("user32.dll", ExactSpelling = true)]
    internal static extern IntPtr GetNextDlgTabItem(IntPtr hDlg, IntPtr hCtl, [MarshalAs(UnmanagedType.Bool)] bool bPrevious);

    [DllImport("user32.dll")]
    public static extern void SetFocus(IntPtr hwnd);
  }
}