namespace Tamarix
{
    public class Theme
    {
        public string ThemeName { get; set; }

        /* Colors */
        public Color TitleBarColor;
        public Color BackgroundColor;
        public Color ForegroundColor;
        public Color ForegroundColorHover;
        public Color ForegroundColorActive;
        public Color TextColor;
        public Color AccentColor;
        public Color AccentColorInactive;
        public Color AccentColorDarker;
        public Color MenuButtonColor;
        public Color MenuButtonColorActive;

        public Color DialogShadeColor;

        /* Brushes */
        public IBrush ButtonBackground;
        public IBrush ToolButtonBackground;
        public IBrush ToolButtonActive;
        public StyleBox DialogBackground;
        public StyleBox TextFieldBackground;

        /* Sizes */
        public float DefaultFontSize;
        public int ButtonRounding;

        public int ToolButtonRounding;



#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Theme(string name)
        {
            ThemeName = name;
        }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.



        public static Theme Default = new("Default")
        {
            TitleBarColor = new Color(45, 45, 45),
            BackgroundColor = new Color(39, 39, 39),
            ForegroundColor = new Color(45, 45, 45),//new Color(0,0,255,50),//
            ForegroundColorHover = new Color(255, 255, 255, 50),//new Color(0,0,255,50),//
            ForegroundColorActive = new Color(35, 35, 35, 200),//new Color(0,0,255,50),//
            TextColor = Colors.White,
            AccentColor = new Color(16, 194, 116),
            AccentColorInactive = new Color(116,116,116),
            AccentColorDarker = new Color(6, 125, 73),

            MenuButtonColor = new Color(69, 69, 69),
            MenuButtonColorActive = new Color(59, 59, 59),
            DialogBackground = new StyleBox(
               fillBrush: new SolidColorBrush(new Color(39,39,39)),
               borderRadius: 8
            ),
            DialogShadeColor = new Color(0, 0, 0, 100),


            //ButtonBackground = new GradientColorBrush(new Color[] { new Color(0xFFFF7547), new Color(0xFFD70064) }, null),
            ButtonBackground = new SolidColorBrush(new Color(69, 69, 69)),
            TextFieldBackground = new StyleBox(
                fillBrush: new SolidColorBrush(new Color(30,30,30))
            ),
            DefaultFontSize = 18,
            ButtonRounding = 4
        };
        public static Theme Current = Default;
    }
}
