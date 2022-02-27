using Amplitude.Mercury.Presentation;
using System;
using UnityEngine;

namespace DevTools.Humankind.GUITools.UI
{
    public class HexPointer : HexOverlay
    {
        private int _nextTileIndex;
        public Color HexColor { get; set; } = Color.cyan;

        public void SetNextTileIndex(int tileIndex) => _nextTileIndex = tileIndex;

        public override int GetNextTileIndex() => _nextTileIndex;

        protected override void Repaint()
        {
            HexDrawingService.SetHexagonBorderColor(DebugLayerID, OffsetCoords, Color.cyan);
            HexDrawingService.SetHexagonInnerBorderWidth(DebugLayerID, OffsetCoords, 1f);
            HexDrawingService.SetHexagonColor(
                DebugLayerID,
                OffsetCoords,
                HexColor);
        }

        public HexPointer(Action onTileChange) : base(onTileChange)
        {
        }
    }
}
