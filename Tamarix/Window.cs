using Silk.NET.Core;
using Silk.NET.GLFW;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.SDL;
using Silk.NET.Windowing;
using SkiaSharp;
using Tamarix.Inspector;
using Tamarix.Views;
using Dialog = Tamarix.Views.Dialog;
using MouseButton = Tamarix.Views.MouseButton;
using win = Silk.NET.Windowing.Window;
namespace Tamarix
{
    public class Window : StackLayout
    {

        public static Window? Current;
        public DialogContainerStack DialogContainer;
        public TWindowManager? WindowManager;
        public TamarixApp App;
        /* WOYY LANJUTIN F5 show /hide inspector*/
        public Window? inspectorWindow;
        public bool HasCustomTitleBar { get; set; }
        public View TitleBar;
        public bool IsClosed { get; set; } = false;
        /// <summary>
        /// Title of the window (desktop only)
        /// </summary>
        public string Title { get; set; }
        public string? Icon { get; }
        public IView? SilkView { get; }

        /// <summary>
        /// Internal Silk.NET window implementation
        /// </summary>
        public IView _window;

        public View? CurrentViewFocus;

        /// <summary>
        /// Background color of the window
        /// </summary>
        public Color? BackgroundColor;

        /// <summary>
        /// The skia context, unused on android
        /// </summary>
        public GRContext? SkiaCtx;

        /// <summary>
        /// Android manages the skia context and surface
        /// </summary>
        public bool IsAndroid = false; // this is so that we dont need to manage silk or skia
        public SKSurface? Surface;

        /// <summary>
        /// Silk.NET input context
        /// </summary>
        public IInputContext _input;

#if ANDROID
        /// <summary>
        /// The android constructor
        /// </summary>
        /// <param name="title"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Window(string title, int width, int height)
        {
            this.Title = title;
            this.Width = width;
            this.Height = height;
            IsAndroid = true;
        }
#endif
        /// <summary>
        /// Creates a new window. Will initialize differently on android, on android don't use icon and view, set them to null
        /// </summary>
        /// <param name="title">Title of the window (desktop only)</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="icon">Path to the icon relative to executable (desktop only)</param>
        /// <param name="view">Silk's IView if using Silk.NET (desktop only)</param>
        /// <param name="isAndroid">If the platform is android</param>
        public Window(string title, int width, int height, string? icon = null, IView? view = null, bool isAndroid = false)
        {
            this.Title = title;
            Icon = icon;
            SilkView = view;
            this.Width = width;
            this.Height = height;
            IsAndroid = isAndroid;
            _initialize();
        }

        private void Window_KeyChar(IKeyboard arg1, char arg2)
        {
            var evt = new UIEvent() { c = arg2 };
            OnKeyChar(ref evt);
        }

        private void Window_MouseMove(IMouse arg1, System.Numerics.Vector2 arg2)
        {
            //Console.WriteLine($"{arg2.X} {arg2.Y}");
            SetCursor(MouseCursor.Arrow);
            var evt = new UIEvent()
            {
                x = (int)arg2.X,
                y = (int)arg2.Y
            };
            OnMouseMove(ref evt);
        }

        private void Window_MouseDown(IMouse arg1, Silk.NET.Input.MouseButton arg2)
        {
            var evt = new UIEvent()
            {
                x = (int)arg1.Position.X + X,
                y = (int)arg1.Position.Y + Y
            };
            OnMouseButton(ref evt, arg2 == Silk.NET.Input.MouseButton.Left ? MouseButton.Left : MouseButton.Right, true);
        }

        public override void OnMouseButton(ref UIEvent evt, MouseButton button, bool pressed)
        {
            base.OnMouseButton(ref evt, button, pressed);
        }

        private void Window_MouseUp(IMouse arg1, Silk.NET.Input.MouseButton arg2)
        {
            var evt = new UIEvent()
            {
                x = (int)arg1.Position.X,
                y = (int)arg1.Position.Y
            };
            OnMouseButton(ref evt, arg2 == Silk.NET.Input.MouseButton.Left ? MouseButton.Left : MouseButton.Right, false);
        }

        private void Window_KeyUp(IKeyboard arg1, Silk.NET.Input.Key arg2, int arg3)
        {
#if DEBUG
            // Toggle Debug Draw
            if (arg2 == Silk.NET.Input.Key.F3)
                View.DrawDebug = !View.DrawDebug;
            // Toggle Inspector Window
            if (arg2 == Silk.NET.Input.Key.F5)
            {
                if (inspectorWindow != null)
                {
                    // Close inspector
                    App.RemoveWindow(inspectorWindow);
                    inspectorWindow = null;
                }
                else
                {
                    // Show inspector
                    inspectorWindow = new InspectorWindow(this);
                    App.AddWindow(inspectorWindow);
                }
            }
#endif
            var evt = new UIEvent();
            evt.Key = (Key?)arg2;
            OnKeyUp(ref evt);
        }

        private void Window_KeyDown(IKeyboard arg1, Silk.NET.Input.Key arg2, int arg3)
        {
            var evt = new UIEvent();
            evt.Key = (Key?)arg2;
            OnKeyDown(ref evt);
        }

        /// <summary>
        /// Loads the PNG icon file and set it to windows. 
        /// Doesn't support ICO files. Only PNG
        /// </summary>
        /// <param name="path"></param>
        public void LoadPNGIcon(string path)
        {
            if (_window is IWindow)
            {
                using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
                {
                    var result = StbImageSharp.ImageResult.FromStream(stream);
                    var icon = new RawImage(result.Width, result.Height, result.Data);
                    (_window as IWindow)?.SetWindowIcon(ref icon);
                }
            }
        }


        /// <summary>
        /// Close the window 
        /// </summary>
        public void Close()
        {
            OnClosing();
            _window.Close(); IsClosed = true;
        }

        /// <summary>
        /// Center the window on the screen (desktop only)
        /// </summary>
        public void Center()
        {
            if (_window is IWindow)
                (_window as IWindow)?.Center();
        }

        public void _initialize()
        {
            if (!IsAndroid)
            {
                Silk.NET.Windowing.Glfw.GlfwWindowing.Use();
                var opts = WindowOptions.Default;
                opts.Title = Title;
                opts.Size = new Silk.NET.Maths.Vector2D<int>(Width, Height);
                opts.IsVisible = true;
                
                // IDK why but doing SwapBuffers() manually speeds up drawing
                opts.IsContextControlDisabled = true;
                opts.ShouldSwapAutomatically = false;
                
                if (SilkView == null)
                    _window = win.GetWindowPlatform(false)!.CreateWindow(opts);
                else
                    _window = SilkView;
                _window.Load += Initialize;
                _window.Closing += Dispose;
                _window.Render += (delta) => { if (!_window.IsClosing) Render(delta); };
                _window.Update += Update;
                _window.FramebufferResize += Resize;
                _window.Resize += Resize;
                _window.Initialize();


                _input = _window.CreateInput();
                for (var i = 0; i < _input.Keyboards.Count; i++)
                {
                    _input.Keyboards[i].KeyDown += Window_KeyDown;
                    _input.Keyboards[i].KeyUp += Window_KeyUp;
                    _input.Keyboards[i].KeyChar += Window_KeyChar;
                }
                for (var i = 0; i < _input.Mice.Count; i++)
                {
                    _input.Mice[i].MouseUp += Window_MouseUp;
                    _input.Mice[i].MouseDown += Window_MouseDown;
                    _input.Mice[i].MouseMove += Window_MouseMove;
                }

            }
        }


        /// <summary>
        /// Initialize the window, preparing it's size context and rendering context
        /// </summary>
        public override void Initialize()
        {
            Window.Current = this;
            OnInitialized();
            base.Initialize();
            SkiaInit();
            DialogContainer = new DialogContainerStack();
            AddChild(DialogContainer);
            UpdateChildPos();
            if (Icon != null) LoadPNGIcon(Icon);
        }

        /// <summary>
        /// Display the vendor and GL version (OpenGL only)
        /// </summary>
        public void DisplayVendorAndVersion()
        {
            GL gl = _window.CreateOpenGL();
            Console.WriteLine($"{gl.GetStringS(GLEnum.Vendor)}; {gl.GetStringS(GLEnum.Version)}");
        }

        /// <summary>
        /// Used by android
        /// </summary>
        /// <param name="surface"></param>
        public void RenderOn(SKSurface surface)
        {
            // This should not be needed on android
            //_window.GLContext?.MakeCurrent();

            var ctx = surface.Canvas;
            ctx.DrawColor(BackgroundColor?.SkColor ?? Theme.Current.BackgroundColor.SkColor);
            OnDraw(ctx);
            RenderChildren(ctx);
            //ctx.DrawRect(0, 0, 50, 50, new SKPaint() { Color=SKColors.Red});

            // This should not be needed on android
            //_window.SwapBuffers();
        }

        /// <summary>
        /// Render the window
        /// </summary>
        /// <param name="deltaTime">Time since last frame</param>
        public void Render(double deltaTime)
        {
            _window.GLContext?.MakeCurrent();
            Current = this;
            if (SkiaCtx == null) return;
            var ctx = Surface?.Canvas;
            ctx!.DrawColor(BackgroundColor?.SkColor ?? Theme.Current.BackgroundColor.SkColor);
            OnDraw(ctx!);
            RenderChildren(ctx!);
            SkiaCtx.Flush();
            _window.SwapBuffers();
        }

        /// <summary>
        /// Update the window and its controls
        /// </summary>
        /// <param name="deltaTime">Time since last frame</param>
        public override void Update(double deltaTime)
        {
            //Console.WriteLine(deltaTime);
            Current = this;
            base.Update(deltaTime);
            OnUpdate(deltaTime);
            //UpdateChildPos(); // TODO: should only be called when necessary
            UpdateChildren(deltaTime);
        }

        /// <summary>
        /// Frees the memory from everything
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            SkiaCtx?.Dispose();
            Surface?.Dispose();
        }

        /// <summary>
        /// Resize the window and its canvas
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void Resize(int width, int height)
        {
            Width = width;
            Height = height;
            SkiaResize(width, height);
            UpdateChildPos();

            // Fix window resizing pausing render
            _window.DoUpdate();
            _window.DoRender();
        }

        public void Resize(Vector2D<int> size)
        {
            Resize(size.X, size.Y);
        }

        public void SkiaInit()
        {
            if (SkiaCtx == null && !IsAndroid)
            {
                SkiaCtx = GRContext.CreateGl(options: new GRContextOptions()
                {
                    AvoidStencilBuffers = true,
                });
                if (SkiaCtx == null) throw new Exception("Error creating Skia Context for GL");
                SkiaResize(Width, Height);
            }
        }

        GRGlFramebufferInfo fbi = new(0, (uint)InternalFormat.Rgba8);
        public void SkiaResize(int w, int h)
        {
            if (!IsAndroid)
            {
                if (SkiaCtx == null) SkiaInit();
                var beRenderTarget = new GRBackendRenderTarget(w, h, 0, 0, fbi);

                // Recreate the window surface
                Surface?.Dispose();
                Surface = SKSurface.Create(SkiaCtx, beRenderTarget, GRSurfaceOrigin.BottomLeft, SKColorType.Rgba8888, null, null);
                if (Surface == null)
                {
                    throw new Exception("Error creating window surface");
                }
            }
        }

        public void SetCursor(MouseCursor cursor)
        {
            StandardCursor newCursor;
            switch (cursor)
            {
                case MouseCursor.Arrow:
                    newCursor = StandardCursor.Arrow;
                    break;
                case MouseCursor.Hand:
                    newCursor = StandardCursor.Hand;
                    break;
                case MouseCursor.Edit:
                    newCursor = StandardCursor.IBeam;
                    break;
                default:
                    newCursor = StandardCursor.Arrow;
                    throw new Exception("Cursor not implemented yet");
            }
            if (_window != null && _window is IWindow)
            {

                for (var i = 0; i < _input.Mice.Count; i++)
                {
                    _input.Mice[i].Cursor.Type = CursorType.Standard;
                    _input.Mice[i].Cursor.StandardCursor = newCursor;
                }
            }
        }

        /// <summary>
        /// Check if certain position is a draggable part of the window
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool IsHoveringTitleBar(out int xout, out int yout)
        {
            xout = (int)_input.Mice[0].Position.X;
            yout = (int)_input.Mice[0].Position.Y;
            var ht = _isChildTitleBar(this, (int)xout, (int)yout);
            return ht;
        }
        /// <summary>
        /// Checks if the child at position is a draggable titleBar
        /// </summary>
        /// <param name="view"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private bool _isChildTitleBar(View view, int x, int y)
        {
            if (view.IsContaining(x, y))
            {
                if (view is Container)
                {
                    if (view is SingleChildContainer)
                    {
                        View child = (view as SingleChildContainer)!.Child!;
                        if (child is Container)
                            return _isChildTitleBar(child, x, y);
                        else if (child is IWindowDragger)
                            return child.IsContaining(x, y);
                        else if (view is IWindowDragger)
                            return !child.IsContaining(x, y);
                    }
                    else if (view is MultiChildContainer)
                    {
                        if (view is IWindowDragger)
                        {
                            return (view as MultiChildContainer)!.IsChildrenContaining(x, y) == false;
                        }
                        else
                        {
                            foreach (var c in (view as MultiChildContainer)!.Children)
                            {
                                if (c is Container)
                                {
                                    if (_isChildTitleBar(c, x, y))
                                        return true;
                                }
                                else if (c is IWindowDragger)
                                {
                                    if (c.IsContaining(x, y))
                                        return true;
                                }
                                else // c is regular view, not a dragger
                                {
                                    return false;
                                }
                            }
                        }
                    }
                    return (view is IWindowDragger); // if container and a ITitleBarView and not touching anything
                }
                return (view is IWindowDragger); // if non container and a ITitleBarView
            }
            return false;
        }
        /// <summary>
        /// Sets the current window views
        /// </summary>
        /// <param name="view"></param>
        public void SetView(View view)
        {
            Children.Clear();
            Children.Add(view);
            UpdateChildPos();
        }

        /// <summary>
        /// Push a dialog to the stack and show it
        /// </summary>
        /// <param name="dialog"></param>
        public void AddDialog(Dialog dialog)
        {
            DialogContainer.AddChild(dialog);
            DialogContainer.UpdateChildPos();
        }

        /// <summary>
        /// Minimize or restore the window (Desktop only)
        /// </summary>
        public unsafe void MaximizeOrRestore()
        {
            var glfw = Glfw.GetApi();
            var handle = (WindowHandle*)_window.Native.Glfw.Value;
            if (glfw.GetWindowAttrib(handle, WindowAttributeGetter.Maximized))
                glfw.RestoreWindow(handle);
            else
                glfw.MaximizeWindow(handle);
        }

        /// <summary>
        /// Check natively if a window is maximized (Desktop Only)
        /// </summary>
        /// <returns></returns>
        public unsafe bool IsMaximized()
        {
            var glfw = Glfw.GetApi();
            var handle = (WindowHandle*)_window.Native.Glfw.Value;
            return glfw.GetWindowAttrib(handle, WindowAttributeGetter.Maximized);
        }

        public unsafe void Minimize()
        {
            var glfw = Glfw.GetApi();
            glfw.IconifyWindow((WindowHandle*)_window.Native.Glfw.Value);
        }

        /*** Overridable functions ***/
        /// <summary>
        /// When the window is required to draw
        /// </summary>
        /// <param name="canvas">Skia context</param>
        public virtual void OnDraw(SKCanvas canvas) { }
        /// <summary>
        /// Called every frame
        /// </summary>
        /// <param name="deltaTime">time since last frame</param>
        public virtual void OnUpdate(double deltaTime) { }
        /// <summary>
        /// When the window is about to close
        /// </summary>
        public virtual void OnClosing() { }
        /// <summary>
        /// When the window is ready
        /// </summary>
        public virtual void OnInitialized() { }
    }
}