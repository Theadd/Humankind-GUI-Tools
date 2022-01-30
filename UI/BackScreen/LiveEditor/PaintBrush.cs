using System;
using Amplitude;
using Amplitude.Mercury;
using Amplitude.Mercury.Interop;
using Amplitude.Mercury.Interop.AI.Data;
using Amplitude.Mercury.Presentation;
using Amplitude.Mercury.Sandbox;
using Modding.Humankind.DevTools;
using Snapshots = Amplitude.Mercury.Interop.AI.Snapshots;

namespace DevTools.Humankind.GUITools.UI
{
    public partial class PaintBrush : ITileEx
    {
        public Tile Tile { get; set; }
        public int TileIndex { get; set; }
        private WorldPosition WorldPosition { get; set; }
        private DistrictInfo District { get; set; }

        public HexTileType HexTile { get; private set; } = HexTileType.None;

        private HexTileType LastHexTile { get; set; } = HexTileType.None;
        private int LastTileIndex { get; set; }

        public void Paint()
        {
            OnCreate?.Invoke();
            OnCreate = null;
            ActionNameOnCreate = string.Empty;
            UpdateTile();
        }

        public void Erase()
        {
            OnDestroy?.Invoke();
            OnDestroy = null;
            ActionNameOnDestroy = string.Empty;
            UpdateTile();
        }

        private void PrePaint()
        {
            var brush = LiveEditorMode.ActivePaintBrush; // ConstructibleDefinition
            var brushType = LiveEditorMode.BrushType;   // LiveBrushType

            if ((brushType | LiveBrushType.Unit) != brushType)
            {
                if ((HexTile | HexTileType.Settlement) == HexTile)
                {
                    if ((HexTile | HexTileType.CityCenter) != HexTile)
                    {
                        // Settlement is an Outpost, then upgrade Outpost to city
                        Create(() => EvolveOutpostToCityAt(TileIndex), EvolveOutpostAction);
                    }
                }
                else
                {
                    // It wasn't a Settlement, paint district or repaint a different one instead
                    if ((brushType | LiveBrushType.District) == brushType)
                    {
                        Create(() => CreateDistrictAt(TileIndex, brush.Name), CreateDistrictAction);
                    }
                }
            }
            else
            {
                // brushType is Unit
                if (Tile.Army == null)
                {
                    Create(() => CreateArmyAt(TileIndex, brush.Name), CreateArmyAction);
                }
                else
                {
                    Create(() => AddUnitToArmy(TileIndex, brush.Name), AddUnitToArmyAction);
                }
            }
        }
        
        private void PreErase()
        {
            if (HexTile != HexTileType.None)
            {
                if ((HexTile | HexTileType.Army) == HexTile)
                {
                    Destroy(() => RemoveUnitFromArmyAt(TileIndex), RemoveUnitAction, true);
                }
                else
                {
                    if ((HexTile | HexTileType.Settlement) == HexTile)
                    {
                        Destroy(() => DestroySettlementAt(TileIndex), DestroySettlementAction);
                    }
                    else if ((HexTile | HexTileType.District) == HexTile)
                    {
                        Destroy(() => DestroyDistrictAt(TileIndex), DestroyDistrictAction);
                    }
                }
            }
        }

        public void Debug()
        {
            Loggr.Log(Tile);
            Loggr.Log(District);
            Loggr.Log(HexTile.ToString());
            if (Tile.Army != null)
            {
                Loggr.Log(Tile.Army);
                PresentationArmy armyAtPosition = Presentation
                    .PresentationEntityFactoryController
                    .GetArmyAtPosition(Presentation.PresentationCursorController.CurrentHighlightedPosition) as PresentationArmy;
                
                Loggr.Log(armyAtPosition);
            }
        }

        public void UpdateTile()
        {
            TileIndex = LiveEditorMode.HexPainter.TileIndex;
            Tile = Snapshots.World.Tiles[TileIndex];
            WorldPosition = Tile.WorldPosition;
            HexTile = HexTileType.None;

            if (LastTileIndex != TileIndex)
                LastTileIndex = 0;
            
            if (ActionController.TryGetDistrictInfoAt(WorldPosition, out DistrictInfo district))
            {
                District = district; 
                
                if (district.DistrictDefinitionName == PresentationDistrict.CampDistrictDefinition ||
                    district.DistrictDefinitionName == PresentationDistrict.BeforeCampDistrictDefinition)
                    HexTile |= HexTileType.Settlement;
                
                else if (district.DistrictDefinitionName == PresentationDistrict.CityCenterDistrictDefinition)
                    HexTile |= HexTileType.Settlement | HexTileType.CityCenter;
                else
                    HexTile |= HexTileType.District;
            }

            if (Tile.Army != null)
                HexTile |= HexTileType.Army;
            
            PrePaint();
            PreErase();
        }

    }
}
