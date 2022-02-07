using System.Collections.Generic;
using StyledGUI.VirtualGridElements;
using UnityEngine;

namespace DevTools.Humankind.GUITools.UI
{
    public partial class ConstructiblesToolbox
    {
        public BackScreenWindow Window { get; set; }
        public ConstructiblesGrid ConstructiblesGrid { get; set; }
        public Vector2 ScrollViewPosition { get; set; } = Vector2.zero;
        public float VScrollbarWidth { get; set; } = 10f;
        public RectOffset ScrollViewPadding { get; set; } = new RectOffset(8, 8, 0, 0);
        public int ActiveTab { get; set; } = 0;

        private List<Vector2> _storedScrollViewPositions = new List<Vector2>() {Vector2.zero, Vector2.zero};
        
        public void Draw(Rect targetRect)
        {
            GUILayout.BeginArea(targetRect);
            {
                if ((int)ConstructiblesGrid.FixedWidth != (int)(targetRect.width - VScrollbarWidth - ScrollViewPadding.left - ScrollViewPadding.right))
                    ConstructiblesGrid.FixedWidth = (int)(targetRect.width - VScrollbarWidth - ScrollViewPadding.left - ScrollViewPadding.right);
                
                Draw();
            }
            GUILayout.EndArea();
        }

        public void Draw()
        {
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();

            DrawToolboxHeader();
            
            DrawTabs();
            
            ScrollViewPosition = GUILayout.BeginScrollView(
                ScrollViewPosition, 
                false, 
                false, 
                "horizontalscrollbar",
                "verticalscrollbar",
                "scrollview");
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(ScrollViewPadding.left);
                GUILayout.BeginVertical(GUILayout.Width(ConstructiblesGrid.FixedWidth));
                {
                    if (Event.current.type == EventType.MouseDown)
                        ConstructiblesGrid.VirtualGrid.FindCellAtPosition(Event.current.mousePosition, OnClickHandler);

                    ConstructiblesGrid.Render();

                    GUILayout.Space(8f);
                }
                GUILayout.EndVertical();
                //GUILayout.Space(ScrollViewPadding.right);
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            GUILayout.Space(1f);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
        
        public void OnClickHandler(ICell cell)
        {
            ConstructiblesGrid.VirtualGrid.Cursor.AddToSelection();
        }
    }
}