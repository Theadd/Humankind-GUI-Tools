using System.Collections.Generic;
using UnityEngine;

namespace DevTools.Humankind.GUITools.UI.SceneInspector
{
    public class EntityGroup
    {
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public bool Collapsed { get; set; } = true;

        public List<EntityGroup> Groups { get; set; } = new List<EntityGroup>();
        public List<VirtualSceneEntity> Entities { get; set; } = new List<VirtualSceneEntity>();
    }
    
    public class VirtualSceneEntity
    {
        public string Name { get; set; }
        public string TypeName { get; set; }
        public string Path { get; set; }
        public MonoBehaviour Instance { get; set; }
        public int Index { get; set; }
    }
}
