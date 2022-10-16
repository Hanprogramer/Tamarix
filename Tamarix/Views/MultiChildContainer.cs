
using SkiaSharp;

namespace Tamarix.Views
{
    public class MultiChildContainer : Container
    {
        public virtual List<View> Children { get; set; }
        public MultiChildContainer(List<View>? Children = null)
        {
            if (Children != null)
                this.Children = Children;
            else
                this.Children = new();
        }

        public override void Initialize()
        {
            base.Initialize();
            foreach (var child in Children)
            {
                child.Initialize();
                if (child is Container)
                    (child as Container)!.Initialize();
            }
        }

        public virtual void AddChild(View child, bool dontUpdateChildPos = false)
        {
            Children.Add(child);
            if (!dontUpdateChildPos)
                UpdateChildPos();
        }
        public virtual void RemoveChild(View child)
        {
            Children.Remove(child);
        }
        public override void UpdateChildPos()
        {
            foreach (var child in Children)
            {
                if (child is Container)
                    (child as Container)!.UpdateChildPos();
            }
        }
        public override void UpdateChildren(double deltaTime)
        {
            // Frees the children
            bool done = false;
            while (!done)
            {
                bool found = false;
                foreach (var child in Children)
                {
                    if (child.IsFreeing)
                    {
                        RemoveChild(child);
                        found = true;
                        break;
                    }
                }
                if (!found)
                    done = true;
            }
            foreach (var child in Children)
            {
                child.Update(deltaTime);
                if (child is Container)
                    (child as Container)!.UpdateChildren(deltaTime);
            }
        }
        public override void RenderChildren(SKCanvas canvas)
        {
            foreach (var child in Children)
            {
                child.Draw(canvas);
                if (child is Container)
                    (child as Container)!.RenderChildren(canvas);
            }
        }

        public override void Dispose()
        {
            foreach (var child in Children)
            {
                child.Dispose();
                if (child is Container)
                    (child as Container)!.Dispose();
            }
        }

        public override void OnMouseEnter(ref UIEvent evt)
        {
            if (evt.Handled) return;
            base.OnMouseEnter(ref evt);
            foreach (var child in Children.Reverse<View>())
            {
                if (evt.Handled) return;
                child.OnMouseEnter(ref evt);
                if (child is Container)
                    (child as Container)!.OnMouseEnter(ref evt);
            }
        }

        public override void OnMouseLeave(ref UIEvent evt)
        {
            if (evt.Handled) return;
            base.OnMouseLeave(ref evt);
            foreach (var child in Children.Reverse<View>())
            {
                if (evt.Handled) return;
                child.OnMouseLeave(ref evt);
                if (child is Container)
                    (child as Container)!.OnMouseLeave(ref evt);
            }
        }

        public override void OnMouseMove(ref UIEvent evt)
        {
            base.OnMouseMove(ref evt);
            //if (!IsContaining(x, y)) return;
            foreach (var child in Children.Reverse<View>())
            {
                child.OnMouseMove(ref evt);
            }
        }

        public override void OnMouseButton(ref UIEvent evt, MouseButton button, bool pressed)
        {
            if (evt.Handled) return;
            base.OnMouseButton(ref evt, button, pressed);
            if (!IsContaining(evt.x, evt.y)) return;
            foreach (var child in Children.Reverse<View>())
            {

                if (evt.Handled) break;
                if (!child.IsContaining(evt.x, evt.y)) continue;
                child.OnMouseButton(ref evt, button, pressed);
                //Console.WriteLine($"{this.GetType()} {button} pressed:{pressed} {evt.x} {evt.y}");
            }
        }

        public override bool IsChildrenContaining(int x, int y)
        {
            if (!IsContaining(x, y)) return false;
            var result = false;
            foreach (var child in Children.Reverse<View>())
            {
                if (child is Container)
                {
                    if ((child as Container)!.IsChildrenContaining(x, y))
                        result = true;
                }
                else if (child.IsContaining(x, y))
                    result = true;
            }
            return result;
        }

        public override void OnKeyDown(ref UIEvent evt)
        {
            if (evt.Handled) return;
            base.OnKeyDown(ref evt);
            foreach (var child in Children.Reverse<View>())
            {
                if (evt.Handled) break;
                child.OnKeyDown(ref evt);
            }
        }

        public override void OnKeyUp(ref UIEvent evt)
        {
            if (evt.Handled) return;
            base.OnKeyUp(ref evt);
            foreach (var child in Children.Reverse<View>())
            {
                if (evt.Handled) break;
                child.OnKeyUp(ref evt);
            }
        }
        public override void OnKeyChar(ref UIEvent evt)
        {
            if (evt.Handled) return;
            base.OnKeyChar(ref evt);
            foreach (var child in Children.Reverse<View>())
            {
                if (evt.Handled) break;
                child.OnKeyChar(ref evt);
            }
        }
    }
}
