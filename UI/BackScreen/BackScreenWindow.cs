using System;
using Amplitude.Framework.Overlay;
using Amplitude.Framework.Runtime;
using Amplitude.Mercury.Overlay;
using Amplitude.UI;
using Modding.Humankind.DevTools;
using UnityEngine;
using Modding.Humankind.DevTools.DeveloperTools.UI;

namespace DevTools.Humankind.GUITools.UI
{
    public class BackScreenWindow : BackScreenWindowBase
    {
        public override bool ShouldBeVisible => true;
        public override bool ShouldRestoreLastWindowPosition => false;
        public override string UniqueName => "BackScreenWindow";
        public override void OnDrawUI(int _)
        {
            GUILayout.BeginArea(new Rect(0f, 0f, WindowRect.width, WindowRect.height));
            GUILayout.BeginHorizontal();
            
            GUILayout.BeginVertical();
            
            GUILayout.Label("HELLO WORLD WHAT THE FAAA =)");
            GUILayout.Label("HELLO WORLD WHAT THE FAAA =)");
            GUILayout.Label("HELLO WORLD WHAT THE FAAA =)");
            GUILayout.Button("THIS IS A BUTTON");
            GUILayout.Label("HELLO WORLD WHAT THE FAAA =)");
            GUILayout.Label("HELLO WORLD WHAT THE FAAA =)");
            
            GUILayout.EndVertical();
            
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        public override void OnZeroGUI()
        {
            // Update stuff here
            ScreenOverlay.InnerUITransform.X = 300f;
            ScreenOverlay.InnerUITransform.Y = 300f;
            ScreenOverlay.InnerUITransform.Width = 300f;
            ScreenOverlay.InnerUITransform.Height = 300f;
            ScreenOverlay.InnerUITransform.VisibleSelf = true;
            
            // TODO: HERE;  // Loggr.Log(ScreenOverlay.InnerUITransform);
        }
    }
}
