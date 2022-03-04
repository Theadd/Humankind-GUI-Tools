using System;
using Amplitude;
using DevTools.Humankind.GUITools.Collections;
using UnityEngine;

namespace DevTools.Humankind.GUITools.UI
{
//    
//        public StringHandle Value { get; set; }
//        
//        public static implicit operator StringHandle(CustomTooltip c) => c.Value;
//
//        public static explicit operator CustomTooltip(StringHandle s) =>
//            new CustomTooltip() { Value = s };
//        
//        public static bool operator ==(CustomTooltip x, CustomTooltip y) =>
//            (x?.Value.Handle ?? 0) == (y?.Value.Handle ?? 0);
//
//        public static bool operator !=(CustomTooltip x, CustomTooltip y) =>
//            (x?.Value.Handle ?? 0) != (y?.Value.Handle ?? 0);
//
//        public bool Equals(CustomTooltip other) =>
//            this.Value.Handle == (other?.Value.Handle ?? 0);
//        
//        public override bool Equals(object x)
//        {
//            switch (x)
//            {
//                case null:
//                    return this.Value.Handle == 0;
//                case CustomTooltip customTooltip:
//                    return customTooltip.Value.Handle == this.Value.Handle;
//                case StringHandle stringHandle:
//                    return stringHandle.Handle == this.Value.Handle;
//                default:
//                    return false;
//            }
//        }
//        
//        public override int GetHashCode() => this.Value.Handle;
//    }

    public class TooltipContainer : MetaContainer
    {
        public float DelayIn { get; set; } = 0.35f;
        public float DelayOut { get; set; } = 1f;
    }

    public static class TooltipOverlay
    {
        private static bool _shouldBeVisible = false;
        private static float t = 0;
        public static bool IsVisible { get; private set; }

        // Range is: 0.0f => x <= 1.0f
        public static Vector2 Center { get; set; } = new Vector2(0.5f, 0f);
        public static RectOffset Margin { get; set; } = new RectOffset(300, 300, 200, 200);
        public static Rect GlobalRect { get; private set; } = Rect.zero;
        public static Vector2 MaxSize { get; set; } = new Vector2(560f, 800f);
        

        public static TooltipContainer CurrentValue { get; private set; } = new TooltipContainer();

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
            using (var areaScope = new GUILayout.AreaScope(new Rect(10, 10, 100, 100)))
            {
                GUILayout.Button("Click me");
                GUILayout.Button("Or me");
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
                    UpdateRect();
                }
            }
        }

        private static void UpdateRect()
        {
            var pivot = new Vector2(Screen.width * Mathf.Clamp01(Center.x),
                Screen.height * Mathf.Clamp01(Center.y));
        }
    }
}
