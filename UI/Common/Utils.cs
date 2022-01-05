using UnityEngine;

namespace DevTools.Humankind.GUITools.UI
{
    public static class Utils
    {
        public static Texture2D WhiteTexture = CreateSinglePixelTexture2D(new Color(1f, 1f, 1f, 1f));
        
        public static Texture2D CreateSinglePixelTexture2D(Color color)
        {
            Texture2D tex = new Texture2D(1,1);
            tex.SetPixel(0, 0, color);
            tex.Apply();

            return tex;
        }

        public static void DrawHorizontalLine(float alpha = 0.45f) =>
            GUI.DrawTexture(GUILayoutUtility.GetRect(GUILayoutUtility.GetLastRect().width, 1f),
                WhiteTexture, ScaleMode.StretchToFill, true, 
                1f, new Color(255, 255, 255, alpha), 0,0);

        public static void DrawH1(string title)
        {
            GUILayout.Label("<size=20><b><color=#FFFFFFFF>" + title.ToUpper() + "</color></b></size>");
            DrawHorizontalLine();
            GUILayout.Space(12f);
        }
        
        public static void DrawH2(string title)
        {
            GUILayout.Label("<size=16><b><color=#FFFFFFCC>" + title.ToUpper() + "</color></b></size>");
            DrawHorizontalLine(0.3f);
            GUILayout.Space(8f);
        }
        
        public static void DrawText(string text)
        {
            GUILayout.Label("<size=13><color=#FFFFFFBC>" + text + "</color></size>", "Text");
        }
    }
}
