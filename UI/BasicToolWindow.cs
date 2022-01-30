using System;
using System.Reflection;
using Amplitude;
using Amplitude.Framework;
using Amplitude.Graphics;
using Amplitude.Mercury;
using Amplitude.Mercury.AI;
using Amplitude.Mercury.Audio;
using UnityEngine;
using Modding.Humankind.DevTools;
using Modding.Humankind.DevTools.Core;
using Modding.Humankind.DevTools.DeveloperTools.UI;
using Amplitude.Mercury.Interop;
using Amplitude.Mercury.Presentation;
using Amplitude.Mercury.Terrain;
using Amplitude.Mercury.UI;
using Amplitude.UI;
using Amplitude.Framework;
using Amplitude.Framework.Overlay;
using Amplitude.Mercury.Audio;
using Amplitude.Wwise.Audio;
using System.Reflection;
using Amplitude.Mercury.Input;
using UnityEngine;

namespace DevTools.Humankind.GUITools.UI
{
    public class BasicToolWindow : FloatingToolWindow
    {
        public override bool ShouldBeVisible => true;
        public override bool ShouldRestoreLastWindowPosition => true;
        public override string WindowTitle { get; set; } = "BASIC TOOL WINDOW";
        public override Rect WindowRect { get; set; } = new Rect(300, 300, 900, 900);

        public static FieldInfo EmpireEndGameStatusField =
            R.GetField<Amplitude.Mercury.Simulation.MajorEmpire>("EmpireEndGameStatus", R.NonPublicInstance);

        private int loop = 0;

        private GUIStyle bgStyle = new GUIStyle(UIController.DefaultSkin.FindStyle("PopupWindow.Sidebar.Highlight"))
        {
            normal = new GUIStyleState()
            {
                background = Utils.CreateSinglePixelTexture2D(new Color(0, 0, 0, 0.8f)),
                textColor = Color.white
            },
            hover = new GUIStyleState()
            {
                background = null,
                textColor = Color.white
            }
        };

        public Texture EmpireIconTexture { get; set; }

        public override void OnDrawUI()
        {
            var asMajorEmpire = HumankindGame.Empires[0].Simulation;
            EmpireEndGameStatus endGameStatus = (EmpireEndGameStatus) EmpireEndGameStatusField.GetValue(asMajorEmpire);

            if (loop < 20)
            {
                if (loop > 18)
                {
                    // EmpireIcon = UIController.GameUtils.GetEmpireIcon(1);
                    EmpireIcon = UIController.GameUtils.GetEmpireBannerOrnament(0);
                    // EmpireIcon.GetAsset();
                    EmpireIcon.RequestAsset();
                    EmpireIconTexture = EmpireIcon.GetAsset();
                    Loggr.Log(EmpireIconTexture);
                    Loggr.LogAll(EmpireIcon);
                    EmpireIcon.RequestAsset();
                    Loggr.LogAll(EmpireIcon.GetAsset());
                }

                if (loop == 16 && endGameStatus == EmpireEndGameStatus.Resigned)
                {
                    EmpireEndGameStatusField.SetValue(asMajorEmpire, EmpireEndGameStatus.InGame);
                }

                loop++;
            }

            GUILayout.BeginVertical(bgStyle);
            {
                DrawUITexture();

                StaticString staticString = StaticString.Empty;
                if (GUILayout.Button(AudioEvents.CitySirenPlayed.ToString(), Array.Empty<GUILayoutOption>()))
                {
                    staticString = AudioEvents.CitySirenPlayed;
                }

                if (GUILayout.Button(AudioEvents.CitySirenStopped.ToString(), Array.Empty<GUILayoutOption>()))
                {
                    staticString = AudioEvents.CitySirenStopped;
                }

                if (!StaticString.IsNullOrEmpty(staticString))
                {
                    /*Services.GetService<Amplitude.Wwise.Audio.IAudioModulesService>().SendAudioEvent(staticString, new object[]
                    {
                        city.SimulationEntityGUID
                    });*/
                }

                DrawValue("rotationX", ActionController.FreeCamera.FreeCam?.rotationX.ToString());
                DrawValue("rotationY", ActionController.FreeCamera.FreeCam?.rotationY.ToString());
                DrawValue("Position", ActionController.FreeCamera.FreeCam?.GetTransform().position.ToString());
                DrawValue("localRotation",
                    ActionController.FreeCamera.FreeCam?.GetTransform().localRotation.ToString());
                DrawValue("FreeCam rotation", ActionController.FreeCamera.FreeCam?.GetTransform().rotation.ToString());
                DrawValue("nearClipPlane", ActionController.Camera?.nearClipPlane.ToString());
                DrawValue("farClipPlane", ActionController.Camera?.farClipPlane.ToString());
                DrawValue("fieldOfView", ActionController.Camera?.fieldOfView.ToString());
                DrawValue("Camera rotation", ActionController.Camera?.transform.rotation.ToString());
                DrawValue("WorldPosition Highlighted",
                    Presentation.PresentationCursorController.CurrentHighlightedPosition.ToString());

                // UpdateDebugger();
                // OnDrawTerrainDebugger();

                Utils.DrawHorizontalLine(0.6f);

                GUILayout.BeginHorizontal();
                {
                    GUI.enabled = HumankindGame.IsGameLoaded;
                    GUILayout.Label("<size=11><b>CAMERA, LEFTCTRL + ...</b></size>");
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("<size=10><b>1: Loggr => CameraController</b></size>"))
                        Loggr.Log(ActionController.FreeCamera);
                    if (GUILayout.Button("<size=10><b>2: Loggr => FreeCamera</b></size>"))
                        Loggr.Log(ActionController.FreeCamera?.FreeCam);

                    if (GUILayout.Button("<size=10><b>3: Loggr => PresentationCameraMover</b></size>"))
                        Loggr.Log(ActionController.FreeCamera?.presCamMover);

                    if (GUILayout.Button("<size=10><b>4: Loggr => Camera</b></size>"))
                        Loggr.Log(ActionController.FreeCamera?.cameraCam);
                    GUI.enabled = true;
                }
                GUILayout.EndHorizontal();
                OnDrawWindowClientArea(0);
            }
            GUILayout.EndVertical();
        }

        private void DrawValue(string title, string value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("<size=11><b>" + title.ToUpper() + "</b></size>");
            GUILayout.FlexibleSpace();
            GUILayout.Label(value, "RightAlignedLabel");
            GUILayout.EndHorizontal();
        }

        public Texture2D HeaderImage { get; set; } =
            Modding.Humankind.DevTools.DevTools.Assets.Load<Texture2D>("GameplayOrientation_Warmonger");

        public UITexture EmpireIcon { get; set; } = UIController.GameUtils.GetEmpireIcon(0);

        public void DrawUITexture()
        {
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(GUILayout.ExpandWidth(false), GUILayout.Width(78f), GUILayout.ExpandHeight(false),
                GUILayout.Height(78f));
            GUILayoutUtility.GetRect(64f, 64f);
            GUI.DrawTexture(
                new Rect(18f, 10f, 64f, 64f),
                HeaderImage,
                ScaleMode.ScaleToFit,
                true,
                1f,
                new Color32(255, 255, 255, 240),
                0,
                0
            );
            GUILayout.EndVertical();
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false), GUILayout.Height(82f),
                GUILayout.MaxHeight(82f));
            GUILayout.Label(
                ("<size=4>\n</size><size=10><b>  Showcasing</b> a fully featured demo of an " +
                 "<b>in-game<size=3>\n\n</size>  Tool</b> made with <color=#1199EECC><b>Humankind Modding DevTools</b></color>" +
                 "</size><size=3>\n</size>").ToUpper(), "Text");
            GUILayout.FlexibleSpace();
            GUI.color = new Color(1f, 1f, 1f, 0.5f);
            GUILayout.Label(R.Text.Size(R.Text.Bold("GUITooltip.ToUpper()"), 9), "Tooltip");
            GUI.color = Color.white;
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
        /*
        private static StaticString[] reflectedEventNames;
        private Vector2 scrollPosition = Vector2.zero;

        private IAudioModulesService AudioModulesService { get; set; }

        protected override void OnDrawWindowClientArea(int instanceId)
        {
            GUI.color = Color.white;
            GUI.backgroundColor = Color.white;
            GUILayout.BeginVertical((GUIStyle) "Widget.ClientArea", GUILayout.ExpandWidth(true), GUILayout.Height(400f));
            if (this.AudioModulesService != null)
            {
                if (BasicToolWindow.reflectedEventNames == null)
                    BasicToolWindow.GenerateReflectedEventNames();
                this.scrollPosition = GUILayout.BeginScrollView(this.scrollPosition);
                for (int index = 0; index < BasicToolWindow.reflectedEventNames.Length; ++index)
                {
                    StaticString reflectedEventName = BasicToolWindow.reflectedEventNames[index];
                    if (GUILayout.Button(reflectedEventName.ToString()))
                        this.AudioModulesService.SendAudioEvent(reflectedEventName);
                }
                GUILayout.EndScrollView();
            }
            else
            {
                GUILayout.Label("Waiting for the audio module service...");
                if (Event.current.type == EventType.Repaint)
                    this.AudioModulesService = Services.GetService<IAudioModulesService>();
            }
            GUILayout.EndVertical();
        }

        private static void GenerateReflectedEventNames()
        {
            FieldInfo[] fields = typeof (AudioEvents).GetFields(BindingFlags.Static | BindingFlags.Public);
            BasicToolWindow.reflectedEventNames = new StaticString[fields.Length];
            for (int index = 0; index < fields.Length; ++index)
                BasicToolWindow.reflectedEventNames[index] = (StaticString) fields[index].GetValue((object) null);
        }
        */

        [SerializeField] private ushort inputFilterGroup = 13;
        [SerializeField] private ushort inputFilterPrority = 1;
        private IInputFilterService inputFilterService;
        private InputFilterDeviceMask devicesToFilter;
        private int inputFilterHandle = -1;

        protected override void OnDrawWindowClientArea(int instanceId)
        {
            if (!this.IsVisible)
                return;
            GUI.color = Color.white;
            GUI.backgroundColor = Color.white;
            GUILayout.BeginVertical((GUIStyle) "Widget.ClientArea", GUILayout.ExpandWidth(true));
            if (this.inputFilterService == null)
                this.DrawWindow_NoService();
            else
                this.DrawClientArea();
            base.OnDrawWindowClientArea(instanceId);
        }

        private void DrawWindow_NoService()
        {
            GUILayout.Label("Waiting for the input filter service...");
            if (Event.current.type != EventType.Repaint)
                return;
            this.inputFilterService = Services.GetService<IInputFilterService>();
        }

        private void DrawClientArea()
        {
            bool flag1 = (uint) (this.devicesToFilter & InputFilterDeviceMask.Keyboard) > 0U;
            bool flag2 = (uint) (this.devicesToFilter & InputFilterDeviceMask.Mouse) > 0U;
            bool flag3 = (uint) (this.devicesToFilter & InputFilterDeviceMask.Gamepad) > 0U;
            bool flag4 = (uint) (this.devicesToFilter & InputFilterDeviceMask.Touch) > 0U;
            GUILayout.BeginVertical();
            GUILayout.Label("Select input device to block:");
            bool flag5 = GUILayout.Toggle(flag1, "Keyboard");
            bool flag6 = GUILayout.Toggle(flag2, "Mouse");
            bool flag7 = GUILayout.Toggle(flag3, "Gamepad");
            bool flag8 = GUILayout.Toggle(flag4, "Touch");
            if (GUI.changed)
            {
                InputFilterDeviceMask filterMask =
                    (InputFilterDeviceMask) (0 | (flag5 ? 1 : 0) | (flag6 ? 2 : 0) | (flag7 ? 4 : 0) | (flag8 ? 8 : 0));
                if (filterMask != this.devicesToFilter)
                    this.SetDeviceToFilter(filterMask);
            }

            GUILayout.EndVertical();
        }

        private void SetDeviceToFilter(InputFilterDeviceMask filterMask)
        {
            if (this.inputFilterHandle != -1)
                this.inputFilterHandle = this.inputFilterService.DestroyFilter(this.inputFilterHandle);
            this.devicesToFilter = filterMask;
            if (this.devicesToFilter == InputFilterDeviceMask.None)
                return;
            this.inputFilterHandle = this.inputFilterService.CreateFilter(this.devicesToFilter, this.inputFilterGroup,
                this.inputFilterPrority, true);
        }
    }
}
