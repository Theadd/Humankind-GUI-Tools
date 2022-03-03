using UnityEngine;

namespace StyledGUI
{
    public static class GUILayoutTree
    {
        public static float OneLevelDepthSize = 12f;

        public static void BeginRow(float depth = 0)
        {
            GUILayout.BeginHorizontal(Styles.TreeBackgroundRowStyle);
            GUILayout.Space(OneLevelDepthSize * depth);
            GUILayout.BeginHorizontal();
        }
        
        public static bool EndRow()
        {
            GUILayout.EndHorizontal();
            GUILayout.EndHorizontal();
            
            if (Event.current.type == EventType.MouseUp)
            {
                if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
                    return true;
            }

            return false;
        }
    }
}
