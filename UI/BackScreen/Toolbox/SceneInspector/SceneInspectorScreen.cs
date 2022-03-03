using System;
using System.Collections.Generic;
using System.Linq;
using Amplitude.Extensions;
using Amplitude.Mercury.Presentation;
using Amplitude.UI;
using Modding.Humankind.DevTools;
using Modding.Humankind.DevTools.DeveloperTools.UI;
using StyledGUI;
using UnityEngine;

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
                PresentationVirtualGameObjectRoot.RenderContent(Renderer.Using(PresentationHierarchyBuilder));
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
                Loggr.Log((GUIStyle)"PopupWindow.SectionHeader");
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
                    UIVirtualGameObjectRoot.RenderContent(Renderer.Using(UIHierarchyBuilder));
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
