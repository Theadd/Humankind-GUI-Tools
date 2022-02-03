using System;
using Amplitude;
using Amplitude.Mercury;
using Amplitude.Mercury.Interop;
using Amplitude.Mercury.Interop.AI.Data;
using Amplitude.Mercury.Interop.AI.Entities;
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
        private int EmpireIndex { get; set; }
        private Settlement EntitySettlement { get; set; }
        private Territory EntityTerritory { get; set; }

        public HexTileType HexTile { get; private set; } = HexTileType.None;

        private HexTileType LastHexTile { get; set; } = HexTileType.None;
        private int LastTileIndex { get; set; }

        private static DistrictInfo InvalidDistrict { get; set; } =
            new DistrictInfo() { EmpireIndex = byte.MaxValue, TileIndex = -1 };

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

        private bool PrePaint()
        {
            var brush = LiveEditorMode.ActivePaintBrush; // ConstructibleDefinition
            var brushType = LiveEditorMode.BrushType;   // LiveBrushType

            if ((brushType | LiveBrushType.Unit) != brushType)
            {
                if (EmpireIndex != -1)
                {
                    if ((HexTile | HexTileType.Settlement) == HexTile)
                    {
                        // If Settlement is an Outpost, upgrade Outpost to city
                        if ((HexTile | HexTileType.CityCenter) != HexTile)
                            return Create(() => EvolveOutpostToCityAt(TileIndex), EvolveOutpostAction);
                    }
                    else
                    {
                        // It wasn't a Settlement, paint district or repaint a different one instead
                        if ((brushType | LiveBrushType.District) == brushType)
                            return Create(() => CreateDistrictAt(TileIndex, brush.Name), CreateDistrictAction);
                    }
                }
                else
                {
                    // Over an unclaimed territory, let's create an outpost
                    return Create(() => CreateCampAt(TileIndex), CreateCampAction);
                }
            }
            else
            {
                // brushType is Unit
                
                // If there's no army on that tile, create it
                if (Tile.Army == null)
                    return Create(() => CreateArmyAt(TileIndex, brush.Name), CreateArmyAction);

                // There's already an army there, add unit to army instead.
                return Create(() => AddUnitToArmy(TileIndex, brush.Name), AddUnitToArmyAction);
            }

            return false;
        }
        
        private bool PreErase()
        {
            if (HexTile != HexTileType.None)
            {
                if ((HexTile | HexTileType.Army) == HexTile)
                {
                    return Destroy(() => RemoveUnitFromArmyAt(TileIndex), RemoveUnitAction, true);
                }
                else
                {
                    if ((HexTile | HexTileType.AdministrativeCenter) == HexTile)
                        return Destroy(() => DetachTerritoryFromCity(EntitySettlement.TileIndex, TileIndex), DetachTerritoryAction);
                    
                    if ((HexTile | HexTileType.Settlement) == HexTile)
                    {
                        return Destroy(() => DestroySettlementAt(TileIndex), DestroySettlementAction);
                    }
                    else if ((HexTile | HexTileType.District) == HexTile)
                    {
                        return Destroy(() => DestroyDistrictAt(TileIndex), DestroyDistrictAction);
                    }
                }
            }

            return false;
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
            EmpireIndex = -1;
            EntitySettlement = null;
            EntityTerritory = null;
            District = InvalidDistrict;

            if (LastTileIndex != TileIndex)
                LastTileIndex = 0;
            
            if (TryGetEntitiesAt(TileIndex, out int empireIndex, out Settlement settlement, out Territory territory))
            {
                EmpireIndex = empireIndex;
                EntitySettlement = settlement;
                EntityTerritory = territory;

                if (EntitySettlement.Empire is MajorEmpire)
                    HexTile |= HexTileType.MajorEmpire;
                
                if (EntitySettlement.Empire is MinorEmpire)
                    HexTile |= HexTileType.MinorEmpire;
                
                if (ActionController.TryGetDistrictInfoAt(WorldPosition, out DistrictInfo district))
                {
                    District = district;
                    var districtName = district.DistrictDefinitionName.ToString();
                    
                    // Loggr.Log(district.DistrictDefinitionName.ToString(), ConsoleColor.Magenta);
                    
                    if (districtName == "Extension_Base_AdministrativeCenter")
                        HexTile |= HexTileType.AdministrativeCenter;

                    if (district.DistrictDefinitionName == PresentationDistrict.CampDistrictDefinition ||
                        district.DistrictDefinitionName == PresentationDistrict.BeforeCampDistrictDefinition)
                        HexTile |= HexTileType.Settlement;

                    else if (district.DistrictDefinitionName == PresentationDistrict.CityCenterDistrictDefinition)
                        HexTile |= HexTileType.Settlement | HexTileType.CityCenter;
                    else
                        HexTile |= HexTileType.District;
                }
            }

            if (Tile.Army != null)
                HexTile |= HexTileType.Army;

            if (!PrePaint())
            {
                OnCreate = null;
                ActionNameOnCreate = string.Empty;
            }
            if (!PreErase())
            {
                OnDestroy = null;
                ActionNameOnDestroy = string.Empty;
            }
            // Loggr.Log("ACTIONS: " + ActionNameOnCreate + " / " + ActionNameOnDestroy, ConsoleColor.Magenta);
        }

    }
}
