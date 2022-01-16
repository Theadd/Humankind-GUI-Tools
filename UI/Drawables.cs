using System;
using UnityEngine;
using Modding.Humankind.DevTools.DeveloperTools.UI;

namespace DevTools.Humankind.GUITools.UI
{
     public class ScrollableGrid {
        public Vector2 ScrollViewPosition = Vector2.zero;
        public string Style { get; set; } = "PopupWindow.ListGrid";
        public string ContainerStyle { get; set; } = "PopupWindow.ScrollViewGridContainer";
        public Color BackgroundColor { get; set; } = Color.black;
        public int ItemsPerRow { get; set; } = 3;
        public float Height { get; set; } = 120f;
        public float MaxHeight { get; set; } = 230f;
        public float MinHeight { get; set; } = 120f;
        public bool FixedHeight { get; set; } = false;
        public bool AlwaysShowHorizontal { get; set; } = false;
        public bool AlwaysShowVertical { get; set; } = false;
        public int SelectedIndex { get; set; } = 0;
        public string HorizontalScrollbarStyle { get; set; } = "horizontalscrollbar";
        public string VerticalScrollbarStyle { get; set; } = "verticalscrollbar";
        public string ScrollViewStyle { get; set; } = "scrollview";
        public object[] Items { get; set; }
        public ScrollableGrid() { }

        public int Draw<T>() where T : GUIContent
        {
            ScrollViewPosition = GUILayout.BeginScrollView(
                ScrollViewPosition, 
                AlwaysShowHorizontal, 
                AlwaysShowVertical, 
                HorizontalScrollbarStyle,
                VerticalScrollbarStyle,
                ScrollViewStyle,
                new GUILayoutOption[]
            {
                GUILayout.Height(Height + 4)
            });
                GUILayout.BeginVertical(ContainerStyle);
                    var prev = GUI.backgroundColor;
                    GUI.backgroundColor = BackgroundColor;
                    SelectedIndex = GUILayout.SelectionGrid(SelectedIndex, Items as T[], ItemsPerRow, Style);

                    if (Event.current.type == EventType.Repaint && !FixedHeight) {
                        var gridHeight = GUILayoutUtility.GetLastRect().height;
                        gridHeight = gridHeight <= MinHeight ? MinHeight : (gridHeight >= MaxHeight ? MaxHeight : gridHeight);
                        
                        if (Height != gridHeight)
                            Height = gridHeight;
                    }
                    GUI.backgroundColor = prev;
                GUILayout.EndVertical();
            GUILayout.EndScrollView();

            return SelectedIndex;
        }
    }

    public class StaticGrid {
        public Vector2 ScrollViewPosition = Vector2.zero;
        public int ItemsPerRow { get; set; } = 8;
        public string Style { get; set; } = "PopupWindow.Grid";
        public string ContainerStyle { get; set; } = "PopupWindow.GridContainer";
        public Color BackgroundColor { get; set; } = Color.black;
        public int SelectedIndex { get; set; } = 0;
        public object[] Items { get; set; }
        public StaticGrid() { }
        public int Draw<T>() where T : GUIContent
        {
            GUILayout.BeginHorizontal(ContainerStyle);
                var prev = GUI.backgroundColor;
                GUI.backgroundColor = BackgroundColor;
                SelectedIndex = GUILayout.SelectionGrid(SelectedIndex, Items as T[], ItemsPerRow, Style);
                GUI.backgroundColor = prev;
            GUILayout.EndHorizontal();

            return SelectedIndex;
        }
    }

}
