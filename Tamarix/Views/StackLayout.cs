namespace Tamarix.Views
{
    public class StackLayout : MultiChildContainer
    {
        public override void UpdateChildPos()
        {
            foreach (var child in Children)
            {
                child.X = X;
                child.Y = Y;
                child.Width = Width;
                child.Height = Height;
            }
            base.UpdateChildPos();
            foreach (var child in Children)
            {
                child.X = X;
                child.Y = Y;
                child.Width = Width;
                child.Height = Height;
            }
        }
    }
}
