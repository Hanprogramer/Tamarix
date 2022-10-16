using SkiaSharp;

namespace Tamarix.Views
{
    public class MenuButton : ClickableView
    {
        public double animAlpha = 0;
        public SKPaint Paint = new SKPaint();
        /// <summary>
        ///  used in backgroud if needed, not implemented yet
        /// </summary>
        public Color? Color { get; set; }
        public Color? HoverColor { get; set; }
        public Color? ClickColor { get; set; }
        public int? RoundRadius { get; set; }
        public override Margin Margin { get; set; } = new Margin(4,4,8,8);
        public override Padding Padding { get; set; } = new Padding(10,10,16,16);
        public override MouseCursor? Cursor { get; set; } = MouseCursor.Arrow;
        public bool Transparent { get; set; } = false;


        public IBrush? Brush { get; set; }

        public override void Draw(SKCanvas canvas)
        {
            base.Draw(canvas);
#if ANDROID
            Paint.Color = Color?.SkColor ?? Theme.Current.ForegroundColor.SkColor;
#else
            if (State == ClickableState.Hover)
                Paint.Color = HoverColor?.SkColor ?? Theme.Current.MenuButtonColor.SkColor;
#endif
            if (State == ClickableState.Clicked)
                Paint.Color = ClickColor?.SkColor ?? Theme.Current.MenuButtonColorActive.SkColor;
            var rect = new SKRect(
                X - Padding.Left,
                Y - Padding.Top,
                X + Width + Padding.Right,
                Y + Height + Padding.Bottom
                );
            var rounding = RoundRadius ?? Theme.Current.ButtonRounding;
            var paint2 = new SKPaint()
            {
                Color = Paint.Color.WithAlpha((byte)(animAlpha * 255))
            };



            var ppath = new SKPath();
            ppath.AddRoundRect(rect, rounding, rounding);
            canvas.DrawPath(ppath, paint2);

        }
        public MenuButton(Action? OnClick = null) : base(null, OnClick) { }
        public MenuButton(View child, Action? OnClick = null) : base(child, OnClick)
        {
            Width = child.Width;
            Height = child.Height;
        }
        public MenuButton(string label, Action? OnClick = null) : base(OnClick: OnClick)
        {
            Child = new Label(label);
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
            Child.X = X;
            Child.Y = Y;
            Child.Width = Width;
            Child.Height = Height;
        }

        public override void Update(double deltaTime)
        {
            base.Update(deltaTime);
            const float animSpeed = 10;
            if (State == ClickableState.Hover || State == ClickableState.Clicked)
            {
                animAlpha += animSpeed * deltaTime;
                if (animAlpha > 1)
                    animAlpha = 1;
            }
            else
            {
                animAlpha -= animSpeed * deltaTime;
                if (animAlpha < 0)
                    animAlpha = 0;
            }

        }

        public override void OnMouseButton(ref UIEvent evt, MouseButton button, bool pressed)
        {
            base.OnMouseButton(ref evt, button, pressed);
            if (pressed)
            {
                animAlpha = 1f;
            }
        }

        public override bool IsChildrenContaining(int x, int y)
        {
            return IsContaining(x, y);
        }
    }
}
