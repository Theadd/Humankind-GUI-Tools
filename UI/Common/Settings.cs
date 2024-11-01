using System;
using DevTools.Humankind.GUITools.UI.PauseMenu;
using Modding.Humankind.DevTools;
using Modding.Humankind.DevTools.Core;
using Modding.Humankind.DevTools.DeveloperTools.UI;
using UnityEngine;

namespace DevTools.Humankind.GUITools.UI
{
    public abstract class ToolSetting<T>
    {
        public T Value { get; set; }
        public string Name;
        public string Tooltip;
        public GUIContent Content;

        public ToolSetting(string name, string tooltip, T defaultValue)
        {
            Name = name;
            Tooltip = tooltip;
            Value = defaultValue;
            Content = new GUIContent(Name, Tooltip);
        }

        public abstract bool Draw();
    }

    public class CheckboxSetting : ToolSetting<bool>
    {
        public string TooltipOn;
        public GUIContent ToggleContent;
        public GUIContent ToggleContentOn;

        public CheckboxSetting(string text, string tooltip, bool defaultValue) : base(text, tooltip,
            defaultValue)
        {
            ToggleContent = new GUIContent("<size=9><b> OFF</b></size>", Tooltip);
            ToggleContentOn = new GUIContent("<size=9><b>ON</b></size>", TooltipOn ?? Tooltip);
        }

        public CheckboxSetting(string text, string tooltip = "", string tooltipOn = null,
            bool defaultValue = true)
            : this(text, tooltip, defaultValue)
        {
            TooltipOn = tooltipOn;
        }

        public bool Draw(GUIStyle rowStyle)
        {
            GUILayout.BeginHorizontal(rowStyle);

            DrawContent();

            GUILayout.EndHorizontal();
            GUILayout.Space(8f);

            return Value;
        }

        public override bool Draw()
        {
            GUILayout.BeginHorizontal();

            DrawContent();

            GUILayout.EndHorizontal();

            return Value;
        }

        private void DrawContent()
        {
            GUILayout.Space(6f);
            GUILayout.BeginVertical(GUILayout.Width(36f), GUILayout.Height(24f));
            GUILayout.Space(4f);
            Value = GUILayout.Toggle(Value, Value ? ToggleContentOn : ToggleContent, "Checkbox");
            GUILayout.EndVertical();
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            GUILayout.Label(
                (GUI.enabled ? "<size=12><color=#FFFFFFBC>" : "<size=12><color=#FFFFFF88>") + Name +
                "</color></size>", "Text");
            GUILayout.EndVertical();
        }
    }

    public static class GlobalSettings
    {
        public static bool ShouldHideTools => ViewController.IsGloballyDisabled ||
                                              (ViewController.View != ViewType.InGame
                                               || (ViewController.View == ViewType.InGame
                                                   && ViewController.ViewMode != ViewModeType.Normal));

        public static CheckboxSetting WindowTransparency = new CheckboxSetting(
            "Toggles the background transparency of all tool windows.",
            "Toggle on/off the background transparency of all tool windows",
            true
        );

        public static CheckboxSetting WindowTitleBar = new CheckboxSetting(
            "Shows/hides all tool windows title bar unless the tool manages this setting by itself.",
            "Click to <b>show</b> all tool windows title bar by default",
            "Click to <b>hide</b> all tool windows title bar by default",
            true
        );

        public static CheckboxSetting HideToolbarWindow = new CheckboxSetting(
            "Shows/Hides the Toolbar window.", "", false
        );
        /*public static CheckboxSetting HideToolsInGameMenu = new CheckboxSetting(
            "Hide all Tool Windows in the Game Menu screen, including the Toolbar window.",
            "", false
        );*/

        public static CheckboxSetting CheatingTools =
            new CheckboxSetting(R.Text.Bold("Cheating Tools section".ToUpperInvariant()));

        public static CheckboxSetting ProfilingTools =
            new CheckboxSetting(R.Text.Bold("Profiling Tools section".ToUpperInvariant()), "", null, false);

        public static CheckboxSetting DeveloperTools =
            new CheckboxSetting(R.Text.Bold("Developer Tools section".ToUpperInvariant()));

        public static CheckboxSetting ExperimentalTools =
            new CheckboxSetting(R.Text.Bold("Experimental Tools section".ToUpperInvariant()));

        public static CheckboxSetting MilitaryTool = new CheckboxSetting("Military Tool.");
        public static CheckboxSetting ArmyTool = new CheckboxSetting("Army Tools.");
        public static CheckboxSetting TechnologyTool = new CheckboxSetting("Technology Tool.");
        public static CheckboxSetting ResourcesTool = new CheckboxSetting("Resources Tool.");
        public static CheckboxSetting FramerateTool = new CheckboxSetting("Framerate Tool.");
        public static CheckboxSetting AutoTurnTool = new CheckboxSetting("Auto Turn Tool.");
        public static CheckboxSetting AITool = new CheckboxSetting("AI Tools.");

        public static CheckboxSetting GPUProfilerTool =
            new CheckboxSetting("GPU Profiler Tool.", "", null, false);

        public static CheckboxSetting MemoryProfilerTool =
            new CheckboxSetting("Memory Profiler Tool.", "", null, false);

        public static CheckboxSetting AffinityTool = new CheckboxSetting("Affinity Tool.", "", null, false);

        public static CheckboxSetting ArchetypesTool =
            new CheckboxSetting("Archetypes Tool.", "", null, false);

        public static CheckboxSetting GraphicsTool = new CheckboxSetting("Graphics Tool.", "", null, false);
        public static CheckboxSetting BattleAITool = new CheckboxSetting("Battle AI Tools.");
        public static CheckboxSetting CivicsTool = new CheckboxSetting("Civics Tool.");

        public static CheckboxSetting CollectiblesTool =
            new CheckboxSetting("Collectibles Tool.", "", null, false);

        public static CheckboxSetting DiplomacyTool = new CheckboxSetting("Diplomacy Tool.", "", null, false);

        public static CheckboxSetting TerrainPickingTool =
            new CheckboxSetting("Terrain Picking Tool.", "", null, true);

        public static CheckboxSetting GameInfoTool = new CheckboxSetting("Game Info Tool.", "", null, false);

        public static CheckboxSetting DistrictPainterTool =
            new CheckboxSetting("District Painter Tool.", "", null, false);

        public static CheckboxSetting SettlementTools =
            new CheckboxSetting("Settlement Tools.", "", null, false);

        public static CheckboxSetting StatisticsAndAchievementsTool =
            new CheckboxSetting("Statistics & Achievements Tool.", "", null, false);

        public static CheckboxSetting FameTool = new CheckboxSetting("Fame Tool.", "", null, false);
        public static CheckboxSetting EndGameTool = new CheckboxSetting("End Game Tool.", "", null, false);


        public static void WritePlayerPreferences(FloatingToolWindow Window)
        {
            PlayerPrefs.SetInt(Window.GetPlayerPrefKey("CheatingTools"), CheatingTools.Value ? 1 : 0);
            PlayerPrefs.SetInt(Window.GetPlayerPrefKey("ProfilingTools"), ProfilingTools.Value ? 1 : 0);
            PlayerPrefs.SetInt(Window.GetPlayerPrefKey("DeveloperTools"), DeveloperTools.Value ? 1 : 0);
            PlayerPrefs.SetInt(Window.GetPlayerPrefKey("ExperimentalTools"), ExperimentalTools.Value ? 1 : 0);
            PlayerPrefs.SetInt(Window.GetPlayerPrefKey("MilitaryTool"), MilitaryTool.Value ? 1 : 0);
            PlayerPrefs.SetInt(Window.GetPlayerPrefKey("ArmyTool"), ArmyTool.Value ? 1 : 0);
            PlayerPrefs.SetInt(Window.GetPlayerPrefKey("TechnologyTool"), TechnologyTool.Value ? 1 : 0);
            PlayerPrefs.SetInt(Window.GetPlayerPrefKey("ResourcesTool"), ResourcesTool.Value ? 1 : 0);
            PlayerPrefs.SetInt(Window.GetPlayerPrefKey("FramerateTool"), FramerateTool.Value ? 1 : 0);
            PlayerPrefs.SetInt(Window.GetPlayerPrefKey("AutoTurnTool"), AutoTurnTool.Value ? 1 : 0);
            PlayerPrefs.SetInt(Window.GetPlayerPrefKey("AITool"), AITool.Value ? 1 : 0);
            PlayerPrefs.SetInt(Window.GetPlayerPrefKey("GPUProfilerTool"), GPUProfilerTool.Value ? 1 : 0);
            PlayerPrefs.SetInt(Window.GetPlayerPrefKey("MemoryProfilerTool"),
                MemoryProfilerTool.Value ? 1 : 0);
            PlayerPrefs.SetInt(Window.GetPlayerPrefKey("AffinityTool"), AffinityTool.Value ? 1 : 0);
            PlayerPrefs.SetInt(Window.GetPlayerPrefKey("ArchetypesTool"), ArchetypesTool.Value ? 1 : 0);
            PlayerPrefs.SetInt(Window.GetPlayerPrefKey("GraphicsTool"), GraphicsTool.Value ? 1 : 0);
            PlayerPrefs.SetInt(Window.GetPlayerPrefKey("BattleAITool"), BattleAITool.Value ? 1 : 0);
            PlayerPrefs.SetInt(Window.GetPlayerPrefKey("CivicsTool"), CivicsTool.Value ? 1 : 0);
            PlayerPrefs.SetInt(Window.GetPlayerPrefKey("CollectiblesTool"), CollectiblesTool.Value ? 1 : 0);
            PlayerPrefs.SetInt(Window.GetPlayerPrefKey("DiplomacyTool"), DiplomacyTool.Value ? 1 : 0);
            PlayerPrefs.SetInt(Window.GetPlayerPrefKey("TerrainPickingTool"),
                TerrainPickingTool.Value ? 1 : 0);
            PlayerPrefs.SetInt(Window.GetPlayerPrefKey("GameInfoTool"), GameInfoTool.Value ? 1 : 0);
            PlayerPrefs.SetInt(Window.GetPlayerPrefKey("DistrictPainterTool"),
                DistrictPainterTool.Value ? 1 : 0);
            PlayerPrefs.SetInt(Window.GetPlayerPrefKey("SettlementTools"), SettlementTools.Value ? 1 : 0);
            PlayerPrefs.SetInt(Window.GetPlayerPrefKey("StatisticsAndAchievementsTool"),
                StatisticsAndAchievementsTool.Value ? 1 : 0);
            PlayerPrefs.SetInt(Window.GetPlayerPrefKey("FameTool"), FameTool.Value ? 1 : 0);
            PlayerPrefs.SetInt(Window.GetPlayerPrefKey("EndGameTool"), EndGameTool.Value ? 1 : 0);

            PlayerPrefs.SetInt(Window.GetPlayerPrefKey("WindowTransparency"),
                WindowTransparency.Value ? 1 : 0);
            PlayerPrefs.SetInt(Window.GetPlayerPrefKey("WindowTitleBar"), WindowTitleBar.Value ? 1 : 0);
            // PlayerPrefs.SetInt(Window.GetPlayerPrefKey("HideToolbarWindow"), HideToolbarWindow.Value ? 1 : 0);
            // PlayerPrefs.SetInt(Window.GetPlayerPrefKey("HideToolsInGameMenu"), HideToolsInGameMenu.Value ? 1 : 0);
        }

        public static void ReadPlayerPreferences(FloatingToolWindow window)
        {
            CheatingTools.Value =
                PlayerPrefs.GetInt(window.GetPlayerPrefKey("CheatingTools"), CheatingTools.Value ? 1 : 0) !=
                0;
            ProfilingTools.Value = PlayerPrefs.GetInt(window.GetPlayerPrefKey("ProfilingTools"),
                ProfilingTools.Value ? 1 : 0) != 0;
            DeveloperTools.Value = PlayerPrefs.GetInt(window.GetPlayerPrefKey("DeveloperTools"),
                DeveloperTools.Value ? 1 : 0) != 0;
            ExperimentalTools.Value = PlayerPrefs.GetInt(window.GetPlayerPrefKey("ExperimentalTools"),
                ExperimentalTools.Value ? 1 : 0) != 0;
            MilitaryTool.Value =
                PlayerPrefs.GetInt(window.GetPlayerPrefKey("MilitaryTool"), MilitaryTool.Value ? 1 : 0) != 0;
            ArmyTool.Value =
                PlayerPrefs.GetInt(window.GetPlayerPrefKey("ArmyTool"), ArmyTool.Value ? 1 : 0) != 0;
            TechnologyTool.Value = PlayerPrefs.GetInt(window.GetPlayerPrefKey("TechnologyTool"),
                TechnologyTool.Value ? 1 : 0) != 0;
            ResourcesTool.Value =
                PlayerPrefs.GetInt(window.GetPlayerPrefKey("ResourcesTool"), ResourcesTool.Value ? 1 : 0) !=
                0;
            FramerateTool.Value =
                PlayerPrefs.GetInt(window.GetPlayerPrefKey("FramerateTool"), FramerateTool.Value ? 1 : 0) !=
                0;
            AutoTurnTool.Value =
                PlayerPrefs.GetInt(window.GetPlayerPrefKey("AutoTurnTool"), AutoTurnTool.Value ? 1 : 0) != 0;
            AITool.Value = PlayerPrefs.GetInt(window.GetPlayerPrefKey("AITool"), AITool.Value ? 1 : 0) != 0;
            GPUProfilerTool.Value = PlayerPrefs.GetInt(window.GetPlayerPrefKey("GPUProfilerTool"),
                GPUProfilerTool.Value ? 1 : 0) != 0;
            MemoryProfilerTool.Value = PlayerPrefs.GetInt(window.GetPlayerPrefKey("MemoryProfilerTool"),
                MemoryProfilerTool.Value ? 1 : 0) != 0;
            AffinityTool.Value =
                PlayerPrefs.GetInt(window.GetPlayerPrefKey("AffinityTool"), AffinityTool.Value ? 1 : 0) != 0;
            ArchetypesTool.Value = PlayerPrefs.GetInt(window.GetPlayerPrefKey("ArchetypesTool"),
                ArchetypesTool.Value ? 1 : 0) != 0;
            GraphicsTool.Value =
                PlayerPrefs.GetInt(window.GetPlayerPrefKey("GraphicsTool"), GraphicsTool.Value ? 1 : 0) != 0;
            BattleAITool.Value =
                PlayerPrefs.GetInt(window.GetPlayerPrefKey("BattleAITool"), BattleAITool.Value ? 1 : 0) != 0;
            CivicsTool.Value =
                PlayerPrefs.GetInt(window.GetPlayerPrefKey("CivicsTool"), CivicsTool.Value ? 1 : 0) != 0;
            CollectiblesTool.Value = PlayerPrefs.GetInt(window.GetPlayerPrefKey("CollectiblesTool"),
                CollectiblesTool.Value ? 1 : 0) != 0;
            DiplomacyTool.Value =
                PlayerPrefs.GetInt(window.GetPlayerPrefKey("DiplomacyTool"), DiplomacyTool.Value ? 1 : 0) !=
                0;
            TerrainPickingTool.Value = PlayerPrefs.GetInt(window.GetPlayerPrefKey("TerrainPickingTool"),
                TerrainPickingTool.Value ? 1 : 0) != 0;
            GameInfoTool.Value =
                PlayerPrefs.GetInt(window.GetPlayerPrefKey("GameInfoTool"), GameInfoTool.Value ? 1 : 0) != 0;
            DistrictPainterTool.Value = PlayerPrefs.GetInt(window.GetPlayerPrefKey("DistrictPainterTool"),
                DistrictPainterTool.Value ? 1 : 0) != 0;
            SettlementTools.Value = PlayerPrefs.GetInt(window.GetPlayerPrefKey("SettlementTools"),
                SettlementTools.Value ? 1 : 0) != 0;
            StatisticsAndAchievementsTool.Value = PlayerPrefs.GetInt(
                window.GetPlayerPrefKey("StatisticsAndAchievementsTool"),
                StatisticsAndAchievementsTool.Value ? 1 : 0) != 0;
            FameTool.Value =
                PlayerPrefs.GetInt(window.GetPlayerPrefKey("FameTool"), FameTool.Value ? 1 : 0) != 0;
            EndGameTool.Value =
                PlayerPrefs.GetInt(window.GetPlayerPrefKey("EndGameTool"), EndGameTool.Value ? 1 : 0) != 0;

            WindowTransparency.Value = PlayerPrefs.GetInt(window.GetPlayerPrefKey("WindowTransparency"),
                WindowTransparency.Value ? 1 : 0) != 0;
            WindowTitleBar.Value = PlayerPrefs.GetInt(window.GetPlayerPrefKey("WindowTitleBar"),
                WindowTitleBar.Value ? 1 : 0) != 0;
            HideToolbarWindow.Value =
                false; // PlayerPrefs.GetInt(Window.GetPlayerPrefKey("HideToolbarWindow"), HideToolbarWindow.Value ? 1 : 0) != 0;
            // HideToolsInGameMenu.Value = PlayerPrefs.GetInt(Window.GetPlayerPrefKey("HideToolsInGameMenu"), HideToolsInGameMenu.Value ? 1 : 0) != 0;
        }

        private static int _revisionId => PluginInfo.REVISION;

        public static void ResetPlayerPreferencesIfNecessary(FloatingToolWindow window)
        {
            if (window is InGameMenuWindow repository)
            {
                string revisionIdKey = repository.GetPlayerPrefKey("REVISION_ID");
                if (_revisionId != PlayerPrefs.GetInt(revisionIdKey, 0))
                {
                    Loggr.Log("PREVIOUS REVISION_ID DIFFERS, RESETTING PLAYER PREFERENCES...",
                        ConsoleColor.DarkMagenta);
                    PlayerPrefs.DeleteAll();
                    PlayerPrefs.Save();

                    PlayerPrefs.SetInt(revisionIdKey, _revisionId);
                    PlayerPrefs.Save();
                }
            }
            else
            {
                throw new Exception("Invalid call to ResetPlayerPreferencesIfNecessary()");
            }
        }
    }
}
