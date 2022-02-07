using System;
using Amplitude.Framework;
using BepInEx.Configuration;
using Modding.Humankind.DevTools;

namespace DevTools.Humankind.GUITools.UI
{
    public static class TestingPlayground
    {

        public static void Run()
        {
            if (ViewController.View == ViewType.InGame && !LiveEditorMode.Enabled)
            {
                MappedActions.ToggleLiveEditorMode();
            }
            
            HumankindDevTools.RegisterAction(
                new KeyboardShortcut(UnityEngine.KeyCode.End), 
                "Testing_PrintVars", 
                PrintVars);
        }

        public static void PrintVars()
        {
            Loggr.LogAll(Amplitude.Mercury.Presentation.Presentation.PresentationCursorController);
            Loggr.Log(Amplitude.Mercury.Presentation.Presentation.PresentationCursorController.GetFirstTargetUnderMouse());
            Loggr.Log(
                "" + Application.Version.Accessibility.ToString() + " is >= Protected? " +
                (Application.Version.Accessibility >= Accessibility.Protected).ToString(), ConsoleColor.DarkGreen);
            
        }
    }
}
