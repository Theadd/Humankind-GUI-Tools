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
        public static string CameraKeysGroup = "CAMERA / SCREEN PRESENTATION";
        public static string InteractionKeysGroup = "GAME INTERACTION";
        public static string UserInterfacesKeysGroup = "GAME UI / GUI TOOLS";
        
        public static KeyMap[] Keys { get; set; } = new KeyMap[]
        {
            new KeyMap("ToggleGameOverviewWindow")
            {
                DisplayName = "GAME OVERVIEW FULLSCREEN OVERLAY",
                Action = MainTools.ToggleGameOverviewWindow,
                Key = new KeyboardShortcut(KeyCode.Tab),
                GroupName = GlobalKeysGroup
            },
            new KeyMap("ToggleHideToolbarWindow")
            {
                DisplayName = "HIDE TOOLBAR WINDOW",
                Action = MainTools.ToggleHideToolbarWindow,
                Key = new KeyboardShortcut(KeyCode.Home),
                GroupName = GlobalKeysGroup
            },
            new KeyMap("TogglePresentationFogOfWar")
            {
                DisplayName = "PRESENTATION FOG OR WAR",
                Action = ActionController.TogglePresentationFogOfWar,
                Key = KeyboardShortcut.Empty,
                GroupName = CameraKeysGroup
            },
            new KeyMap("ToggleFreeCameraMode")
            {
                DisplayName = "FREE CAMERA MODE",
                Action = ActionController.ToggleFreeCameraMode,
                Key = KeyboardShortcut.Empty,
                GroupName = CameraKeysGroup
            },
            new KeyMap("SwitchCameraFieldOfView")
            {
                DisplayName = "SWITCH MAIN CAMERA FIELD OF VIEW, VALUES: [15, 35, 65]",
                Action = ActionController.SwitchCameraFieldOfView,
                Key = KeyboardShortcut.Empty,
                GroupName = CameraKeysGroup
            },
            new KeyMap("ToggleGodMode")
            {
                DisplayName = "TOGGLE GOD MODE CURSOR",
                Action = ActionController.ToggleGodMode,
                Key = KeyboardShortcut.Empty,
                GroupName = InteractionKeysGroup
            },
            new KeyMap("GiveVisionAtCursorPosition")
            {
                DisplayName = "GIVE VISION UNDER CURSOR",
                Action = ActionController.GiveVisionAtCursorPosition,
                Key = KeyboardShortcut.Empty,
                GroupName = InteractionKeysGroup
            },
            new KeyMap("CreateCityAtCursorPosition")
            {
                DisplayName = "CREATE CITY UNDER CURSOR",
                Action = ActionController.CreateCityAtCursorPosition,
                Key = KeyboardShortcut.Empty,
                GroupName = InteractionKeysGroup
            },
            new KeyMap("CreateExtensionDistrictIndistryUnderCursor")
            {
                DisplayName = "CREATE INDUSTRIAL DISTRICT UNDER CURSOR",
                Action = ActionController.CreateExtensionDistrictIndistryUnderCursor,
                Key = KeyboardShortcut.Empty,
                GroupName = InteractionKeysGroup
            },
            new KeyMap("DestroyDistrictOrSettlementUnderCursor")
            {
                DisplayName = "DESTROY DISTRICT OR SETTLEMENT UNDER CURSOR",
                Action = ActionController.DestroyDistrictOrSettlementUnderCursor,
                Key = KeyboardShortcut.Empty,
                GroupName = InteractionKeysGroup
            },
            
            new KeyMap("ToggleTooltipsVisibility")
            {
                DisplayName = "TOGGLE TOOLTIPS VISIBILITY",
                Action = ActionController.ToggleTooltipsVisibility,
                Key = KeyboardShortcut.Empty,
                GroupName = UserInterfacesKeysGroup
            },
            new KeyMap("ToggleFrontiersVisibility")
            {
                DisplayName = "TOGGLE FRONTIERS VISIBILITY",
                Action = ActionController.ToggleFrontiersVisibility,
                Key = KeyboardShortcut.Empty,
                GroupName = UserInterfacesKeysGroup
            },
            new KeyMap("ToggleAmplitudeUIVisibility")
            {
                DisplayName = "ENABLE/DISABLE VISIBILITY OF GAME UI",
                Action = ActionController.ToggleAmplitudeUIVisibility,
                Key = KeyboardShortcut.Empty,
                GroupName = UserInterfacesKeysGroup
            },
            new KeyMap("ToggleHideAllGUITools")
            {
                DisplayName = "VISIBILITY OVERRIDE FOR ALL GUI TOOLS",
                Action = MainTools.ToggleHideAllUIWindows,
                Key = new KeyboardShortcut(KeyCode.Insert),
                GroupName = UserInterfacesKeysGroup
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
