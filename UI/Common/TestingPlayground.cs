using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Amplitude.Graphics;
using Amplitude.Mercury.Terrain;
using Amplitude.Mercury.Terrain.Fx;
using BepInEx.Configuration;
using DevTools.Humankind.GUITools.UI.PauseMenu;
using Modding.Humankind.DevTools;
using Modding.Humankind.DevTools.Core;
using Modding.Humankind.DevTools.DeveloperTools.UI;
using StyledGUI;
using UnityEngine;

namespace DevTools.Humankind.GUITools.UI
{
    public static class TestingPlayground
    {
        private static bool _switchedToolboxTab = false;
        private static bool _didRunOnce = false;
        
        public static void Run()
        {
            if (ViewController.View == ViewType.InGame && !LiveEditorMode.Enabled)
            {
                MappedActions.ToggleLiveEditorMode();
            }

            /*if (ViewController.View == ViewType.InGame)
            {
                PopupToolWindow.Open<CameraLayersToolWindow>(w => MainTools.CameraLayersWindow = w);
                
                var cameraLayers = RenderContextAccess.GetInstance<CameraLayersProvider>(1);
                Loggr.Log("\n\ncameraLayers:\n");
                Loggr.Log(cameraLayers);

                var terrainTypeNames = AssetReferenceRepository.Instance()
                    .StringNames(AssetReferenceRepository.TerrainTypeCriteriaIndex);
                
                Loggr.Log(terrainTypeNames);
            }*/
            
            HumankindDevTools.RegisterAction(
                new KeyboardShortcut(UnityEngine.KeyCode.End), 
                "Testing_PrintVars", 
                PrintVars);
        }

        /// <summary>
        ///     Repeatedly called but with quite a lot of frames in between (see BackScreenWindow.OnZeroGUI).
        /// </summary>
        public static void OnZeroUpdate()
        {
            if (!_switchedToolboxTab && LiveEditorMode.Enabled && ToolboxController.Toolbox != null)
            {
                ToolboxController.Toolbox.SetActiveTab(4);
                _switchedToolboxTab = true;
            }

            if (!_didRunOnce && ViewController.View == ViewType.InGame)
            {
                
                
                
                // Loggr.Log("INITIALIZING UniverseLib...");
                // UniverseLib.Universe.Init();
                // Loggr.Log("UniverseLib initialized.");
                /*var graphicsService = SceneInspectorRaycaster.GetCameraGraphicService;
                var workingCamera = graphicsService.Camera;

                workingCamera.tag = "MainCamera";

                Camera mainCamera = Camera.main;
//                Loggr.Log(mainCamera);
//                Loggr.Log(workingCamera);
//                Loggr.Log("IGUALS: " + (workingCamera == mainCamera));
//                Loggr.Log("workingCameraPath: " + workingCamera.gameObject.GetPath());
//                Loggr.Log("mainCameraPath: " + (mainCamera?.gameObject.GetPath() ?? "NuLL"));
//                
                GameObject taggedCamera = GameObject.FindWithTag("MainCamera");
                
                Loggr.Log("taggedCamera: ");
                Loggr.Log(taggedCamera);*/
                // var t = taggedCamera.tag;

                
                // Loggr.Log(graphicsService);
                // Loggr.Log(cam);
                // Loggr.Log("CameraGraphicService PATH = " + graphicsService.gameObject.GetPath(), ConsoleColor.Red);
                // Loggr.Log("Camera PATH = " + cam.gameObject.GetPath(), ConsoleColor.Red);

                /*var type = typeof(World);

                var props = type.GetProperties();
                var props2 = AccessTools.GetDeclaredProperties(type);
                var propNames = AccessTools.GetPropertyNames(type);
                var tile = new WorldTile() {TileIndex = 24};
                Loggr.Log(props);
                Loggr.Log(props2);
                Loggr.Log(propNames.ToArray());
                Loggr.LogAll(WorldMap.World);
                Loggr.LogAll(tile);
                var tileEx = typeof(WorldTile).GetExtensionMethods();
                var iTileEx = typeof(IWorldTile).GetExtensionMethods();
                var tileEx2 = typeof(WorldTile).GetRuntimeMethods().ToArray();
                var tileEx3 = typeof(WorldTile).GetMethods();
                var tileMethods = tile.GetType().GetRuntimeMethods().ToArray();
                Loggr.LogAll(tileMethods.Select(m => m.Name).ToArray());
                Loggr.Log("!---------------------------------------------\n");
                Loggr.LogAll(tileEx.Select(m => m.Name).ToArray());
                Loggr.LogAll(tileEx2.Select(m => m.Name).ToArray());
                Loggr.LogAll(tileEx3.Select(m => m.Name).ToArray());
                Loggr.LogAll(iTileEx.Select(m => m.Name).ToArray());
                var voType = VirtualObjectType.Create(typeof(WorldTile), tile);
                Loggr.Log(voType.Members.Select(m => m.ToString()).ToArray());*/
                // var allTileEx = typeof(WorldTile).GetAllBaseExtensionMethods();
                // Loggr.LogAll(allTileEx.Select(m => m.Name).ToArray());
                // TODO: type.GetRuntimeProperties()
                
                // Loggr.Log(Font.GetOSInstalledFontNames());

                
                _didRunOnce = true;
                
                /*var inspectorType = UniverseLib.ReflectionUtility.GetTypeByName("UnityExplorer.InspectorManager");
                var cacheObjectBaseType = UniverseLib.ReflectionUtility.GetTypeByName("UnityExplorer.CacheObject.CacheObjectBase");
                if (inspectorType == null)
                {
                    Loggr.Log("ReflectionUtility.GetTypeByName(\"UnityExplorer.InspectorManager\") = NULL",
                        ConsoleColor.Red);
                    return;
                }

                var inspectMethod = inspectorType.GetMethod("Inspect", UniverseLib.ReflectionUtility.FLAGS, null,
                    new Type[] {typeof(Type)}, null);
                var inspectObjectMethod = inspectorType.GetMethod(
                    "Inspect", 
                    UniverseLib.ReflectionUtility.FLAGS, 
                    null,
                    new Type[] {typeof(object), cacheObjectBaseType}, 
                    null);

                inspectMethod.Invoke(null, new object[] {typeof(World)});
                Loggr.Log("UnityExplorer Inspector called successfully!");*/

                //Inspect(Type type)

            }
        }
        
        

        public static void PrintVars()
        {
//            Loggr.LogAll(Amplitude.Mercury.Presentation.Presentation.PresentationCursorController);
//            Loggr.Log(Amplitude.Mercury.Presentation.Presentation.PresentationCursorController.GetFirstTargetUnderMouse());
//            Loggr.Log(
//                "" + Application.Version.Accessibility.ToString() + " is >= Protected? " +
//                (Application.Version.Accessibility >= Accessibility.Protected).ToString(), ConsoleColor.DarkGreen);
//            
        }

        #region UNICODE

        public class CollapsibleCategory<T>
        {
            public string Title { get; set; } = string.Empty;
            public bool Collapsed { get; set; } = true;
            public T Key { get; set; }
        }
        
        private static Dictionary<UnicodeCategory, string[]> UnicodeCharacters = new Dictionary<UnicodeCategory, string[]>();
        private static CollapsibleCategory<UnicodeCategory>[] Categories { get; set; }
        
        private static void GetUnicodeCharactersByCategory()
        {
            var charInfo = Enumerable.Range(0, 0x110000)
                .Where(x => x < 0x00d800 || x > 0x00dfff)
                .Select(char.ConvertFromUtf32)
                .Where(s => s.Length < 2)
                .GroupBy(s => char.GetUnicodeCategory(s, 0))
                .ToDictionary(g => g.Key);

            var keys = charInfo.Keys.ToArray();

            Categories = charInfo.Keys.ToArray().Select(k => new CollapsibleCategory<UnicodeCategory>()
            {
                Key = k,
                Title = "<b>" + R.Text.SplitCamelCase(k.ToString()).ToUpper() + "</b> <size=12> (" + charInfo[k].Count() + " chars)</size>"
            }).ToArray();

            foreach (var key in keys)
            {
                int i = 0;
                var groupsOfValues = charInfo[key]
                    .Select(c => "" + c)
                    .GroupBy(s => i++ / 100)
                    .Select(g => g);

                var chunksOfValues = groupsOfValues
                    .Select(group => string.Join(" ", group.Select(v => v)))
                    .ToArray();
                UnicodeCharacters.Add(key, chunksOfValues);
            }
        }

        private static bool _initialized = false;

        public static void DrawUnicodeCharacters()
        {
            if (!_initialized)
            {
                GetUnicodeCharactersByCategory();
                _initialized = true;
                
            }
            GUILayout.BeginHorizontal();
            // GUILayout.Space(8f);
            GUILayout.BeginVertical();

            foreach (var category in Categories)
            {
                category.Collapsed = GUILayout.Toggle(
                    category.Collapsed, 
                    (category.Collapsed ? " ⁤  " : " ⁢  ") + category.Title, 
                    Styles.CollapsibleSectionToggleStyle);
                if (!category.Collapsed)
                {
                    GUILayout.BeginHorizontal(Styles.Alpha50BlackBackgroundStyle);
                    GUILayout.BeginVertical(Styles.SmallPaddingStyle);
                    {
                        var chunks = UnicodeCharacters[category.Key];
                        foreach (var chunk in chunks)
                        {
                            GUILayout.TextArea(
                                chunk,
                                Styles.UnicodeSymbolTextStyle);
                        }
                    }
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                    GUILayout.Space(4f);
                    
                }
            }
            GUILayout.Space(16f);
            GUILayout.EndVertical();
            // GUILayout.Space(8f);
            GUILayout.EndHorizontal();
        }
        
        #endregion UNICODE
    }
}
