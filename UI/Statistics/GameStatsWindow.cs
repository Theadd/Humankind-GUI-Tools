using System.Linq;
using UnityEngine;
using Modding.Humankind.DevTools;
using Modding.Humankind.DevTools.DeveloperTools.UI;
using Amplitude.Mercury.Interop;
using Amplitude.Framework;
using Amplitude.Mercury.UI;
using StyledGUI;

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
        
        private int activeTab = 0;
        private int previousActiveTab = 0;

        private string[] tabNames = {"OVERVIEW", "KEYBOARD SHORTCUTS"};
        
        public GameGrid GameOverviewGrid { get; set; }
        public CommonHeadersGrid HeadersGrid { get; set; }
        public KeyboardShortcutsGrid ShortcutsGrid { get; set; }

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
        
        private GUIStyle bgStyleAlt = new GUIStyle(UIController.DefaultSkin.FindStyle("PopupWindow.Sidebar.Highlight")) {
            normal = new GUIStyleState() {
                background = Utils.CreateSinglePixelTexture2D(new Color(0.85f, 0.75f, 0f, 0.45f)),
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

                Snapshot.SetLocalEmpireIndex(localEmpireIndex);

                GameOverviewGrid = new GameGrid()
                {
                    Snapshot = Snapshot,
                    VirtualGrid = new VirtualGrid()
                    {
                        Grid = new GameOverviewGrid(),
                        Distribution = HumankindGame.Empires.Select((e, i) => i).ToArray()
                    }
                };
                HeadersGrid = new CommonHeadersGrid()
                {
                    Snapshot = Snapshot,
                    VirtualGrid = new VirtualGrid()
                    {
                        Grid = new StaticHeaderGrid(),
                        DrawRowHeaders = false,
                        DrawSectionHeaders = false
                    }
                };
                ShortcutsGrid = new KeyboardShortcutsGrid()
                {
                    VirtualGrid = new VirtualGrid()
                    {
                        Grid = new ShortcutsGrid(),
                        ExpandWidthOnSingleColumnGrid = false
                    }
                };
                
                ShortcutsGrid.VirtualGrid.RowHeaderCellSpan = ShortcutsGrid.VirtualGrid.Grid.CellSpan8;
                ShortcutsGrid.VirtualGrid.SectionHorizontalLineWidth = ShortcutsGrid.VirtualGrid.Grid.GetCellWidth() * 8;

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
            HumankindGame.Update();
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
                        
                        GameOverviewGrid.Snapshot = Snapshot;
                        GameOverviewGrid.IsDirty = true;
                        HeadersGrid.IsDirty = true;
                    }
                }
            }

            if (!isValidSnapshot)
            {
                GUILayout.BeginVertical();
                GUILayout.Space(4f);
                GUILayout.Label(" ");
                GUILayout.Space(4f);
                GUILayout.EndVertical();

                return;
            }

            GUILayout.BeginVertical(bgStyle, GUILayout.Width(Screen.width), GUILayout.Height(Screen.height));
            
            DrawCommonContent();

            BeginBackgroundScrollView();
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(60f);
                    GUILayout.FlexibleSpace();

                    GUILayout.BeginVertical();
                    {
                        
                        
                        GUILayout.Space(85f);

                        if (activeTab == 0)
                        {
                            GameOverviewGrid.Render();
                        }
                        else if (activeTab == 1)
                        {
                            if (previousActiveTab != 1)
                                ShortcutsGrid.ResyncKeyMappings();
                            
                            ShortcutsGrid.Render();
                        }
                        
                        GUILayout.Space(80f);

                        GUILayout.FlexibleSpace();

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
                "scrollview",
                GUILayout.ExpandHeight(true));
        }

        

        private GUIStyle TabButtonStyle = new GUIStyle(UIController.DefaultSkin.FindStyle("TabButton")) {
            fixedHeight = 28f,
            fontSize = 14,
            fontStyle = FontStyle.Bold,
            padding = new RectOffset(22, 22, 0, 5),
            normal = new GUIStyleState() {
                background = Utils.CreateSinglePixelTexture2D(new Color(0.85f, 0.75f, 0f, 0.45f)),
                textColor = Color.white
            },
            onNormal = new GUIStyleState()
            {
                background = Utils.TransparentTexture,
                textColor = new Color32(85, 136, 254, 255)
            },
            onHover = new GUIStyleState()
            {
                background = null,
                textColor = Color.white
            },
            onActive = new GUIStyleState()
            {
                background = null,
                textColor = Color.white
            },
            active = new GUIStyleState()
            {
                background = Utils.TransparentTexture,
                textColor = new Color32(85, 136, 254, 255)
            },
            hover = new GUIStyleState()
            {
                background = Utils.CreateSinglePixelTexture2D(new Color32(85, 136, 254, 155)),
                textColor = Color.white
            }
        };
        
        private void DrawCommonContent()
        {
            GUILayout.BeginHorizontal(bgStyleAlt);
            GUILayout.Space(271f);
            GUILayout.BeginVertical();
            GUILayout.Space(26f);
            HeadersGrid.Render();
            GUILayout.Space(22f);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.BeginHorizontal(bgStyleAlt, GUILayout.Height(28f));
            GUILayout.Space(395f);
            GUILayout.EndHorizontal();
            previousActiveTab = activeTab;
            activeTab = GUILayout.Toolbar(activeTab, tabNames, TabButtonStyle, GUI.ToolbarButtonSize.FitToContents);
            GUILayout.BeginHorizontal(bgStyleAlt, GUILayout.Height(28f));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndHorizontal();
        }

        private void DrawBackButton()
        {
            GUI.backgroundColor = Color.white;
            if (GUI.Button(new Rect(26f, 26f, 223f, 45f), "<b>BACK TO THE GAME</b>", backButtonStyle))
            {
                MainTools.ToggleGameOverviewWindow();
                ViewController.ViewMode = ViewModeType.Auto;
            }
            GUI.backgroundColor = Color.white;
        }
    }
}
