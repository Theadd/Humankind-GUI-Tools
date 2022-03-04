using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Amplitude.Extensions;
using Amplitude.Mercury.Presentation;
using Amplitude.Mercury.Sandbox;
using Amplitude.UI;
using Modding.Humankind.DevTools;
using Modding.Humankind.DevTools.Core;
using Modding.Humankind.DevTools.DeveloperTools.UI;
using StyledGUI;
using UniverseLib;
using UnityEngine;
using UniverseLib.Utility;

namespace DevTools.Humankind.GUITools.UI.SceneInspector
{
    public class SceneInspectorScreen
    {

        private bool _drawOnInspectorGUI = false;
        private bool _initialized = false;

        private VirtualGameObject UIVirtualGameObjectRoot { get; set; } = new VirtualGameObject() {Collapsed = false};
        private VirtualGameObject PresentationVirtualGameObjectRoot { get; set; } = new VirtualGameObject() {Collapsed = false};
        private HierarchyBuilder UIHierarchyBuilder { get; set; }
        private HierarchyBuilder PresentationHierarchyBuilder { get; set; }
        public IVirtualSceneRenderer Renderer { get; set; }

        private void Initialize()
        {
            /* TODO: REMOVE */ // WireRenderer.Attach();
            Renderer = new VirtualSceneRenderer();
            UIHierarchyBuilder = new HierarchyBuilder()
            {
                RootGameObject = GameObject.Find("/WindowsRoot")
            };
            PresentationHierarchyBuilder = new HierarchyBuilder()
            {
                RootGameObject = SceneInspectorController.PresentationGO
            };
            _initialized = true;
        }
        
        private static readonly MethodInfo DestroyTerrainAtMethod =
            typeof(Amplitude.Mercury.Simulation.World).GetMethod("DestroyTerrainAt", R.NonPublicInstance);

        private string text = UniverseLib.Utility.SignatureHighlighter.Parse(typeof(Amplitude.Mercury.Simulation.World),
            true, DestroyTerrainAtMethod);
        private string text2 = UniverseLib.Utility.SignatureHighlighter.HighlightMethod(DestroyTerrainAtMethod);

        public static readonly Color StringOrange = new Color(0.83f, 0.61f, 0.52f);
        public static readonly Color EnumGreen = new Color(0.57f, 0.76f, 0.43f);
        public static readonly Color KeywordBlue = new Color(0.3f, 0.61f, 0.83f);
        public static readonly string keywordBlueHex = KeywordBlue.ToHex();
        public static readonly Color NumberGreen = new Color(0.71f, 0.8f, 0.65f);
        
        private string[] colors = new[]
        {
            "#a8a8a8", "#92c470", "#3a8d71", "#2df7b2", "#0fba3a", "#9b9b82", "#8d8dc6", "#c266ff", "#b55b02",
            "#ff8000", "#588075", "#55a38e", "#a6e9e9", "#" + StringOrange.ToHex(), "#" + EnumGreen.ToHex(), "#" + KeywordBlue.ToHex(), "#" + NumberGreen.ToHex(), "#000000"
        };
        

        
        public void Draw()
        {
            if (!_initialized)
                Initialize();
            
            GUILayout.BeginVertical();
            {
                DrawCursorInfo();
                GUILayout.Space(12f);
                GUILayout.BeginVertical();
                _drawOnInspectorGUI = GUILayout.Toggle(_drawOnInspectorGUI, "DRAW ORIGINAL INSPECTOR GUI");

                //var text = UniverseLib.Utility.SignatureHighlighter.Parse(typeof(Amplitude.Mercury.Simulation.World), true, DestroyTerrainAtMethod);
                //var text2 = UniverseLib.Utility.SignatureHighlighter.HighlightMethod(DestroyTerrainAtMethod);
                GUILayout.Label(text);
                GUILayout.Label(text, Styles.TreeInlineTextStyle);
                GUILayout.Label(text2);
                GUILayout.Label(text2, Styles.TreeInlineTextStyle);
                
                GUILayout.BeginHorizontal();
                var i = -1;
                foreach (var color in Colors.Values)
                {
                    GUILayout.Label($"<color={color}><size=12> <color=#000000EE>{++i}</color></size></color>", Styles.UnicodeIconStyle, GUILayout.Width(20f), GUILayout.ExpandWidth(false));
                }
                GUILayout.EndHorizontal();

                /*for (var i = 0; i < colors.Length - 1; i += 2)
                {
                    GUILayout.Label($"<color={colors[i]}FF>HelloWorld</color>.<color={colors[i+1]}FF>AnywhereDestroyAll</color>() ", Styles.TreeInlineTextStyle);
                }*/
                
                GUILayout.EndVertical();
                GUILayout.Space(30f);
                
                if (LiveEditorMode.EditorMode == EditorModeType.Inspector && _drawOnInspectorGUI)
                    Presentation.PresentationCursorController.OnInspectorGUI();

                DrawUIHierarchy();
                
                if (GUILayout.Button("UPDATE PresentationEntity HIERARCHY"))
                {
                    UpdateHierarchy<PresentationEntity>();
                }
                if (GUILayout.Button("UPDATE PresentationCursorTarget HIERARCHY"))
                {
                    UpdateHierarchy<PresentationCursorTarget>();
                }
                if (GUILayout.Button("UPDATE PresentationEntityFeedback HIERARCHY"))
                {
                    UpdateHierarchy<PresentationEntityFeedback>();
                }
                GUILayout.Space(12f);
                DrawPresentationHierarchy();

            }
            GUILayout.EndVertical();
            
            Renderer.Finish();
        }

        private void DrawCursorInfo()
        {
            GUILayout.BeginVertical(Styles.SmallPaddingStyle);
            {
                GUILayout.BeginHorizontal(Styles.Alpha50BlackBackgroundStyle);
                {
                    var pos = SceneInspectorController.GetMousePosition();
                    var worldPos = Presentation.PresentationCursorController.CurrentHighlightedPosition;
                    var tileIndex = worldPos.ToTileIndex();
                    
                    GUILayout.Label($"<size=16></size><size=12>   <size=13>X</size>: <color=#33DD33DC>{pos.x}</color>  <size=13>Y</size>: <color=#33DD33DC>{pos.y}</color></size>", Styles.UnicodeLabelStyle);
                    GUILayout.FlexibleSpace();
                    GUILayout.Label($"<size=12></size><size=13>   [<color=#33DD33DC>{worldPos.Column}</color>, <color=#33DD33DC>{worldPos.Row}</color>]    <size=16>⌬</size>   <color=#33DD33DC>{tileIndex}</color></size>", Styles.UnicodeLabelStyle);
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }

        private void DrawPresentationHierarchy()
        {
            GUILayout.BeginVertical(Styles.Alpha50BlackBackgroundStyle);
            {
                PresentationVirtualGameObjectRoot.RenderTree(Renderer.Using(PresentationHierarchyBuilder));
            }
            GUILayout.EndVertical();
        }

        private bool DrawGUISectionHeader(string title, string rightSideContent)
        {
            GUILayout.BeginHorizontal(rightSideContent, Styles.SectionHeaderToggleAreaStyle);
            {
                GUILayout.BeginHorizontal(Styles.SmallPaddingStyle);
                {
                    GUI.color = Styles.BlueTextColor;
                    GUILayout.Label(title, Styles.Fixed20pxHeightTextStyle);
                    GUI.color = Color.white;
                    GUILayout.FlexibleSpace();
                    // GUILayout.Label(rightSideContent, Styles.Fixed20pxHeightTextStyle);

                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndHorizontal();
            if (Event.current.type == EventType.MouseUp)
            {
                if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
                    return true;
            }

            return false;
        }

        private bool _uiHierarchyCollapsed = false;
        private bool _showUIHierarchyContent = false;
        
        private void DrawUIHierarchy()
        {
            if (GUILayout.Button(new GUIContent("DEBUG STYLE", "TEST TOOLTIP for debug style button")))
            {
                Loggr.Log(GUI.skin.label);
                Loggr.Log(Styles.TreeInlineTextStyle);
                Loggr.Log(text);
                Loggr.Log(text2);
                Loggr.Log(colors);
            }

            GUILayout.Label(GUI.tooltip);
            
            var hierarchyNotEmpty = UIVirtualGameObjectRoot.Children.Count > 0 ||
                                    UIVirtualGameObjectRoot.Components.Count > 0;
            GUI.enabled = hierarchyNotEmpty;
            var actionText = hierarchyNotEmpty ? 
                (_uiHierarchyCollapsed ? "  " : "  ") 
                : "<size=12>NO HITS (EMPTY)</size>  ";
            if (DrawGUISectionHeader("GAME UI", actionText))
            {
                _uiHierarchyCollapsed = !_uiHierarchyCollapsed;
            }
            if (_showUIHierarchyContent)
            {
                GUILayout.BeginVertical(Styles.Alpha50BlackBackgroundStyle);
                {
                    UIVirtualGameObjectRoot.RenderTree(Renderer.Using(UIHierarchyBuilder));
                }
                GUILayout.EndVertical();
            }

            if (Event.current.type == EventType.Repaint)
                _showUIHierarchyContent = !_uiHierarchyCollapsed && hierarchyNotEmpty;

            GUI.enabled = true;
        }

        public void RebuildUIHierarchyUsing(UITransform[] entities)
        {
            var withSiblings = entities
                .SelectMany(ut => ut.gameObject.GetComponents<MonoBehaviour>())
                .ToArray();
            
            // UIVirtualGameObjectRoot = UIHierarchyBuilder.Build(entities);
            UIVirtualGameObjectRoot = UIHierarchyBuilder.Build(withSiblings);
        }

        private void UpdateHierarchy<T>() where T : MonoBehaviour
        {
            PresentationVirtualGameObjectRoot = PresentationHierarchyBuilder.Build(
                PresentationHierarchyBuilder
                    .RootGameObject
                    .GetComponentsInChildren<T>(true));
        }

    }

}
