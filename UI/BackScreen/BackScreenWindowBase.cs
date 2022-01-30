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
    public abstract class BackScreenWindowBase : PopupToolWindow
    {
        public virtual Rect WindowRect { get; set; } = new Rect (0, 0, Screen.width, Screen.height);
        public virtual string WindowGUIStyle { get; set; } = "PopupWindow.Sidebar";
        public ScreenUIOverlay ScreenOverlay { get; set; }
        public abstract string UniqueName { get; }
        public int WindowID => Math.Abs(GetInstanceID());
        
        public abstract void OnDrawUI(int _);

        public abstract void OnZeroGUI();

        protected int loop = 0;
        protected int maxLoop = 40;
        
        protected override void Awake()
        {
            WindowStartupLocation = new Vector2(WindowRect.x, WindowRect.y);
            Title = string.IsNullOrEmpty(Title) ? "BACK SCREEN WINDOW" : Title;
            base.Awake();
            Width = WindowRect.width;
        }
        
        void OnGUI () {
            GUI.skin = UIController.DefaultSkin;
            GUI.color = Color.white;
            GUI.backgroundColor = Color.white;
            GUI.enabled = true;

            if (Event.current.type == EventType.Repaint)
            {
                if (loop == 0)
                {
                    SyncScreenUIOverlay();
                    OnZeroGUI();
                }

                if (loop++ > maxLoop)
                    loop = 0;
            }
            WindowRect = GUI.Window (WindowID, WindowRect, OnDrawUI, string.Empty, WindowGUIStyle);
        }

        public void SyncScreenUIOverlay()
        {
            if (ScreenOverlay == null)
            {
                ScreenOverlay = ScreenUIOverlay.Find(UniqueName).FullScreenSync();
            }
            else
            {
                ScreenOverlay.FullScreenSync();
            }
        }
        
        protected override void OnBecomeVisible() { }
        protected override void OnBecomeInvisible() { }
        
        public override Rect GetWindowRect() => WindowRect;

        public override void SetWindowRect(Rect rect)
        {
            throw new NotSupportedException();
        }

        public override void Close(bool saveVisibilityStateBeforeClosing = false)
        {
            ToolboxController.Unload();
            ScreenUIOverlay.Unload();
            base.Close(false);
        }
    }
}
