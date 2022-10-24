using Silk.NET.GLFW;
using Silk.NET.Windowing;
namespace Tamarix
{
    /// <summary>
    /// Contains APIs for managing multiple windows within one render loop.
    /// </summary>
    public class TWindowManager : IDisposable
    {
        private object _syncRoot = new object();

        /// <summary>
        /// The windows managed by this instance.
        /// </summary>
        // ReSharper disable once ReturnTypeCanBeEnumerable.Global
        public ICollection<IWindow> Windows { get; } = new List<IWindow>();

        /// <summary>
        /// Gets or sets a value indicating whether this manager's render loop is active.
        /// </summary>
        public bool IsRunning { get; set; }
        public List<IWindow> removals { get; private set; }

        /// <summary>
        /// Gets the IWindow with a context current on this thread.
        /// </summary>
        /// <returns>The IWindow with a context current on this thread.</returns>
        public IWindow GetCurrent()
        {

            return Windows.FirstOrDefault(x => x.GLContext?.IsCurrent ?? false)!;
        }

        /// <summary>
        /// Adds a IWindow to this manager.
        /// </summary>
        /// <param name="IWindow">The IWindow to add.</param>
        public void AddWindow(IWindow IWindow)
        {
            lock (_syncRoot)
            {
                Windows.Add(IWindow);

            }
        }

        /// <summary>
        /// Removes the given IWindow from this manager.
        /// </summary>
        /// <param name="IWindow">The IWindow to remove.</param>
        /// <returns>
        /// Whether the removal was successful or not. One reason why this might return false is that the given IWindow
        /// isn't being managed by this manager.
        /// </returns>
        public bool RemoveWindow(IWindow win)
        {
            lock (_syncRoot)
            {
                //TODO: remove the window manager from the Tamarix window
                //win.WindowManager = null;
                return Windows.Remove(win);
            }
        }

        /// <summary>
        /// Executes a render loop encompassing all windows within this manager.
        /// </summary>
        /// <param name="open">Indicates whether the IWindow manager should open the child windows.</param>
        public void Run(bool open = true)
        {
            IsRunning = true;
            if (open)
            {
                lock (_syncRoot)
                {
                    foreach (var IWindow in Windows)
                    {
                        IWindow.Initialize();
                    }
                }
            }

            removals = new List<IWindow>();
            while (IsRunning)
            {
                lock (_syncRoot)
                {
                    for (int i = 0; i < Windows.Count; i++)
                    {
                        var IWindow = Windows.ElementAt(i);
                        //Console.WriteLine($"{IWindow.Title} : {IWindow.IsClosing}");
                        if (IWindow.IsClosing)
                        {
                            removals.Add(IWindow);
                        }

                        if (IWindow.API.API == ContextAPI.OpenGL || IWindow.API.API == ContextAPI.OpenGLES)
                        {
                            try
                            {
                                IWindow.MakeCurrent();
                            }
                            catch (GlfwException e)
                            {
                                Console.WriteLine($"GlfwException[handle: {IWindow.Handle}]: {e.Message}");
                                continue;
                            }
                        }
                        IWindow.DoEvents();
                        IWindow.DoUpdate();
                        IWindow.DoRender();

                    }

                    foreach (var IWindow in removals)
                    {
                        Windows.Remove(IWindow);
                        IWindow.Reset();
                        IWindow.Dispose();
                        removals.Remove(IWindow);
                        break;
                    }


                    if (Windows.Count() == 0)
                    {
                        Stop();
                    }
                }
            }

            IsRunning = false;
        }

        /// <summary>
        /// Stops the manager's render loop. This doesn't do anything to the windows managed by it.
        /// </summary>
        public void Stop()
        {
            IsRunning = false;
        }

        /// <summary>
        /// Restarts this manager's render loop without reopening the windows.
        /// </summary>
        public void Restart()
        {
            Run(false);
        }

        /// <summary>
        /// Closes all windows managed by this manager.
        /// </summary>
        public void CloseAll()
        {
            lock (_syncRoot)
            {
                foreach (var IWindow in Windows)
                {
                    IWindow.Close();
                }
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            IsRunning = false;
        }
    }
}