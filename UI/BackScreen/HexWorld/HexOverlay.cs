using System;
using Amplitude;
using Amplitude.Graphics;
using Amplitude.Mercury.AI;
using Amplitude.Mercury.Interop.AI;
using Amplitude.Mercury.Interop.AI.Data;
using Amplitude.Mercury.Presentation;
using Amplitude.Mercury.Terrain;
using UnityEngine;

namespace DevTools.Humankind.GUITools.UI
{
    public abstract class HexOverlay : ITileEx
    {
        private bool _clear = true;

        public HexOverlay(Action onTileChange)
        {
            OnTileChange = onTileChange;
        }

        public TerrainDebuggingService HexDrawingService { get; set; }
        public int DebugLayerID { get; set; }
        public bool IsVisible { get; set; } = true;
        protected int PrevTileIndex { get; set; } = 0;
        public int TileIndex { get; set; } = 0;
        public Tile Tile { get; set; }
        public Hexagon.OffsetCoords OffsetCoords { get; protected set; } = new Hexagon.OffsetCoords(0, 0);
        private Action OnTileChange { get; set; }

        public abstract int GetNextTileIndex();
        
        /// <summary>
        ///     Override if you have to clear previous painted hexagons other than the one at OffsetCoords.
        /// </summary>
        protected virtual void Clear() => HexDrawingService.ClearHexagon(DebugLayerID, OffsetCoords);

        protected abstract void Repaint();

        public void Draw()
        {
            if (HexDrawingService == null)
            {
                HexDrawingService = RenderContextAccess.GetInstance<TerrainDebuggingService>(0);
                DebugLayerID = HexDrawingService.BindDebugLayer(GetType().ToString());
            }

            if (IsVisible)
            {
                var tileIndex = GetNextTileIndex();
                ref Tile local1 = ref Snapshots.World.Tiles[tileIndex];

                DebugControl.CursorPosition = local1.WorldPosition;
                
                if (!DebugControl.CursorPosition.IsWorldPositionValid())
                    return;

                if (PrevTileIndex != tileIndex)
                {
                    TileIndex = tileIndex;
                    Tile = Snapshots.World.Tiles[tileIndex];
                    Clear();
                    OffsetCoords = local1.WorldPosition.ToHexagonOffsetCoords();
                    OnTileChange();

                    PrevTileIndex = tileIndex;

                    Repaint();

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
