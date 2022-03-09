using BepInEx;

namespace DevTools.Humankind.GUITools
{
    // [BepInPlugin(PLUGIN_GUID, "GUITools", "1.3.1.0")]
    
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("Modding.Humankind.DevTools")]
    [BepInDependency("DevTools.Humankind.SharedAssets")]
    [BepInDependency("UniverseLib.Mono", BepInDependency.DependencyFlags.SoftDependency)]
    public class GUITools : BaseUnityPlugin 
    {
        // const string PLUGIN_GUID = "DevTools.Humankind.GUITools";

        private void Awake()
        {
            MainTools.IsDebugModeEnabled = true;
            MainTools.Main();
        }

        private void OnDestroy()
        {
            MainTools.Unload(false);
            Destroy(gameObject);
        }
    }
}
