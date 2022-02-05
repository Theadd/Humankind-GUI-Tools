using System;
using System.Collections.Generic;
using Modding.Humankind.DevTools;
using Modding.Humankind.DevTools.DeveloperTools.UI;
using StyledGUI.VirtualGridElements;
using UnityEngine;

namespace DevTools.Humankind.GUITools.UI
{
    public class BackScreenToolbox
    {
        public BackScreenWindow Window { get; set; }
        public ConstructiblesGrid ConstructiblesGrid { get; set; }
        public Vector2 ScrollViewPosition { get; set; } = Vector2.zero;
        public float VScrollbarWidth { get; set; } = 10f;
        public RectOffset ScrollViewPadding { get; set; } = new RectOffset(8, 8, 0, 0);

        public int ActiveTab { get; set; } = 0;

        private string[] tabNames =
        {
            "<size=14><b> UNITS </b></size>", 
            "<size=14><b> DISTRICTS </b></size>"
        };
        
        private string[] displayModeNames =
        {
            "<size=10><b>LIST</b></size>", 
            "<size=10><b>GRID</b></size>"
        };

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

        private void DrawToolboxHeader()
        {
            GUILayout.Space(8f);

            GUILayout.BeginHorizontal();
            {
                GUILayout.Button("<size=10><b>SELECT NONE</b></size>");
                GUILayout.Button("<size=10><b>ZOOM IN</b></size>");
                GUILayout.Button("<size=10><b>ZOOM OUT</b></size>");

                GUILayout.FlexibleSpace();

                var shouldDisplayAsGrid =
                    (GUILayout.Toolbar(ToolboxController.IsDisplayModeGrid ? 1 : 0, displayModeNames) == 1);

                if (shouldDisplayAsGrid != ToolboxController.IsDisplayModeGrid)
                {
                    ToolboxController.IsDisplayModeGrid = shouldDisplayAsGrid;
                }

                GUILayout.Space(8f);
            }
            GUILayout.EndHorizontal();
            
            GUILayout.Space(30f);
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
                "scrollview",
                new GUILayoutOption[]
                {
                    // GUILayout.Height(300f)
                });
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

        private void DrawTabs()
        {
            GUILayout.Space(6f);
            GUILayout.BeginHorizontal();
            GUILayout.Space(12f);
            
            var activeTab = GUILayout.Toolbar(ActiveTab, tabNames, "TabButton", GUI.ToolbarButtonSize.FitToContents);

            if (activeTab != ActiveTab)
            {
                _storedScrollViewPositions[ActiveTab] = ScrollViewPosition;
                ScrollViewPosition = _storedScrollViewPositions[activeTab];
                ActiveTab = activeTab;
                ConstructiblesGrid.VirtualGrid.VisibleViews = new[] { ActiveTab };
            }
            GUILayout.EndHorizontal();
            Utils.DrawHorizontalLine();
        }

        public void OnClickHandler(ICell cell)
        {
            ConstructiblesGrid.VirtualGrid.Cursor.AddToSelection();
        }
    }
}