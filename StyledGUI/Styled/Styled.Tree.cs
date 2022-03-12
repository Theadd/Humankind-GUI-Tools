using UnityEngine;

namespace StyledGUI
{
    public static partial class Styled
    {
        public static float TreeLevelDepthSize = 12f;
        public static RectOffset TreeRowPadding = new RectOffset(2, 0, 0, 0);
        public static RectOffset TreeItemPadding = new RectOffset(22, 8, 0, 0);
        public static Rect LastItemRect { get; set; } = Rect.zero;

        public static void BeginTreeRow(float depth = 0)
        {
            GUILayout.BeginVertical(Styles.TreeBackgroundRowStyle);
            if (TreeRowPadding.top != 0)
                GUILayout.Space(TreeRowPadding.top);
            
            GUILayout.BeginHorizontal();
            GUILayout.Space(TreeRowPadding.left + TreeLevelDepthSize * depth);
        }
        
        public static bool EndTreeRow()
        {
            GUILayout.Space(TreeRowPadding.right);
            GUILayout.EndHorizontal();
            if (TreeRowPadding.bottom != 0)
                GUILayout.Space(TreeRowPadding.bottom);
            GUILayout.EndVertical();
            
            if (Event.current.type == EventType.MouseUp)
            {
                if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
                    return true;
            }

            return false;
        }
        
        public static void BeginTreeItem(float depth = 0)
        {
            GUILayout.BeginVertical(Styles.TreeBackgroundRowStyle);
            if (TreeItemPadding.top != 0)
                GUILayout.Space(TreeItemPadding.top);
            
            GUILayout.BeginHorizontal();
            GUILayout.Space(TreeRowPadding.left + TreeItemPadding.left + TreeLevelDepthSize * depth);
        }
        
        public static bool EndTreeItem()
        {
            GUILayout.Space(TreeRowPadding.right + TreeItemPadding.right);
            GUILayout.EndHorizontal();
            if (TreeItemPadding.bottom != 0)
                GUILayout.Space(TreeItemPadding.bottom);
            GUILayout.EndVertical();

            LastItemRect = GUILayoutUtility.GetLastRect();
                
            if (Event.current.type == EventType.MouseUp)
            {
                if (LastItemRect.Contains(Event.current.mousePosition))
                    return true;
            }

            return false;
        }
    }
}
