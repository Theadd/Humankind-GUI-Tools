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
        public static DataTypeDefinitionsToolbox Toolbox { get; set; }
        public static KeyboardShortcut ToolboxPreviewKey { get; set; } = new KeyboardShortcut(KeyCode.LeftControl);
        public static KeyboardShortcut StickedToolboxKey { get; set; } = new KeyboardShortcut(KeyCode.Space, KeyCode.LeftControl);
        public static bool IsVisible { get; private set; } = false;
        public static bool IsSticked { get; set; } = true;
        public static bool IsDocked { get; private set; } = true;
        public static RectOffset DockedMargin { get; set; } = new RectOffset(-2, 0, -2, -2);
        public static float PanelWidth { get; set; } = 400f;
        private static BackScreenWindow _backScreenWindow;

        public static void Initialize(BackScreenWindow window)
        {
            _backScreenWindow = window;
            AutoResize();
            
            DataTypeStore.Rebuild();
            
            Toolbox = new DataTypeDefinitionsToolbox()
            {
                Window = window,
                TypeDefinitionsGrid = new DataTypeDefinitionsGrid()
                {
                    Snapshot = new DataTypeStoreSnapshot()
                    {
                        Districts = DataTypeStore.Districts,
                        Units = DataTypeStore.Units,
                        Curiosities = DataTypeStore.Curiosities,
                    },
                    VirtualGrid = new VirtualGrid()
                    {
                        Grid = new DataTypeDefinitionsStyledGrid(),
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

            Toolbox.TypeDefinitionsGrid.VirtualGrid.Cursor.OnSelectionChange += LiveEditorMode.UpdatePaintBrush;
            Toolbox.TypeDefinitionsGrid.GridModeChunkSize = 5;
            IsDisplayModeGrid = true;
        }

        private static Color CellTintColorOnListMode { get; set; } = new Color(0, 0, 0, 0.6f);
        private static Color CellTintColorAltOnListMode { get; set; } = new Color(0, 0, 0, 0.2f);
        private static Color CellTintColorOnGridMode { get; set; } = new Color(1f, 1f, 1f, 0.6f);
        private static Color CellTintColorAltOnGridMode { get; set; } = new Color(1f, 1f, 1f, 0.2f);

        public static bool IsDisplayModeGrid
        {
            get => Toolbox.TypeDefinitionsGrid.VirtualGrid.Grid.DisplayMode == VirtualGridDisplayMode.Grid;
            set
            {
                Toolbox.TypeDefinitionsGrid.VirtualGrid.Grid.DisplayMode =
                    value ? VirtualGridDisplayMode.Grid : VirtualGridDisplayMode.List;
                Toolbox.TypeDefinitionsGrid.VirtualGrid.AlternateType =
                    value ? VirtualGridAlternateType.Columns : VirtualGridAlternateType.Rows;
                
                if (value)
                {
                    Toolbox.TypeDefinitionsGrid.VirtualGrid.Grid.CellTintColor = CellTintColorOnGridMode;
                    Toolbox.TypeDefinitionsGrid.VirtualGrid.Grid.CellTintColorAlt = CellTintColorAltOnGridMode;
                }
                else
                {
                    Toolbox.TypeDefinitionsGrid.VirtualGrid.Grid.CellTintColor = CellTintColorOnListMode;
                    Toolbox.TypeDefinitionsGrid.VirtualGrid.Grid.CellTintColorAlt = CellTintColorAltOnListMode;
                }
                
                Toolbox.TypeDefinitionsGrid.VirtualGrid.ExpandWidthOnSingleColumnGrid = !value;
                Toolbox.TypeDefinitionsGrid.GridModeChunkSize = Toolbox.TypeDefinitionsGrid.GridModeChunkSize * 1;
            }
        }

        public static void SetToolboxRect(Rect newRect)
        {
            ToolboxRect = newRect;
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
                
                Toolbox.Draw(ToolboxRect);
                
                return true;
            }

            if (IsVisible)
                OnHide();
            
            return false;
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
                Toolbox.TypeDefinitionsGrid.VirtualGrid.Cursor.OnSelectionChange -= LiveEditorMode.UpdatePaintBrush;
        }
    }
}
