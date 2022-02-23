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

        private void Initialize()
        {
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
        }

        private void DrawPresentationHierarchy()
        {
            GUILayout.BeginVertical(Styles.Alpha50BlackBackgroundStyle);
            {
                PresentationVirtualGameObjectRoot.RenderContent();
            }
            GUILayout.EndVertical();
        }

        private void DrawGUISectionHeader(string title, string additionalInfo)
        {
            GUILayout.BeginHorizontal(Styles.Alpha65WhiteBackgroundStyle);
            {
                GUILayout.BeginHorizontal(Styles.SmallPaddingStyle);
                {
                    GUILayout.Label(title, Styles.Fixed20pxHeightTextStyle);
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(additionalInfo, Styles.Fixed20pxHeightTextStyle);

                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndHorizontal();
        }
        
        private void DrawUIHierarchy()
        {
            if (GUILayout.Button("DEBUG STYLE"))
            {
                Loggr.Log((GUIStyle)"PopupWindow.SectionHeader");
            }

            DrawGUISectionHeader("GAME UI", "NO HITS (EMPTY)");
            // GUILayout.Label("GAME UI", "PopupWindow.SectionHeader");
            if (UIVirtualGameObjectRoot.Children.Count > 0 || UIVirtualGameObjectRoot.Components.Count > 0)
            {
                GUILayout.BeginVertical(Styles.Alpha50BlackBackgroundStyle);
                {
                    UIVirtualGameObjectRoot.RenderContent();
                }
                GUILayout.EndVertical();
            }
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
