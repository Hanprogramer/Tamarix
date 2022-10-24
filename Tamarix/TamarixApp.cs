using Silk.NET.Windowing;

namespace Tamarix
{
    public class TamarixApp
    {
        public TWindowManager WindowManager { get; set; }
        public TamarixApp()
        {
            WindowManager = new TWindowManager();
            //Silk.NET.Windowing.Sdl.SdlWindowing.Use();
        }

        public void Start()
        {
            WindowManager.Run();
        }

        public void Stop()
        {
            WindowManager.CloseAll();
            WindowManager.Stop();
        }

        public void AddWindow(Window window)
        {
            window.WindowManager = WindowManager;
            window.App = this;
            WindowManager.AddWindow((window._window as IWindow)!);
        }

        public void RemoveWindow(Window window)
        {
            window.WindowManager = null;
            window.App = null;
            WindowManager.RemoveWindow((window._window as IWindow)!);
        }
    }
}