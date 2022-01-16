using System;
using UnityEngine;

namespace StyledGUI
{
    /*public interface IGridStyles : ICellSpan
    {
        
    }*/

    public interface IGridStyles : ICellSpan
    {
        GUIStyle RowStyle { get; set; }
        GUIStyle StaticRowStyle { get; set; }
        GUIStyle CenteredStaticRowStyle { get; set; }
        GUIStyle CellStyle { get; set; }
        GUIStyle ColorableCellStyle { get; set; }
        GUIStyle InlineCellContentStyle { get; set; }
        GUIStyle RowHeaderStyle { get; set; }
        GUIStyle CellButtonStyle { get; set; }
    }

    public class GridStyles<T> : CellSpanGrid, IGridStyles where T : class
    {

        public GUIStyle RowStyle { get; set; } = new GUIStyle(StyledGUIUtility.DefaultSkin.FindStyle("PopupWindow.Row"))
        {
            alignment = TextAnchor.LowerRight,
            padding = new RectOffset(0, 0, 0, 0),
            margin = new RectOffset(0, 0, 0, 0),
            hover = new GUIStyleState()
            {
                background = Graphics.CreateSinglePixelTexture2D(new Color32(85, 136, 254, 70)),
                textColor = Color.white
            }
        };

        public GUIStyle StaticRowStyle { get; set; } = new GUIStyle(StyledGUIUtility.DefaultSkin.FindStyle("PopupWindow.Row"))
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

        public GUIStyle CenteredStaticRowStyle { get; set; } = new GUIStyle(StyledGUIUtility.DefaultSkin.FindStyle("PopupWindow.Row"))
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

        public GUIStyle CellStyle { get; set; } = new GUIStyle(StyledGUIUtility.DefaultSkin.FindStyle("PopupWindow.Grid"))
        {
            fontSize = 12,
            normal = new GUIStyleState() {
                background = Graphics.CreateSinglePixelTexture2D(new Color(1f, 1f, 1f, 0.35f)),
                textColor = Color.white
            },
            hover = new GUIStyleState() {
                background = null,
                textColor = Color.white
            },
        };

        public GUIStyle ColorableCellStyle { get; set; } = new GUIStyle(StyledGUIUtility.DefaultSkin.FindStyle("PopupWindow.Grid"))
        {
            fontSize = 12,
            normal = new GUIStyleState() {
                background = Graphics.CreateSinglePixelTexture2D(new Color(1f, 1f, 1f, 0.65f)),
                textColor = Color.white
            },
            hover = new GUIStyleState() {
                background = Graphics.CreateSinglePixelTexture2D(new Color(1f, 1f, 1f, 1f)),
                textColor = Color.white
            }
        };

        public static readonly RectOffset RectOffsetZero = new RectOffset(0, 0, 0, 0);

        public GUIStyle InlineCellContentStyle { get; set; } = new GUIStyle(StyledGUIUtility.DefaultSkin.FindStyle("PopupWindow.Grid"))
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

        public GUIStyle RowHeaderStyle { get; set; } = new GUIStyle(StyledGUIUtility.DefaultSkin.FindStyle("PopupWindow.Grid"))
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

        public GUIStyle CellButtonStyle { get; set; } = new GUIStyle(StyledGUIUtility.DefaultSkin.button) {
            fontSize = 12,
            margin = StyledGUIUtility.DefaultSkin.FindStyle("PopupWindow.Grid").margin,
            padding = StyledGUIUtility.DefaultSkin.FindStyle("PopupWindow.Grid").padding,
            normal = new GUIStyleState() {
                background = Graphics.CreateSinglePixelTexture2D(new Color32(85, 136, 254, 150)),
                textColor = Color.white
            },
            hover = new GUIStyleState() {
                background = Graphics.CreateSinglePixelTexture2D(new Color32(85, 136, 254, 230)),
                textColor = Color.white
            },
            active = new GUIStyleState() {
                background = Graphics.CreateSinglePixelTexture2D(new Color(1f, 1f, 1f, 0.35f)),
                textColor = new Color32(40, 86, 240, 255)
            }
        };

        public GridStyles<T> Row(GUIStyle style, params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal(style, options);

            return this;
        }

        public GridStyles<T> Row(params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal(RowStyle, options);

            return this;
        }

        public GridStyles<T> EndRow()
        {
            GUILayout.EndHorizontal();

            return this;
        }

        public GridStyles<T> EmptyRow(params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal(StaticRowStyle, options);
                GUILayout.Label(" ", RowHeaderStyle);
            GUILayout.EndHorizontal();

            return this;
        }

        public GridStyles<T> VerticalStack(params GUILayoutOption[] options)
        {
            GUILayout.BeginVertical(StaticRowStyle, options);

            return this;
        }

        public GridStyles<T> EndVerticalStack(params GUILayoutOption[] options)
        {
            GUILayout.EndVertical();

            return this;
        }

        public GridStyles<T> Cell(string text, GUIStyle style, params GUILayoutOption[] options)
        {
            GUILayout.Label(text, style, options);

            return this;
        }

        public GridStyles<T> Cell(string text, params GUILayoutOption[] options)
        {
            GUILayout.Label(text, CellStyle, options);

            return this;
        }

        public GridStyles<T> Cell(string text, Color color, params GUILayoutOption[] options)
        {
            var prevColor = GUI.backgroundColor;
            GUI.backgroundColor = color;

            GUILayout.Label("<size=11>" + text + "</size>", CellStyle, options);

            GUI.backgroundColor = prevColor;

            return this;
        }

        public GridStyles<T> Cell(string text, GUIStyle style, Color color, params GUILayoutOption[] options)
        {
            var prevColor = GUI.backgroundColor;
            GUI.backgroundColor = color;

            GUILayout.Label("<size=11>" + text + "</size>", style, options);

            GUI.backgroundColor = prevColor;

            return this;
        }

        public GridStyles<T> RowHeader(string text, GUIStyle style, params GUILayoutOption[] options)
        {
            GUILayout.Label("<size=10><b>" + text + "</b></size>", style, options);

            return this;
        }

        public GridStyles<T> RowHeader(string text, params GUILayoutOption[] options)
        {
            GUILayout.Label("<size=10>" + text + "</size>", RowHeaderStyle, options);

            return this;
        }

        public virtual GridStyles<T> Iterate(Action<T> action) => this;
        public virtual GridStyles<T> Iterate(Action<T, int> action) => this;
        public virtual GridStyles<T> Iterate(Action<T, int, int> action) => this;

        public virtual GridStyles<T> CellButton(string text, Action<T> action, params GUILayoutOption[] options) => this;
        public virtual GridStyles<T> CellButton(string text, Action<T, int> action, params GUILayoutOption[] options) => this;
        public virtual GridStyles<T> CellButton(string text, Action<T, int, int> action, params GUILayoutOption[] options) => this;

        public GridStyles<T> DrawHorizontalLine(float alpha = 0.3f)
        {   
            Graphics.DrawHorizontalLine(alpha);

            return this;
        }

        public GridStyles<T> DrawHorizontalLine(float alpha, float width)
        {   
            Graphics.DrawHorizontalLine(alpha, width);

            return this;
        }

        public GridStyles<T> Space(float size)
        {   
            GUILayout.Space(size);

            return this;
        }
    }
}
