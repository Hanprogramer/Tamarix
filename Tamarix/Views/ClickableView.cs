namespace Tamarix.Views
{
    public class ClickableView : SingleChildContainer
    {
        event Action? Click;
        public ClickableView(View? child = null, Action? OnClick = null) : base(child)
        {
            if (OnClick != null)
                Click += OnClick;
        }

        public ClickableState State { get; set; } = ClickableState.Normal;



        public override void OnMouseButton(ref UIEvent evt, MouseButton button, bool pressed)
        {
            if (evt.Handled) return;
            base.OnMouseButton(ref evt, button, pressed);
            if (pressed) //TODO: button filtering?
            {
                State = ClickableState.Clicked;
            }
            else
            {
                if (State == ClickableState.Clicked)
                {
                    if (evt.Handled) return;
                    Click?.Invoke();
                }
                State = ClickableState.Hover;
            }
        }

        public override void OnMouseEnter(ref UIEvent evt)
        {
            if (evt.Handled) return;
            base.OnMouseEnter(ref evt);
            State = ClickableState.Hover;
        }

        public override void OnMouseLeave(ref UIEvent evt)
        {
            //if (evt.Handled) return;
            base.OnMouseLeave(ref evt);
            State = ClickableState.Normal;
        }

        public override void OnMouseMove(ref UIEvent evt)
        {
            base.OnMouseMove(ref evt);
            if (IsContaining(evt.x, evt.y))
            {
                if (State != ClickableState.Clicked)
                {
                    OnMouseEnter(ref evt);
                }
                evt.Handled = true;
            }
            else
            {
                OnMouseLeave(ref evt);
            }
        }
    }
}
