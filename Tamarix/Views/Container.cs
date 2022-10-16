using SkiaSharp;
namespace Tamarix.Views
{
    public abstract class Container : View
    {
        public abstract void UpdateChildPos();
        public abstract void UpdateChildren(double deltaTime);
        public abstract void RenderChildren(SKCanvas canvas);

        public abstract override void Dispose();
        public abstract bool IsChildrenContaining(int x, int y);

    }
}
