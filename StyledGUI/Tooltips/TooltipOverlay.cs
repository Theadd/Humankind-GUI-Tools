using DevTools.Humankind.GUITools.Collections;
using StyledGUI;
using UnityEngine;

namespace DevTools.Humankind.GUITools.UI
{
    public static class TooltipOverlay
    {
        private static bool _shouldBeVisible = false;
        private static float t = 0;
        private static float dt = 0;
        
        public static bool IsVisible { get; private set; }

        // Range is: 0.0f => x <= 1.0f
        public static Vector2 Center { get; set; } = new Vector2(0.5f, 0f);
        public static RectOffset Margin { get; set; } = new RectOffset(300, 300, 200, 200);
        public static Rect GlobalRect { get; private set; } = Rect.zero;
        public static Vector2 Size { get; set; } = new Vector2(480f, 800f);

        public static TooltipContainer CurrentValue { get; private set; } =
            TooltipContainer.Empty;

        private static string TooltipValue { get; set; } = string.Empty;

        public static void Run()
        {
            if (IsVisible)
                Render();

            _shouldBeVisible = t < 1f && dt >= 1f;
            t += Time.deltaTime / CurrentValue.DelayOut;
            dt += Time.deltaTime / CurrentValue.DelayIn;

            if (Event.current.type == EventType.Repaint)
            {
                if (IsVisible != _shouldBeVisible)
                {
                    IsVisible = _shouldBeVisible;
                    BackScreenWindow.ForceExpandToFitTooltipOverlay = IsVisible;
                }
            }
        }
        
        private static void Render()
        {
            using (var areaScope = new GUILayout.AreaScope(GlobalRect, "ⓘ", Styles.TooltipOverlayStyle))
            {
                GUILayout.BeginVertical("ⓘ", Styles.TooltipContainerStyle);
                GUILayout.Label(TooltipValue);
                GUILayout.EndVertical();
            }
        }

        // public static void SetTooltip(StringHandle tooltip) =>
        //     SetTooltip((TooltipContainer) tooltip);

        public static void SetTooltip(StringHandle tooltip)
        { 
            if (tooltip != StringHandle.Empty)
            {
                t = 0;
                _shouldBeVisible = true;

                if (CurrentValue.Value != tooltip)
                {
                    dt = 0;
                    CurrentValue = new TooltipContainer(tooltip);
                    
                    // TODO: async
                    TooltipValue = tooltip.GetLocalizedDescription();

                    // TODO: var stored = Storage.Get<TextContent>(CurrentValue.Value);
                    
                    // Loggr.Log(stored);
                    // Loggr.Log(JsonUtility.ToJson(stored));
                    UpdateRect();
                }
            }
        }

        private static void UpdateRect()
        {
            // TODO: Make use of something similar to RectTransform
            var pivot = new Vector2(Screen.width * Mathf.Clamp01(Center.x),
                Screen.height * Mathf.Clamp01(Center.y));

            var rect = new Rect(
                Mathf.Round(pivot.x - (Size.x / 2f)),
                Mathf.Round(pivot.y - (Size.y / 2f)),
                Mathf.Round(Size.x),
                Mathf.Round(Size.y));

            var moveX = (rect.x < Margin.left) ? Margin.left - rect.x : 0;
            var moveY = (rect.y < Margin.top) ? Margin.top - rect.y : 0;
            rect.Set(rect.x + moveX, rect.y + moveY, rect.width, rect.height);

            GlobalRect = rect;
        }
    }
}
