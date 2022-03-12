using Amplitude.Mercury.Presentation;
using System;
using UnityEngine;

namespace DevTools.Humankind.GUITools.UI
{
    public class HoveredHexPainter : HexOverlay
    {
        public Color HexColor { get; set; } = new Color(1f, 1f, 1f, 0.3f);

        public override int GetNextTileIndex() =>
            Presentation.PresentationCursorController.CurrentHighlightedPosition.ToTileIndex();

        protected override void Repaint()
        {
            HexDrawingService.SetHexagonBorderColor(DebugLayerID, OffsetCoords, Color.green);
            HexDrawingService.SetHexagonInnerBorderWidth(DebugLayerID, OffsetCoords, 1f);
            HexDrawingService.SetHexagonColor(
                DebugLayerID,
                OffsetCoords,
                HexColor);
        }

        public HoveredHexPainter(Action onTileChange) : base(onTileChange)
        {
        }
    }
}
