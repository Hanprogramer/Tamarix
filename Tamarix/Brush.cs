using SkiaSharp;

namespace Tamarix
{
    /// <summary>
    /// Base class for all brushes
    /// </summary>
    public interface IBrush
    {
        public void Draw(SKCanvas canvas, SKPath path);
        public void DrawRect(SKCanvas canvas, SKRect rect);
        public void DrawRoundRect(SKCanvas canvas, SKRoundRect rect);
    }

    /// <summary>
    /// Normal solid color brush
    /// </summary>
    public class SolidColorBrush : IBrush
    {
        public Color Color { get; set; }
        private SKPaint paint = new SKPaint();

        public SolidColorBrush(Color color)
        {
            this.Color = color;
            this.paint.Color = Color.SkColor;
        }

        public void Draw(SKCanvas canvas, SKPath path)
        {
            canvas.DrawPath(path, paint);
        }

        public void DrawRect(SKCanvas canvas, SKRect rect)
        {
            canvas.DrawRect(rect, paint);
        }

        public void DrawRoundRect(SKCanvas canvas, SKRoundRect rect)
        {
            canvas.DrawRoundRect(rect, paint);
        }
    }

    /// <summary>
    /// Gradient brush content
    /// </summary>
    public struct Gradient
    {
        public Color[] Colors;
        public float[]? Positions;
        public SKPaint paint = new SKPaint();
        public SKRect rect = new SKRect();

        public Gradient(Color[] colors, float[]? positions)
        {
            Colors = colors;
            Positions = positions;
            Initialize();
        }

        public void Initialize()
        {
            SKColor[] colors = new SKColor[Colors.Length];
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = Colors[i].SkColor;
            }

            paint.Shader = SKShader.CreateLinearGradient(
                rect.Location,
                rect.Location + rect.Size.ToPoint(),
                colors,
                Positions,
                SKShaderTileMode.Clamp
           );
        }
    }

    /// <summary>
    /// Gradient brush, can contains many colors
    /// </summary>
    public class GradientColorBrush : IBrush
    {
        public Gradient Gradient;

        public GradientColorBrush(Gradient gradient)
        {
            Gradient = gradient;
        }

        public GradientColorBrush(Color[] colors, float[]? positions)
        {
            Gradient = new Gradient(colors, positions);
        }


        public void Draw(SKCanvas canvas, SKPath path)
        {
            if (path.GetBounds(out var rect))
            {
                Gradient.rect = rect;
                Gradient.Initialize();
                canvas.DrawPath(path, Gradient.paint);
            }

        }

        public void DrawRect(SKCanvas canvas, SKRect rect)
        {
            Gradient.rect = rect;
            Gradient.Initialize();
            canvas.DrawRect(rect, Gradient.paint);
        }

        public void DrawRoundRect(SKCanvas canvas, SKRoundRect rect)
        {
            Gradient.rect = rect.Rect;
            Gradient.Initialize();
            canvas.DrawRoundRect(rect, Gradient.paint);
        }

        public void DrawRoundRect(SKCanvas canvas, SKRect rect, float rx, float ry)
        {
            Gradient.rect = rect;
            Gradient.Initialize();
            canvas.DrawRoundRect(rect, rx, ry, Gradient.paint);
        }
    }
}
