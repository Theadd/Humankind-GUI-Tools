using UnityEngine;

namespace StyledGUI
{
    public static class GUILayoutTree
    {
        public static float OneLevelDepthSize = 12f;
        public static RectOffset RowPadding = new RectOffset(4, 0, 0, 0);
        public static RectOffset ItemPadding = new RectOffset(25, 8, 0, 0);

        public static void BeginRow(float depth = 0)
        {
            GUILayout.BeginVertical(Styles.TreeBackgroundRowStyle);
            if (RowPadding.top != 0)
                GUILayout.Space(RowPadding.top);
            
            GUILayout.BeginHorizontal();
            GUILayout.Space(RowPadding.left + OneLevelDepthSize * depth);
        }
        
        public static bool EndRow()
        {
            GUILayout.Space(RowPadding.right);
            GUILayout.EndHorizontal();
            if (RowPadding.bottom != 0)
                GUILayout.Space(RowPadding.bottom);
            GUILayout.EndVertical();
            
            if (Event.current.type == EventType.MouseUp)
            {
                if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
                    return true;
            }

            return false;
        }
        
        public static void BeginItem(float depth = 0)
        {
            GUILayout.BeginVertical(Styles.TreeBackgroundRowStyle);
            if (ItemPadding.top != 0)
                GUILayout.Space(ItemPadding.top);
            
            GUILayout.BeginHorizontal();
            GUILayout.Space(RowPadding.left + ItemPadding.left + OneLevelDepthSize * depth);
        }
        
        public static bool EndItem()
        {
            GUILayout.Space(RowPadding.right + ItemPadding.right);
            GUILayout.EndHorizontal();
            if (ItemPadding.bottom != 0)
                GUILayout.Space(ItemPadding.bottom);
            GUILayout.EndVertical();
            
            if (Event.current.type == EventType.MouseUp)
            {
                if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
                    return true;
            }

            return false;
        }
    }
}
