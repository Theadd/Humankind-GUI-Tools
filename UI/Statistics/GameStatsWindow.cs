using UnityEngine;
using Modding.Humankind.DevTools;
using Modding.Humankind.DevTools.DeveloperTools.UI;
using Amplitude.Mercury.Interop;
using Amplitude.Framework;
using Amplitude.Mercury.UI;

namespace DevTools.Humankind.GUITools.UI
{
    public class GameStatsWindow : FloatingToolWindow
    {
        public override bool ShouldBeVisible => true;   // PauseMenu.InGameMenuController.IsEndGameWindowVisible;
        public override bool ShouldRestoreLastWindowPosition => false;
        public override string WindowTitle { get; set; } = "GAME STATS WINDOW";
        public override Rect WindowRect { get; set; } = new Rect (0, 0, Screen.width, Screen.height);

        public static bool IsVisibleFullscreen = false;

        public static Vector2 ScrollViewPosition = Vector2.zero;

        private static int localEmpireIndex = 0;
        // Snapshot currently being displayed
        private static GameStatsSnapshot Snapshot;
        // True when the snapshot current snapshot can be "live" updated or it is a previously saved one.
        private static bool isLiveSnapshot;

        private GameStatisticsGrid grid;

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

        private GUIStyle backButtonStyle = new GUIStyle(UIController.DefaultSkin.toggle) {
            margin = new RectOffset(1, 1, 1, 1)
        };
        
        private GUIStyle CommonHeaderStyle = new GUIStyle(UIController.DefaultSkin.toggle) {
            margin = new RectOffset(1, 1, 1, 1),
            alignment = TextAnchor.LowerRight
        };

        private static bool initialized = false;
        private static bool isValidSnapshot = false;
        private static int loop = 0;
        private static bool isLateRepaint = false;
        private static bool forceUpdate = false;

        private Amplitude.Mercury.UI.UIManager UIManagerService;

        public override void OnGUIStyling()
        {
            base.OnGUIStyling();
            GUI.backgroundColor = Color.white;
            WindowRect = new Rect (0, 0, Screen.width, Screen.height);
        }

        public static void ResetLoop()
        {
            loop = 0;
            isLateRepaint = false;
            forceUpdate = true;
        }

        private void Initialize()
        {
            if (HumankindGame.IsGameLoaded)
            {
                isLiveSnapshot = true;
                localEmpireIndex = (int)Snapshots.GameSnapshot.PresentationData.LocalEmpireInfo.EmpireIndex;

                Snapshot = new GameStatsSnapshot();
                grid = new GameStatisticsGrid();

                Snapshot.SetLocalEmpireIndex(localEmpireIndex);

                initialized = true;
                isValidSnapshot = true;
                IsVisibleFullscreen = true;

                UIManagerService = (Services.GetService<IUIService>() as Amplitude.Mercury.UI.UIManager);
                if (UIManagerService == null)
                    return;

                UIManagerService.IsUiVisible = false;
                UIManagerService.AreTooltipsVisible = false;
                
            }
            
        }

        public override void Close(bool saveVisibilityStateBeforeClosing = false)
        {
            Unload();
            base.Close(false);
        }

        private void Unload()
        {
            IsVisibleFullscreen = false;
            isValidSnapshot = false;
            initialized = false;
            loop = 0;
            isLateRepaint = false;
            forceUpdate = false;

            if (UIManagerService == null) 
                    return;
            UIManagerService.IsUiVisible = true;
            UIManagerService.AreTooltipsVisible = true;
        }

        public override void OnDrawUI()
        {
            isLateRepaint = loop == 0 && Event.current.type == EventType.Repaint;
            if (Event.current.type == EventType.Repaint)
            {
                loop = loop == 20 ? 0 : loop + 1;
                if (initialized && localEmpireIndex != (int)Snapshots.GameSnapshot.PresentationData.LocalEmpireInfo.EmpireIndex)
                    forceUpdate = true;
            }

            if (isLateRepaint || !initialized || forceUpdate)
            {
                if (!initialized)
                {
                    Initialize();
                }
                else
                {
                    forceUpdate = false;
                    if (HumankindGame.IsGameLoaded)
                    {

                        localEmpireIndex = (int)Snapshots.GameSnapshot.PresentationData.LocalEmpireInfo.EmpireIndex;

                        Snapshot
                            .Snapshot()
                            .SetLocalEmpireIndex(localEmpireIndex);
                    }
                }
            }

            if (!isValidSnapshot)
            {
                GUILayout.BeginVertical();
                GUILayout.Space(4f);
                GUILayout.Label("  NOT A VALID SNAPSHOT  ");
                GUILayout.Space(4f);
                GUILayout.EndVertical();

                return;
            }

            grid.SetSnapshot(Snapshot);
            
            GUILayout.BeginVertical(bgStyle, GUILayout.Width(Screen.width), GUILayout.Height(Screen.height));
            

            BeginBackgroundScrollView();
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(60f);
                    GUILayout.FlexibleSpace();

                    GUILayout.BeginVertical();
                    {
                        GUILayout.Space(24f);
                        DrawCommonContent();
                        
                        GUILayout.Space(72f);
                        GUILayout.FlexibleSpace();
                        
                        DrawWindowContent();
                        // DrawWindowContent();
                        
                        GUILayout.FlexibleSpace();
                        GUILayout.Space(72f);

                    }
                    GUILayout.EndVertical();

                    GUILayout.FlexibleSpace();
                    GUILayout.Space(60f);
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            DrawBackButton();

            GUILayout.EndVertical();
        }

        private void BeginBackgroundScrollView()
        {
            ScrollViewPosition = GUILayout.BeginScrollView(
                ScrollViewPosition, 
                false, 
                false, 
                "horizontalscrollbar",
                "verticalscrollbar",
                //bgStyle,
                "scrollview",
                GUILayout.Height(Screen.height));
        }

        private void DrawCommonContent()
        {
            GUILayout.BeginHorizontal();
            grid.DrawCommonHeader();
            GUILayout.EndHorizontal();
        }

        private void DrawWindowContent()
        {
            GUILayout.BeginVertical();
                GUILayout.Space(4f);

                grid.Draw();

                GUILayout.Space(4f);
            GUILayout.EndVertical();
        }

        private void DrawBackButton()
        {
            GUI.backgroundColor = Color.white;
            if (GUI.Button(new Rect(24f, 24f, 223f, 38f), "<b>BACK TO THE GAME</b>", backButtonStyle))
            {
                MainTools.ToggleGameOverviewWindow();
            }
            GUI.backgroundColor = Color.white;
        }
    }
}
