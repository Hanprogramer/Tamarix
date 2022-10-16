using SkiaSharp;

namespace Tamarix.Views
{
    public class Button : ClickableView
    {
        public double rippleX, rippleY, rippleProgress, speedMultiplier = 1, rippleAlpha = 0;
        public SKPaint Paint = new SKPaint();
        public Color? Color { get; set; }
        public Color? HoverColor { get; set; }
        public Color? ClickColor { get; set; }
        public int? RoundRadius { get; set; }
        public override Margin Margin { get; set; } = new Margin(8);
        public override Padding Padding { get; set; } = new Padding(16);
        public override MouseCursor? Cursor { get; set; } = MouseCursor.Hand;
        public bool Transparent { get; set; } = false;


        public IBrush? Brush { get; set; }

        public override void Draw(SKCanvas canvas)
        {
            base.Draw(canvas);
#if ANDROID
            Paint.Color = Color?.SkColor ?? Theme.Current.ForegroundColor.SkColor;
#else
            if (State == ClickableState.Normal)
                Paint.Color = Color?.SkColor ?? Theme.Current.ForegroundColor.SkColor;
            else if (State == ClickableState.Hover)
                Paint.Color = HoverColor?.SkColor ?? Theme.Current.ForegroundColorHover.SkColor;
#endif
            var rect = new SKRect(
                X - Padding.Left,
                Y - Padding.Top,
                X + Width + Padding.Right,
                Y + Height + Padding.Bottom
                );
            var rounding = RoundRadius ?? Theme.Current.ButtonRounding;

            var ppath = new SKPath();
            ppath.AddRoundRect(rect, rounding, rounding);
            if (!Transparent)
                (Brush ?? Theme.Current.ButtonBackground).Draw(canvas, ppath);
            if (State == ClickableState.Hover)
            {
                canvas.DrawPath(ppath, Paint);
            }
            var paint2 = new SKPaint()
            {
                Color = new SKColor(255, 255, 255, (byte)(255 * 0.5 * rippleAlpha)),
            };


            canvas.Save();
            canvas.ClipPath(ppath);
            canvas.DrawCircle((float)rippleX, (float)rippleY,
                (float)(rippleProgress / 100f) * Math.Max(Width, Height) * 4, paint2
                );
            canvas.Restore();

        }
        public Button(Action? OnClick = null) : base(null, OnClick) { }
        public Button(View child, Action? OnClick = null) : base(child, OnClick)
        {
            Width = child.Width;
            Height = child.Height;
        }
        public Button(string label, Action? OnClick = null, Color? textColor = null) : base(OnClick: OnClick)
        {
            Child = new Label(label) { Color = textColor };
            Width = Child.MeasuredWidth + Child.Margin.Width;
            Height = Child.MeasuredHeight + Child.Margin.Height;
        }


        public override void Initialize()
        {
            base.Initialize();
        }

        public override void UpdateChildPos()
        {
            base.UpdateChildPos();
            if (Child == null)
                return;
            Child.X = X;
            Child.Y = Y;
            Child.Width = Width;
            Child.Height = Height;
        }

        public override void Update(double deltaTime)
        {
            base.Update(deltaTime);
            if (State == ClickableState.Normal || State == ClickableState.Hover)
            {
                rippleAlpha -= 1.5 * deltaTime;
                if (rippleAlpha < 0)
                    rippleAlpha = 0;
            }

            if (rippleProgress < 100)
            {
                rippleProgress += 300 * deltaTime * speedMultiplier;
            }
            else
            {
                rippleProgress = 100;
            }
        }

        public override void OnMouseButton(ref UIEvent evt, MouseButton button, bool pressed)
        {
            if (evt.Handled) return;
            base.OnMouseButton(ref evt, button, pressed);
            if (pressed)
            {
                rippleX = evt.x;
                rippleY = evt.y;
                rippleProgress = 0;
                speedMultiplier = 0.7f;
                rippleAlpha = 1f;
            }
            else
            {
                speedMultiplier = 1f;
            }
        }

        public override void OnMouseMove(ref UIEvent evt)
        {
            if (evt.Handled) return;
            base.OnMouseMove(ref evt);
            if (State == ClickableState.Clicked)
            {
                rippleX = evt.x;
                rippleY = evt.y;
            }
        }

        public override bool IsChildrenContaining(int x, int y)
        {
            return IsContaining(x, y);
        }
    }
}
