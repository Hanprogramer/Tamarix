namespace Tamarix.Views
{
    public static class AlertDialog
    {
        public static Dialog Builder(DialogContainerStack containerStack,
            string title,
            string? message = null,
            List<View>? children = null,
            string? positiveAction = null,
            string? negativeAction = null,
            string? cancelAction = null,
            bool cancelable = true,
            Dialog.OnClickShadeEvent? onClickShadeEvent = null)
        {
            var dlg = new Dialog(containerStack, onClickShade: onClickShadeEvent);

            var actionBar = new LinearLayout(Orientation.Horizontal);
            if (positiveAction != null)
                actionBar.AddChild(new Button(positiveAction, textColor: Theme.Current.AccentColor) { Transparent = true });
            if (negativeAction != null)
                actionBar.AddChild(new Button(negativeAction) { Transparent = true });
            if (cancelAction != null && cancelable)
                actionBar.AddChild(new Button(cancelAction, () => { dlg.Close(); }) { Transparent = true });


            var content = new LinearLayout(Orientation.Vertical)
            {
                Children = {
                    new Label(title)
                    {
                        FontSize = 24
                    },
                }
            };
            if (message != null)
                content.AddChild(new Label(message));
            if (children != null)
                content.AddChild(new LinearLayout(Orientation.Vertical, children));

            content.AddChild(actionBar);
            dlg.Child = content;

            return dlg;
        }
    }
}
