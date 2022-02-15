using System;
using Amplitude.Framework;
using BepInEx.Configuration;
using Modding.Humankind.DevTools;
using UnityEngine;
using Application = Amplitude.Framework.Application;

namespace DevTools.Humankind.GUITools.UI
{
    public static class TestingPlayground
    {
        private static bool _switchedToolboxTab = false;
        
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

        /// <summary>
        ///     Repeatedly called but with quite a lot of frames in between (see BackScreenWindow.OnZeroGUI).
        /// </summary>
        public static void OnZeroUpdate()
        {
            if (!_switchedToolboxTab && LiveEditorMode.Enabled && ToolboxController.Toolbox != null)
            {
                ToolboxController.Toolbox.SetActiveTab(4);
                _switchedToolboxTab = true;
            }
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
