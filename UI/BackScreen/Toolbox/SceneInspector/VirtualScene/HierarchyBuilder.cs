using System.Linq;
using UnityEngine;

namespace DevTools.Humankind.GUITools.UI.SceneInspector
{

    public interface IRenderableScene
    {
        GameObject RootGameObject { get; set; }
    }
    
    public class HierarchyBuilder : IRenderableScene
    {
        public GameObject RootGameObject { get; set; }

        private VirtualGameObject Root { get; set; } = new VirtualGameObject() {Collapsed = false};
        
        public VirtualScene VirtualHierarchy => _virtualHierarchy ?? (_virtualHierarchy = new VirtualScene());
        private VirtualScene _virtualHierarchy;

        public VirtualGameObject Build(MonoBehaviour[] entities)
        {
            VirtualHierarchy.RootGameObject = RootGameObject;
            VirtualHierarchy.Rebuild(entities);
            
            Root = new VirtualGameObject() {Collapsed = false};
            
            foreach (var entity in VirtualHierarchy.Entities)
            {
                VirtualGameObject group = GetOrCreateGroup(entity.Path, true);
                group.Components.Add(entity);
            }

            return Root;
        }
        
        private VirtualGameObject GetOrCreateGroup(string path, bool createCollapsed = false)
        {
            if (path == "" || path == "/")
                return Root;

            VirtualGameObject parent;
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

            var match = parent.Children.FirstOrDefault(g => g.Path == path);
            if (match == default)
            {
                match = new VirtualGameObject()
                {
                    Name = groupName,
                    Path = path,
                    Collapsed = createCollapsed
                };
                
                parent.Children.Add(match);
            }

            return match;
        }
    }
}
