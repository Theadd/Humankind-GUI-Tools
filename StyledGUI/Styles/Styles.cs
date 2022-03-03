using UnityEngine;

namespace StyledGUI
{
    public static partial class Styles
    {
        public static Texture2D Alpha80WhitePixel { get; set; } =
            Graphics.CreateSinglePixelTexture2D(new Color(1f, 1f, 1f, 0.80f));
        public static Texture2D Alpha65WhitePixel { get; set; } =
            Graphics.CreateSinglePixelTexture2D(new Color(1f, 1f, 1f, 0.65f));
        public static Texture2D Alpha35WhitePixel { get; set; } =
            Graphics.CreateSinglePixelTexture2D(new Color(1f, 1f, 1f, 0.35f));
        public static Texture2D Alpha35BlackPixel { get; set; } =
            Graphics.CreateSinglePixelTexture2D(new Color(0, 0, 0, 0.35f));
        public static Texture2D Alpha50BlackPixel { get; set; } =
            Graphics.CreateSinglePixelTexture2D(new Color(0, 0, 0, 0.5f));
        public static Texture2D WhitePixel { get; set; } =
            Graphics.CreateSinglePixelTexture2D(new Color(1f, 1f, 1f, 1f));
        public static Texture2D ButtonNormalPixel { get; set; } =
            Graphics.CreateSinglePixelTexture2D(new Color32(85, 136, 254, 150));
        public static Texture2D ButtonHoverPixel { get; set; } =
            Graphics.CreateSinglePixelTexture2D(new Color32(85, 136, 254, 230));
        public static Texture2D ButtonActivePixel { get; set; } =
            Graphics.CreateSinglePixelTexture2D(new Color(1f, 1f, 1f, 0.35f));
        public static Texture2D RowHoverPixel { get; set; } =
            Graphics.CreateSinglePixelTexture2D(new Color32(85, 136, 254, 70));

        public static Color GreenTextColor { get; set; } = (Color) new Color32(80, 230, 80, 220);
        public static Color WhiteTextColor { get; set; } = (Color) new Color32(250, 250, 250, 255);
        public static Color WhitePearlTextColor { get; set; } = (Color) new Color32(200, 200, 200, 255);
        public static Color DarkTextColor { get; set; } = (Color) new Color32(10, 10, 10, 190);
        public static Color BlueTextColor { get; set; } = (Color) new Color32(40, 86, 240, 255);
        public static Color GoldTextColor { get; set; } = new Color(0.85f, 0.75f, 0f, 0.85f);

        public static GUIStyle Alpha50BlackBackgroundStyle { get; set; } = new GUIStyle()
        {
            normal = new GUIStyleState()
            {
                background = Alpha50BlackPixel,
                textColor = Color.white
            }
        };
        
        public static GUIStyle Alpha65WhiteBackgroundStyle { get; set; } = new GUIStyle()
        {
            normal = new GUIStyleState()
            {
                background = Alpha65WhitePixel,
                textColor = Color.white
            }
        };
        
        public static GUIStyle SmallPaddingStyle { get; set; } = new GUIStyle()
        {
            padding = new RectOffset(8, 8, 8, 8),
        };
        
        public static GUIStyle RowStyle { get; set; } =
            new GUIStyle(StyledGUIUtility.DefaultSkin.FindStyle("PopupWindow.Row"))
            {
                alignment = TextAnchor.LowerRight,
                padding = new RectOffset(0, 0, 0, 0),
                margin = new RectOffset(0, 0, 0, 0),
                hover = new GUIStyleState()
                {
                    background = RowHoverPixel,
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
                    background = Alpha35WhitePixel,
                    textColor = Color.white
                },
                hover = new GUIStyleState()
                {
                    background = null,
                    textColor = Color.white
                },
            };

        public static GUIStyle ColorableCellToggleStyle { get; set; } = new GUIStyle(StyledGUIUtility.DefaultSkin.button)
        {
            fontSize = 12,
            margin = StyledGUIUtility.DefaultSkin.FindStyle("PopupWindow.Grid").margin,
            padding = StyledGUIUtility.DefaultSkin.FindStyle("PopupWindow.Grid").padding,
            normal = new GUIStyleState()
            {
                background = Alpha65WhitePixel,
                textColor = Color.white
            },
            hover = new GUIStyleState()
            {
                background = ButtonHoverPixel,
                textColor = Color.white
            },
            active = new GUIStyleState()
            {
                background = ButtonActivePixel,
                textColor = BlueTextColor
            },
            onNormal = new GUIStyleState()
            {
                background = ButtonNormalPixel,
                textColor = Color.white
            }
        };
        
        public static GUIStyle ColorableCellStyle { get; set; } =
            new GUIStyle(StyledGUIUtility.DefaultSkin.FindStyle("PopupWindow.Grid"))
            {
                fontSize = 12,
                normal = new GUIStyleState()
                {
                    background = Alpha65WhitePixel,
                    textColor = Color.white
                },
                hover = new GUIStyleState()
                {
                    background = WhitePixel,
                    textColor = Color.white
                }
            };
        
        public static readonly RectOffset RectOffsetZero = new RectOffset(0, 0, 0, 0);

        public static GUIStyle CellImageStyle { get; set; } = new GUIStyle(ColorableCellStyle)
        {
            padding = RectOffsetZero,
            normal = new GUIStyleState()
            {
                background = Alpha35BlackPixel,
                textColor = Color.white
            },
            hover = new GUIStyleState()
            {
                background = ButtonHoverPixel,
                textColor = Color.white
            },
        };
        
        public static GUIStyle ActiveCellImageStyle { get; set; } = new GUIStyle(ColorableCellStyle)
        {
            padding = RectOffsetZero,
        };
        
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
                background = ButtonNormalPixel,
                textColor = Color.white
            },
            hover = new GUIStyleState()
            {
                background = ButtonHoverPixel,
                textColor = Color.white
            },
            active = new GUIStyleState()
            {
                background = ButtonActivePixel,
                textColor = BlueTextColor
            }
        };
        
        public static GUIStyle ToggleCaptureStyle { get; set; } = new GUIStyle(StyledGUIUtility.DefaultSkin.toggle) {
            fontSize = 12,
            margin = StyledGUIUtility.DefaultSkin.FindStyle("PopupWindow.Grid").margin,
            padding = StyledGUIUtility.DefaultSkin.FindStyle("PopupWindow.Grid").padding,
            normal = new GUIStyleState()
            {
                background = Alpha35WhitePixel,
                textColor = GreenTextColor
            },
            hover = new GUIStyleState() {
                background = StyledGUIUtility.DefaultSkin.toggle.hover.background,
                textColor = WhitePearlTextColor
            },
            active = new GUIStyleState() {
                background = StyledGUIUtility.DefaultSkin.textField.onNormal.background,
                textColor = WhiteTextColor
            },
            onNormal = new GUIStyleState() {
                background = StyledGUIUtility.DefaultSkin.textField.onNormal.background,
                textColor = WhiteTextColor
            },
            onHover = new GUIStyleState() {
                background = StyledGUIUtility.DefaultSkin.textField.onNormal.background,
                textColor = WhiteTextColor
            },
            onActive = new GUIStyleState() {
                background = StyledGUIUtility.DefaultSkin.textField.onNormal.background,
                textColor = WhiteTextColor
            },
        };
        
        public static GUIStyle Fixed20pxHeightTextStyle { get; set; } =
            new GUIStyle(StyledGUIUtility.DefaultSkin.FindStyle("PopupWindow.Grid"))
            {
                fontSize = 12,
                border = RectOffsetZero,
                padding = new RectOffset(6, 6, 0, 0),
                margin = RectOffsetZero,
                overflow = RectOffsetZero,
                fixedHeight = 20f,
                alignment = TextAnchor.MiddleLeft,
                richText = true,
                wordWrap = false,
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
    }
}
