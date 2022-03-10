using UnityEngine;

namespace StyledGUI
{
    public static partial class Styles
    {
        private static GUIStyle _alertLabelStyle;

        public static GUIStyle AlertLabelStyle => _alertLabelStyle ?? (_alertLabelStyle =
            new GUIStyle(StyledGUIUtility.DefaultSkin.FindStyle("Text"))
            {
                margin = new RectOffset(24, 24, 16, 16),
                padding = new RectOffset(12, 8, 8, 8),
                border = new RectOffset(8, 2, 8, 8),
                overflow = new RectOffset(0, 0, 0, 0),
                font = null,
                fontSize = 12,
                normal = new GUIStyleState()
                {
                    background = StyledGUIUtility.DefaultSkin.textArea.hover.background,
                    textColor = Color.white
                },
            });
        
    }
}
