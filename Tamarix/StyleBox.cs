using SkiaSharp;
namespace Tamarix
{
    public class StyleBox
    {
        private int borderRadius;
        public IBrush FillBrush;
        public IBrush? OutlineBrush;
        public int BorderWidth { get; set; }
        public int BorderRadius
        {
            get { return borderRadius; }
            set { borderRadius = value; }
        }

        public StyleBox(IBrush fillBrush, IBrush? outlineBrush = null, int borderWidth = 0, int borderRadius = 0)
        {
            FillBrush = fillBrush;
            OutlineBrush = outlineBrush;
            BorderWidth = borderWidth;
            BorderRadius = borderRadius;
        }
        public void Draw(SKCanvas canvas, int x, int y, int w, int h)
        {
            var rr = new SKRoundRect(new SKRect { Left = x, Top = y, Right = x + w, Bottom = y + h }, BorderRadius);
            FillBrush.DrawRoundRect(canvas, rr);
            OutlineBrush?.DrawRoundRect(canvas, rr);
        }

        public void DrawPos(SKCanvas canvas, int x1, int y1, int x2, int y2)
        {
            Draw(canvas, x1, y1, x2 - x1, y2 - y1);
        }
    }
}
