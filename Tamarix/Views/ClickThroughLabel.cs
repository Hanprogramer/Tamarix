namespace Tamarix.Views
{
    public class ClickThroughLabel : Label
    {
        public ClickThroughLabel(string? text = null) : base(text)
        {
        }

        public override bool IsContaining(int x, int y)
        {
            return false;
        }
    }
}
