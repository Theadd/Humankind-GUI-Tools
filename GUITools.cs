using BepInEx;
using Modding.Humankind.DevTools;
using UnityEngine;

namespace DevTools.Humankind.GUITools
{
    [BepInPlugin(PLUGIN_GUID, "GUITools", "1.3.1.0")]
    [BepInDependency("Modding.Humankind.DevTools")]
    [BepInDependency("DevTools.Humankind.SharedAssets")]
    public class GUITools : BaseUnityPlugin 
    {
        const string PLUGIN_GUID = "DevTools.Humankind.GUITools";
        
        private void Awake()
        {
            MainTools.IsDebugModeEnabled = false;
            MainTools.Main();
        }

        private void OnDestroy()
        {
            MainTools.Unload(false);
            Destroy(gameObject);
        }
    }
}
