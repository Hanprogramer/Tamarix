using Silk.NET.Vulkan;
using Tamarix.Views;

namespace Tamarix.Inspector
{
    public class InspectorWindow : Window
    {
        private readonly Window window;
        LinearLayout MainLayout;
        public InspectorWindow(Window window) : base("Inspector", 600, 600, null, null, false)
        {
            LayoutInflater inflater = new();
            var source = ResourceManager.ReadAssetFile(GetType(), "InspectorWindow.xml");
            SetView(inflater.Inflate(source, "MainWindow.xml"));

            MainLayout = (LinearLayout?)FindView("InspectorMainLayout");
            if (MainLayout == null)
                throw new Exception("Error finding view");
            this.window = window;
            RefreshElementTree();
        }

        public void RefreshElementTree()
        {
            // Child next to it would be dialogs, since window is StackLayout
            // TODO: separate the dialog stack with view stack.
            View root = window.Children[0];
            MainLayout.Children.Clear();
            _parseElement(root, MainLayout);
            UpdateChildPos();
        }

        private void _parseElement(View v, MultiChildContainer tree, int level = 0)
        {
            View result = new InspectorElementItem();
            if (v is Container)
            {
                LinearLayout container = new LinearLayout(Orientation.Vertical);
                container.CrossAxisAlignment = Alignment.Stretch;
                container.AddChild(new InspectorElementItem(v.GetType().Name, null, null, level));
                if (v is MultiChildContainer c)
                {
                    foreach (var child in c.Children)
                    {
                        _parseElement(child, container, level + 1);
                    }
                }
                else if (v is SingleChildContainer s && s.Child != null)
                {
                    _parseElement(s.Child, container, level + 1);
                }
                result = container;
            }
            else
                result = new InspectorElementItem(v.GetType().Name, null, null, level);

            tree.AddChild(result);
        }
    }
}
