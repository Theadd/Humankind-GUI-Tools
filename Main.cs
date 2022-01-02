using System;
using Modding.Humankind.DevTools;
using Modding.Humankind.DevTools.DeveloperTools.UI;
using DevTools.Humankind.GUITools.UI;
using HarmonyLib;
using Amplitude.Mercury.Presentation;
using System.Reflection;

namespace DevTools.Humankind.GUITools
{

    public static class GUIToolsMain {

        //private static string scriptsPath = Path.Combine(Paths.GameRootPath, "scripts");

        public static void Main() {
            /*Loggr.Log(scriptsPath, ConsoleColor.Magenta);
            Loggr.Log((scriptsPath + "\\obj\\Debug\\Whatever.cs").Substring(scriptsPath.Length, 5), ConsoleColor.Magenta);
            var files = Directory.GetFiles(scriptsPath, "*.cs", SearchOption.AllDirectories)
                .Where(path => path.Substring(scriptsPath.Length, 5) != "\\obj\\");

            Loggr.Log(string.Join("\n", files), ConsoleColor.DarkYellow);*/
            Loggr.Log("ONE", ConsoleColor.DarkYellow);
            UIOverlay.DEBUG_DRAW_OVERLAY = true;
            Loggr.Log("TWO", ConsoleColor.DarkYellow);
            // PopupToolWindow.Open<ArmyToolsWindow>(w => armyTools = w);  
            //UIManager.ShowWindow<ArmyToolsWindow>();
            // PopupToolWindow.Open<TechnologyToolsWindow>(w => techTools = w); 
            PopupToolWindow.Open<MainToolbar>(w => Toolbar = w);
            Loggr.Log("THREE", ConsoleColor.DarkYellow);

            PopupToolWindow.Open<BasicToolWindow>(w => basicWindow = w);
            
            // PopupToolWindow.Open<AutoTurnToolWindow>(w => autoTurn = w);
            // PopupToolWindow.Open<MilitaryToolsWindow>(w => militaryTools = w);
            
            // PopupToolWindow.Open<AIToolWindow>(w => aiTools = w);
            Loggr.Log("FOUR", ConsoleColor.DarkYellow);

            AccessTools.PropertySetter(typeof(GodMode), "Enabled")?.Invoke(null, new object[] { false });
            // Loggr.Log("GOD MODE ENABLED IS = " + GodMode.Enabled);


            /*MethodInfo GodModeSetter = AccessTools.PropertySetter(typeof(GodMode), "Enabled");
 
            Loggr.Log("GOT SETTER? " + (GodModeSetter != null));
            if (GodModeSetter != null)
            {
                GodModeSetter.Invoke(null, new object[] { true });
                Loggr.Log("GOD MODE ENABLED IS = " + GodMode.Enabled);
            }*/


        }

        public static void Unload() {
            basicWindow?.Close();
            autoTurn?.Close();
            Toolbar?.Close();
            militaryTools?.Close();
            resourceTools?.Close();
            framerateTools?.Close();
            aiTools?.Close();
            //armyTools?.Close();
            techTools?.Close();

        }
        private static BasicToolWindow basicWindow;
        //private static ArmyToolsWindow armyTools;
        private static TechnologyToolsWindow techTools;
        private static AutoTurnToolWindow autoTurn;
        private static MilitaryToolsWindow militaryTools;
        private static ResourceToolsWindow resourceTools;
        private static FramerateToolWindow framerateTools;
        private static AIToolWindow aiTools;
        public static MainToolbar Toolbar { get; set; }
    }

}
