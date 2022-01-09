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

        // public static BasicToolWindow BasicWindow { get; set; }

        public static void Main()
        {
            if (IsDebugModeEnabled) Debug();
            
            PopupToolWindow.Open<MainToolbar>(w => Toolbar = w);
            PopupToolWindow.Open<InGameMenuWindow>(w => InGameMenu = w);

            // PopupToolWindow.Open<BasicToolWindow>(w => BasicWindow = w);
   
            HumankindDevTools.RegisterAction(
                new KeyboardShortcut(UnityEngine.KeyCode.Insert), 
                "ToggleHideAllGUITools", 
                ToggleHideAllUIWindows);
            
            // Maps [ESC] key to: GodMode.Enabled = false
            HumankindDevTools.RegisterAction(new KeyboardShortcut(UnityEngine.KeyCode.Escape), "CancelGodMode", CancelGodMode);
        }

        public static void ToggleHideAllUIWindows() => FloatingToolWindow.HideAllGUITools = !FloatingToolWindow.HideAllGUITools;

        public static void CancelGodMode() => AccessTools.PropertySetter(typeof(GodMode), "Enabled")?.Invoke(null, new object[] { false });

        public static void Unload() => Unload(true);

        public static void Unload(bool saveState = false) {
            Toolbar?.Close(saveState);
            InGameMenu?.Close();
            // BasicWindow?.Close();
        }
        
        private static void Debug()
        {
            var scriptsPath = Path.Combine(Paths.GameRootPath, "scripts");
            var files = Directory.GetFiles(scriptsPath, "*.cs", SearchOption.AllDirectories)
                .Where(path => path.Substring(scriptsPath.Length, 5) != "\\obj\\");

            Loggr.Log(string.Join("\n", files), ConsoleColor.DarkYellow);

            // Draw a colored border for all UIOverlays backing a FloatingToolWindow derived class
            UIOverlay.DEBUG_DRAW_OVERLAY = true;
            // More verbose console output
            Modding.Humankind.DevTools.DevTools.QuietMode = false;
        }
    }
}
