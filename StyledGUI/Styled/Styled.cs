using UnityEngine;

namespace StyledGUI
{
    public static partial class Styled
    {

        public static void Alert(string message) => Alert(message, Colors.DeepSkyBlue);

        public static void Alert(string message, Colorable color)
        {
            var prev = GUI.backgroundColor;
            GUI.backgroundColor = color;
            GUILayout.Label($"<color={color}>{message}</color>", Styles.AlertLabelStyle);
            GUI.backgroundColor = prev;
        }
    }
}
