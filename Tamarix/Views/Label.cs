using SkiaSharp;

namespace Tamarix.Views
{
    public class Label : View
    {
        private string content = "";
        public string Text { get => content; set => SetText(value); }
        float? fontSize;
        public float? FontSize { get => fontSize; set => SetFontSize(value); }
        public TextAlign textAlign = TextAlign.Center;
        private void SetFontSize(float? value)
        {
            fontSize = value;
            SetText(content);
        }

        public Color? Color { get; set; } = null;
        private SKPaint Paint = new SKPaint();
        public Label(string? text = null) : base()
        {
            if (text != null)
                SetText(text);
            Paint.IsAntialias = true;
            Paint.Color = Theme.Current.TextColor.SkColor;
            FontSize = Theme.Current.DefaultFontSize;
        }

        public void SetText(string value)
        {
            content = value;
            SKRect rect = new SKRect();
            Paint.TextSize = FontSize ?? Theme.Current.DefaultFontSize;
            Paint.MeasureText(content, ref rect);
            MinMeassuredWidth = (int)rect.Width;
            MinMeassuredHeight = (int)rect.Height;
            MeasuredWidth = MinMeassuredWidth;
            MeasuredHeight = MinMeassuredHeight;
            //Console.WriteLine($"W{Width}, H{Height}");
        }

        public override void Update(double deltaTime)
        {
            base.Update(deltaTime);
            //Console.WriteLine($"x:{X}, y:{Y}, {Width},{Height}");
        }

        public override void Draw(SKCanvas canvas)
        {
            base.Draw(canvas);
            if (Color != null)
                Paint.Color = Color.SkColor;
            Paint.TextSize = FontSize ?? Theme.Current.DefaultFontSize;
            SKRect textSize = new SKRect();
            Paint.MeasureText(content, ref textSize);
            if (textAlign == TextAlign.Center)
                canvas.DrawText(content, X + (Width / 2) - (textSize.Width / 2), Y + (Height / 2) + (textSize.Height / 2), Paint);
            else
                throw new NotImplementedException();
        }

    }
}