using Modding.Humankind.DevTools.DeveloperTools.UI;
using UnityEngine;

namespace StyledGUI
{
    public static partial class Styles
    {
        private static GUIStyle _treeRowBgStyle;

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
        
    }
}
