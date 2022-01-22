using System;
using UnityEngine;

namespace StyledGUI
{
    public static class Styles
    {
        public static GUIStyle RowStyle { get; set; } =
            new GUIStyle(StyledGUIUtility.DefaultSkin.FindStyle("PopupWindow.Row"))
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

        public static GUIStyle StaticRowStyle { get; set; } =
            new GUIStyle(StyledGUIUtility.DefaultSkin.FindStyle("PopupWindow.Row"))
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

        public static GUIStyle CenteredStaticRowStyle { get; set; } =
            new GUIStyle(StyledGUIUtility.DefaultSkin.FindStyle("PopupWindow.Row"))
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

        public static GUIStyle CellStyle { get; set; } =
            new GUIStyle(StyledGUIUtility.DefaultSkin.FindStyle("PopupWindow.Grid"))
            {
                fontSize = 12,
                normal = new GUIStyleState()
                {
                    background = Graphics.CreateSinglePixelTexture2D(new Color(1f, 1f, 1f, 0.35f)),
                    textColor = Color.white
                },
                hover = new GUIStyleState()
                {
                    background = null,
                    textColor = Color.white
                },
            };

        public static GUIStyle ColorableCellStyle { get; set; } =
            new GUIStyle(StyledGUIUtility.DefaultSkin.FindStyle("PopupWindow.Grid"))
            {
                fontSize = 12,
                normal = new GUIStyleState()
                {
                    background = Graphics.CreateSinglePixelTexture2D(new Color(1f, 1f, 1f, 0.65f)),
                    textColor = Color.white
                },
                hover = new GUIStyleState()
                {
                    background = Graphics.CreateSinglePixelTexture2D(new Color(1f, 1f, 1f, 1f)),
                    textColor = Color.white
                }
            };

        public static readonly RectOffset RectOffsetZero = new RectOffset(0, 0, 0, 0);

        public static GUIStyle InlineCellContentStyle { get; set; } =
            new GUIStyle(StyledGUIUtility.DefaultSkin.FindStyle("PopupWindow.Grid"))
            {
                fontSize = 12,
                border = RectOffsetZero,
                padding = RectOffsetZero,
                margin = RectOffsetZero,
                overflow = RectOffsetZero,
                normal = new GUIStyleState()
                {
                    background = null,
                    textColor = Color.white,
                },
                hover = new GUIStyleState()
                {
                    background = null,
                    textColor = Color.white,
                },
                stretchWidth = false
            };

        public static GUIStyle RowHeaderStyle { get; set; } =
            new GUIStyle(StyledGUIUtility.DefaultSkin.FindStyle("PopupWindow.Grid"))
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

        public static GUIStyle CellButtonStyle { get; set; } = new GUIStyle(StyledGUIUtility.DefaultSkin.button)
        {
            fontSize = 12,
            margin = StyledGUIUtility.DefaultSkin.FindStyle("PopupWindow.Grid").margin,
            padding = StyledGUIUtility.DefaultSkin.FindStyle("PopupWindow.Grid").padding,
            normal = new GUIStyleState()
            {
                background = Graphics.CreateSinglePixelTexture2D(new Color32(85, 136, 254, 150)),
                textColor = Color.white
            },
            hover = new GUIStyleState()
            {
                background = Graphics.CreateSinglePixelTexture2D(new Color32(85, 136, 254, 230)),
                textColor = Color.white
            },
            active = new GUIStyleState()
            {
                background = Graphics.CreateSinglePixelTexture2D(new Color(1f, 1f, 1f, 0.35f)),
                textColor = new Color32(40, 86, 240, 255)
            }
        };
        
        public static GUIStyle ToggleCaptureStyle { get; set; } = new GUIStyle(StyledGUIUtility.DefaultSkin.toggle) {
            fontSize = 12,
            margin = StyledGUIUtility.DefaultSkin.FindStyle("PopupWindow.Grid").margin,
            padding = StyledGUIUtility.DefaultSkin.FindStyle("PopupWindow.Grid").padding,
            normal = new GUIStyleState()
            {
                background = Graphics.CreateSinglePixelTexture2D(new Color(1f, 1f, 1f, 0.35f)),
                textColor = new Color32(80, 230, 80, 220)
            },
            /*normal = new GUIStyleState() {
                background = StyledGUIUtility.DefaultSkin.toggle.hover.background,
                textColor = new Color32(20, 20, 20, 255)
            },
            hover = new GUIStyleState() {
                background = StyledGUIUtility.DefaultSkin.toggle.active.background,
                textColor = new Color32(20, 20, 20, 255)
            },*/
            hover = new GUIStyleState() {
                background = StyledGUIUtility.DefaultSkin.toggle.hover.background,
                textColor = new Color32(200, 200, 200, 255)
            },
            active = new GUIStyleState() {
                background = StyledGUIUtility.DefaultSkin.textField.onNormal.background,
                textColor = new Color32(250, 250, 250, 255)
            },
            onNormal = new GUIStyleState() {
                background = StyledGUIUtility.DefaultSkin.textField.onNormal.background,
                textColor = new Color32(250, 250, 250, 255)
            },
            onHover = new GUIStyleState() {
                background = StyledGUIUtility.DefaultSkin.textField.onNormal.background,
                textColor = new Color32(250, 250, 250, 255)
            },
            onActive = new GUIStyleState() {
                background = StyledGUIUtility.DefaultSkin.textField.onNormal.background,
                textColor = new Color32(250, 250, 250, 255)
            },
        };
    }
}
