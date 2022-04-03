using System;
using Amplitude.Framework.Overlay;
using Amplitude.Framework.Runtime;
using Amplitude.Mercury.Overlay;
using Amplitude.UI;
using DevTools.Humankind.GUITools.UI.SceneInspector;
using Modding.Humankind.DevTools;
using UnityEngine;
using Modding.Humankind.DevTools.DeveloperTools.UI;

namespace DevTools.Humankind.GUITools.UI
{
    public abstract class BackScreenWindowBase : PopupToolWindow
    {
        public virtual Rect WindowRect { get; set; } = new Rect(0, 0, Screen.width, Screen.height);
        public virtual string WindowGUIStyle { get; set; } = "PopupWindow.Sidebar";
        public ScreenUIOverlay ScreenOverlay { get; set; }
        public abstract string UniqueName { get; }
        public int WindowID => Math.Abs(GetInstanceID());

        public abstract void OnDrawUI(int _);

        public abstract void OnZeroGUI();

        private int loop = 0;
        private bool _doLazyLoop = true;
        private bool _doDrawUI = true;
        protected int maxLoop = 30; // TODO: Changed from 40 to 10 for quicker development on live reload env

        protected override void Awake()
        {
            WindowStartupLocation = new Vector2(WindowRect.x, WindowRect.y);
            Title = string.IsNullOrEmpty(Title) ? "BACK SCREEN WINDOW" : Title;
            base.Awake();
            Width = WindowRect.width;
        }

        void OnGUI()
        {
            GUI.skin = UIController.DefaultSkin;
            GUI.color = Color.white;
            GUI.backgroundColor = Color.white;
            GUI.enabled = true;

            if (Event.current.type == EventType.Repaint)
            {
                if (loop == 0)
                {
                    _doLazyLoop = !ViewController.IsGloballyDisabled;
                    _doDrawUI = _doLazyLoop;
                    SyncScreenUIOverlay();
                }

                if (loop++ > maxLoop)
                    loop = 0;
                
                if (_doLazyLoop)
                {
                    OnZeroGUI();
                    _doLazyLoop = false;
                }
            }

            if (_doDrawUI)
                WindowRect = GUI.Window(WindowID, WindowRect, OnDrawUI, string.Empty, WindowGUIStyle);
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

        protected override void OnBecomeVisible()
        {
        }

        protected override void OnBecomeInvisible()
        {
        }

        public override Rect GetWindowRect() => WindowRect;

        public override void SetWindowRect(Rect rect)
        {
            throw new NotSupportedException();
        }

        public override void Close(bool saveVisibilityStateBeforeClosing = false)
        {
            PlacementCursorController.Unload();
            ToolboxController.Unload();
            ScreenUIOverlay.Unload();
            SceneInspectorController.Unload();
            base.Close(false);
        }
    }
}
