using System.Runtime.InteropServices;

namespace Tamarix.DesktopWindows
{
    public class WinFormNativeWindow : NativeWindow
    {
        #region WIN32 API
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct CREATESTRUCT
        {
            public IntPtr lpCreateParams;
            public IntPtr hInstance;
            public IntPtr hMenu;
            public IntPtr hwndParent;
            public int cy;
            public int cx;
            public int y;
            public int x;
            public int style;
            public IntPtr lpszName;
            public IntPtr lpszClass;
            public int dwExStyle;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct MARGINS
        {
            public int leftWidth;
            public int rightWidth;
            public int topHeight;
            public int bottomHeight;
        }



        [DllImport("dwmapi.dll")]
        static extern int DwmExtendFrameIntoClientArea(nint hwnd, ref MARGINS margins);
        [DllImport("user32.dll")]
        static extern int GetClientRect(nint hwnd, ref RECT rect);
        public enum DWMWINDOWATTRIBUTE : uint
        {
            NCRenderingEnabled = 1,
            NCRenderingPolicy,
            TransitionsForceDisabled,
            AllowNCPaint,
            CaptionButtonBounds,
            NonClientRtlLayout,
            ForceIconicRepresentation,
            Flip3DPolicy,
            ExtendedFrameBounds,
            HasIconicBitmap,
            DisallowPeek,
            ExcludedFromPeek,
            Cloak,
            Cloaked,
            FreezeRepresentation,
            /* extra */

            DWMWA_PASSIVE_UPDATE_MODE,
            DWMWA_USE_HOSTBACKDROPBRUSH,
            DWMWA_USE_IMMERSIVE_DARK_MODE = 20,
            DWMWA_WINDOW_CORNER_PREFERENCE = 33,
            DWMWA_BORDER_COLOR,
            DWMWA_CAPTION_COLOR,
            DWMWA_TEXT_COLOR,
            DWMWA_VISIBLE_FRAME_BORDER_THICKNESS,
            DWMWA_LAST
            /* https://docs.microsoft.com/en-us/windows/win32/api/dwmapi/ne-dwmapi-dwmwindowattribute */

        }

        [DllImport("dwmapi.dll", PreserveSig = true)]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE attr, ref int attrValue, int attrSize);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]


        private static extern bool SetWindowPos(nint hWnd, nint hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);
        [Flags()]
        private enum RedrawWindowFlags : uint
        {
            /// <summary>
            /// Invalidates the rectangle or region that you specify in lprcUpdate or hrgnUpdate.
            /// You can set only one of these parameters to a non-NULL value. If both are NULL, RDW_INVALIDATE invalidates the entire window.
            /// </summary>
            Invalidate = 0x1,

            /// <summary>Causes the OS to post a WM_PAINT message to the window regardless of whether a portion of the window is invalid.</summary>
            InternalPaint = 0x2,

            /// <summary>
            /// Causes the window to receive a WM_ERASEBKGND message when the window is repainted.
            /// Specify this value in combination with the RDW_INVALIDATE value; otherwise, RDW_ERASE has no effect.
            /// </summary>
            Erase = 0x4,

            /// <summary>
            /// Validates the rectangle or region that you specify in lprcUpdate or hrgnUpdate.
            /// You can set only one of these parameters to a non-NULL value. If both are NULL, RDW_VALIDATE validates the entire window.
            /// This value does not affect internal WM_PAINT messages.
            /// </summary>
            Validate = 0x8,

            NoInternalPaint = 0x10,

            /// <summary>Suppresses any pending WM_ERASEBKGND messages.</summary>
            NoErase = 0x20,

            /// <summary>Excludes child windows, if any, from the repainting operation.</summary>
            NoChildren = 0x40,

            /// <summary>Includes child windows, if any, in the repainting operation.</summary>
            AllChildren = 0x80,

            /// <summary>Causes the affected windows, which you specify by setting the RDW_ALLCHILDREN and RDW_NOCHILDREN values, to receive WM_ERASEBKGND and WM_PAINT messages before the RedrawWindow returns, if necessary.</summary>
            UpdateNow = 0x100,

            /// <summary>
            /// Causes the affected windows, which you specify by setting the RDW_ALLCHILDREN and RDW_NOCHILDREN values, to receive WM_ERASEBKGND messages before RedrawWindow returns, if necessary.
            /// The affected windows receive WM_PAINT messages at the ordinary time.
            /// </summary>
            EraseNow = 0x200,

            Frame = 0x400,

            NoFrame = 0x800
        }

        [DllImport("user32.dll")]
        static extern bool RedrawWindow(nint hWnd, nint lprcUpdate, nint hrgnUpdate, RedrawWindowFlags flags);
        // This static method is required because legacy OSes do not support
        // SetWindowLongPtr
        public static IntPtr SetWindowLongPtr(nint hWnd, int nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size == 8)
                return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
            else
                return new IntPtr(SetWindowLong32(hWnd, nIndex, dwNewLong.ToInt32()));
        }

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern int SetWindowLong32(nint hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLongPtr64(nint hWnd, int nIndex, IntPtr dwNewLong);
        [StructLayout(LayoutKind.Sequential)]
        struct PAINTSTRUCT
        {
            public IntPtr hdc;
            public bool fErase;
            public RECT rcPaint;
            public bool fRestore;
            public bool fIncUpdate;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)] public byte[] rgbReserved;
        }
        [DllImport("user32.dll")]
        static extern IntPtr BeginPaint(IntPtr hwnd, out PAINTSTRUCT lpPaint);
        [DllImport("user32.dll")]
        static extern bool EndPaint(IntPtr hWnd, [In] ref PAINTSTRUCT lpPaint);
        const int COLOR_WINDOWTEXT = 8;
        [DllImport("gdi32.dll")]
        static extern IntPtr CreateSolidBrush(uint crColor);

        [DllImport("user32.dll")]
        static extern int FillRect(IntPtr hDC, [In] ref RECT lprc, IntPtr hbr);
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left, Top, Right, Bottom;

            public RECT(int left, int top, int right, int bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }

            public RECT(System.Drawing.Rectangle r) : this(r.Left, r.Top, r.Right, r.Bottom) { }

            public int X
            {
                get { return Left; }
                set { Right -= (Left - value); Left = value; }
            }

            public int Y
            {
                get { return Top; }
                set { Bottom -= (Top - value); Top = value; }
            }

            public int Height
            {
                get { return Bottom - Top; }
                set { Bottom = value + Top; }
            }

            public int Width
            {
                get { return Right - Left; }
                set { Right = value + Left; }
            }

            public System.Drawing.Point Location
            {
                get { return new System.Drawing.Point(Left, Top); }
                set { X = value.X; Y = value.Y; }
            }

            public System.Drawing.Size Size
            {
                get { return new System.Drawing.Size(Width, Height); }
                set { Width = value.Width; Height = value.Height; }
            }

            public static implicit operator System.Drawing.Rectangle(RECT r)
            {
                return new System.Drawing.Rectangle(r.Left, r.Top, r.Width, r.Height);
            }

            public static implicit operator RECT(System.Drawing.Rectangle r)
            {
                return new RECT(r);
            }

            public static bool operator ==(RECT r1, RECT r2)
            {
                return r1.Equals(r2);
            }

            public static bool operator !=(RECT r1, RECT r2)
            {
                return !r1.Equals(r2);
            }

            public bool Equals(RECT r)
            {
                return r.Left == Left && r.Top == Top && r.Right == Right && r.Bottom == Bottom;
            }

            public override bool Equals(object obj)
            {
                if (obj is RECT)
                    return Equals((RECT)obj);
                else if (obj is System.Drawing.Rectangle)
                    return Equals(new RECT((System.Drawing.Rectangle)obj));
                return false;
            }

            public override int GetHashCode()
            {
                return ((System.Drawing.Rectangle)this).GetHashCode();
            }

            public override string ToString()
            {
                return string.Format(System.Globalization.CultureInfo.CurrentCulture, "{{Left={0},Top={1},Right={2},Bottom={3}}}", Left, Top, Right, Bottom);
            }
        }
        [StructLayout(LayoutKind.Sequential)]
        struct NCCALCSIZE_PARAMS
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public RECT[] rgrc;
            public WINDOWPOS lppos;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWPOS
        {
            public IntPtr hwnd;
            public IntPtr hwndInsertAfter;
            public int x;
            public int y;
            public int cx;
            public int cy;
            public uint flags;
        }
        public enum WindowLongFlags : int
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
        [Flags]
        enum WindowStyles : uint
        {
            WS_OVERLAPPED = 0x00000000,
            WS_POPUP = 0x80000000,
            WS_CHILD = 0x40000000,
            WS_MINIMIZE = 0x20000000,
            WS_VISIBLE = 0x10000000,
            WS_DISABLED = 0x08000000,
            WS_CLIPSIBLINGS = 0x04000000,
            WS_CLIPCHILDREN = 0x02000000,
            WS_MAXIMIZE = 0x01000000,
            WS_BORDER = 0x00800000,
            WS_DLGFRAME = 0x00400000,
            WS_VSCROLL = 0x00200000,
            WS_HSCROLL = 0x00100000,
            WS_SYSMENU = 0x00080000,
            WS_THICKFRAME = 0x00040000,
            WS_GROUP = 0x00020000,
            WS_TABSTOP = 0x00010000,

            WS_MINIMIZEBOX = 0x00020000,
            WS_MAXIMIZEBOX = 0x00010000,

            WS_CAPTION = WS_BORDER | WS_DLGFRAME,
            WS_TILED = WS_OVERLAPPED,
            WS_ICONIC = WS_MINIMIZE,
            WS_SIZEBOX = WS_THICKFRAME,
            WS_TILEDWINDOW = WS_OVERLAPPEDWINDOW,

            WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,
            WS_POPUPWINDOW = WS_POPUP | WS_BORDER | WS_SYSMENU,
            WS_CHILDWINDOW = WS_CHILD,
            WS_CUSTOM_BORDERLESS = WS_OVERLAPPED | WS_THICKFRAME | WS_CAPTION | WS_SYSMENU | WS_MAXIMIZEBOX | WS_MINIMIZEBOX,

            //Extended Window Styles

            WS_EX_DLGMODALFRAME = 0x00000001,
            WS_EX_NOPARENTNOTIFY = 0x00000004,
            WS_EX_TOPMOST = 0x00000008,
            WS_EX_ACCEPTFILES = 0x00000010,
            WS_EX_TRANSPARENT = 0x00000020,

            //#if(WINVER >= 0x0400)

            WS_EX_MDICHILD = 0x00000040,
            WS_EX_TOOLWINDOW = 0x00000080,
            WS_EX_WINDOWEDGE = 0x00000100,
            WS_EX_CLIENTEDGE = 0x00000200,
            WS_EX_CONTEXTHELP = 0x00000400,

            WS_EX_RIGHT = 0x00001000,
            WS_EX_LEFT = 0x00000000,
            WS_EX_RTLREADING = 0x00002000,
            WS_EX_LTRREADING = 0x00000000,
            WS_EX_LEFTSCROLLBAR = 0x00004000,
            WS_EX_RIGHTSCROLLBAR = 0x00000000,

            WS_EX_CONTROLPARENT = 0x00010000,
            WS_EX_STATICEDGE = 0x00020000,
            WS_EX_APPWINDOW = 0x00040000,

            WS_EX_OVERLAPPEDWINDOW = (WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE),
            WS_EX_PALETTEWINDOW = (WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST),
            //#endif /* WINVER >= 0x0400 */

            //#if(WIN32WINNT >= 0x0500)

            WS_EX_LAYERED = 0x00080000,
            //#endif /* WIN32WINNT >= 0x0500 */

            //#if(WINVER >= 0x0500)

            WS_EX_NOINHERITLAYOUT = 0x00100000, // Disable inheritence of mirroring by children
            WS_EX_LAYOUTRTL = 0x00400000, // Right to left mirroring
                                          //#endif /* WINVER >= 0x0500 */

            //#if(WIN32WINNT >= 0x0500)

            WS_EX_COMPOSITED = 0x02000000,
            WS_EX_NOACTIVATE = 0x08000000
            //#endif /* WIN32WINNT >= 0x0500 */

        }
        public const int WM_NCHITTEST = 0x0084;
        public const int WM_NCCALCSIZE = 0x0083;
        public const int WM_CREATE = 0x0001;
        public const int WM_ERASEBKGND = 0x0014;
        public const int WM_MOUSELEAVE = 0x02A3;
        public const int WM_ACTIVATE = 0x0006;
        public const int WM_PAINT = 0xF;

        public static IntPtr HT_LEFT = new IntPtr(10);
        public static IntPtr HT_RIGHT = new IntPtr(11);
        public static IntPtr HT_TOP = new IntPtr(12);
        public static IntPtr HT_TOPLEFT = new IntPtr(13);
        public static IntPtr HT_TOPRIGHT = new IntPtr(14);
        public static IntPtr HT_BOTTOM = new IntPtr(15);
        public static IntPtr HT_BOTTOMLEFT = new IntPtr(16);
        public static IntPtr HT_BOTTOMRIGHT = new IntPtr(17);

        private static IntPtr HT_CAPTION = new IntPtr(2);
        private static IntPtr IntPtrOne = new IntPtr(1);

        #endregion


        /// <summary>
        /// Sometimes it is smoother to use hardware resizing window, but less stabble and wobbly
        /// </summary>
        public static bool UseHardwareResizing = false;
        Window window;
        public WinFormNativeWindow(Window window)
        {
            this.window = window;
            this.AssignHandle(window._window.Native.Win32.Value.Hwnd);
            _window_Load();
        }

        private void _window_Load()
        {
            //Console.WriteLine("Extending to client area");

            var v = 2;
            DwmSetWindowAttribute(this.Handle, (DWMWINDOWATTRIBUTE)2, ref v, 4);
            var v2 = 1;
            DwmSetWindowAttribute(this.Handle, DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE, ref v2, 4);
            SetWindowLongPtr(Handle, (int)WindowLongFlags.GWL_STYLE, new IntPtr((long)WindowStyles.WS_CUSTOM_BORDERLESS));
            var margins = new MARGINS()
            {
                leftWidth = 0,
                rightWidth = 0,
                topHeight = 0,
                bottomHeight = 0,
            };
            DwmExtendFrameIntoClientArea(window._window.Native.Win32.Value.Hwnd, ref margins);
            forceChildRefresh();
            if (UseHardwareResizing)
            {
                window._window.Resize -= window.Resize;
                window._window.FramebufferResize -= window.Resize;
            }

        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                //case WM_ERASEBKGND:
                //    m.Result = IntPtr.Zero;
                //    return;
                //case WM_PAINT:
                //    PAINTSTRUCT ps;
                //    IntPtr hdc = BeginPaint(Handle, out ps);
                //    // Paint everything black
                //    FillRect(hdc, ref ps.rcPaint, CreateSolidBrush(120));
                //    EndPaint(Handle, ref ps);
                //    m.Result = IntPtr.Zero;
                //    break;
                //case WM_ACTIVATE:

                //    var margins = new MARGINS()
                //    {
                //        leftWidth = 1,
                //        rightWidth = 1,
                //        topHeight = 1,
                //        bottomHeight = 1,
                //    };
                //    DwmExtendFrameIntoClientArea(window._window.Native.Win32.Value.Hwnd, ref margins);
                //    break;
                case WM_NCHITTEST:
                    // How it works is by tricking Windows OS that we're actually clicking on the window border 
                    var isMaximized = NativeMethods.isMaximized(Handle);
                    int x = 0, y = 0; // client mouse position
                    var isTitleBar = window.IsHoveringTitleBar(out x, out y);
                    DefWndProc(ref m);
                    if (m.Result.ToInt32() == 1 /*HT_CLIENT*/)
                    {
                        const int size_margin_v = 8;
                        const int size_margin_h = 18;
                        if (!isMaximized)
                        {
                            if (window.HasCustomTitleBar && y < size_margin_v)
                            {
                                m.Result = HT_TOP;
                                if (x < size_margin_h)
                                    m.Result = HT_TOPLEFT;
                                if (x > window.Width - size_margin_h)
                                    m.Result = HT_TOPRIGHT;
                            }
                            else if (isMaximized == false)
                            {
                                if (window.HasCustomTitleBar && y > window.Height - size_margin_v)
                                {
                                    m.Result = HT_BOTTOM;
                                    if (x < size_margin_h)
                                        m.Result = HT_BOTTOMLEFT;
                                    if (x > window.Width - size_margin_h)
                                        m.Result = HT_BOTTOMRIGHT;
                                }
                                else
                                {
                                    if (x < size_margin_h)
                                        m.Result = HT_LEFT;
                                    else if (x > window.Width - size_margin_v)
                                        m.Result = HT_RIGHT;
                                    else if (isTitleBar)
                                    {
                                        m.Result = HT_CAPTION;
                                    }
                                }
                            }
                        }
                        else if (isTitleBar)
                        {
                            m.Result = HT_CAPTION;
                        }
                        if (m.Result.ToInt32() != 1)
                        {  // if the result is changed
                            var evts = new Tamarix.Views.UIEvent()
                            {
                                x = -1,
                                y = -1
                            };
                            window.OnMouseMove(ref evts);
                        }

                        return;
                    }
                    break;

                case WM_NCCALCSIZE:
                    // This is to hide the built in title bar but doesnt remove it
                    if (window.HasCustomTitleBar)
                    {
                        NCCALCSIZE_PARAMS lParams = (NCCALCSIZE_PARAMS)m.GetLParam(typeof(NCCALCSIZE_PARAMS));
                        if (!NativeMethods.isMaximized(Handle))
                        {
                            lParams.rgrc[0].Top -= 45;
                            //lParams.rgrc[0].Right += 11;
                            //lParams.rgrc[0].Left -= 11;
                            //lParams.rgrc[0].Bottom += 11;

                            //lParams.rgrc[0].Top -= 40;
                            //lParams.rgrc[0].Right += 10;
                            //lParams.rgrc[0].Left -= 10;
                            //lParams.rgrc[0].Bottom += 10;
                        }
                        else
                        {
                            lParams.rgrc[0].Top -= 35;
                            lParams.rgrc[0].Right += 5;
                            lParams.rgrc[0].Left -= 5;
                            lParams.rgrc[0].Bottom += 5;
                        }
                        if (UseHardwareResizing)
                        {
                            var w = lParams.rgrc[0].Width - 12 - 8;
                            var h = lParams.rgrc[0].Height - 42 - 8;
                            window.Resize(w, h);
                        }
                        Marshal.StructureToPtr(lParams, m.LParam, false);
                        m.Result = IntPtr.Zero; // WVR_REDRAW

                    }
                    break;

                case WM_MOUSELEAVE:
                    var evt = new Tamarix.Views.UIEvent() { x = -1, y = -1 };

                    window.OnMouseMove(ref evt);
                    break;
            }
            base.WndProc(ref m);
        }


        public const int SWP_NOMOVE = 0x0002;
        public const int SWP_NOACTIVATE = 0x0010;
        void forceChildRefresh()
        {
            RECT rc = new RECT() { Height = 32, Width = 32 };
            _ = GetClientRect(Handle, ref rc);
            int width = rc.Right - rc.Left;
            int height = rc.Bottom - rc.Top;
            //window.Resize(width, height);
            SetWindowPos(Handle, 0, 0, 0, width, height, 0x0040);
            SetWindowPos(Handle, 0, 0, 0, width + 1, height + 1, SWP_NOMOVE | SWP_NOACTIVATE);
            SetWindowPos(Handle, 0, 0, 0, width, height, SWP_NOMOVE | SWP_NOACTIVATE);

            //RedrawWindow(Handle, 0, 0, RedrawWindowFlags.Invalidate);

        }

    }
}
