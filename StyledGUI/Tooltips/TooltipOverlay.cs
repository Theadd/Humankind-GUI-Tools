using System;
using Amplitude;
using DevTools.Humankind.GUITools.Collections;
using DevTools.Humankind.GUITools.UI.Humankind;
using Modding.Humankind.DevTools;
using StyledGUI;
using UnityEngine;

namespace DevTools.Humankind.GUITools.UI
{
    public static class TooltipOverlay
    {
        private static bool _shouldBeVisible = false;
        private static float t = 0;
        
        public static bool IsVisible { get; private set; }

        // Range is: 0.0f => x <= 1.0f
        public static Vector2 Center { get; set; } = new Vector2(0.5f, 0f);
        public static RectOffset Margin { get; set; } = new RectOffset(300, 300, 200, 200);
        public static Rect GlobalRect { get; private set; } = Rect.zero;
        public static Vector2 Size { get; set; } = new Vector2(560f, 800f);
        
        public static TooltipContainer CurrentValue { get; private set; } = new TooltipContainer();

        private static string TooltipValue { get; set; } = string.Empty;

        public static void Run()
        {
            if (IsVisible)
                Render();

            _shouldBeVisible = t < 1f;
            t += Time.deltaTime / CurrentValue.DelayOut;

            if (Event.current.type == EventType.Repaint)
                IsVisible = _shouldBeVisible;
        }
        
        private static void Render()
        {
            using (var areaScope = new GUILayout.AreaScope(GlobalRect, "TOOLTIP OVERLAY AREA", Styles.TooltipOverlayStyle))
            {
                GUILayout.BeginVertical(Styles.TooltipContainerStyle);
                GUILayout.Label(TooltipValue);
                GUILayout.EndVertical();
            }
        }

        public static void SetTooltip(StringHandle tooltip) =>
            SetTooltip((TooltipContainer) tooltip);

        public static void SetTooltip(TooltipContainer tooltip)
        {
            if (tooltip != (TooltipContainer) null)
            {
                t = 0;
                _shouldBeVisible = true;

                if (CurrentValue != tooltip)
                {
                    CurrentValue = tooltip;
                    // TODO: async
                    TooltipValue = CurrentValue.Value.GetLocalizedDescription();

                    var stored = Storage.Get<ITextContent>(CurrentValue);
                    
                    Loggr.Log(stored);
                    Loggr.Log(JsonUtility.ToJson(stored));
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
