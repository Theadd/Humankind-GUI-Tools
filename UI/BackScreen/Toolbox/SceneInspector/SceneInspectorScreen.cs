using Amplitude.Mercury.Presentation;
using UnityEngine;

namespace DevTools.Humankind.GUITools.UI.SceneInspector
{
    public class SceneInspectorScreen
    {

        public void Draw()
        {
            GUILayout.BeginVertical();
            {
                if (GUILayout.Button("ENABLE INSPECTOR UNDER MOUSE"))
                {
                    LiveEditorMode.EditorMode = EditorModeType.Inspector;
                }
                GUILayout.Space(30f);
                
                if (LiveEditorMode.EditorMode == EditorModeType.Inspector)
                    Presentation.PresentationCursorController.OnInspectorGUI();
            }
            GUILayout.EndVertical();
        }
    }
}
