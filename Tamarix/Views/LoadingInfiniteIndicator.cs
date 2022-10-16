using SkiaSharp;

namespace Tamarix.Views
{
    public class LoadingInfiniteIndicator : View
    {
        public float AngleSize { get; set; } = 120;
        public override Padding Padding { get; set; } = new Padding(8);
        public override Margin Margin { get; set; } = new Margin(16);


        private Color? color;
        public Color? Color { get => color; set { color = value; paint.Color = color?.SkColor ?? Theme.Current.AccentColor.SkColor; } }

        int strokeWidth = 8;
        public int StrokeWidth
        {
            get => strokeWidth;
            set
            {
                strokeWidth = value;
                paint.StrokeWidth = value;
            }
        }

        private float AnglePos = 0;
        SKPaint paint;
        public LoadingInfiniteIndicator()
        {
            Width = 48;
            Height = 48;
            paint = new SKPaint()
            {
                Color = Theme.Current.AccentColor.SkColor,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = strokeWidth,
                IsAntialias = true,
                StrokeCap = SKStrokeCap.Round,
                IsStroke = true
            };
        }
        public override void Draw(SKCanvas canvas)
        {
            //Update(1);
            base.Draw(canvas);
            SKRect rect = new SKRect(X, Y, X + Width, Y + Height);
            using (SKPath path = new SKPath())
            {
                path.AddArc(rect, AnglePos, AngleSize);
                canvas.DrawPath(path, paint);
            }
        }

        public override void Update(double deltaTime)
        {
            base.Update(deltaTime);
            AnglePos += (float)deltaTime * 400f;
            if (AnglePos > 360.0) AnglePos = 0;
            //AnglePos += .01f;
        }
    }
}
