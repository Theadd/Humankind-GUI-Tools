using System;
using System.IO;
using System.Linq;
using Modding.Humankind.DevTools;
using Modding.Humankind.DevTools.DeveloperTools.UI;
using DevTools.Humankind.GUITools.UI;
using HarmonyLib;
using Amplitude.Mercury.Presentation;
using BepInEx;
using DevTools.Humankind.GUITools.UI.PauseMenu;

namespace DevTools.Humankind.GUITools
{
    public static class MainTools
    {
        public static bool IsDebugModeEnabled { get; set; } = true;
        public static MainToolbar Toolbar { get; set; }
        public static InGameMenuWindow InGameMenu { get; set; }

        public static void Main()
        {
            if (IsDebugModeEnabled) Debug();
            
            PopupToolWindow.Open<MainToolbar>(w => Toolbar = w);
            PopupToolWindow.Open<InGameMenuWindow>(w => InGameMenu = w);

            // AccessTools.PropertySetter(typeof(GodMode), "Enabled")?.Invoke(null, new object[] { false });
   
            HumankindDevTools.RegisterAction(
                new BepInEx.Configuration.KeyboardShortcut(UnityEngine.KeyCode.Insert), 
                "ToggleHideAllGUITools", 
                ToggleHideAllUIWindows);
        }

        public static void ToggleHideAllUIWindows()
        {
            FloatingToolWindow.HideAllGUITools = !FloatingToolWindow.HideAllGUITools;
        }

        public static void Unload() {
            Toolbar?.Close();
            InGameMenu?.Close();
        }
        
        private static void Debug()
        {
            var scriptsPath = Path.Combine(Paths.GameRootPath, "scripts");
            var files = Directory.GetFiles(scriptsPath, "*.cs", SearchOption.AllDirectories)
                .Where(path => path.Substring(scriptsPath.Length, 5) != "\\obj\\");

            Loggr.Log(string.Join("\n", files), ConsoleColor.DarkYellow);
            UIOverlay.DEBUG_DRAW_OVERLAY = true;
        }
    }
}
