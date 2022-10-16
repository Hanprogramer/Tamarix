using SkiaSharp;

namespace Tamarix.Views
{
    public class Dialog : SingleChildContainer
    {
        public DialogContainerStack ContainerStack { get; set; }
        public StyleBox? StyleBox { get; set; }
        public override Padding Padding { get; set; } = new Padding(16);
        public override int MinWidth { get; set; } = 64;
        public override int MinHeight { get; set; } = 64;
        public bool Cancelable { get; set; } = true;
        public delegate bool OnClickShadeEvent();
        public event OnClickShadeEvent? OnClickShade;
        int yto = 0;
        int ystart = 0;
        int ycurrent = 0;
        bool isClosing = false;

        static SKPaint shadePaint = new SKPaint()
        {
            Color = Theme.Current.DialogShadeColor.SkColor
        };
        private float animProgress = 0;

        public Dialog(DialogContainerStack containerStack, bool cancellable = true, OnClickShadeEvent? onClickShade = null) : base()
        {
            ContainerStack = containerStack;
            OnClickShade = onClickShade;
            Cancelable = cancellable;
            Y = -(Window.Current?.Height ?? 512);
        }

        public override void Draw(SKCanvas canvas)
        {
            base.Draw(canvas);
            shadePaint.Color = shadePaint.Color.WithAlpha((byte)((animProgress) * 125f));
            canvas.DrawRect(X, Y, Width, Height, shadePaint);
            if (Child != null)
            {
                (StyleBox ?? Theme.Current.DialogBackground).Draw(canvas,
                    Child.X - Child.Padding.Left - Padding.Left,
                    Child.Y - Child.Padding.Top - Padding.Top,
                    Child.MeasuredWidth + Padding.Width,
                    Child.MeasuredHeight + Padding.Height);
            }
        }

        public override void UpdateChildPos()
        {
            base.UpdateChildPos();
            if (Window.Current != null && Child != null)
            {
                Width = Window.Current.Width;
                Height = Window.Current.Height;
                Child!.X = (int)(Window.Current.Width / 2.0 - Child!.Width / 2.0);
                Child!.Y = ycurrent + (int)((Window.Current.Height - ycurrent) / 2.0 - Child!.Height / 2.0);

                if (Window.Current.HasCustomTitleBar)
                {
                    var h = Window.Current.TitleBar.MeasuredHeight;
                    yto = h;
                    Y = h;
                    Height = Window.Current.Height - h;
                }
            }

        }

        public override void OnMouseMove(ref UIEvent evt)
        {
            base.OnMouseMove(ref evt);
            if (IsContaining(evt.x, evt.y))
            {
                if (Child != null)
                {
                    if (evt.x > Child.X + Child.Width + Padding.Width || evt.y > Child.Y + Child.Height + Padding.Height || evt.x < Child.X - Padding.Left || evt.y < Child.Y - Padding.Top)
                    {
                        // If moved on shade, show a fake 
                        evt.x = -1;
                        evt.y = -1;
                    }
                    else
                    {
                        evt.Handled = true;
                    }
                }
            }
        }

        public override void OnMouseEnter(ref UIEvent evt)
        {
            evt.Handled = true;
            base.OnMouseEnter(ref evt);
        }

        public override void OnMouseLeave(ref UIEvent evt)
        {
            evt.Handled = true;
            base.OnMouseLeave(ref evt);
        }

        public override void OnMouseButton(ref UIEvent evt, MouseButton button, bool pressed)
        {
            if (Child != null)
            {
                if (evt.x > Child.X + Child.Width + Padding.Width || evt.y > Child.Y + Child.Height + Padding.Height || evt.x < Child.X - Padding.Left || evt.y < Child.Y - Padding.Top)
                {
                    // Clicked on the shade, closes the dialog
                    if (OnClickShade?.Invoke() ?? true && Cancelable)
                    {
                        // If true, closes the dialog
                        Close();
                        evt.Handled = true;
                    }
                    else
                    {
                    }
                }
            }
            base.OnMouseButton(ref evt, button, pressed);
            evt.Handled = true;
        }

        public override bool IsContaining(int x, int y)
        {
            // Dont include paddings, because the padding is only for drawing
            return (x > X) && (y > Y)
                && x < (X + Width) && y < (Y + Height);
        }

        public override void Update(double deltaTime)
        {
            base.Update(deltaTime);
            if (isClosing)
            {

                if (animProgress > 0.1)
                {
                    animProgress -= 0.1f;
                    ycurrent = (int)Lerp(ystart, yto, animProgress);
                    UpdateChildPos();
                }
                else
                {
                    QueueFree();
                }
            }
            else
            {
                if (animProgress < 1)
                {
                    animProgress += 0.1f;
                    ycurrent = (int)Lerp(ystart, yto, animProgress);
                    UpdateChildPos();
                }
            }

        }
        float Lerp(float firstFloat, float secondFloat, float by)
        {
            return firstFloat * (1 - by) + secondFloat * by;
        }

        public void Close()
        {
            isClosing = true;
        }
    }
}
