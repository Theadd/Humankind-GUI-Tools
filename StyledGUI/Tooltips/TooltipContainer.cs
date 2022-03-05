using DevTools.Humankind.GUITools.Collections;
using UnityEngine;

namespace DevTools.Humankind.GUITools.UI
{
    public class TooltipContainer : MetaContainer
    {
        public float DelayIn { get; set; } = 0.35f;
        public float DelayOut { get; set; } = 1f;
        public TextAnchor Anchor { get; set; } = TextAnchor.MiddleCenter;
        public Vector2 Size { get; set; } = Vector2.zero;
    }
}
