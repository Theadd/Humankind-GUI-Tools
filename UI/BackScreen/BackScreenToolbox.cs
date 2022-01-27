using System;
using Modding.Humankind.DevTools;
using UnityEngine;

namespace DevTools.Humankind.GUITools.UI
{
    public class BackScreenToolbox
    {
        public BackScreenWindow Window { get; set; }
        // public string TextFilter { get; set; } = string.Empty;

        public void Draw(Rect targetRect)
        {
            GUILayout.BeginArea(targetRect);
            {
                Draw();
            }
            GUILayout.EndArea();
        }

        public void Draw()
        {
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            {
                // TextFilter = GUILayout.TextField(TextFilter);
                
                if (GUILayout.Button("LOCK FULLSCREEN MOUSE EVENTS"))
                    Window.ScreenOverlay.LockFullScreenMouseEvents = true;
                if (GUILayout.Button("UNLOCK FULLSCREEN MOUSE EVENTS"))
                    Window.ScreenOverlay.LockFullScreenMouseEvents = false;
                if (GUILayout.Button("ENABLE FULLSCREEN BACKGROUND"))
                    Window.ScreenOverlay.EnableFullScreenBackgroundColor = true;
                if (GUILayout.Button("DISABLE FULLSCREEN BACKGROUND"))
                    Window.ScreenOverlay.EnableFullScreenBackgroundColor = false;
                if (GUILayout.Button("LOG ScreenOverlay"))
                    Loggr.Log(Window.ScreenOverlay);
                if (GUILayout.Button("LOG ScreenOverlay.InnerUITransform"))
                    Loggr.Log(Window.ScreenOverlay.InnerUITransform);
                if (GUILayout.Button("LOG ScreenOverlay.UITransform"))
                    Loggr.Log(Window.ScreenOverlay.UITransform);
                if (GUILayout.Button("LoadIfNecessary ScreenOverlay.InnerControl "))
                    Window.ScreenOverlay.InnerControl.LoadIfNecessary();
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
    }
}