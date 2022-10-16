namespace Tamarix.Views
{
    public class LinearLayout : MultiChildContainer
    {
        public bool ResizeSelf { get; set; } = true;
        public Orientation Orientation { get; set; }
        public Alignment MainAxisAlignment { get; set; }
        public Alignment CrossAxisAlignment { get; set; }
        public LinearLayout(Orientation orientation, List<View>? children = null,
            Alignment mainAxisAlignment = Alignment.Center, Alignment crossAxisAlignment = Alignment.Center
            ) : base(children)
        {
            if (children != null)
                Children = children;
            else
                Children = new List<View>();
            MainAxisAlignment = mainAxisAlignment;
            CrossAxisAlignment = crossAxisAlignment;
            Orientation = orientation;
        }

        public override void UpdateChildPos()
        {
            int xpos = X, ypos = Y;
            if (Orientation == Orientation.Horizontal)
            {
                if (MainAxisAlignment == Alignment.Center)
                {
                    xpos = (MeasuredWidth / 2);

                    var totalWidth = 0;
                    foreach (var child in Children)
                    {
                        totalWidth += child.MeasuredWidth + child.Margin.Width;
                    }
                    if (ResizeSelf)
                        Width = totalWidth;
                    xpos -= totalWidth / 2;
                    foreach (var child in Children)
                    {
                        child.X = X + xpos + child.Padding.Left + child.Margin.Left;
                        xpos += child.MeasuredWidth + child.Margin.Width;
                    }
                }
                else if (MainAxisAlignment == Alignment.Begin)
                {
                    xpos = X;
                    foreach (var child in Children)
                    {
                        child.X = xpos + child.Padding.Left + child.Margin.Left;
                        xpos += child.MeasuredWidth + child.Margin.Width;
                    }

                    // Sets self size

                    var totalWidth = 0;
                    foreach (var child in Children)
                    {
                        totalWidth += child.MeasuredWidth + child.Margin.Width;
                    }
                    if (ResizeSelf)
                        Width = totalWidth;
                }

                if (CrossAxisAlignment == Alignment.Center)
                {
                    var maxHeight = 0;
                    foreach (var child in Children)
                    {
                        if (child.MeasuredHeight + child.Margin.Height > maxHeight)
                            maxHeight = child.MeasuredHeight + child.Margin.Height;
                    }
                    ypos = (MeasuredHeight / 2);
                    if (ResizeSelf)
                        Height = maxHeight;
                    foreach (var child in Children)
                    {
                        child.Y = Y + ypos - ((child.Height) / 2);
                    }
                }
                else if (CrossAxisAlignment == Alignment.Stretch)
                {
                    foreach (var child in Children)
                    {
                        child.Height = Height - child.Padding.Height - child.Margin.Height;
                        child.Y = Y + child.Padding.Top + child.Margin.Top;
                    }
                }

            }

            else if (Orientation == Orientation.Vertical)
            {
                if (MainAxisAlignment == Alignment.Center)
                {
                    ypos = (MeasuredHeight / 2);

                    var totalHeight = 0;
                    foreach (var child in Children)
                    {
                        totalHeight += child.MeasuredHeight + child.Margin.Height;
                    }
                    if (ResizeSelf)
                        Height = totalHeight;
                    ypos -= totalHeight / 2;
                    foreach (var child in Children)
                    {
                        child.Y = Y + ypos + child.Padding.Top + child.Margin.Top;
                        ypos += child.MeasuredHeight + child.Margin.Height;
                    }
                }
                else if (MainAxisAlignment == Alignment.Begin)
                {
                    ypos = Y;
                    foreach (var child in Children)
                    {
                        child.Y = ypos + child.Padding.Top + child.Margin.Top;
                        ypos += child.MeasuredHeight + child.Margin.Height;
                    }

                    // Sets self size

                    var totalHeight = 0;
                    foreach (var child in Children)
                    {
                        totalHeight += child.MeasuredHeight + child.Margin.Height;
                    }
                    if (ResizeSelf)
                        Height = totalHeight;
                }

                if (CrossAxisAlignment == Alignment.Center)
                {
                    var maxWidth = 0;
                    foreach (var child in Children)
                    {
                        if (child.MeasuredWidth + child.Margin.Width > maxWidth)
                            maxWidth = child.MeasuredWidth + child.Margin.Width;
                    }
                    xpos = (MeasuredWidth / 2);
                    if (ResizeSelf)
                        Width = maxWidth;
                    foreach (var child in Children)
                    {
                        child.X = X + xpos - ((child.Width) / 2);
                    }
                }
                else if (CrossAxisAlignment == Alignment.Stretch)
                {
                    foreach (var child in Children)
                    {
                        child.Width = Width - child.Padding.Width - child.Margin.Width;
                        child.X = X + child.Padding.Left + child.Margin.Left;
                    }
                }

            }
            //{
            //    if (MainAxisAlignment == Alignment.Center)
            //    {
            //        ypos = (MeasuredHeight / 2);

            //        var totalHeight = 0;
            //        foreach (var child in Children)
            //        {
            //            totalHeight += child.MeasuredHeight + child.Margin.Height;
            //        }
            //        if (ResizeSelf)
            //            Height = totalHeight;
            //        ypos -= totalHeight / 2;
            //    }

            //    if (CrossAxisAlignment == Alignment.Center)
            //    {
            //        var maxWidth = 0;
            //        foreach (var child in Children)
            //        {
            //            if (maxWidth < child.MeasuredWidth + child.Margin.Width)
            //                maxWidth = child.MeasuredWidth + child.Margin.Width;
            //        }
            //        xpos = (MeasuredWidth / 2);
            //        if (ResizeSelf)
            //            Width = maxWidth;
            //    }

            //    foreach (var child in Children)
            //    {
            //        child.X = X + xpos - ((child.Width) / 2);
            //        child.Y = Y + ypos + child.Padding.Top + child.Margin.Top;
            //        ypos += child.MeasuredHeight + child.Margin.Height;
            //    }
            //}

            //Console.WriteLine($"{this} {X} {Y} {Width} {Height}");
            base.UpdateChildPos();
        }

    }
}
