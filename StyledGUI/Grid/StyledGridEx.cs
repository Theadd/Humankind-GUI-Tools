using UnityEngine;

namespace StyledGUI
{
    public static class StyledGridEx
    {
        public static IStyledGrid Row(this IStyledGrid self, GUIStyle style, params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal(style, options);

            return self;
        }
        
        public static IStyledGrid Row(this IStyledGrid self, params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal(Styles.RowStyle, options);

            return self;
        }

        public static IStyledGrid EndRow(this IStyledGrid self)
        {
            GUILayout.EndHorizontal();

            return self;
        }

        public static IStyledGrid EmptyRow(this IStyledGrid self, params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal(Styles.StaticRowStyle, options);
            GUILayout.Label(" ", Styles.RowHeaderStyle);
            GUILayout.EndHorizontal();

            return self;
        }
        
        public static IStyledGrid VerticalStack(this IStyledGrid self, params GUILayoutOption[] options)
        {
            GUILayout.BeginVertical(Styles.StaticRowStyle, options);

            return self;
        }

        public static IStyledGrid EndVerticalStack(this IStyledGrid self, params GUILayoutOption[] options)
        {
            GUILayout.EndVertical();

            return self;
        }

        public static IStyledGrid Cell(this IStyledGrid self, string text, GUIStyle style, params GUILayoutOption[] options)
        {
            GUILayout.Label(text, style, options);

            return self;
        }

        public static IStyledGrid Cell(this IStyledGrid self, string text, params GUILayoutOption[] options)
        {
            GUILayout.Label(text, Styles.CellStyle, options);

            return self;
        }
        
        public static IStyledGrid Cell(this IStyledGrid self, string text, Color color, params GUILayoutOption[] options)
        {
            var prevColor = GUI.backgroundColor;
            GUI.backgroundColor = color;

            GUILayout.Label("<size=11>" + text + "</size>", Styles.CellStyle, options);

            GUI.backgroundColor = prevColor;

            return self;
        }

        public static IStyledGrid Cell(this IStyledGrid self, string text, GUIStyle style, Color color, params GUILayoutOption[] options)
        {
            var prevColor = GUI.backgroundColor;
            GUI.backgroundColor = color;

            GUILayout.Label("<size=11>" + text + "</size>", style, options);

            GUI.backgroundColor = prevColor;

            return self;
        }

        public static IStyledGrid RowHeader(this IStyledGrid self, string text, GUIStyle style, params GUILayoutOption[] options)
        {
            GUILayout.Label("<size=10><b>" + text + "</b></size>", style, options);

            return self;
        }
        
        public static IStyledGrid RowHeader(this IStyledGrid self, string text, params GUILayoutOption[] options)
        {
            GUILayout.Label("<size=10>" + text + "</size>", Styles.RowHeaderStyle, options);

            return self;
        }

        
        public static IStyledGrid DrawHorizontalLine(this IStyledGrid self, float alpha = 0.3f)
        {   
            Graphics.DrawHorizontalLine(alpha);

            return self;
        }

        public static IStyledGrid DrawHorizontalLine(this IStyledGrid self, float alpha, float width)
        {   
            Graphics.DrawHorizontalLine(alpha, width);

            return self;
        }

        public static IStyledGrid Space(this IStyledGrid self, float size)
        {   
            GUILayout.Space(size);

            return self;
        }
    }
}
