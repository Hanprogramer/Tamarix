using System.Runtime.CompilerServices;
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
                    var orientation = Orientation.Horizontal; // default value
                    if (node.Attributes != null)
                    {
                        var _c = node.Attributes.GetNamedItem("Orientation");
                        if (_c != null && _c.Value != null)
                            orientation = (_c.Value == "Vertical") ? Orientation.Vertical : Orientation.Horizontal;
                    }
                    view = new LinearLayout(orientation);
                    break;
            }

            if (view == null)
                throw new Exception($"Unknown XML node: {node.Name}");



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
                    if (node.ChildNodes.Count > 1)
                        throw new Exception($"Element <{node.Name}> is a single child container, can only contains one view");
                    if (node.FirstChild == null)
                        throw new Exception($"Tag <{node.Name}> doesn't have a child");
                    var childView = _inflate(node.FirstChild);
                    if (childView == null)
                        throw new Exception($"Cannot inflate child of <{node.Name}>");

                    scc.Child = childView;
                }
                else
                {
                    throw new Exception($"Element <{node.Name}> is not a container, thus can't contain any children");
                }
            }
            return view;
        }
    }
}
