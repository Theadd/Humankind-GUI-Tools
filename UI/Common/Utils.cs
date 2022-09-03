using UnityEngine;
using Modding.Humankind.DevTools.DeveloperTools.UI;
using Modding.Humankind.DevTools;

namespace DevTools.Humankind.GUITools.UI
{
    public static class Utils
    {
        public static Texture2D WhiteTexture { get; set; } = CreateSinglePixelTexture2D(new Color(1f, 1f, 1f, 1f));
        public static Texture2D BlackTexture { get; set; } = CreateSinglePixelTexture2D(new Color(0, 0, 0, 1f));
        public static Texture2D TransparentTexture { get; set; } = CreateSinglePixelTexture2D(new Color(1f, 1f, 1f, 0f));
        
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

        public static void DrawH1(string title, bool paddingBottom = true)
        {
            GUILayout.Label("<size=20><b><color=#FFFFFFFF>" + title.ToUpper() + "</color></b></size>");
            DrawHorizontalLine();
            if (paddingBottom) GUILayout.Space(12f);
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

        // UNIT UTILL

        public static Sprite LoadUnitSprite(Amplitude.StaticString unitName) => LoadUnitSprite(unitName.ToString());
        public static Sprite LoadUnitSprite(string unitName) => SharedAssets.SharedAssets.Load<Sprite>(unitName);
        public static Texture2D LoadTexture(string name) => SharedAssets.SharedAssets.Load<Texture2D>(name);
        public static string NormalizeUnitName(string unitName) => unitName.Substring(unitName.IndexOf('_') + 1);

        // FIDS GAINS AS IMAGE

        private static GUIStyle inlineLabel = null;
        public static GUIStyle InlineLabel => inlineLabel ?? (inlineLabel = new GUIStyle(UIController.DefaultSkin.FindStyle("RightAlignedLabel")) {
            stretchWidth = true,
            imagePosition = ImagePosition.ImageLeft,
            alignment = TextAnchor.UpperRight,
            name = "InlineLabel",
            // fontStyle = FontStyle.Bold,
            // normal = UIController.DefaultSkin.toggle.normal
            /*normal = new GUIStyleState() {
                background = UIController.DefaultSkin.button.normal.background,
                textColor = Color.white
            },*/
            margin = new RectOffset(10, 0, 0, 4)
        });
        public static Texture MoneyTexture = SharedAssets.SharedAssets.Load<Texture2D>("Money");
        public static Texture ScienceTexture = SharedAssets.SharedAssets.Load<Texture2D>("Science");
        public static Texture FoodTexture = SharedAssets.SharedAssets.Load<Texture2D>("Food");
        public static Texture IndustryTexture = SharedAssets.SharedAssets.Load<Texture2D>("Industry");
        public static Texture InfluenceTexture = SharedAssets.SharedAssets.Load<Texture2D>("Influence");
        public static Texture FaithTexture = SharedAssets.SharedAssets.Load<Texture2D>("Faith");
        public static Texture StabilityTexture = SharedAssets.SharedAssets.Load<Texture2D>("Stability");
        public static Texture LeftArrowHeadTexture = SharedAssets.SharedAssets.Load<Texture2D>("LeftArrowHead");

        public static Font SegoeUISymbolFont { get; set; } =
            SharedAssets.SharedAssets.Load<Font>("seguisym");
        public static Font SourceCodeProRegularFont { get; set; } =
            Modding.Humankind.DevTools.DevTools.Assets.Load<Font>("sourcecodepro-regular");

        public static GUIContent GetLootInfoContent(string text)
        {
            var sub = text.Split(new char[] {' '}, 3);
            Texture tex;
            string tooltip = "";

            switch (sub[1])
            {
                case "[MoneyColored]":
                    tex = MoneyTexture;
                    tooltip = sub[0] + " Money";
                    break;
                case "[ScienceColored]":
                    tex = ScienceTexture;
                    tooltip = sub[0] + " Science";
                    break;
                case "[CultureColored]":
                    tex = InfluenceTexture;
                    tooltip = sub[0] + " Influence";
                    break;
                case "[FoodColored]":
                    tex = FoodTexture;
                    tooltip = sub[0] + " Food";
                    break;
                default:
                    tex = TransparentTexture;
                    sub[0] = text;
                    tooltip = text;
                    break;
            }

            return new GUIContent(sub[0], tex , tooltip);
        }

        public static void DrawLootInfoLabel(string text)
        {
            GUIContent raw = GetLootInfoContent(text);

            GUILayout.BeginHorizontal();
                GUILayout.Label(new GUIContent(raw.text, raw.tooltip), InlineLabel);
                GUI.DrawTexture(GUILayoutUtility.GetRect(18f, 18f),
                    raw.image, ScaleMode.StretchToFill, true, 
                    1f, Color.white, 0, 0);
            GUILayout.EndHorizontal();
        }

    }
}
