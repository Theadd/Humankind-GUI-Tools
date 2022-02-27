using System.Collections.Generic;
using UnityEngine;

namespace DevTools.Humankind.GUITools.UI.SceneInspector
{
    public interface IVirtualEntity
    {
        string Name { get; set; }
    }
    
    public class VirtualGameObject : IVirtualEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public bool Collapsed { get; set; } = true;

        public List<VirtualGameObject> Children { get; set; } = new List<VirtualGameObject>();
        public List<VirtualComponent> Components { get; set; } = new List<VirtualComponent>();
    }
    
    public class VirtualComponent : IVirtualEntity
    {
        public string Name { get; set; }
        public string TypeName { get; set; }
        public string Path { get; set; }
        public MonoBehaviour Instance { get; set; }
        public int Index { get; set; }
    }
}
