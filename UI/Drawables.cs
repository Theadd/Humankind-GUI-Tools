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

    public abstract class DrawableGridBase
    {
        public float CellWidth;
        public float CellSpace;
        public GUILayoutOption CellSpan1;
        public GUILayoutOption CellSpan2;
        public GUILayoutOption CellSpan3;
        public GUILayoutOption CellSpan4;
        public GUILayoutOption CellSpan5;
        public GUILayoutOption CellSpan6;
        public GUILayoutOption CellSpan7;
        public GUILayoutOption CellSpan8;
        public GUILayoutOption CellSpan9;
        public GUILayoutOption CellSpan10;
        public GUILayoutOption CellSpan(int numCells) => GUILayout.Width(CellWidth * numCells + CellSpace * (numCells - 1));

        public DrawableGridBase()
        {
            Resize(34f, 1f);
        }

        public DrawableGridBase(float cellWidth, float cellSpace)
        {
            Resize(cellWidth, cellSpace);
        }

        public DrawableGridBase Resize(float cellWidth, float cellSpace)
        {
            CellWidth = cellWidth;
            CellSpace = cellSpace;
            CellSpan1 = CellSpan(1);
            CellSpan2 = CellSpan(2);
            CellSpan3 = CellSpan(3);
            CellSpan4 = CellSpan(4);
            CellSpan5 = CellSpan(5);
            CellSpan6 = CellSpan(6);
            CellSpan7 = CellSpan(7);
            CellSpan8 = CellSpan(8);
            CellSpan9 = CellSpan(9);
            CellSpan10 = CellSpan(10);

            return this;
        }
    }

    public class DrawableGrid<T> : DrawableGridBase where T : class
    {

        public GUIStyle RowStyle { get; set; } = new GUIStyle(UIController.DefaultSkin.FindStyle("PopupWindow.Row"))
        {
            alignment = TextAnchor.LowerRight,
            padding = new RectOffset(0, 0, 0, 0),
            margin = new RectOffset(0, 0, 0, 0),
            hover = new GUIStyleState()
            {
                background = Utils.CreateSinglePixelTexture2D(new Color32(85, 136, 254, 70)),
                textColor = Color.white
            }
        };

        public GUIStyle StaticRowStyle { get; set; } = new GUIStyle(UIController.DefaultSkin.FindStyle("PopupWindow.Row"))
        {
            alignment = TextAnchor.LowerRight,
            padding = new RectOffset(0, 0, 0, 0),
            margin = new RectOffset(0, 0, 0, 0),
            normal = new GUIStyleState()
            {
                background = null,
                textColor = Color.white
            },
            hover = new GUIStyleState()
            {
                background = null,
                textColor = Color.white
            }
        };

        public GUIStyle CenteredStaticRowStyle { get; set; } = new GUIStyle(UIController.DefaultSkin.FindStyle("PopupWindow.Row"))
        {
            alignment = TextAnchor.LowerCenter,
            padding = new RectOffset(0, 0, 0, 0),
            margin = new RectOffset(0, 0, 0, 0),
            normal = new GUIStyleState()
            {
                background = null,
                textColor = Color.white
            },
            hover = new GUIStyleState()
            {
                background = null,
                textColor = Color.white
            }
        };

        public GUIStyle CellStyle { get; set; } = new GUIStyle(UIController.DefaultSkin.FindStyle("PopupWindow.Grid"))
        {
            fontSize = 12,
            normal = new GUIStyleState() {
                background = Utils.CreateSinglePixelTexture2D(new Color(1f, 1f, 1f, 0.35f)),
                textColor = Color.white
            },
            hover = new GUIStyleState() {
                background = null,
                textColor = Color.white
            },
        };

        public GUIStyle ColorableCellStyle { get; set; } = new GUIStyle(UIController.DefaultSkin.FindStyle("PopupWindow.Grid"))
        {
            fontSize = 12,
            normal = new GUIStyleState() {
                background = Utils.CreateSinglePixelTexture2D(new Color(1f, 1f, 1f, 0.65f)),
                textColor = Color.white
            },
            hover = new GUIStyleState() {
                background = Utils.CreateSinglePixelTexture2D(new Color(1f, 1f, 1f, 1f)),
                textColor = Color.white
            }
        };

        public static readonly RectOffset RectOffsetZero = new RectOffset(0, 0, 0, 0);

        public GUIStyle InlineCellContentStyle { get; set; } = new GUIStyle(UIController.DefaultSkin.FindStyle("PopupWindow.Grid"))
        {
            fontSize = 12,
            border = RectOffsetZero,
            padding = RectOffsetZero,
            margin = RectOffsetZero,
            overflow = RectOffsetZero,
            normal = new GUIStyleState() {
                background = null,
                textColor = Color.white,
            },
            hover = new GUIStyleState() {
                background = null,
                textColor = Color.white,
            },
            stretchWidth = false
        };

        public GUIStyle RowHeaderStyle { get; set; } = new GUIStyle(UIController.DefaultSkin.FindStyle("PopupWindow.Grid"))
        {
            fontSize = 12,
            alignment = TextAnchor.LowerRight,
            padding = new RectOffset(0, 8, 4, 4),
            normal = new GUIStyleState()
            {
                background = null,
                textColor = Color.white
            }
        };

        public GUIStyle CellButtonStyle { get; set; } = new GUIStyle(UIController.DefaultSkin.button) {
            fontSize = 12,
            margin = UIController.DefaultSkin.FindStyle("PopupWindow.Grid").margin,
            padding = UIController.DefaultSkin.FindStyle("PopupWindow.Grid").padding,
            normal = new GUIStyleState() {
                background = Utils.CreateSinglePixelTexture2D(new Color32(85, 136, 254, 150)),
                textColor = Color.white
            },
            hover = new GUIStyleState() {
                background = Utils.CreateSinglePixelTexture2D(new Color32(85, 136, 254, 230)),
                textColor = Color.white
            },
            active = new GUIStyleState() {
                background = Utils.CreateSinglePixelTexture2D(new Color(1f, 1f, 1f, 0.35f)),
                textColor = new Color32(40, 86, 240, 255)
            }
        };

        public DrawableGrid<T> Row(GUIStyle style, params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal(style, options);

            return this;
        }

        public DrawableGrid<T> Row(params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal(RowStyle, options);

            return this;
        }

        public DrawableGrid<T> EndRow()
        {
            GUILayout.EndHorizontal();

            return this;
        }

        public DrawableGrid<T> EmptyRow(params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal(StaticRowStyle, options);
                GUILayout.Label(" ", RowHeaderStyle);
            GUILayout.EndHorizontal();

            return this;
        }

        public DrawableGrid<T> VerticalStack(params GUILayoutOption[] options)
        {
            GUILayout.BeginVertical(StaticRowStyle, options);

            return this;
        }

        public DrawableGrid<T> EndVerticalStack(params GUILayoutOption[] options)
        {
            GUILayout.EndVertical();

            return this;
        }

        public DrawableGrid<T> Cell(string text, GUIStyle style, params GUILayoutOption[] options)
        {
            GUILayout.Label(text, style, options);

            return this;
        }

        public DrawableGrid<T> Cell(string text, params GUILayoutOption[] options)
        {
            GUILayout.Label(text, CellStyle, options);

            return this;
        }

        public DrawableGrid<T> Cell(string text, Color color, params GUILayoutOption[] options)
        {
            var prevColor = GUI.backgroundColor;
            GUI.backgroundColor = color;

            GUILayout.Label("<size=11>" + text + "</size>", CellStyle, options);

            GUI.backgroundColor = prevColor;

            return this;
        }

        public DrawableGrid<T> Cell(string text, GUIStyle style, Color color, params GUILayoutOption[] options)
        {
            var prevColor = GUI.backgroundColor;
            GUI.backgroundColor = color;

            GUILayout.Label("<size=11>" + text + "</size>", style, options);

            GUI.backgroundColor = prevColor;

            return this;
        }

        public DrawableGrid<T> RowHeader(string text, GUIStyle style, params GUILayoutOption[] options)
        {
            GUILayout.Label("<size=10><b>" + text + "</b></size>", style, options);

            return this;
        }

        public DrawableGrid<T> RowHeader(string text, params GUILayoutOption[] options)
        {
            GUILayout.Label("<size=10>" + text + "</size>", RowHeaderStyle, options);

            return this;
        }

        public virtual DrawableGrid<T> Iterate(Action<T> action) => this;
        public virtual DrawableGrid<T> Iterate(Action<T, int> action) => this;
        public virtual DrawableGrid<T> Iterate(Action<T, int, int> action) => this;

        public virtual DrawableGrid<T> CellButton(string text, Action<T> action, params GUILayoutOption[] options) => this;
        public virtual DrawableGrid<T> CellButton(string text, Action<T, int> action, params GUILayoutOption[] options) => this;
        public virtual DrawableGrid<T> CellButton(string text, Action<T, int, int> action, params GUILayoutOption[] options) => this;

        public DrawableGrid<T> DrawHorizontalLine(float alpha = 0.3f)
        {   
            Utils.DrawHorizontalLine(alpha);

            return this;
        }

        public DrawableGrid<T> DrawHorizontalLine(float alpha, float width)
        {   
            Utils.DrawHorizontalLine(alpha, width);

            return this;
        }

        public DrawableGrid<T> Space(float size)
        {   
            GUILayout.Space(size);

            return this;
        }
    }
}
