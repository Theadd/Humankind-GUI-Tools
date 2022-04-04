using BepInEx;
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
        // const string PLUGIN_GUID = "DevTools.Humankind.GUITools";

        private void Awake()
        {
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
