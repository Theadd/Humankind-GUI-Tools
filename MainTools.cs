using System;
using Modding.Humankind.DevTools;
using Modding.Humankind.DevTools.DeveloperTools.UI;
using DevTools.Humankind.GUITools.UI;
using DevTools.Humankind.GUITools.UI.PauseMenu;
using StyledGUI;

namespace DevTools.Humankind.GUITools
{
    public static class MainTools
    {
        public static bool IsDebugModeEnabled { get; set; } = true;

        public static MainToolbar Toolbar { get; set; }

        public static InGameMenuWindow InGameMenu { get; set; }

        // public static BasicToolWindow BasicWindow { get; set; }

        // public static GameInfoToolWindow GameInfoWindow { get; set; }
        // public static DistrictPainterToolWindow DistrictPainterWindow { get; set; }
        // public static SettlementToolsWindow SettlementWindow { get; set; }
        // public static StatisticsAndAchievementsToolWindow StatisticsAndAchievementsWindow { get; set; }
        public static ScenarioEditorToolWindow ScenarioEditorWindow { get; set; }
        public static GameStatsWindow StatsWindow { get; set; }
        public static BackScreenWindow BackScreen { get; set; }

        public static EndGameStatisticsWindow EndGameWindow { get; set; }
        // public static CameraLayersToolWindow CameraLayersWindow { get; set; }

        public static void Main()
        {
            if (IsDebugModeEnabled) Debug();

            ViewController.Initialize(true);

            UIController.OnceGUIHasLoaded(() =>
            {
                StyledGUIUtility.DefaultSkin = UIController.DefaultSkin;
                StyledGUIUtility.UnicodeSymbolsFont = Utils.SegoeUISymbolFont;
            });
            PopupToolWindow.Open<BackScreenWindow>(w => BackScreen = w);
            PopupToolWindow.Open<MainToolbar>(w => Toolbar = w);
            PopupToolWindow.Open<InGameMenuWindow>(w => InGameMenu = w);

            // if (IsDebugModeEnabled && IsTestingPlaygroundEnabled)
            //    TestingPlayground.Run();
        }

        public static void ToggleHideToolbarWindow()
        {
            if (ViewController.IsGloballyDisabled) return;
            GlobalSettings.HideToolbarWindow.Value = !GlobalSettings.HideToolbarWindow.Value;
        }

        public static void ToggleHideAllUIWindows() =>
            Loggr.Log("HIDDING ALL GUI TOOLS WINDOWS IS TEMPORARILY DISABLED",
                ConsoleColor
                    .Magenta); //FloatingToolWindow.HideAllGUITools = !FloatingToolWindow.HideAllGUITools);

        // public static void CancelGodMode() => AccessTools.PropertySetter(typeof(GodMode), "Enabled")?.Invoke(null, new object[] { false });

        /*public static void ToggleBasicToolWindow()
        {
            if (BasicWindow == null)
            {
                PopupToolWindow.Open<BasicToolWindow>(w => BasicWindow = w);
                return;
            }

            BasicWindow.Close();
            BasicWindow = null;
        }*/

        public static void ToggleGameOverviewWindow()
        {
            if (ViewController.IsGloballyDisabled) return;
            if (StatsWindow == null)
            {
                PopupToolWindow.Open<GameStatsWindow>(w => StatsWindow = w);
                return;
            }

            StatsWindow.Close();
            StatsWindow = null;
        }

        public static void ToggleScenarioEditorWindow()
        {
            if (ViewController.IsGloballyDisabled) return;
            if (ScenarioEditorWindow == null)
            {
                PopupToolWindow.Open<ScenarioEditorToolWindow>(w => ScenarioEditorWindow = w);
                return;
            }

            ScenarioEditorWindow.Close();
            ScenarioEditorWindow = null;
        }

        public static void ToggleEndGameStatisticsWindow()
        {
            if (ViewController.IsGloballyDisabled) return;
            if (EndGameWindow == null)
            {
                PopupToolWindow.Open<EndGameStatisticsWindow>(w => EndGameWindow = w);
                ViewController.ViewMode = ViewModeType.EndGame;
                return;
            }

            CloseEndGameStatisticsWindow();
        }

        public static void CloseEndGameStatisticsWindow()
        {
            if (ViewController.IsGloballyDisabled) return;
            if (EndGameWindow != null)
            {
                EndGameWindow.Close(false);
                EndGameWindow = null;
            }
        }

        public static bool IsGameOverviewEnabled => StatsWindow != null;
        public static bool IsEndGameWindowEnabled => EndGameWindow != null;
        public static bool IsScenarioEditorWindowEnabled => ScenarioEditorWindow != null;

        public static void Unload() => Unload(true);

        public static void Unload(bool saveState = false)
        {
            if (FeatureFlags.WireRenderer)
                WireRenderer.Detach();

            Toolbar?.Close(saveState);
            InGameMenu?.Close();
            // BasicWindow?.Close();
            // CameraLayersWindow?.Close();
            // GameInfoWindow?.Close();
            // DistrictPainterWindow?.Close();
            // SettlementWindow?.Close();
            // StatisticsAndAchievementsWindow?.Close();
            // FameWindow?.Close();
            EndGameWindow?.Close();
            StatsWindow?.Close();
            BackScreen?.Close();
            ScenarioEditorWindow?.Close();
            ScreenLocker.Unload();
        }

        private static void Debug()
        {
            /*var scriptsPath = Path.Combine(Paths.GameRootPath, "scripts");
            var files = Directory.GetFiles(scriptsPath, "*.cs", SearchOption.AllDirectories)
                .Where(path => path.Substring(scriptsPath.Length, 5) != "\\obj\\");

            Loggr.Log(string.Join("\n", files), ConsoleColor.DarkYellow);*/

            // When true, draws a colored border for all UIOverlays backing a FloatingToolWindow derived class
            UIOverlay.DEBUG_DRAW_OVERLAY = false;
            // When not true, adds more verbosity to console output
            Modding.Humankind.DevTools.DevTools.QuietMode = FeatureFlags.QuietMode;
        }
    }
}
