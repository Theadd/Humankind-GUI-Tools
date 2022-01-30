using UnityEngine;

namespace StyledGUI
{
    public static class Graphics
    {
        public static Texture2D WhiteTexture = CreateSinglePixelTexture2D(new Color(1f, 1f, 1f, 1f));
        public static Texture2D BlackTexture = CreateSinglePixelTexture2D(new Color(0, 0, 0, 1f));
        public static Texture2D TransparentTexture = CreateSinglePixelTexture2D(new Color(1f, 1f, 1f, 0f));
        
        public static Texture2D CreateSinglePixelTexture2D(Color color)
        {
            Texture2D tex = new Texture2D(1,1);
            tex.SetPixel(0, 0, color);
            tex.Apply();

            return tex;
        }

        public static void DrawHorizontalLine(float alpha = 0.45f)
        {
            var r = GUILayoutUtility.GetRect(1f, 1f);
            GUI.DrawTexture(new Rect(r.x, r.y, r.width - 3f, 1f),
                WhiteTexture, ScaleMode.StretchToFill, true, 
                1f, new Color(1f, 1f, 1f, alpha), 0,0);
        }

        public static void DrawHorizontalLine(float alpha, float width)
        {
            var r = GUILayoutUtility.GetRect(1f, 1f);
            GUI.DrawTexture(new Rect(r.x, r.y, width, 1f),
                WhiteTexture, ScaleMode.StretchToFill, true, 
                1f, new Color(1f, 1f, 1f, alpha), 0,0);
        }
    }
}
