using System;
using System.Linq;
using Amplitude;
using Amplitude.Mercury.Data.Simulation;
using BepInEx.Configuration;
using DevTools.Humankind.GUITools.UI.SceneInspector;
using Modding.Humankind.DevTools;
using Modding.Humankind.DevTools.DeveloperTools.UI;
using StyledGUI.VirtualGridElements;
using UnityEngine;

namespace DevTools.Humankind.GUITools.UI
{
    [Flags]
    public enum LiveBrushType
    {
        None = 0,
        Unit = 1,
        District = 2,
        NavalUnit = 4,
        AirUnit = 8,
        LandUnit = 16,
        MissileUnit = 32,
    }

    public enum EditorModeType
    {
        TilePainter,
        Settlement,
        Inspector
    }
    
    public static class LiveEditorMode
    {
        public static readonly string ToolboxPreviewActionName = "ShowToolboxWhileHoldingKey";
        public static readonly string StickedToolboxActionName = "ToggleStickyToolbox";
        public static readonly string CreateUnderCursorActionName = "CreateConstructibleUnderCursor";
        public static readonly string DestroyUnderCursorActionName = "DestroyAnythingUnderCursor";
        public static readonly string DebugUnderCursorActionName = "DebugTileUnderCursor";
        public static bool Enabled { get; set; } = false;
        public static EditorModeType EditorMode { get; set; } = EditorModeType.TilePainter;
        public static HexOverlay HexPainter { get; set; }
        public static ConstructibleDefinition ActivePaintBrush { get; private set; } = null;
        public static LiveBrushType BrushType { get; private set; } = LiveBrushType.None;

        private static KeyboardShortcut CreateKey { get; set; }
        private static KeyboardShortcut DestroyKey { get; set; }
        private static KeyboardShortcut DebugKey { get; set; }

        private static PaintBrush BrushPainter { get; set; }
        private static bool IsMouseOverUIControls { get; set; }

        public static void Initialize()
        {
            HexPainter = new HexOverlay(HandleOnTileChange);
            BrushPainter = new PaintBrush();
            IsMouseOverUIControls = false;
            UpdateKeyMappings();
        }

        private static void HandleOnTileChange()
        {
            BrushPainter.UpdateTile();
        }

        public static void UpdateKeyMappings()
        {
            ToolboxController.ToolboxPreviewKey =
                KeyMappings.Keys.First(map => map.ActionName == ToolboxPreviewActionName).Key;
            ToolboxController.StickedToolboxKey =
                KeyMappings.Keys.First(map => map.ActionName == StickedToolboxActionName).Key;
            
            CreateKey = KeyMappings.Keys.First(map => map.ActionName == CreateUnderCursorActionName).Key;
            DestroyKey = KeyMappings.Keys.First(map => map.ActionName == DestroyUnderCursorActionName).Key;
            DebugKey = KeyMappings.Keys.First(map => map.ActionName == DebugUnderCursorActionName).Key;
        }

        public static void UpdatePaintBrush()
        {
            var gridCell = ToolboxController.Toolbox.TypeDefinitionsGrid.VirtualGrid.Cursor.SelectedCell;

            if (gridCell != null && gridCell.Cell is Clickable4xCell cell)
            {
                ActivePaintBrush = UIController.GameUtils.GetConstructibleDefinition(new StaticString(cell.UniqueName));
            }
            else if (gridCell != null && gridCell.Cell is ClickableImageCell imageCell)
            {
                ActivePaintBrush = UIController.GameUtils.GetConstructibleDefinition(new StaticString(imageCell.UniqueName));
            }
            else
            {
                ActivePaintBrush = null;
            }
            UpdateBrushType();
        }

        private static int _dirtyBrushCounter = 1;

        public static void Run()
        {
            if (!Enabled)
                return;

            if (EditorMode == EditorModeType.TilePainter && !InGameUIController.IsMouseCovered)
            {
                if (BrushPainter.IsDirty)
                {
                    if (_dirtyBrushCounter-- == 0)
                    {
                        _dirtyBrushCounter = 1;
                        BrushPainter.UpdateTile();
                    }
                }
                
                // if (DebugKey.IsPressed()) BrushPainter.Debug();
                // if (CreateKey.IsPressed()) BrushPainter.Paint();
                // if (DestroyKey.IsPressed()) BrushPainter.Erase();
                
                if (DebugKey.IsDown()) BrushPainter.Debug();
                if (CreateKey.IsDown()) BrushPainter.Paint();
                if (DestroyKey.IsDown()) BrushPainter.Erase();
            }

            if (EditorMode == EditorModeType.Inspector /* TODO: && !InGameUIController.IsMouseCovered*/)
            {
                SceneInspectorController.Run();
            }
        }

        private static void UpdateBrushType()
        {
            BrushType = LiveBrushType.None;

            if (ActivePaintBrush == null)
                return;

            if (ActivePaintBrush is UnitDefinition)
            {
                BrushType = LiveBrushType.Unit;
                
                if (ActivePaintBrush is AirUnitDefinition) BrushType |= LiveBrushType.AirUnit;
                if (ActivePaintBrush is MissileUnitDefinition) BrushType |= LiveBrushType.MissileUnit;
                if (ActivePaintBrush is LandUnitDefinition) BrushType |= LiveBrushType.LandUnit;
                if (ActivePaintBrush is SettlerUnitDefinition) BrushType |= LiveBrushType.LandUnit;
                if (ActivePaintBrush is NavalUnitDefinition) BrushType |= LiveBrushType.NavalUnit;
                if (ActivePaintBrush is NavalTransportDefinition) BrushType |= LiveBrushType.NavalUnit;
                if (ActivePaintBrush is MissileUnitDefinition) BrushType |= LiveBrushType.MissileUnit;
                if (ActivePaintBrush is MissileUnitDefinition) BrushType |= LiveBrushType.MissileUnit;
                if (ActivePaintBrush is MissileUnitDefinition) BrushType |= LiveBrushType.MissileUnit;
                if (ActivePaintBrush is MissileUnitDefinition) BrushType |= LiveBrushType.MissileUnit;
            }
            
            if (ActivePaintBrush is DistrictDefinition)
            {
                BrushType = LiveBrushType.District;
                
                // ...
            }
        }
    }
}
