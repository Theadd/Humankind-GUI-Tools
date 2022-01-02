using UnityEngine;
using Modding.Humankind.DevTools;
using Modding.Humankind.DevTools.DeveloperTools.UI;

namespace DevTools.Humankind.GUITools.UI
{
    public class BasicToolWindow : FloatingToolWindow
    {
        public override bool ShouldBeVisible => true;
        public override bool ShouldRestoreLastWindowPosition => true;
        public override string WindowTitle { get; set; } = "BASIC TOOL WINDOW";
        public override void OnDrawUI()
        {
            GUILayout.BeginHorizontal();
                GUILayout.Label("1. HELLO WORLD!");
                if (GUILayout.Button("I'M A BUTTON!"))
                    Loggr.Log("BUTTON CLICKED! AND AGAIN YEAH!");
            GUILayout.EndHorizontal();
        }
    }
}
