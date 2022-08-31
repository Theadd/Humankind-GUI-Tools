using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Configuration;
using Modding.Humankind.DevTools;
using Modding.Humankind.DevTools.DeveloperTools.UI;
using UnityEngine;
using StyledGUI;

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

        private static Dictionary<string, string> KeyDisplayValues { get; set; } = new Dictionary<string, string>();

        public static bool TryGetKeyDisplayValue(string actionName, out string displayValue) =>
            KeyDisplayValues.TryGetValue(actionName, out displayValue);

        public static void Trigger(string actionName)
        {
            try
            {
                KeyMap key = Keys.First(k => k.ActionName == actionName);

                if (key.Action != null)
                {
                    key.Action.Invoke();
                }
            }
            catch (Exception e)
            {
                if (!Modding.Humankind.DevTools.DevTools.QuietMode)
                    Loggr.Log(e);
            }
            
        }
        
        public static KeyMap[] Keys { get; set; } = new KeyMap[]
        {
            new KeyMap("ToggleGameOverviewWindow")
            {
                DisplayName = "GAME OVERVIEW FULLSCREEN OVERLAY",
                Action = MappedActions.ToggleGameOverviewWindow,
                Key = new KeyboardShortcut(KeyCode.Tab),
                GroupName = GlobalKeysGroup,
                IsEditable = false,
                IsRemovable = false
            },
            new KeyMap("BackToNormalModeInGameView")
            {
                DisplayName = "DEFAULT VIEW MODE (IN GAME)",
                Action = MappedActions.BackToNormalModeInGameView,
                Key = new KeyboardShortcut(KeyCode.F2),
                GroupName = GlobalKeysGroup,
                IsEditable = true,
                IsRemovable = true
            },
            new KeyMap("ToggleHideToolbarWindow")
            {
                DisplayName = "HIDE TOOLBAR WINDOW",
                Action = MappedActions.ToggleHideToolbarWindow,
                Key = new KeyboardShortcut(KeyCode.Space),
                GroupName = GlobalKeysGroup
            },
            new KeyMap("ToggleScenarioEditorWindow")
            {
                DisplayName = "SCENARIO EDITOR WINDOW",
                Action = MappedActions.ToggleScenarioEditorWindow,
                Key = new KeyboardShortcut(KeyCode.F4, KeyCode.LeftShift),
                GroupName = GlobalKeysGroup,
                IsEditable = true,
                IsRemovable = true
            },
            new KeyMap("TogglePresentationFogOfWar")
            {
                DisplayName = "PRESENTATION FOG OF WAR",
                Action = MappedActions.TogglePresentationFogOfWar,
                Key = new KeyboardShortcut(KeyCode.LeftArrow, KeyCode.UpArrow, KeyCode.RightArrow, KeyCode.DownArrow),
                GroupName = CameraKeysGroup
            },
            new KeyMap("ToggleFreeCameraMode")
            {
                DisplayName = "FREE CAMERA MODE",
                Action = MappedActions.ToggleFreeCameraMode,
                Key = new KeyboardShortcut(KeyCode.F3),
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
                Action = MappedActions.ToggleLiveEditorMode,
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
                Key = new KeyboardShortcut(KeyCode.Mouse1),
                GroupName = LiveEditorKeysGroup,
                IsGlobalShortcut = false,
                IsRemovable = false,
            },
            new KeyMap(LiveEditorMode.DestroyUnderCursorActionName, LiveEditorMode.UpdateKeyMappings)
            {
                DisplayName = "DESTROY ARMY/DISTRICT/SETTLEMENT/ETC",
                Action = null,
                Key = new KeyboardShortcut(KeyCode.Mouse1, KeyCode.LeftShift),
                GroupName = LiveEditorKeysGroup,
                IsGlobalShortcut = false,
                IsRemovable = false,
            },
            new KeyMap(LiveEditorMode.DebugUnderCursorActionName, LiveEditorMode.UpdateKeyMappings)
            {
                DisplayName = "PRINT TILE DEBUG INFO TO CONSOLE",
                Action = null,
                Key = KeyboardShortcut.Empty,
                GroupName = LiveEditorKeysGroup,
                IsGlobalShortcut = false,
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
                Key = KeyboardShortcut.Empty,
                GroupName = UserInterfacesKeysGroup,
                IsEditable = false,
                SaveAndRestore = false,
            },
            
        };

        public static void Apply()
        {
            KeyDisplayValues = new Dictionary<string, string>();

            foreach (var key in Keys)
            {
                AddKeyDisplayName(key);
                if (key.IsGlobalShortcut)
                    HumankindDevTools.RegisterAction(key.Key, key.ActionName, key.Action);
            }

            BackScreenWindow.IsDirty = true;
        }

        private static void AddKeyDisplayName(KeyMap key)
        {
            if (key.Key.Equals(KeyboardShortcut.Empty))
                return;
            
            string displayValue = "<color=#22EE22FF>" + key.Key.AsDisplayValue() + "</color>";
            
            KeyDisplayValues.Add(key.ActionName, displayValue);
        }

        public static void WritePlayerPreferences(FloatingToolWindow Window)
        {
            foreach (var key in Keys)
            {
                if (key.SaveAndRestore)
                    PlayerPrefs.SetString(Window.GetPlayerPrefKey(key.ActionName), key.Key.Serialize());
            }
        }

        public static void ReadPlayerPreferences(FloatingToolWindow Window)
        {
            foreach (var key in Keys)
            {
                if (key.SaveAndRestore)
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
        public bool SaveAndRestore { get; set; } = true;

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
