using UnityEngine;

namespace StyledGUI
{
    public static partial class Styled
    {

        public static void Alert(string message) => Alert(message, Colors.LightBlue);

        public static void Alert(string message, Colorable color)
        {
            var prev = GUI.color;
            GUI.color = color;
            GUILayout.Label(message, Styles.AlertLabelStyle);
            GUI.color = Color.white;
            GUILayout.Label(message, Styles.AlertLabelStyle);
            GUI.color = prev;
            GUILayout.Label(message, Styles.AlertLabelStyle);
        }
    }
}
