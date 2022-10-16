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
            WindowManager.AddWindow((window._window as IWindow)!);
        }
    }
}