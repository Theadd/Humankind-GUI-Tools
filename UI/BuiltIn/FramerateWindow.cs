using Amplitude.Mercury.Overlay;
using UnityEngine;
using Modding.Humankind.DevTools.DeveloperTools.UI;

namespace DevTools.Humankind.GUITools.UI.BuiltIn
{
    public class FramerateWindow : FloatingWindow_Framerate
    {
        public static GUISkin Skin = null;
        
        protected override void Awake()
        {
            this.WindowStartupLocation = new Vector2(100f, 350f);
            this.Title = "Framerate";
            base.Awake();
        }
        
        void OnGUI() {
            if (IsVisible)
            {
                GUI.skin = Skin != null ? Skin : UIManager.DefaultSkin;
                DrawWindow();
            }
        }

        protected override void OnDrawWindowTitle(int instanceId) =>
            WindowUtils.DrawWindowTitleBar(this);
        
        protected override void OnBecomeVisible() => this.SyncUIOverlay(base.OnBecomeVisible);
        protected override void OnBecomeInvisible() => this.SyncUIOverlay(base.OnBecomeInvisible);
    }
}