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
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("ENABLE INSPECTOR UNDER MOUSE"))
                {
                    LiveEditorMode.EditorMode = EditorModeType.Inspector;
                }

                _drawOnInspectorGUI = GUILayout.Toggle(_drawOnInspectorGUI, "DRAW ORIGINAL INSPECTOR GUI");
                GUILayout.EndHorizontal();
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
        
        private void DrawUIHierarchy()
        {
            GUILayout.BeginVertical(/*Styles.Alpha50BlackBackgroundStyle*/);
            {
                UIVirtualGameObjectRoot.RenderContent();
            }
            GUILayout.EndVertical();
        }

        public void RebuildUIHierarchyUsing(UITransform[] entities)
        {
            UIVirtualGameObjectRoot = UIHierarchyBuilder.Build(entities);
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
