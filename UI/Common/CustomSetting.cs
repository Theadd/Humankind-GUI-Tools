using UnityEngine;
using Modding.Humankind.DevTools.Core;

namespace DevTools.Humankind.GUITools.UI
{
    public abstract class CustomSetting<T>
    {
        public T Value { get; set; }
        public string Name;
        public string Tooltip;
        public GUIContent Content;

        public CustomSetting(string name, string tooltip, T defaultValue)
        {
            Name = R.Text.Bold(R.Text.Size(name, 11));
            Tooltip = tooltip;
            Value = defaultValue;
            Content = new GUIContent(Name, Tooltip);
        }

        public abstract bool Draw();
    }

    public class BooleanSetting : CustomSetting<bool>
    {
        public BooleanSetting(string name, string tooltip, bool defaultValue) : base(name, tooltip, defaultValue) {}

        public override bool Draw() => (Value = GUILayout.Toggle(Value, Content));
    }
}
