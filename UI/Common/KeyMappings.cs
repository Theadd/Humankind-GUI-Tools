using System;
using BepInEx.Configuration;
using Modding.Humankind.DevTools;
using Modding.Humankind.DevTools.DeveloperTools.UI;
using UnityEngine;

namespace DevTools.Humankind.GUITools.UI
{
    public static class KeyMappings
    {
        public static string GlobalKeysGroup = "GLOBAL KEYS";
        public static string OtherKeysGroup = "OTHER KEYS";
        
        public static KeyMap[] Keys { get; set; } = new KeyMap[]
        {
            new KeyMap("ToggleGameOverviewWindow")
            {
                DisplayName = "GAME OVERVIEW FULLSCREEN OVERLAY",
                Action = MainTools.ToggleGameOverviewWindow,
                Key = new KeyboardShortcut(UnityEngine.KeyCode.Tab),
                GroupName = GlobalKeysGroup
            },
            new KeyMap("ToggleHideToolbarWindow")
            {
                DisplayName = "HIDE TOOLBAR WINDOW",
                Action = MainTools.ToggleHideToolbarWindow,
                Key = new KeyboardShortcut(UnityEngine.KeyCode.Home),
                GroupName = GlobalKeysGroup
            },
            new KeyMap("ToggleHideAllGUITools")
            {
                DisplayName = "HIDE ALL GUI WINDOWS",
                Action = MainTools.ToggleHideAllUIWindows,
                Key = new KeyboardShortcut(UnityEngine.KeyCode.Insert),
                GroupName = OtherKeysGroup
            },
        };

        public static void Apply()
        {
            Loggr.Log("IN KeyMappings.Apply()", ConsoleColor.Green);

            foreach (var key in Keys)
            {
                if (key.IsGlobalShortcut)
                    HumankindDevTools.RegisterAction(key.Key, key.ActionName, key.Action);
            }
        }

        public static void WritePlayerPreferences(FloatingToolWindow Window)
        {
            Loggr.Log("IN KeyMappings.WritePlayerPreferences()", ConsoleColor.Green);

            foreach (var key in Keys)
            {
                PlayerPrefs.SetString(Window.GetPlayerPrefKey(key.ActionName), key.Key.Serialize());
            }
        }

        public static void ReadPlayerPreferences(FloatingToolWindow Window)
        {
            Loggr.Log("IN KeyMappings.ReadPlayerPreferences()", ConsoleColor.Green);
            
            foreach (var key in Keys)
            {
                key.Key = KeyboardShortcut.Deserialize(
                    PlayerPrefs.GetString(Window.GetPlayerPrefKey(key.ActionName), key.Key.Serialize()));
            }
        }
    }

    public class KeyMap
    {
        public KeyboardShortcut Key { get; set; }
        public string ActionName { get; private set; }
        public string DisplayName { get; set; }
        public Action Action { get; set; }
        public string GroupName { get; set; }
        public bool IsGlobalShortcut { get; set; } = true;

        public KeyMap(string actionName)
        {
            ActionName = actionName.Replace(" ", "_");
        }
    }
}
