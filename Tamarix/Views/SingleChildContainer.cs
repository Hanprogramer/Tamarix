
using SkiaSharp;

namespace Tamarix.Views
{
    public class SingleChildContainer : Container
    {
        View? child;
        public View? Child
        {
            get => child;
            set
            {
                child = value;
                if (child != null)
                {
                    Width = child.Width;
                    Height = child.Height;
                }
            }
        }
        public SingleChildContainer(View? child = null) : base()
        {
            Child = child;
        }

        public override void Initialize()
        {
            base.Initialize();
            Child?.Initialize();
        }

        public override void UpdateChildPos()
        {
            if (Child is Container)
                (Child as Container)!.UpdateChildPos();
        }
        public override void UpdateChildren(double deltaTime)
        {
            if (Child?.IsFreeing ?? false)
                Child = null;
            Child?.Update(deltaTime);
            if (Child is Container)
                (Child as Container)!.UpdateChildren(deltaTime);
        }
        public override void RenderChildren(SKCanvas canvas)
        {
            Child?.Draw(canvas);
            if (Child is Container)
                (Child as Container)!.RenderChildren(canvas);
        }

        public override void Dispose()
        {
            Child?.Dispose();
            if (Child is Container)
                (Child as Container)!.Dispose();
        }

        public override void OnMouseEnter(ref UIEvent evt)
        {
            if (evt.Handled) return;
            base.OnMouseEnter(ref evt);
            Child?.OnMouseEnter(ref evt);
            if (Child is Container)
                (Child as Container)!.OnMouseEnter(ref evt);
        }

        public override void OnMouseLeave(ref UIEvent evt)
        {
            if (evt.Handled) return;
            base.OnMouseLeave(ref evt);
            Child?.OnMouseLeave(ref evt);
            if (Child is Container)
                (Child as Container)!.OnMouseLeave(ref evt);
        }

        public override void OnMouseMove(ref UIEvent evt)
        {
            base.OnMouseMove(ref evt);
            //if (!Child.IsContaining(x, y)) return;
            Child?.OnMouseMove(ref evt);
            if (Child is Container)
                (Child as Container)!.OnMouseMove(ref evt);
        }

        public override void OnMouseButton(ref UIEvent evt, MouseButton button, bool pressed)
        {
            if (evt.Handled) return;
            base.OnMouseButton(ref evt, button, pressed);
            if (evt.Handled) return;
            if (!Child?.IsContaining(evt.x, evt.y) ?? false) return;
            Child?.OnMouseButton(ref evt, button, pressed);
        }

        public override bool IsChildrenContaining(int x, int y)
        {
            if (Child is Container)
                return (Child as Container)!.IsChildrenContaining(x, y);
            else
                return Child?.IsContaining(x, y) ?? false;
        }

        public override void OnKeyDown(ref UIEvent evt)
        {
            if (evt.Handled) return;
            if (Child != null)
            {
                Child.OnKeyDown(ref evt);
            }
            base.OnKeyDown(ref evt);
        }

        public override void OnKeyUp(ref UIEvent evt)
        {
            if (evt.Handled) return;
            if (Child != null)
            {
                Child.OnKeyUp(ref evt);
            }
            base.OnKeyUp(ref evt);
        }

        public override void OnKeyChar(ref UIEvent evt)
        {
            if (evt.Handled) return;
            if (Child != null)
            {
                Child.OnKeyChar(ref evt);
            }
            base.OnKeyChar(ref evt);
        }

        public override View? FindView(string id)
        {
            if (id == Id)
                return this;
            if (Child != null)
            {
                if (Child.Id == id)
                    return Child;
                if (Child is Container c)
                    return c.FindView(id);
            }

            return null;
        }
    }
}
