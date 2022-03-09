using BepInEx;
using DevTools.Humankind.GUITools.UI;

namespace DevTools.Humankind.GUITools
{
    // [BepInPlugin(PLUGIN_GUID, "GUITools", "1.3.1.0")]
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
            MainTools.Main();
        }

        private void OnDestroy()
        {
            MainTools.Unload(false);
            Destroy(gameObject);
        }
    }
}
