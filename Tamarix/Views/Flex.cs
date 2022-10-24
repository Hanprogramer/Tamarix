namespace Tamarix.Views
{
    public class Flex : MultiChildContainer
    {
        public Orientation Orientation { get; set; }

        public Flex(Orientation orientation)
        {
            Orientation = orientation;
        }

        public void AddChild(View view, float weight = 1)
        {
            if (view is FlexChild f)
                AddFlexChild(f);
            else
                AddFlexChild(new FlexChild(view, weight));
        }

        public void AddFlexChild(FlexChild view)
        {
            Children.Add(view);
        }



        public override void UpdateChildPos()
        {
            var freeSpace = Orientation == Orientation.Horizontal ? Width : Height;
            var totalWeight = 0f;
            var FlexChildren = Children.Cast<FlexChild>();
            foreach (var child in FlexChildren)
            {
                if (child.Weight == 0)
                {
                    if (Orientation == Orientation.Horizontal)
                    {
                        freeSpace -= child.MeasuredWidth + child.Margin.Width;
                    }
                    else if (Orientation == Orientation.Vertical)
                    {
                        freeSpace -= child.MeasuredHeight + child.Margin.Height;
                    }
                }
                else
                    totalWeight += child.Weight;

            }


            if (Orientation == Orientation.Horizontal)
            {
                int xpos = X, ypos = Y;
                foreach (var child in FlexChildren)
                {
                    child.Height = Height - child.Margin.Height - child.Padding.Height;

                    child.X = xpos + child.Margin.Left + child.Padding.Left;
                    child.Y = ypos + child.Margin.Top + child.Padding.Top;
                    if (child.Weight != 0)
                    {
                        // Change the size according to its weight
                        child.Width = (int)((child.Weight / totalWeight) * freeSpace) - child.Margin.Width - child.Padding.Width;
                    }
                    xpos += child.MeasuredWidth + child.Margin.Width;
                }
            }
            if (Orientation == Orientation.Vertical)
            {
                int xpos = X, ypos = Y;
                foreach (var child in FlexChildren)
                {
                    child.Width = Width - child.Margin.Width - child.Padding.Width;

                    child.X = xpos + child.Margin.Left + child.Padding.Left;
                    child.Y = ypos + child.Margin.Top + child.Padding.Top;
                    if (child.Weight != 0)
                    {
                        // Change the size according to its weight
                        child.Height = (int)((child.Weight / totalWeight) * freeSpace) - child.Margin.Height - child.Padding.Height;
                    }
                    ypos += child.MeasuredHeight + child.Margin.Height;
                }
            }

            // Update the child's children after
            base.UpdateChildPos();
        }

        public override void AddChild(View child, bool dontUpdateChildPos = false)
        {
            //throw new Exception("Can't add regular View to Flex, use FlexChild instead");
            if (child is FlexChild f)
                AddFlexChild(f);
            else
                AddChild(child, 0);
        }

        public override void RemoveChild(View child)
        {
            throw new Exception("Can't remove regular View from Flex, use FlexChild instead");
        }

    }
    public interface IFlexContent
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public Padding Padding { get; set; }
        public Margin Margin { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }
    public class FlexChild : SingleChildContainer, IFlexContent
    {
        public float Weight { get; set; } = 0;
        public override int Width { get => Child.Width; set => Child.Width = value; }
        public override int Height { get => Child.Height; set => Child.Height = value; }
        public override Padding Padding { get => Child.Padding; set => Child.Padding = value; }
        public override Margin Margin { get => Child.Margin; set => Child.Margin = value; }
        public override MouseCursor? Cursor { get => Child.Cursor; set => Child.Cursor = value; }
        public override int X { get => Child.X; set => Child.X = value; }
        public override int Y { get => Child.Y; set => Child.Y = value; }

        public FlexChild(View view, float weight)
        {
            Child = view;
            Weight = weight;
        }
    }
    //public class SpacerFlexChild : IFlexContent
    //{

    //    int width, height;
    //    Padding padding;
    //    Margin margin;
    //    int x, y;

    //    public SpacerFlexChild(int width, int height, Padding padding, Margin margin, int x, int y)
    //    {
    //        Width = width;
    //        Height = height;
    //        Padding = padding;
    //        Margin = margin;
    //        X = x;
    //        Y = y;
    //    }

    //    public int Width { get => width + Padding.Width; set => width = value; }
    //    public int Height { get => height + Padding.Height; set => height = value; }
    //    public Padding Padding { get => padding; set => padding = value; }
    //    public Margin Margin { get => margin; set => margin = value; }
    //    public int X { get => x; set => x = value; }
    //    public int Y { get => y; set => y = value; }
    //}

}
