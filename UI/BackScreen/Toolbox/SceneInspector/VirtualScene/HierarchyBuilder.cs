using System.Linq;
using UnityEngine;

namespace DevTools.Humankind.GUITools.UI.SceneInspector
{
    public class HierarchyBuilder
    {
        public GameObject RootGameObject { get; set; }

        private EntityGroup Root { get; set; } = new EntityGroup() {Collapsed = false};
        
        public VirtualScene VirtualHierarchy => _virtualHierarchy ?? (_virtualHierarchy = new VirtualScene());
        private VirtualScene _virtualHierarchy;

        public EntityGroup Build(MonoBehaviour[] entities)
        {
            VirtualHierarchy.RootGameObject = RootGameObject;
            VirtualHierarchy.Rebuild(entities);
            
            Root = new EntityGroup() {Collapsed = false};
            
            foreach (var entity in VirtualHierarchy.Entities)
            {
                EntityGroup group = GetOrCreateGroup(entity.Path, true);
                group.Entities.Add(entity);
            }

            return Root;
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
