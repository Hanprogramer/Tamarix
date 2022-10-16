using SkiaSharp;

namespace Tamarix.Views
{
    public class View
    {
        public virtual int Width { get; set; } = 0;
        public virtual int Height { get; set; } = 0;
        public virtual int MinWidth { get; set; } = 0;
        public virtual int MinHeight { get; set; } = 0;
        public static bool DrawDebug = false;
        public virtual MouseCursor? Cursor { get; set; }

        public static SKPaint DebugPaint = new SKPaint() { Color = new SKColor(255, 0, 0, 50) };
        public virtual Padding Padding { get; set; }
        public virtual Margin Margin { get; set; }
        public bool IsFreeing = false;
        public string? Id;
        /// <summary>
        /// Get the width + padding, excluding margin
        /// </summary>
        public int MeasuredWidth
        {
            get => Width + Padding.Width;
            set { Width = value; }
        }

        /// <summary>
        /// Get the height + padding, excluding margin
        /// </summary>
        public int MeasuredHeight
        {
            get => Height + Padding.Height;
            set { Height = value; }
        }
        public int MinMeassuredWidth
        {
            get => MinWidth + Padding.Left + Padding.Right;
            set { MinWidth = value; }
        }
        public int MinMeassuredHeight
        {
            get => MinHeight + Padding.Top + Padding.Bottom;
            set { MinHeight = value; }
        }


        public virtual int X { get; set; }
        public virtual int Y { get; set; }
        public virtual void Initialize()
        {
        }
        public virtual void Update(double deltaTime)
        {
        }
        public virtual void Draw(SKCanvas canvas)
        {
            if (DrawDebug && this is Container and not Button)
            {
                canvas.DrawRect(
                    X - Padding.Left,
                    Y - Padding.Top,
                    Width + Padding.Width,
                    Height + Padding.Height, DebugPaint);
                //Console.WriteLine();
                //Console.WriteLine($"{this} {X} {Y} {Width} {Height}");
            }
        }
        public virtual void Dispose()
        {
        }

        public virtual bool IsContaining(int x, int y)
        {
            return (x > X - Padding.Left) && (y > Y - Padding.Top)
                && x < (X + Width + Padding.Right) && y < (Y + Height + Padding.Bottom);
        }



        public virtual void OnMouseEnter(ref UIEvent evt)
        {

        }
        public virtual void OnMouseLeave(ref UIEvent evt)
        {

        }
        public virtual void OnMouseMove(ref UIEvent evt)
        {

            //Console.WriteLine($"{this.GetType()} {x} {y} handled: {evt.Handled}");
            if (evt.Handled) return;
            if (Cursor != null && IsContaining(evt.x, evt.y))
            {
                Window.Current?.SetCursor((MouseCursor)(Cursor!));
            }
        }
        public virtual void OnMouseButton(ref UIEvent evt, MouseButton button, bool pressed)
        {
            if (pressed)
            {
                if (Window.Current != null)
                    Window.Current!.CurrentViewFocus = this;
            }
        }

        public virtual void OnKeyDown(ref UIEvent evt)
        {

        }

        public virtual void OnKeyUp(ref UIEvent evt)
        {

        }

        public virtual void OnKeyChar(ref UIEvent evt)
        {

        }
        /// <summary>
        /// Asks for the parent container to remove the view safely
        /// </summary>
        public void QueueFree()
        {
            IsFreeing = true;
        }
    }

    public enum MouseButton
    {
        Left,
        Middle,
        Right,
    }
}
