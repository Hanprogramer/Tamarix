using SkiaSharp;

namespace Tamarix.Views
{
    public class TitleBar : Flex, IWindowDragger
    {
        public Window window;
        public override Padding Padding { get; set; } = new Padding(12, 0, 0, 0);
        public override Margin Margin { get; set; } = new Margin(0);

        public LinearLayout MenuBar = new LinearLayout(Orientation.Horizontal)
        {
            MainAxisAlignment = Alignment.Begin,
            CrossAxisAlignment = Alignment.Stretch
        };
        public Color? Color { get; set; }
        private SKPaint paint = new SKPaint()
        {
            Color = Theme.Current.TitleBarColor.SkColor
        };

        public TitleBar(Window window, Image icon, bool showTitle = true) : base(Orientation.Horizontal)
        {
            this.window = window;
            AddChild(new MenuButton(icon)
            {
                Transparent = true,
                Margin = Margin,
                Padding = new Padding(12, 12, 0, 0),
                RoundRadius = 0,
                Width = 32,

            }, 0);
            AddChild(MenuBar, 0);
            AddMenu("File");
            AddMenu("Edit");
            AddMenu("Project");
            AddMenu("Help");
            if (showTitle)
                AddChild(new ClickThroughLabel(window.Title), 1);
            else
                AddFlexChild(new FlexChild(new Spacer(), 1));

            AddChild(new MenuButton(
                Image.FromSVGStream(ResourceManager.GetAsset(typeof(TitleBar), "minimize.svg"), 8, 8),
                OnClick: () => { window.Minimize(); })
            {
                Padding = new Padding(20, 20, 0, 0),
                Margin = new Margin(0),
                Width = 26,
                RoundRadius = 0
            }, 0);
            AddChild(new MenuButton(
                Image.FromSVGStream(ResourceManager.GetAsset(typeof(TitleBar), "maximize.svg"), 8, 8),
                OnClick: () =>
                {
                    window.MaximizeOrRestore();
                })
            {
                Padding = new Padding(20, 20, 0, 0),
                Margin = new Margin(0),
                Width = 26,
                RoundRadius = 0
            }, 0);
            AddChild(new MenuButton(
                Image.FromSVGStream(ResourceManager.GetAsset(typeof(TitleBar), "close.svg"), 8, 8),
                OnClick: () => { window.Close(); })
            {
                Padding = new Padding(20, 20, 0, 0),
                Margin = new Margin(0),
                Width = 26,
                RoundRadius = 0,
                HoverColor = Colors.Red,
                ClickColor = new Color(200, 0, 0)
            }, 0);
        }

        public void AddMenu(string label)
        {
            MenuBar.AddChild(new MenuButton(label)
            {
                Transparent = true,
                Margin = Margin,
                RoundRadius = 0
            }, false);
        }

        public override void Draw(SKCanvas canvas)
        {
            base.Draw(canvas);
            paint.Color = Color?.SkColor ?? Theme.Current.TitleBarColor.SkColor;
            canvas.DrawRect(X - Padding.Left, Y - Padding.Top, MeasuredWidth, MeasuredHeight, paint);
        }
    }
}
