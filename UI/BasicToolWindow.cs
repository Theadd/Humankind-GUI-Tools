using System;
using System.Reflection;
using Amplitude;
using Amplitude.Framework;
using Amplitude.Graphics;
using Amplitude.Mercury;
using Amplitude.Mercury.AI;
using UnityEngine;
using Modding.Humankind.DevTools;
using Modding.Humankind.DevTools.Core;
using Modding.Humankind.DevTools.DeveloperTools.UI;
using Amplitude.Mercury.Interop;
using Amplitude.Mercury.Presentation;
using Amplitude.Mercury.Terrain;
using Amplitude.Mercury.UI;
using Amplitude.UI;

namespace DevTools.Humankind.GUITools.UI
{
    public class BasicToolWindow : FloatingToolWindow
    {
        public override bool ShouldBeVisible => true;
        public override bool ShouldRestoreLastWindowPosition => true;
        public override string WindowTitle { get; set; } = "BASIC TOOL WINDOW";
        public override Rect WindowRect { get; set; } = new Rect (300, 300, 400, 600);

        public static FieldInfo EmpireEndGameStatusField = R.GetField<Amplitude.Mercury.Simulation.MajorEmpire>("EmpireEndGameStatus", R.NonPublicInstance);

        private int loop = 0;
        
        private GUIStyle bgStyle = new GUIStyle(UIController.DefaultSkin.FindStyle("PopupWindow.Sidebar.Highlight")) {
            normal = new GUIStyleState() {
                background = Utils.CreateSinglePixelTexture2D(new Color(0, 0, 0, 0.8f)),
                textColor = Color.white
            },
            hover = new GUIStyleState() {
                background = null,
                textColor = Color.white
            }
        };

        public Texture EmpireIconTexture { get; set; }

        public override void OnDrawUI()
        {
            var asMajorEmpire = HumankindGame.Empires[0].Simulation;
            EmpireEndGameStatus endGameStatus = (EmpireEndGameStatus)EmpireEndGameStatusField.GetValue(asMajorEmpire);

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

        public Texture2D HeaderImage { get; set; } = Modding.Humankind.DevTools.DevTools.Assets.Load<Texture2D>("GameplayOrientation_Warmonger");
        public UITexture EmpireIcon { get; set; } = UIController.GameUtils.GetEmpireIcon(0);

        public void DrawUITexture()
        {
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(GUILayout.ExpandWidth(false), GUILayout.Width(78f), GUILayout.ExpandHeight(false), GUILayout.Height(78f));
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
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false), GUILayout.Height(82f), GUILayout.MaxHeight(82f));
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
    }
}
