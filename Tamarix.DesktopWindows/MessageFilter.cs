using System.Diagnostics;

namespace Tamarix.DesktopWindows
{
    class MessageFilter : IMessageFilter
    {
        public bool PreFilterMessage(ref Message m)
        {
            Debug.WriteLine(m.Msg);
            Console.WriteLine(m.Msg);
            return false;
        }
    }
}
