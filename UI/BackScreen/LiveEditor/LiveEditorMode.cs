using System;
using System.Linq;
using Amplitude;
using Amplitude.Mercury.Data.Simulation;
using BepInEx.Configuration;
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
        Settlement
    }
    
    public static class LiveEditorMode
    {
        public static readonly string ToolboxPreviewActionName = "ShowToolboxWhileHoldingKey";
        public static readonly string StickedToolboxActionName = "ToggleStickyToolbox";
        public static readonly string CreateUnderCursorActionName = "CreateConstructibleUnderCursor";
        public static readonly string DestroyUnderCursorActionName = "DestroyAnythingUnderCursor";
        public static bool Enabled { get; set; } = false;
        public static EditorModeType EditorMode { get; set; } = EditorModeType.TilePainter;

        public static ConstructibleDefinition ActivePaintBrush { get; private set; } = null;
        public static LiveBrushType BrushType { get; private set; } = LiveBrushType.None;

        private static KeyboardShortcut CreateKey { get; set; }
        private static KeyboardShortcut DestroyKey { get; set; }

        public static KeyboardShortcut TestKey { get; set; } = new KeyboardShortcut(KeyCode.F5);

        public static void UpdateKeyMappings()
        {
            ToolboxController.ToolboxPreviewKey =
                KeyMappings.Keys.First(map => map.ActionName == ToolboxPreviewActionName).Key;
            ToolboxController.StickedToolboxKey =
                KeyMappings.Keys.First(map => map.ActionName == StickedToolboxActionName).Key;
            
            CreateKey = KeyMappings.Keys.First(map => map.ActionName == CreateUnderCursorActionName).Key;
            DestroyKey = KeyMappings.Keys.First(map => map.ActionName == DestroyUnderCursorActionName).Key;
        }

        public static void UpdatePaintBrush()
        {
            var gridCell = ToolboxController.Toolbox.ConstructiblesGrid.VirtualGrid.Cursor.SelectedCell;

            if (gridCell != null && gridCell.Cell is Clickable4xCell cell)
            {
                ActivePaintBrush = UIController.GameUtils.GetConstructibleDefinition(new StaticString(cell.UniqueName));
            }
            else
            {
                ActivePaintBrush = null;
            }
            UpdateBrushType();
        }

        public static void Run()
        {
            if (!Enabled)
                return;

            if (TestKey.IsPressed())
            {
                Loggr.Log(DestroyKey.Serialize() + " F5 IS PRESSED! " + CreateKey.Serialize(), ConsoleColor.Magenta);
            }
            if (EditorMode == EditorModeType.TilePainter)
            {
                if (CreateKey.IsDown())
                {
                    Loggr.Log("CREATE KEY DOWN");
                }

                if (DestroyKey.IsPressed())
                {
                    Loggr.Log("DESTROY KEY PRESSED");
                }
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
