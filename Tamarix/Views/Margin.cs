namespace Tamarix.Views
{
    public struct Margin
    {
        public int Left, Right, Top, Bottom;
        public Margin(int all)
        {
            Left = all;
            Right = all;
            Top = all;
            Bottom = all;
        }
        public int Width { get => Left + Right; }
        public int Height { get => Top + Bottom; }
        public Margin(int left, int right, int top, int bottom)
        {
            this.Left = left;
            this.Right = right;
            this.Top = top;
            this.Bottom = bottom;
        }
    }
}
