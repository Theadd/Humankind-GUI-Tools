using System.Linq;
using Amplitude.Framework.Runtime;
using BepInEx.Configuration;
using Modding.Humankind.DevTools;
using StyledGUI;
using UnityEngine;

namespace DevTools.Humankind.GUITools.UI
{
    public static class ToolboxController
    {
        public static Rect ToolboxRect { get; private set; } = Rect.zero;
        public static Rect InputFilterRect { get; private set; } = Rect.zero;
        public static ConstructiblesToolbox Toolbox { get; set; }
        public static KeyboardShortcut ToolboxPreviewKey { get; set; } = new KeyboardShortcut(KeyCode.LeftControl);
        public static KeyboardShortcut StickedToolboxKey { get; set; } = new KeyboardShortcut(KeyCode.Space, KeyCode.LeftControl);
        public static bool IsVisible { get; private set; } = false;
        public static bool IsSticked { get; set; } = true;
        public static bool IsDocked { get; private set; } = true;
        public static string InputFilter { get; set; } = string.Empty;
        public static RectOffset DockedMargin { get; set; } = new RectOffset(-2, 0, -2, -2);
        public static float PanelWidth { get; set; } = 410f;
        private static BackScreenWindow _backScreenWindow;

        public static void Initialize(BackScreenWindow window)
        {
            _backScreenWindow = window;
            AutoResize();
            
            ConstructibleStore.Rebuild();
            
            Toolbox = new ConstructiblesToolbox()
            {
                Window = window,
                ConstructiblesGrid = new ConstructiblesGrid()
                {
                    Snapshot = new ConstructibleStoreSnapshot()
                    {
                        Districts = ConstructibleStore.Districts,
                        Units = ConstructibleStore.Units
                    },
                    VirtualGrid = new VirtualGrid()
                    {
                        Grid = new ConstructiblesStyledGrid(),
                        DrawRowHeaders = false,
                        DrawSectionHeaders = true,
                        VisibleViews = new[] { 0 },
                        AlternateType = VirtualGridAlternateType.Columns,
                        Cursor = new VirtualGridCursor()
                        {
                            SelectionType = VirtualGridSelectionType.OptionalSingleCell,
                        }
                    }
                }
            };

            Toolbox.ConstructiblesGrid.VirtualGrid.Cursor.OnSelectionChange += LiveEditorMode.UpdatePaintBrush;
            Toolbox.ConstructiblesGrid.GridModeChunkSize = 5;
            IsDisplayModeGrid = true;
        }

        public static bool IsDisplayModeGrid
        {
            get => Toolbox.ConstructiblesGrid.VirtualGrid.Grid.DisplayMode == VirtualGridDisplayMode.Grid;
            set
            {
                Toolbox.ConstructiblesGrid.VirtualGrid.Grid.DisplayMode =
                    value ? VirtualGridDisplayMode.Grid : VirtualGridDisplayMode.List;
                Toolbox.ConstructiblesGrid.VirtualGrid.AlternateType =
                    value ? VirtualGridAlternateType.Columns : VirtualGridAlternateType.Rows;
                Toolbox.ConstructiblesGrid.VirtualGrid.ExpandWidthOnSingleColumnGrid = !value;
                Toolbox.ConstructiblesGrid.GridModeChunkSize = Toolbox.ConstructiblesGrid.GridModeChunkSize * 1;
            }
        }

        public static void SetToolboxRect(Rect newRect)
        {
            ToolboxRect = newRect;
            InputFilterRect = new Rect(newRect.x + 12f, newRect.y - 24f, newRect.width, 24f);
        }

        public static bool Draw()
        {
            if (Event.current.type == EventType.Repaint)
            {
                if (StickedToolboxKey.IsDown())
                {
                    IsSticked = !IsSticked;
                }
            }
            
            if (IsSticked || AreKeysHeldDown(ToolboxPreviewKey))
            {
                if (!IsVisible)
                    OnShow();
                
                if (InputFilter.Length > 0)
                    DrawInputFilter();
                
                Toolbox.Draw(ToolboxRect);
                
                if (Event.current.type == EventType.Repaint && Input.inputString.Length > 0)
                    InputFilter = (InputFilter + Input.inputString).TrimStart(' ');
                
                return true;
            }

            InputFilter = string.Empty;
            if (IsVisible)
                OnHide();
            
            return false;
        }

        private static void DrawInputFilter()
        {
            GUILayout.BeginArea(InputFilterRect);
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("<color=#000000FF><b><size=10>FILTER: </size></b></color>");
                    GUILayout.Label("<color=#000000FF><b>" + InputFilter.ToUpper() + "</b></color>");
                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndArea();
        }

        private static bool AreKeysHeldDown(KeyboardShortcut shortcut) => 
            Input.GetKey(shortcut.MainKey) && shortcut.Modifiers.Count(key => !Input.GetKey(key)) == 0;

        private static void OnShow()
        {
            AutoResize();
            
            IsVisible = true;
        }

        private static void OnHide()
        {
            IsVisible = false;
        }

        public static void AutoResize()
        {
            if (IsDocked)
            {
                SetToolboxRect(new Rect(DockedMargin.left, DockedMargin.top, PanelWidth,
                    Screen.height - (DockedMargin.top + DockedMargin.bottom)));
            }
            else if (ToolboxRect.Equals(Rect.zero))
                SetToolboxRect(new Rect(
                    300f, 220f, PanelWidth,
                    Mathf.Min(640f, Mathf.Ceil(Screen.height * 0.66f))));
        }

        public static void SetDocked(bool shouldBeDocked)
        {
            if (_backScreenWindow == null)
            {
                Loggr.Log(new RuntimeException("ToolboxController has not been initialized yet."));
                return;
            }
            IsDocked = shouldBeDocked;
            if (IsDocked)
            {
                _backScreenWindow.ScreenOverlay.InnerCanvas.CornerRadius = 0;
            }
            else
            {
                _backScreenWindow.ScreenOverlay.InnerCanvas.CornerRadius = 7f;
            }
            AutoResize();
        }

        public static void Unload()
        {
            if (Toolbox != null)
                Toolbox.ConstructiblesGrid.VirtualGrid.Cursor.OnSelectionChange -= LiveEditorMode.UpdatePaintBrush;
        }
    }
}
