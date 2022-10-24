using System.Xml;
using Tamarix.Views;
namespace Tamarix
{
    public class LayoutInflater
    {
        public View Inflate(string source, string name)
        {
            XmlDocument doc = new();
            doc.LoadXml(source);
            var view = _inflate(doc);
            if (view != null)
                return view;
            // if view is null parsing failed
            throw new Exception($"Can't inflate: {name}");
        }

        private string? getAttrOrNull(XmlNode node, string attrName)
        {
            if (node.Attributes == null)
                return null;
            var item = node.Attributes.GetNamedItem(attrName);
            if (item == null)
                return null;

            return item.Value;
        }

        /// <summary>
        /// Parses attributes to check is it resource or not
        /// </summary>
        /// <returns>Reource or just string</returns>
        private object parseAttrString(string content, string attrName)
        {
            // trim whitespaces
            content = content.Trim();

            // now check if it's a resource
            if (content.StartsWith("@"))
            {
                var ind = content.IndexOf("(");
                var indClose = content.IndexOf(")");
                if (ind == -1 || indClose == -1)
                    throw new Exception($"Error parsing attribute: {attrName} '{content}'");

                var name = content.Substring(1, ind - 1);
                var value = content.Substring(ind + 1, indClose - ind - 1);

                if (name == "resources")
                {
                    // Resources will fetch EmbeddedReosurce in the sln file
                    if (value.EndsWith(".svg"))
                    {
                        // Create an svg
                        return Image.FromResource(Window.Current!.GetType(), value, 32, 32); //TODO: change default size
                    }
                    else
                        throw new Exception($"Unsupported resource type: {attrName}:{value}");
                }
                else
                    throw new Exception($"Unsupported attribute type: @{name}");
            }

            // Returns just a string
            return content;
        }

        /// <summary>
        /// Actually parse the XML node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private View? _inflate(XmlNode node)
        {
            // skips comment and document node
            if (node.NodeType == XmlNodeType.Comment)
                return null;
            if (node.NodeType == XmlNodeType.Document)
                return _inflate(node.FirstChild!);
            string? id = getAttrOrNull(node, "Id");
            string? weightAttr = getAttrOrNull(node, "Weight");
            int weight = 0;
            if (weightAttr != null)
            {
                if (!int.TryParse(weightAttr, out weight))
                {
                    throw new Exception($"Error parsing attribute Weight: {weightAttr}");
                }
            }
            //Console.WriteLine($"{{{node.NodeType}}}<{node.Name}>");

            View? view = null;

            // Parse the XML elements
            switch (node.Name)
            {
                case "Button":
                    string? content = null;
                    if (node.Attributes != null)
                    {
                        var _c = node.Attributes.GetNamedItem("Content");
                        if (_c != null && _c.Value != null)
                            content = _c.Value;
                    }
                    if (content != null)
                    {
                        var contentValue = parseAttrString(content, "Content");
                        if (contentValue is string contentString)
                            view = new Button(contentString);
                        else if (contentValue is Image img)
                            view = new Button(img);
                        else
                            throw new Exception($"Error: Unsupported value of attribute 'Content' {contentValue}");
                    }
                    else
                        view = new Button();
                    break;



                case "Label":
                    string? text = null;
                    if (node.Attributes != null)
                    {
                        var _c = node.Attributes.GetNamedItem("Text");
                        if (_c != null && _c.Value != null)
                            text = _c.Value;
                    }
                    if (text != null)
                        view = new Label(text);
                    else
                        view = new Label();
                    break;



                case "LinearLayout":
                    {
                        var orientation = Orientation.Horizontal; // default value
                        if (node.Attributes != null)
                        {
                            var _c = node.Attributes.GetNamedItem("Orientation");
                            if (_c != null && _c.Value != null)
                                orientation = (_c.Value == "Vertical") ? Orientation.Vertical : Orientation.Horizontal;


                        }
                        var mainAxisAlignStr = getAttrOrNull(node, "MainAxisAlignment");
                        var crossAxisAlignStr = getAttrOrNull(node, "CrossAxisAlignment");
                        var mainAxisAlign = Alignment.Center;
                        var crossAxisAlign = Alignment.Center;
                        if (mainAxisAlignStr == "Stretch")
                            mainAxisAlign = Alignment.Stretch;
                        else if (mainAxisAlignStr == "Begin")
                            mainAxisAlign = Alignment.Begin;
                        else if (mainAxisAlignStr == "End")
                            mainAxisAlign = Alignment.End;

                        if (crossAxisAlignStr == "Stretch")
                            crossAxisAlign = Alignment.Stretch;
                        else if (crossAxisAlignStr == "Begin")
                            crossAxisAlign = Alignment.Begin;
                        else if (crossAxisAlignStr == "End")
                            crossAxisAlign = Alignment.End;
                        view = new LinearLayout(orientation, null, mainAxisAlign, crossAxisAlign);
                    }
                    break;

                case "Flex":
                    {
                        var orientation = Orientation.Horizontal; // default value
                        if (node.Attributes != null)
                        {
                            var _c = node.Attributes.GetNamedItem("Orientation");
                            if (_c != null && _c.Value != null)
                                orientation = (_c.Value == "Vertical") ? Orientation.Vertical : Orientation.Horizontal;
                        }
                        view = new Flex(orientation);
                    }
                    break;

                case "LoadingInfiniteIndicator":
                    {
                        view = new LoadingInfiniteIndicator();
                    }
                    break;

            }

            if (view == null)
                throw new Exception($"Unknown XML node: {node.Name}");
            view.Id = id;
            if (weightAttr != null)
            {
                // Automatically converts the view into a FlexChild
                view = new FlexChild(view, weight);
            }

            AddChildToView(node, view);
            return view;
        }

        private void AddChildToView(XmlNode node, View view)
        {

            // If can has child
            if (node.ChildNodes.Count > 0)
            {
                if (view is MultiChildContainer container)
                    foreach (XmlNode child in node.ChildNodes)
                    {
                        var childView = _inflate(child);
                        if (childView == null)// TODO: add error handling
                            continue;

                        container.AddChild(childView);
                    }
                else if (view is SingleChildContainer scc)
                {
                    // FlexChild have to be treated differently
                    if (view is FlexChild f)
                    {
                        if (f.Child != null)
                            AddChildToView(node, f.Child);
                    }
                    else
                    {
                        if (node.ChildNodes.Count > 1)
                            throw new Exception($"Element <{node.Name}> is a single child container, can only contains one view");
                        if (node.FirstChild == null)
                            throw new Exception($"Tag <{node.Name}> doesn't have a child");
                        var childView = _inflate(node.FirstChild);
                        if (childView == null)
                            throw new Exception($"Cannot inflate child of <{node.Name}>");

                        scc.Child = childView;
                    }
                }
                else
                {
                    throw new Exception($"Element <{node.Name}> is not a container, thus can't contain any children");
                }
            }
        }
    }
}
