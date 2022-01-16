using System;
using System.IO;
using System.Linq;
using Modding.Humankind.DevTools;
using Modding.Humankind.DevTools.DeveloperTools.UI;
using DevTools.Humankind.GUITools.UI;
using HarmonyLib;
using Amplitude.Mercury.Presentation;
using BepInEx;
using BepInEx.Configuration;
using DevTools.Humankind.GUITools.UI.PauseMenu;

namespace DevTools.Humankind.GUITools
{
    public static class MainTools
    {
        public static bool IsDebugModeEnabled { get; set; } = true;

        public static MainToolbar Toolbar { get; set; }

        public static InGameMenuWindow InGameMenu { get; set; }

        public static BasicToolWindow BasicWindow { get; set; }

        public static GameInfoToolWindow GameInfoWindow { get; set; }
        public static DistrictPainterToolWindow DistrictPainterWindow { get; set; }
        public static SettlementToolsWindow SettlementWindow { get; set; }
        public static StatisticsAndAchievementsToolWindow StatisticsAndAchievementsWindow { get; set; }
        public static FameToolWindow FameWindow { get; set; }
        public static EndGameToolWindow EndGameWindow { get; set; }
        public static GameStatsWindow StatsWindow { get; set; }

        public static void Main()
        {
            if (IsDebugModeEnabled) Debug();
            
            PopupToolWindow.Open<MainToolbar>(w => Toolbar = w);
            PopupToolWindow.Open<InGameMenuWindow>(w => InGameMenu = w);

            // PopupToolWindow.Open<GameInfoToolWindow>(w => GameInfoWindow = w);
            // PopupToolWindow.Open<DistrictPainterToolWindow>(w => DistrictPainterWindow = w);
            // PopupToolWindow.Open<SettlementToolsWindow>(w => SettlementWindow = w);
            // PopupToolWindow.Open<StatisticsAndAchievementsToolWindow>(w => StatisticsAndAchievementsWindow = w);
            // PopupToolWindow.Open<FameToolWindow>(w => FameWindow = w);
            // PopupToolWindow.Open<EndGameToolWindow>(w => EndGameWindow = w);
            // PopupToolWindow.Open<GameStatsWindow>(w => StatsWindow = w);

            // HumankindDevTools.RegisterAction(new KeyboardShortcut(UnityEngine.KeyCode.Home), "ToggleBasicToolWindow", ToggleBasicToolWindow);
            HumankindDevTools.RegisterAction(new KeyboardShortcut(UnityEngine.KeyCode.Home), "ToggleHideToolbarWindow", ToggleHideToolbarWindow);
            HumankindDevTools.RegisterAction(new KeyboardShortcut(UnityEngine.KeyCode.Tab), "ToggleGameOverviewWindow", ToggleGameOverviewWindow);
   
            HumankindDevTools.RegisterAction(
                new KeyboardShortcut(UnityEngine.KeyCode.Insert), 
                "ToggleHideAllGUITools", 
                ToggleHideAllUIWindows);
            
            // Maps [ESC] key to: GodMode.Enabled = false 
            // HumankindDevTools.RegisterAction(new KeyboardShortcut(UnityEngine.KeyCode.Escape), "CancelGodMode", CancelGodMode);
        }

        public static void ToggleHideToolbarWindow() => GlobalSettings.HideToolbarWindow.Value = !GlobalSettings.HideToolbarWindow.Value;

        public static void ToggleHideAllUIWindows() => FloatingToolWindow.HideAllGUITools = !FloatingToolWindow.HideAllGUITools;

        // public static void CancelGodMode() => AccessTools.PropertySetter(typeof(GodMode), "Enabled")?.Invoke(null, new object[] { false });

        public static void ToggleBasicToolWindow()
        {
            if (BasicWindow == null)
            {
                PopupToolWindow.Open<BasicToolWindow>(w => BasicWindow = w);
                return;
            }

            BasicWindow.Close();
            BasicWindow = null;
        }

        public static void ToggleGameOverviewWindow()
        {
            if (StatsWindow == null)
            {
                PopupToolWindow.Open<GameStatsWindow>(w => StatsWindow = w);
                return;
            }

            StatsWindow.Close();
            StatsWindow = null;
        }

        public static void Unload() => Unload(true);

        public static void Unload(bool saveState = false) {
            Toolbar?.Close(saveState);
            InGameMenu?.Close();
            BasicWindow?.Close();
            GameInfoWindow?.Close();
            DistrictPainterWindow?.Close();
            SettlementWindow?.Close();
            StatisticsAndAchievementsWindow?.Close();
            FameWindow?.Close();
            EndGameWindow?.Close();
            StatsWindow?.Close();
        }
        
        private static void Debug()
        {
            var scriptsPath = Path.Combine(Paths.GameRootPath, "scripts");
            var files = Directory.GetFiles(scriptsPath, "*.cs", SearchOption.AllDirectories)
                .Where(path => path.Substring(scriptsPath.Length, 5) != "\\obj\\");

            Loggr.Log(string.Join("\n", files), ConsoleColor.DarkYellow);

            // When true, draws a colored border for all UIOverlays backing a FloatingToolWindow derived class
            UIOverlay.DEBUG_DRAW_OVERLAY = false;
            // When not true, adds more verbosity to console output
            Modding.Humankind.DevTools.DevTools.QuietMode = true;
        }
    }
}
