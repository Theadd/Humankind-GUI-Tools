using System.Linq;
using Amplitude.Extensions;
using Amplitude.Mercury.Presentation;
using Modding.Humankind.DevTools;
using UnityEngine;

namespace DevTools.Humankind.GUITools.UI.SceneInspector
{
    public class SceneInspectorScreen
    {

        private bool _drawOnInspectorGUI = false;
        public static readonly string RootGameObjectName = "Presentation(Clone)";

        private VirtualSceneEntity[] Hierarchy { get; set; } = new VirtualSceneEntity[] { };

        public void Draw()
        {
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
                
                if (GUILayout.Button("UPDATE HIERARCHY"))
                {
                    UpdateHierarchy();
                }
                GUILayout.Space(12f);
                DrawHierarchy();

            }
            GUILayout.EndVertical();
        }

        private void DrawHierarchy()
        {
            GUILayout.BeginVertical();
            {
                foreach (var entity in Hierarchy)
                {
                    GUILayout.BeginVertical();
                    {
                        GUILayout.Label(entity.Path);
                        GUILayout.Label(entity.TypeName + " " + entity.Name);
                    }
                    GUILayout.EndVertical();
                    GUILayout.Space(8f);
                }
            }
            GUILayout.EndVertical();
        }

        private void UpdateHierarchy()
        {
            var root = SceneInspectorController.PresentationGO;

            if (root != null)
            {
                var entities = root.GetComponentsInChildren<PresentationEntity>(true);
                Loggr.Log("Entities FOUND = " + entities.Length);
                Hierarchy = entities.Select(VirtualSceneEntity.From).ToArray();
            }
        }
    }

    public class VirtualSceneEntity
    {
        public string Name { get; set; }
        public string TypeName { get; set; }
        public string Path { get; set; }

        public static VirtualSceneEntity From(PresentationEntity entity)
        {
            if (entity == null)
                return (VirtualSceneEntity) null;

            return new VirtualSceneEntity()
            {
                Name = entity.name,
                TypeName = entity.GetType().Name,
                Path = entity.gameObject.GetPath(SceneInspectorScreen.RootGameObjectName)
            };
        }
    }
}
