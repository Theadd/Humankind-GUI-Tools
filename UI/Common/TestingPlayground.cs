using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Amplitude.Extensions;
using Amplitude.Framework;
using BepInEx.Configuration;
using DevTools.Humankind.GUITools.UI.SceneInspector;
using Modding.Humankind.DevTools;
using Modding.Humankind.DevTools.Core;
using Modding.Humankind.DevTools.DeveloperTools.UI;
using StyledGUI;
using UnityEngine;
using Application = Amplitude.Framework.Application;

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

            /*if (!_didRunOnce)
            {
                var graphicsService = SceneInspectorRaycaster.GetCameraGraphicService;
                var cam = graphicsService.Camera;
                
                Loggr.Log(graphicsService);
                Loggr.Log(cam);
                Loggr.Log("CameraGraphicService PATH = " + graphicsService.gameObject.GetPath(), ConsoleColor.Red);
                Loggr.Log("Camera PATH = " + cam.gameObject.GetPath(), ConsoleColor.Red);
                
                _didRunOnce = true;
            }*/
        }

        public static void PrintVars()
        {
            Loggr.LogAll(Amplitude.Mercury.Presentation.Presentation.PresentationCursorController);
            Loggr.Log(Amplitude.Mercury.Presentation.Presentation.PresentationCursorController.GetFirstTargetUnderMouse());
            Loggr.Log(
                "" + Application.Version.Accessibility.ToString() + " is >= Protected? " +
                (Application.Version.Accessibility >= Accessibility.Protected).ToString(), ConsoleColor.DarkGreen);
            
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
