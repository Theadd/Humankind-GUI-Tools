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
        public static string LiveEditorKeysGroup = "LIVE EDITOR MODE";
        
        public static KeyMap[] Keys { get; set; } = new KeyMap[]
        {
            new KeyMap("ToggleGameOverviewWindow")
            {
                DisplayName = "GAME OVERVIEW FULLSCREEN OVERLAY",
                Action = MainTools.ToggleGameOverviewWindow,
                Key = new KeyboardShortcut(KeyCode.Tab),
                GroupName = GlobalKeysGroup,
                IsEditable = false,
                IsRemovable = false
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
                DisplayName = "PRESENTATION FOG OF WAR",
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
                DisplayName = "SWITCH MAIN CAMERA FIELD OF VIEW (15, 35, 65)",
                Action = ActionController.SwitchCameraFieldOfView,
                Key = KeyboardShortcut.Empty,
                GroupName = CameraKeysGroup
            },
            new KeyMap("ToggleLiveEditor")
            {
                DisplayName = "TOGGLE LIVE EDITOR MODE ON/OFF",
                Action = ActionController.ToggleLiveEditorMode,
                Key = new KeyboardShortcut(KeyCode.F4),
                GroupName = LiveEditorKeysGroup
            },
            new KeyMap(LiveEditorMode.ToolboxPreviewActionName, LiveEditorMode.UpdateKeyMappings)
            {
                DisplayName = "HOLD FOR QUICK USE OF TOOLBOX",
                Action = null,
                Key = new KeyboardShortcut(KeyCode.LeftControl),
                GroupName = LiveEditorKeysGroup,
                IsGlobalShortcut = false,
                IsRemovable = false,
            },
            new KeyMap(LiveEditorMode.StickedToolboxActionName, LiveEditorMode.UpdateKeyMappings)
            {
                DisplayName = "TOGGLE STICKY TOOLBOX",
                Action = null,
                Key = new KeyboardShortcut(KeyCode.Space, KeyCode.LeftControl),
                GroupName = LiveEditorKeysGroup,
                IsGlobalShortcut = false,
                IsRemovable = false,
            },
            new KeyMap(LiveEditorMode.CreateUnderCursorActionName, LiveEditorMode.UpdateKeyMappings)
            {
                DisplayName = "PAINT/CREATE SELECTED CONSTRUCTIBLE",
                Action = null,
                Key = new KeyboardShortcut(KeyCode.Mouse0),
                GroupName = LiveEditorKeysGroup,
                IsGlobalShortcut = false,
                IsRemovable = false,
            },
            new KeyMap(LiveEditorMode.DestroyUnderCursorActionName, LiveEditorMode.UpdateKeyMappings)
            {
                DisplayName = "DESTROY ARMY/DISTRICT/SETTLEMENT/ETC",
                Action = null,
                Key = new KeyboardShortcut(KeyCode.Mouse1),
                GroupName = LiveEditorKeysGroup,
                IsGlobalShortcut = false,
                IsRemovable = false,
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
            new KeyMap("CreateExtensionDistrictIndustryUnderCursor")
            {
                DisplayName = "CREATE INDUSTRIAL DISTRICT UNDER CURSOR",
                Action = ActionController.CreateExtensionDistrictIndustryUnderCursor,
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
        public KeyboardShortcut Key { get => _key; set => ChangeKey(value); }
        public string ActionName { get; private set; }
        public string DisplayName { get; set; }
        public Action Action { get; set; }
        public string GroupName { get; set; }
        public bool IsGlobalShortcut { get; set; } = true;
        public bool IsRemovable { get; set; } = true;
        public bool IsEditable { get; set; } = true;
        
        private Action OnKeyChange { get; set; }
        private KeyboardShortcut _key = KeyboardShortcut.Empty;

        public KeyMap(string actionName, Action onKeyChange = null)
        {
            ActionName = actionName.Replace(" ", "_");
            OnKeyChange = onKeyChange;
        }
        
        private void ChangeKey(KeyboardShortcut newKey)
        {
            if (!_key.Equals(newKey))
            {
                _key = newKey;

                try
                {
                    OnKeyChange?.Invoke();
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }
    }
}
