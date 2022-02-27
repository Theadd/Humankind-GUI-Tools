using Amplitude.Graphics;
using Amplitude.Mercury.AI;
using Amplitude.Mercury.Terrain;
using Amplitude.Mercury.Interop.AI.Data;
using Amplitude.Mercury.Presentation;
using System;
using Amplitude;
using Amplitude.Mercury;
using Amplitude.Mercury.Interop.AI;
using Modding.Humankind.DevTools;
using UnityEngine;

namespace DevTools.Humankind.GUITools.UI
{
    public class HexOverlay : ITileEx
    {
        public TerrainDebuggingService HexDrawingService { get; set; }
        public int DebugLayerID { get; set; }

        public bool IsVisible { get; set; } = true;

        private int PrevTileIndex { get; set; } = 0;

        public Color HexColor { get; set; } = new Color(1f, 1f, 1f, 0.3f);

        public int TileIndex { get; set; } = 0;
        public Tile Tile { get; set; }

        public Hexagon.OffsetCoords OffsetCoords { get; private set; } = new Hexagon.OffsetCoords(0, 0);

        private Action OnTileChange { get; set; }

        private bool _clear = true;

        public HexOverlay(Action onTileChange)
        {
            OnTileChange = onTileChange;
        }

        public void Draw()
        {
            if (HexDrawingService == null)
            {
                HexDrawingService = RenderContextAccess.GetInstance<TerrainDebuggingService>(0);
                DebugLayerID = HexDrawingService.BindDebugLayer(GetType().ToString());
            }

            if (IsVisible)
            {
                var tileIndex = Presentation.PresentationCursorController.CurrentHighlightedPosition.ToTileIndex();
                ref Tile local1 = ref Snapshots.World.Tiles[tileIndex];

                DebugControl.CursorPosition = local1.WorldPosition;
                
                if (!DebugControl.CursorPosition.IsWorldPositionValid())
                    return;

                if (PrevTileIndex != tileIndex)
                {
                    TileIndex = tileIndex;
                    Tile = Snapshots.World.Tiles[tileIndex];
                    HexDrawingService.ClearHexagon(DebugLayerID, OffsetCoords);
                    OffsetCoords = local1.WorldPosition.ToHexagonOffsetCoords();
                    OnTileChange();

                    PrevTileIndex = tileIndex;

                    HexDrawingService.SetHexagonBorderColor(DebugLayerID, OffsetCoords, Color.green);
                    HexDrawingService.SetHexagonInnerBorderWidth(DebugLayerID, OffsetCoords, 1f);
                    HexDrawingService.SetHexagonColor(
                        DebugLayerID,
                        OffsetCoords,
                        HexColor);

                    HexDrawingService.DisplayDebugLayer(DebugLayerID);
                    _clear = false;
                }
                return;
            }
            
            if (!_clear)
            {
                HexDrawingService.ClearHexagons(DebugLayerID);
                _clear = true;
            }
        }
    }
}
