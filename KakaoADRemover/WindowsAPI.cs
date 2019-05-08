using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KakaoADRemover
{
    class WindowsAPI
    {
        // https://docs.microsoft.com/en-us/windows/desktop/api/winuser/nf-winuser-setwindowlonga
        enum WindowLongFlags : Int32
        {
            GWL_EXSTYLE = -20,
            GWLP_HINSTANCE = -6,
            GWLP_HWNDPARENT = -8,
            GWL_ID = -12,
            GWL_STYLE = -16,
            GWL_USERDATA = -21,
            GWL_WNDPROC = -4,
            DWLP_USER = 0x8,
            DWLP_MSGRESULT = 0x0,
            DWLP_DLGPROC = 0x4
        }

        // https://docs.microsoft.com/en-us/windows/desktop/winmsg/extended-window-styles
        enum ExtendedWindowStyles : Int64
        {
            WS_EX_ACCEPTFILES = 0x00000010L,
            WS_EX_APPWINDOW = 0x00040000L,
            WS_EX_CLIENTEDGE = 0x00000200L,
            WS_EX_COMPOSITED = 0x02000000L,
            WS_EX_CONTEXTHELP = 0x00000400L,
            WS_EX_CONTROLPARENT = 0x00010000L,
            WS_EX_DLGMODALFRAME = 0x00000001L,
            WS_EX_LAYERED = 0x00080000,
            WS_EX_LAYOUTRTL = 0x00400000L,
            WS_EX_LEFT = 0x00000000L,
            WS_EX_LEFTSCROLLBAR = 0x00004000L,
            WS_EX_LTRREADING = 0x00000000L,
            WS_EX_MDICHILD = 0x00000040L,
            WS_EX_NOACTIVATE = 0x08000000L,
            WS_EX_NOINHERITLAYOUT = 0x00100000L,
            WS_EX_NOPARENTNOTIFY = 0x00000004L,
            WS_EX_NOREDIRECTIONBITMAP = 0x00200000L,
            WS_EX_OVERLAPPEDWINDOW = (WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE),
            WS_EX_PALETTEWINDOW = (WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST),
            WS_EX_RIGHT = 0x00001000L,
            WS_EX_RIGHTSCROLLBAR = 0x00000000L,
            WS_EX_RTLREADING = 0x00002000L,
            WS_EX_STATICEDGE = 0x00020000L,
            WS_EX_TOOLWINDOW = 0x00000080L,
            WS_EX_TOPMOST = 0x00000008L,
            WS_EX_TRANSPARENT = 0x00000020L,
            WS_EX_WINDOWEDGE = 0x00000100L
        }

        // https://docs.microsoft.com/en-us/windows/desktop/winmsg/window-notifications
        // https://docs.microsoft.com/en-us/windows/desktop/winmsg/window-messages
        enum WindowMessages : Int32
        {
            WM_CLOSE = 0x0010,
            WM_GETTEXT = 0x000D
        }

        public class WindowInfo
        {
            private const int MAX_CHARS = 64;
            private IntPtr handle = IntPtr.Zero;
            private string title = null;
            private string classname = null;

            public IntPtr Handle
            {
                // no set
                get { return handle; }
            }

            public string Title
            {
                // no set
                get { return (string.IsNullOrEmpty(title)) ? "" : title; }
            }
            
            public string ClassName
            {
                // no set
                get { return (string.IsNullOrEmpty(classname)) ? "" : classname; }
            }

            public static WindowInfo getInfo(IntPtr handle)
            {
                WindowInfo info = new WindowInfo();
                info.handle = handle;

                StringBuilder sbWindowTitle = new StringBuilder();
                //SendMessage();
                info.title = sbWindowTitle.ToString();

                StringBuilder sbWindowClassName = new StringBuilder();
                //GetClassName();
                info.classname = sbWindowClassName.ToString();

                return info;
            }
        }

        // https://www.pinvoke.net/default.aspx/user32.SendMessage
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, StringBuilder lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, ref IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, IntPtr lParam);

        // https://www.pinvoke.net/default.aspx/user32.getclassname
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        // https://www.pinvoke.net/default.aspx/user32.getdesktopwindow
        [DllImport("user32.dll", EntryPoint = "GetDesktopWindow")]
        public static extern IntPtr GetDesktopWindow();

        // https://www.pinvoke.net/default.aspx/user32.FindWindowEx
        [DllImport("user32.dll", EntryPoint = "FindWindowEx")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        // https://www.pinvoke.net/default.aspx/user32.showwindow
        [DllImport("user32.dll", EntryPoint = "ShowWindow")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        // https://www.pinvoke.net/default.aspx/user32.getwindowrect
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect); // pinvoke RECT 항목 참조하여 만들것...

    }
}
