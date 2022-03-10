using UnityEngine;

namespace StyledGUI
{
    public static partial class Styles
    {
        private static GUIStyle _tooltipOverlayStyle;
        private static GUIStyle _tooltipContainerStyle;
        private static GUIStyle _tooltipTextStyle;

        // public static GUIStyle DefaultLinkStyle => StyledGUIUtility.DefaultSkin.FindStyle("Link");

        public static GUIStyle TooltipOverlayStyle => _tooltipOverlayStyle ?? (_tooltipOverlayStyle =
            new GUIStyle()
            {
                alignment = TextAnchor.UpperRight,
                normal = new GUIStyleState()
                {
                    background = null,
                    textColor = Colors.SteelBlue
                }
            });
        
        public static GUIStyle TooltipContainerStyle => _tooltipContainerStyle ?? (_tooltipContainerStyle =
            new GUIStyle()
            {
                padding = new RectOffset(12, 12, 12, 12),
                alignment = TextAnchor.UpperRight,
                fontSize = 21,
                normal = new GUIStyleState()
                {
                    background = Alpha85BlackPixel,
                    textColor = Colors.Aquamarine
                }
            });
        
        public static GUIStyle TooltipTextStyle => _tooltipTextStyle ?? (_tooltipTextStyle =
            new GUIStyle(StyledGUIUtility.DefaultSkin.FindStyle("Text"))
            {
                // font = StyledGUIUtility.DefaultSkin.label.font,
                // fontSize = 13,
                font = null,
                fontSize = 12,
                alignment = TextAnchor.UpperCenter,
                padding = new RectOffset(4, 0, 0, 0),
                margin = new RectOffset(0, 0, 0, 0),
                stretchWidth = true,
                stretchHeight = false,
                wordWrap = true,
                clipping = TextClipping.Clip,
                name = "TooltipText",
                normal = new GUIStyleState()
                {
                    background = null,
                    textColor = WhiteTextColor
                }
            });
    }
}
