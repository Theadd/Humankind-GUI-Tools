using System;
using Amplitude;
using Amplitude.Mercury;
using Amplitude.Mercury.Data.Simulation;
using Amplitude.Mercury.Interop;
using Amplitude.Mercury.Interop.AI.Data;
using Amplitude.Mercury.Interop.AI.Entities;
using Amplitude.Mercury.Presentation;
using Amplitude.Mercury.Sandbox;
using Amplitude.Serialization;
using Modding.Humankind.DevTools;
using Snapshots = Amplitude.Mercury.Interop.AI.Snapshots;
using String = System.String;

namespace DevTools.Humankind.GUITools.UI
{
    public partial class PaintBrush : ITileEx
    {
        public Tile Tile { get; set; }
        public int TileIndex { get; set; }
        private WorldPosition WorldPosition { get; set; }
        // private DistrictInfo District { get; set; }
        private int EmpireIndex { get; set; }
        // private Settlement EntitySettlement { get; set; }
        // private Territory EntityTerritory { get; set; }
        private Amplitude.Mercury.Simulation.Settlement SimulationEntitySettlement { get; set; }
        private ResourceType ResourceType { get; set; }
        private string ResourceNum { get; set; }

        public HexTileType HexTile { get; private set; } = HexTileType.None;

        private HexTileType LastHexTile { get; set; } = HexTileType.None;
        private int LastTileIndex { get; set; }

        public bool IsDirty { get; private set; } = false;
        
        public void Paint()
        {
            OnCreate?.Invoke();
            OnCreate = null;
            ActionNameOnCreate = string.Empty;
            IsDirty = true;
        }

        public void Erase()
        {
            OnDestroy?.Invoke();
            OnDestroy = null;
            ActionNameOnDestroy = string.Empty;
            IsDirty = true;
        }

        private bool PrePaint()
        {
            var brush = LiveEditorMode.ActivePaintBrush; // ConstructibleDefinition
            var brushType = LiveEditorMode.BrushType;   // LiveBrushType

            if ((brushType | LiveBrushType.Unit) != brushType)
            {
                if ((HexTile | HexTileType.Mountain) == HexTile)
                    return false;

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
                        if ((HexTile | HexTileType.Resource) == HexTile)
                        {
                            if ((HexTile | HexTileType.Luxury) == HexTile)
                                if ((HexTile | HexTileType.District) == HexTile)
                                    if ((HexTile | HexTileType.Manufactory) != HexTile)
                                        return Create(() => CreateLuxuryManufactoryAt(TileIndex, ResourceNum), CreateLuxuryManufactoryAction);
                                    else
                                        return false;
                                else
                                    return Create(() => CreateLuxuryResourceExtractorAt(TileIndex), CreateLuxuryExtractorAction);
                    
                            if ((HexTile | HexTileType.Strategic) == HexTile)
                                if ((HexTile | HexTileType.District) != HexTile)
                                    return Create(() => CreateStrategicResourceExtractorAt(TileIndex, ResourceNum), CreateStrategicExtractorAction);
                                else
                                    return false;
                        }
                        
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
                if ((HexTile | HexTileType.Army) != HexTile)
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
                    return Destroy(() => RemoveUnitFromArmyAt(TileIndex), 
                        (Tile.Army?.Units.Length == 1) ? RemoveArmyAction : RemoveUnitAction);
                }
                else
                {
                    if ((HexTile | HexTileType.AdministrativeCenter) == HexTile)
                        return Destroy(() => DetachTerritoryFromCity(SimulationEntitySettlement.WorldPosition.ToTileIndex(), TileIndex), DetachTerritoryAction);
                    
                    if ((HexTile | HexTileType.Settlement) == HexTile)
                    {
                        if ((HexTile | HexTileType.CityCenter | HexTileType.MinorEmpire) == HexTile)
                            return Destroy(() => RemoveMinorEmpire(EmpireIndex), RemoveMinorEmpireAction);
                        else    
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
            var found = TryGetSimulationEntitiesAt(
                TileIndex,
                out int empireIndex,
                out ISerializable sTerritory,
                out ISerializable sEmpire,
                out ISerializable sSettlement,
                out ISerializable sDistrict,
                out ISerializable sArmy);

            var territory = sTerritory as Amplitude.Mercury.Simulation.Territory;
            var empire = sEmpire as Amplitude.Mercury.Simulation.Empire;
            var settlement = sSettlement as Amplitude.Mercury.Simulation.Settlement;
            var district = sDistrict as Amplitude.Mercury.Simulation.District;
            var army = sArmy as Amplitude.Mercury.Simulation.Army;
            
            Loggr.Log(territory, ConsoleColor.DarkBlue);
            Loggr.Log(empire, ConsoleColor.DarkBlue);
            Loggr.Log(settlement, ConsoleColor.DarkBlue);
            Loggr.Log(district, ConsoleColor.DarkBlue);
            Loggr.Log(army, ConsoleColor.DarkBlue);
        }

        public void UpdateTile()
        {
            TileIndex = LiveEditorMode.HexPainter.TileIndex;
            Tile = Snapshots.World.Tiles[TileIndex];
            WorldPosition = Tile.WorldPosition;
            HexTile = HexTileType.None;
            ref TileInfo tileInfo = ref Amplitude.Mercury.Interop.Snapshots.GameSnapshot.PresentationData.LocalEmpireInfo.TileInfo.Data[TileIndex];
            EmpireIndex = -1;
            SimulationEntitySettlement = null;
            IsDirty = false;
            string districtName = "";
            var data = Amplitude.Mercury.Interop.Snapshots.WorldSnapshot.PresentationData;
            var resourceInfoIndex = data.FindResourceDepositInfoAt(TileIndex);
            var terrainType = (int)tileInfo.TerrainType < data.TerrainTypeDefinitions.Length ? 
                data.TerrainTypeDefinitions[(int)tileInfo.TerrainType] : null;

            
            if (LastTileIndex != TileIndex)
                LastTileIndex = 0;
            
            if (terrainType != null && terrainType.Name.ToString() == "TerrainType_Mountain")
                HexTile |= HexTileType.Mountain;
            
            if (TryGetSimulationEntitiesAt(
                TileIndex,
                out int empireIndex,
                out ISerializable sTerritory,
                out ISerializable sEmpire,
                out ISerializable sSettlement,
                out ISerializable sDistrict,
                out ISerializable sArmy))
            {
                var territory = sTerritory as Amplitude.Mercury.Simulation.Territory;
                var empire = sEmpire as Amplitude.Mercury.Simulation.Empire;
                var settlement = sSettlement as Amplitude.Mercury.Simulation.Settlement;
                var district = sDistrict as Amplitude.Mercury.Simulation.District;
                var army = sArmy as Amplitude.Mercury.Simulation.Army;
                
                EmpireIndex = empireIndex;
                SimulationEntitySettlement = settlement;

                if (empire is Amplitude.Mercury.Simulation.MajorEmpire)
                    HexTile |= HexTileType.MajorEmpire;
                
                if (empire is Amplitude.Mercury.Simulation.MinorEmpire)
                    HexTile |= HexTileType.MinorEmpire;
                
                if (resourceInfoIndex != -1)
                {
                    ResourceType = data.ResourceDepositInfo[resourceInfoIndex].Resource;
                    var resourceNum = ResourceType.ToString().Substring(ResourceType.ToString().Length - 2);
                    if (int.TryParse(resourceNum, out int num))
                    {
                        ResourceNum = resourceNum;
                        HexTile |= HexTileType.Resource;
                    
                        if (num >= 11)
                            HexTile |= HexTileType.Luxury;
                        else
                            HexTile |= HexTileType.Strategic;

                        if (district != null)
                            districtName = district.DistrictDefinition.Name.ToString().Trim();
                        
                        if (districtName == "" || districtName == "Exploitation")
                        {
                            // Unexploited resource deposit
                        }
                        else
                        {
                            if (districtName.StartsWith("Extension_Wondrous_"))
                                HexTile |= HexTileType.Manufactory;
                            
                            HexTile |= HexTileType.District;
                        }
                    }
                    Loggr.Log("RESOURCE TYPE = " + ResourceType + " [" + resourceNum + "]");
                }
                
                if (district != null)
                {
                    districtName = district.DistrictDefinition.Name.ToString();

                    if (districtName == "Extension_Base_AdministrativeCenter")
                        HexTile |= HexTileType.AdministrativeCenter;

                    if (district.DistrictDefinition.Name == PresentationDistrict.CampDistrictDefinition ||
                        district.DistrictDefinition.Name == PresentationDistrict.BeforeCampDistrictDefinition)
                        HexTile |= HexTileType.Settlement;

                    else if (district.DistrictDefinition.Name == PresentationDistrict.CityCenterDistrictDefinition)
                        HexTile |= HexTileType.Settlement | HexTileType.CityCenter;
                    else if (districtName != "Exploitation")
                        HexTile |= HexTileType.District;
                }

                if (army != null)
                    HexTile |= HexTileType.Army;
            }

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
            
            if (!Modding.Humankind.DevTools.DevTools.QuietMode)
                Loggr.Log(HexTile.ToString() + " " + TileIndex + " >> ACTIONS: " + ActionNameOnCreate + " / " + ActionNameOnDestroy + " \t\t" + districtName, ConsoleColor.Magenta);
        }

    }
}
