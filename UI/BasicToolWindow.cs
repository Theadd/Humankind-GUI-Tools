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

        public void SetupDebugger()
        {
            DebugControl.WantedDebugger = DebugControl.DebuggerType.Terrain;
            DebugControl.UpdatePresentationDebugger = true;
            
            
        }

        public void UpdateDebugger()
        {
            ITerrainPickingService instance = RenderContextAccess.GetInstance<ITerrainPickingService>(0);
            
            Hexagon.OffsetCoords offsetHexCoords = new Hexagon.OffsetCoords();
            if (instance.ScreenPositionToHexagonOffsetCoords((Vector2) Input.mousePosition, ref offsetHexCoords))
            {
                WorldPosition cursorPosition = DebugControl.CursorPosition;
                DebugControl.CursorPosition = new WorldPosition(offsetHexCoords);
                if (DebugControl.CursorPosition != cursorPosition)
                    DebugControl.UpdatePresentationDebugger = true;
            }
            else
            {
                DebugControl.CursorPosition = WorldPosition.Invalid;
                DebugControl.UpdatePresentationDebugger = true;
            }
        }
        
        private void OnDrawTerrainDebugger()
        {
            if (!DebugControl.CursorPosition.IsWorldPositionValid())
            {
                GUILayout.Label("NOT A VALID WORLD POSITION");
                return;
            }
            ref Amplitude.Mercury.Interop.AI.Data.Tile local =
                ref Amplitude.Mercury.Interop.AI.Snapshots.World.Tiles[DebugControl.CursorPosition.ToTileIndex()];
            GUILayout.Label(string.Format("Position {0} | {1}", (object) DebugControl.CursorPosition,
                (object) local.MovementType));
            GUILayout.Label(string.Format("{0} exploitable neighbors", (object) local.AdjacentExploitableTilesCount));
            GUILayout.Label(string.Format("Neighbor FIMS: {0}", (object) local.AdjacentExploitableTilesFims));
            GUILayout.Label(string.Format("Visible {0}",
                (object) Amplitude.Mercury.Presentation.Presentation.PresentationVisibilityController.IsTileVisible(
                    DebugControl.CursorPosition.ToTileIndex())));
            GUILayout.Label(string.Format("Explored {0}",
                (object) Amplitude.Mercury.Presentation.Presentation.PresentationVisibilityController.IsTileExplored(
                    DebugControl.CursorPosition.ToTileIndex())));
            if (!local.GreaterElevationThanAdjacentTiles)
                return;
            GUILayout.Label("None of the neighbors have a higher elevation.");
        }
        
        public override void OnDrawUI()
        {
            var asMajorEmpire = HumankindGame.Empires[0].Simulation;
            EmpireEndGameStatus endGameStatus = (EmpireEndGameStatus)EmpireEndGameStatusField.GetValue(asMajorEmpire);

            if (loop < 20)
            {
                if (loop > 18)
                {
                    
                    
                    
                    // SetupDebugger();
                    // Presentation.PresentationFrontiersController.DisplayAllFrontiers(true);
                    // Loggr.Log(Presentation.PresentationCameraController);
                    // Presentation.PresentationCameraController.SetCameraLayerModifier(PresentationCameraController.CameraModifierID.Diplomacy);
                    //Amplitude.Mercury.Presentation.Presentation.PresentationVisibilityController.VisibilityBufferMask |= PresentationVisibilityController.VisibilityBufferFlags.Timemap;
                    // ReadWriteBuffer1D<VisibilityEntry> visibilityBuffer = constHexBufferProvider.VisibilityBuffer;
                    // int num1 = WorldMapProviderHelper.VisibilityStatusToInt(Amplitude.Mercury.Terrain.VisibilityStatus.Visible);
                }
                if (loop == 16 && endGameStatus == EmpireEndGameStatus.Resigned) 
                {
                    EmpireEndGameStatusField.SetValue(asMajorEmpire, EmpireEndGameStatus.InGame);
                }
                loop++;
            }

            GUILayout.BeginVertical(bgStyle);
            DrawValue("rotationX", ActionController.FreeCamera.FreeCam?.rotationX.ToString());
                DrawValue("rotationY", ActionController.FreeCamera.FreeCam?.rotationY.ToString());
                DrawValue("Position", ActionController.FreeCamera.FreeCam?.GetTransform().position.ToString());
                DrawValue("localRotation", ActionController.FreeCamera.FreeCam?.GetTransform().localRotation.ToString());
                DrawValue("FreeCam rotation", ActionController.FreeCamera.FreeCam?.GetTransform().rotation.ToString());
                DrawValue("nearClipPlane", ActionController.Camera?.nearClipPlane.ToString());
                DrawValue("farClipPlane", ActionController.Camera?.farClipPlane.ToString());
                DrawValue("fieldOfView", ActionController.Camera?.fieldOfView.ToString());
                DrawValue("Camera rotation", ActionController.Camera?.transform.rotation.ToString());
                DrawValue("WorldPosition Highlighted", Presentation.PresentationCursorController.CurrentHighlightedPosition.ToString());

                // UpdateDebugger();
                // OnDrawTerrainDebugger();
                
                Utils.DrawHorizontalLine(0.6f);

                GUILayout.BeginHorizontal();
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
                GUILayout.EndHorizontal();

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

        public static void EnableFogOfWar(bool enable)
        {
            if (!HumankindGame.IsGameLoaded)
            {
                Loggr.Log("UNABLE TO CHANGE FOG OF WAR VALUE WHEN NO GAME IS RUNNING.", ConsoleColor.DarkRed);
                return;
            }

            HumankindGame.Empires[0].EnableFogOfWar(enable);
            HumankindGame.Update();
        }
    }
}
