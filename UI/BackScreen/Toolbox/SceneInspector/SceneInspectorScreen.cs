using System.Collections.Generic;
using System.Linq;
using Amplitude.Extensions;
using Amplitude.Mercury.Presentation;
using Modding.Humankind.DevTools;
using Modding.Humankind.DevTools.DeveloperTools.UI;
using StyledGUI;
using UnityEngine;

namespace DevTools.Humankind.GUITools.UI.SceneInspector
{
    public class SceneInspectorScreen
    {

        private bool _drawOnInspectorGUI = false;
        public VirtualScene VirtualHierarchy => _virtualHierarchy ?? (_virtualHierarchy = new VirtualScene());
        private VirtualScene _virtualHierarchy;

        // private Dictionary<string, EntityGroup> Groups { get; set; } = new Dictionary<string, EntityGroup>();
        public EntityGroup Root { get; private set; } = new EntityGroup() {Collapsed = false};

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
                DrawHierarchy();

            }
            GUILayout.EndVertical();
        }

        private void DrawHierarchy()
        {
            GUILayout.BeginVertical(Styles.Alpha50BlackBackgroundStyle);
            {
                Root.RenderContent();
            }
            GUILayout.EndVertical();
        }

        private void UpdateHierarchy<T>() where T : MonoBehaviour
        {
            VirtualHierarchy.RootGameObject = SceneInspectorController.PresentationGO;
            VirtualHierarchy.Rebuild((MonoBehaviour[])
                VirtualHierarchy
                    .RootGameObject
                    .GetComponentsInChildren<T>(true));
            
            Root = new EntityGroup() {Collapsed = false};
            
            foreach (var entity in VirtualHierarchy.Entities)
            {
                EntityGroup group = GetOrCreateGroup(entity.Path, true);
                group.Entities.Add(entity);
            }
        }

        private EntityGroup GetOrCreateGroup(string path, bool createCollapsed = false)
        {
            if (path == "" || path == "/")
                return Root;

            EntityGroup parent;
            string groupName;
            
            int lastSlashPos = path.LastIndexOf('/');
            
            if (lastSlashPos > 0)
            {
                groupName = path.Substring(lastSlashPos + 1);
                parent = GetOrCreateGroup(path.Substring(0, lastSlashPos));
            }
            else
            {
                groupName = path.Substring(lastSlashPos == 0 ? 1 : 0);
                parent = Root;
            }

            var match = parent.Groups.FirstOrDefault(g => g.Path == path);
            if (match == default)
            {
                match = new EntityGroup()
                {
                    Name = groupName,
                    Path = path,
                    Collapsed = createCollapsed
                };
                
                parent.Groups.Add(match);
            }

            return match;
        }
    }

}
