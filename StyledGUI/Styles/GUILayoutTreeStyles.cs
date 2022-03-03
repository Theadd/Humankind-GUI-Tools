using Modding.Humankind.DevTools.DeveloperTools.UI;
using UnityEngine;

namespace StyledGUI
{
    public static partial class Styles
    {
        private static GUIStyle _treeRowBgStyle;
        private static GUIStyle _treeInlineTextStyle;

        // public static GUIStyle DefaultLinkStyle => StyledGUIUtility.DefaultSkin.FindStyle("Link");

        public static GUIStyle TreeBackgroundRowStyle => _treeRowBgStyle ?? (_treeRowBgStyle =
            new GUIStyle(SectionHeaderToggleAreaStyle)
            {
                normal = new GUIStyleState()
                {
                    background = DefaultLinkStyle.normal.background,
                    textColor = Color.white
                },
                hover = new GUIStyleState()
                {
                    background = RowHoverPixel,
                    textColor = Color.white
                },
            });
        
        public static GUIStyle TreeInlineTextStyle => _treeInlineTextStyle ?? (_treeInlineTextStyle =
            new GUIStyle(StyledGUIUtility.DefaultSkin.FindStyle("Text"))
            {
                font = StyledGUIUtility.DefaultSkin.label.font,
                fontSize = 13,
                alignment = TextAnchor.MiddleLeft,
                padding = new RectOffset(4, 0, 0, 0),
                margin = new RectOffset(0, 0, 0, 0),
                stretchWidth = false,
                stretchHeight = false,
                wordWrap = false,
                clipping = TextClipping.Clip,
                name = "TreeInlineText",
                normal = new GUIStyleState()
                {
                    background = null,
                    textColor = WhiteTextColor
                }
            });
        
    }
}
