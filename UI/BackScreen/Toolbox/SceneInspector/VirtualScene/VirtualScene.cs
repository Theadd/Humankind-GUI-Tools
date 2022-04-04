using System.Linq;
using Amplitude.Extensions;
using UnityEngine;

namespace DevTools.Humankind.GUITools.UI.SceneInspector
{
    public class VirtualScene
    {
        public string RootGameObjectName { get; private set; }
        
        public GameObject RootGameObject
        {
            get => _root;
            set
            {
                _root = value;
                RootGameObjectName = _root.name;
                // _isDirty = true;
            }
        }
        private GameObject _root;

        // private bool _isDirty = true;
        private int _count = 0;

        public VirtualComponent[] Entities { get; private set; } = new VirtualComponent[] { };

        public void Rebuild(MonoBehaviour[] entities)
        {
            _count = -1;
            Entities = entities.Select(CreateEntity).ToArray();
        }

        private VirtualComponent CreateEntity(MonoBehaviour entity)
        {
            return new VirtualComponent()
            {
                Name = entity.name,
                TypeName = entity.GetType().Name,
                Path = entity.gameObject.GetPath(RootGameObjectName),
                Instance = entity,
                Index = ++_count
            };
        }
        
        
        
    }
}
