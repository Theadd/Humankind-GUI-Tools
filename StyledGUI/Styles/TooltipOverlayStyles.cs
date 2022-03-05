using UnityEngine;

namespace StyledGUI
{
    public static partial class Styles
    {
        private static GUIStyle _tooltipOverlayStyle;
        private static GUIStyle _tooltipContainerStyle;

        // public static GUIStyle DefaultLinkStyle => StyledGUIUtility.DefaultSkin.FindStyle("Link");

        public static GUIStyle TooltipOverlayStyle => _tooltipOverlayStyle ?? (_tooltipOverlayStyle =
            new GUIStyle()
            {
                normal = new GUIStyleState()
                {
                    background = Graphics.White215Texture,
                    textColor = Color.black
                }
            });
        
        public static GUIStyle TooltipContainerStyle => _tooltipContainerStyle ?? (_tooltipContainerStyle =
            new GUIStyle()
            {
                alignment = TextAnchor.UpperCenter,
                padding = new RectOffset(12, 12, 12, 12),
                normal = new GUIStyleState()
                {
                    background = Alpha50BlackPixel,
                    textColor = Colors.Aquamarine
                }
            });
    }
}
