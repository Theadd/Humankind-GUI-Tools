using System;
using Modding.Humankind.DevTools;
using StyledGUI.VirtualGridElements;
using UnityEngine;

namespace DevTools.Humankind.GUITools.UI
{
    public class BackScreenToolbox
    {
        public BackScreenWindow Window { get; set; }
        public ConstructiblesGrid ConstructiblesGrid { get; set; }
        public Vector2 ScrollViewPosition { get; set; } = Vector2.zero;

        public int ActiveTab { get; set; } = 0;

        private string[] tabNames =
        {
            "<size=10><b> UNITS </b></size>", 
            "<size=10><b> DISTRICTS </b></size>"
        };
        
        public void Draw(Rect targetRect)
        {
            GUILayout.BeginArea(targetRect);
            {
                Draw();
            }
            GUILayout.EndArea();
        }

        public void Draw()
        {
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            //GUILayout.Space(1f);

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
                GUILayout.BeginVertical();
                {
                    if (Event.current.type == EventType.MouseDown)
                        ConstructiblesGrid.VirtualGrid.FindCellAtPosition(Event.current.mousePosition, OnClickHandler);

                    ConstructiblesGrid.Render();

                    GUILayout.Space(8f);
                    GUILayout.Label("FOCUSED CONTROL: " + GUI.GetNameOfFocusedControl());
                    if (GUILayout.Button("TESTME"))
                    {
                        Loggr.Log("EVENT TYPE = " + Event.current.type.ToString());
                    }
                }
                GUILayout.EndVertical();
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
                ActiveTab = activeTab;
                ConstructiblesGrid.VirtualGrid.VisibleViews = new[] { ActiveTab };
            }
            GUILayout.EndHorizontal();
            Utils.DrawHorizontalLine();
        }

        public void OnClickHandler(ICell cell)
        {
            Loggr.Log("FOUND CELL FOR CLICK! CELL TYPE = " + cell.GetType().Name, ConsoleColor.DarkCyan);
            Loggr.Log(cell);
            ConstructiblesGrid.VirtualGrid.Cursor.AddToSelection();
        }
    }
}