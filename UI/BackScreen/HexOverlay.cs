using Amplitude.Graphics;
using Amplitude.Mercury.AI;
using Amplitude.Mercury.Terrain;
using Amplitude.Mercury.Interop.AI.Data;
using Amplitude.Mercury.Presentation;
using Amplitude.Mercury.UI;
using Amplitude.Mercury.Interop.AI;
using UnityEngine;

namespace DevTools.Humankind.GUITools.UI
{
    public class HexOverlay
    {
        public TerrainDebuggingService HexDrawingService { get; set; }
        public int DebugLayerID { get; set; }

        public bool IsVisible { get; set; } = true;

        private int PrevTileIndex { get; set; } = 0;

        public Color HexColor { get; set; } = new Color(1f, 1f, 1f, 0.3f);

        public HexOverlay() { }

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
                    HexDrawingService.ClearHexagons(DebugLayerID);
                    PrevTileIndex = tileIndex;
                }
                
                HexDrawingService.SetHexagonColor(
                    DebugLayerID, 
                    local1.WorldPosition.ToHexagonOffsetCoords(),
                    HexColor);
                
                HexDrawingService.DisplayDebugLayer(DebugLayerID);

                return;
            }
            
            HexDrawingService.ClearHexagons(DebugLayerID);
        }
    }
}
