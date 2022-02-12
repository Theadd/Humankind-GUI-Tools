using System;
using System.Collections.Generic;
using System.Linq;
using Amplitude;
using Amplitude.Framework;
using Amplitude.Mercury.Data.Simulation;
using Amplitude.UI;
using Modding.Humankind.DevTools.DeveloperTools.UI;
using UnityEngine;

namespace DevTools.Humankind.GUITools.UI
{
    public static class AssetHunter
    {
        private static List<AssetBundle> _dbInUse = new List<AssetBundle>();
        private static string[] _dbNamesInUse = new string[] {};
        private static StaticString _smallKey = new StaticString("Small");
        private static StaticString _defaultSmallKey = new StaticString("Default_Small");
        private static StaticString _pictoKey = new StaticString("Picto");
        private static StaticString _mediumKey = new StaticString("Medium");

        public static string GetSmallTextureAssetPath(IDatatableElement definition)
        {
            try
            {
                UITexture uiTexture = UIController.DataUtils.GetImage(definition.Name, _smallKey);
                if (("" + uiTexture.AssetPath).Length != 0)
                    return "" + uiTexture.AssetPath;

                string path = "" + UIController.DataUtils.GetImage(definition.Name, _defaultSmallKey).AssetPath;
                path = path.Length > 0 ? path : UIController.DataUtils.GetImage(definition.Name, _mediumKey).AssetPath;

                return path;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static Texture2D LoadTexture(IDatatableElement definition) => 
            LoadTextureAt(GetSmallTextureAssetPath(definition));

        public static Texture2D LoadTextureAt(string assetPath)
        {
            if (assetPath != null && assetPath.Length == 0)
                return null;

            AssetBundle bundle = GetAssetBundleContaining(assetPath);
            Texture2D texture = null;
            
            if (bundle != null)
                texture = bundle.LoadAsset<Texture2D>(assetPath);

            return texture;
        }

        private static AssetBundle GetAssetBundleContaining(string assetPath)
        {
            AssetBundle bundle = _dbInUse.Find(b => b.Contains(assetPath));

            if (bundle != default(AssetBundle))
                return bundle;
            
            bundle = AssetBundle.GetAllLoadedAssetBundles()
                .Where(dle => !_dbNamesInUse.Contains(dle.name))
                .FirstOrDefault(b => b.Contains(assetPath));

            if (bundle != default(AssetBundle))
            {
                _dbInUse.Add(bundle);
                _dbNamesInUse = _dbNamesInUse.AsEnumerable().Append(bundle.name).ToArray();
                
                return bundle;
            }

            return null;
        }
    }
    
}
