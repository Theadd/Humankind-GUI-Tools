using BepInEx;
using BepInEx.Configuration;
using DevTools.Humankind.GUITools.UI;
using Modding.Humankind.DevTools;

namespace DevTools.Humankind.GUITools
{
    // [BepInDependency("UniverseLib.Mono", BepInDependency.DependencyFlags.SoftDependency)]

    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("Modding.Humankind.DevTools")]
    [BepInDependency("DevTools.Humankind.SharedAssets")]
    public class GUITools : BaseUnityPlugin
    {
        ConfigEntry<bool> QuietMode { get; set; }
        ConfigEntry<bool> DebugMode { get; set; }
        ConfigEntry<bool> WriteLogToDisk { get; set; }
        ConfigEntry<bool> LiveEditorInspectorTab { get; set; }

        private void Awake()
        {
            QuietMode = Config.Bind("Logging", "QuietMode", true,
                new ConfigDescription("Disables this plugin's own message logger."));
            WriteLogToDisk = Config.Bind("Logging", "WriteLogToDisk", true,
                new ConfigDescription("Enables writing plugin's logger output to BepInEx's log file."));
            DebugMode = Config.Bind("Development", "DebugMode", false,
                new ConfigDescription(
                    "Enables some development features like keeping/restoring ui state when hot-reloading changes."));
            LiveEditorInspectorTab = Config.Bind("Development", "LiveEditorInspectorTab", true,
                new ConfigDescription("Whether to display the Inspector tab in Live Editor Mode."));

            FeatureFlags.QuietMode = QuietMode.Value;
            FeatureFlags.WriteLogToDisk = WriteLogToDisk.Value;
            FeatureFlags.DebugMode = DebugMode.Value;
            FeatureFlags.Inspector = LiveEditorInspectorTab.Value;

            MainTools.IsDebugModeEnabled = FeatureFlags.DebugMode;
            Loggr.WriteLogToDisk = FeatureFlags.WriteLogToDisk;
            Modding.Humankind.DevTools.DevTools.QuietMode = FeatureFlags.QuietMode;
            MainTools.Main();
        }

        private void OnDestroy()
        {
            MainTools.Unload(false);
            Destroy(gameObject);
        }
    }
}
