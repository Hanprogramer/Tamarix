namespace Tamarix.Views
{
    public class UIEvent
    {
        public int Id { get; set; } = 0;
        public bool Handled = false;
        public int x = -1, y = -1;
        public Tamarix.Key? Key { get; set; }
        public char? c;
    }
}
