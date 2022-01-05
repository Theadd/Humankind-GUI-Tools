using System;
using BepInEx;
using Modding.Humankind.DevTools;
using Modding.Humankind.DevTools.DeveloperTools;
using UnityEngine;

namespace DevTools.Humankind.GUITools
{
    [BepInPlugin(PLUGIN_GUID, "GUITools", "1.0.1.0")]
    public class GUITools : BaseUnityPlugin 
    {
        const string PLUGIN_GUID = "DevTools.Humankind.GUITools";

        public static AssetLoader Assets =>
            _assets ?? (_assets = new AssetLoader()
            {
                Assembly = typeof(GUITools).Assembly,
                ManifestResourceName = PLUGIN_GUID + ".Resources.bundled-resources"
            });

        private static AssetLoader _assets;

        private void Start()
        {
            Loggr.Log($"[{PLUGIN_GUID}] Start()");
            // MainTools.Main();
        }
        
        private void Awake()
        {
            Loggr.Log($"[{PLUGIN_GUID}] Awake()");
            MainTools.IsDebugModeEnabled = false;
            MainTools.Main();
        }

        private void OnDestroy()
        {
            MainTools.Unload();
            Destroy(gameObject);
        }
    }
}
