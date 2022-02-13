using UnityEngine;

namespace DevTools.Humankind.GUITools.UI.SceneInspector
{
    public static class SceneInspectorEx
    {
        public static string GetPath(this Transform current) {
            if (current.parent == null)
                return "/" + current.name;
            return current.parent.GetPath() + "/" + current.name;
        }
        
        public static string GetPath(this Component component) {
            return component.transform.GetPath() + "/" + component.GetType().ToString();
        }
    }
}
