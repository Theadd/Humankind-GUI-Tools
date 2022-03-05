using Amplitude.Framework.Overlay;
using Amplitude.Graphics;
using System;
using UnityEngine;
using Amplitude.Mercury.Terrain;
using Modding.Humankind.DevTools.Core;
using Modding.Humankind.DevTools.DeveloperTools.UI;

namespace DevTools.Humankind.GUITools.UI
{
    public class CameraLayersToolWindow : FloatingToolWindow
    {
        public override string WindowTitle { get; set; } = "CAMERA LAYERS TOOL";

        public override string WindowGUIStyle { get; set; } = "PopupWindow";

        public override bool ShouldBeVisible => !GlobalSettings.ShouldHideTools;

        public override bool ShouldRestoreLastWindowPosition => true;

        public override Rect WindowRect { get; set; } = new Rect(130f, 260f, 520f, 500f);

        private Color bgColor = new Color32(255, 255, 255, 230);
        private Color bgColorOpaque = new Color32(255, 255, 255, 255);

        public override void OnGUIStyling()
        {
            base.OnGUIStyling();
            GUI.backgroundColor = GlobalSettings.WindowTransparency.Value ? bgColor : bgColorOpaque;
        }

        public override void OnDrawUI()
        {
            if (GlobalSettings.WindowTitleBar.Value)
                WindowUtils.DrawWindowTitleBar(this);

            OnDrawWindowClientArea(0);
        }

        private CameraLayersProvider cameraLayersProvider;

        protected override void OnDrawWindowClientArea(int instanceId)
        {
            this.cameraLayersProvider =
                (bool) (UnityEngine.Object) this.cameraLayersProvider
                    ? this.cameraLayersProvider
                    : RenderContextAccess.GetInstance<CameraLayersProvider>(0);
            if (!(bool) (UnityEngine.Object) this.cameraLayersProvider)
            {
                GUILayout.Label(string.Format("{0} is not loaded.", (object) typeof (CameraLayersProvider)));
            }
            else
            {
                GUILayout.BeginVertical((GUIStyle) "Widget.ClientArea", GUILayout.ExpandWidth(true));
                this.cameraLayersProvider.DisplayGUI(this.Width);
                GUILayout.EndVertical();
            }
        }

        protected override void OnBecomeInvisible()
        {
            this.cameraLayersProvider = (CameraLayersProvider) null;
            base.OnBecomeInvisible();
        }
    }
}
