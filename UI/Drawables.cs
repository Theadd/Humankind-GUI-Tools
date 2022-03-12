using System;
using System.Linq;
using UnityEngine;
using Modding.Humankind.DevTools.DeveloperTools.UI;

namespace DevTools.Humankind.GUITools.UI
{
    public class ScrollableGrid
    {
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

        public ScrollableGrid()
        {
        }

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

            if (Event.current.type == EventType.Repaint && !FixedHeight)
            {
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

    public class StaticGrid
    {
        public Vector2 ScrollViewPosition = Vector2.zero;
        public int ItemsPerRow { get; set; } = 8;
        public string Style { get; set; } = "PopupWindow.Grid";
        public string ContainerStyle { get; set; } = "PopupWindow.GridContainer";
        public Color BackgroundColor { get; set; } = Color.black;
        public int SelectedIndex { get; set; } = 0;
        public object[] Items { get; set; }

        public StaticGrid()
        {
        }

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

    public class CustomToolbar
    {
        public GUI.ToolbarButtonSize ButtonSize { get; set; } = GUI.ToolbarButtonSize.FitToContents;
        public GUIStyle Style { get; set; }
        public RangeInt[] Sizes { get; private set; } = Array.Empty<RangeInt>();
        public Vector2 ToolbarSize { get; private set; } = Vector2.zero;


        public int Draw(
            int selected,
            GUIContent[] contents,
            params GUILayoutOption[] options)
        {
            // GUIStyle firstStyle;
            // GUIStyle midStyle;
            // GUIStyle lastStyle;
            // GUI.FindStyles(ref Style, out firstStyle, out midStyle, out lastStyle, "left", "mid", "right");
            Vector2 accumulatedSize = new Vector2();
            int length = contents.Length;
            if (Sizes.Length != length)
                Sizes = contents.Select(c => new RangeInt(0, 0)).ToArray();

            // GUIStyle guiStyle1 = length > 1 ? firstStyle : Style;
            // GUIStyle guiStyle2 = length > 1 ? midStyle : Style;
            // GUIStyle guiStyle3 = length > 1 ? lastStyle : Style;
            GUIStyle guiStyle1 = Style;
            GUIStyle guiStyle2 = Style;
            GUIStyle guiStyle3 = Style;
            float num = 0.0f;
            for (int index = 0; index < contents.Length; ++index)
            {
                if (index == length - 2)
                    guiStyle2 = guiStyle3;
                Vector2 currentButtonSize = guiStyle1.CalcSize(contents[index]);
                switch (ButtonSize)
                {
                    case GUI.ToolbarButtonSize.Fixed:
                        if ((double) currentButtonSize.x > (double) accumulatedSize.x)
                        {
                            accumulatedSize.x = currentButtonSize.x;
                            break;
                        }

                        break;
                    case GUI.ToolbarButtonSize.FitToContents:
                        Sizes[index] = new RangeInt((int) accumulatedSize.x, (int) currentButtonSize.x);
                        accumulatedSize.x += currentButtonSize.x;
                        break;
                }

                if ((double) currentButtonSize.y > (double) accumulatedSize.y)
                    accumulatedSize.y = currentButtonSize.y;
                if (index == length - 1)
                    num += (float) guiStyle1.margin.right;
                else
                    num += (float) Mathf.Max(guiStyle1.margin.right, guiStyle2.margin.left);
                guiStyle1 = guiStyle2;
            }

            switch (ButtonSize)
            {
                case GUI.ToolbarButtonSize.Fixed:
                    accumulatedSize.x = accumulatedSize.x * (float) contents.Length + num;
                    break;
                case GUI.ToolbarButtonSize.FitToContents:
                    accumulatedSize.x += num;
                    break;
            }

            ToolbarSize = accumulatedSize;

            return GUI.Toolbar(
                GUILayoutUtility.GetRect(accumulatedSize.x, accumulatedSize.y, Style, options),
                selected,
                contents,
                Style,
                ButtonSize);
        }
    }
}
