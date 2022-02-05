// using Amplitude.Mercury.Simulation;
using Modding.Humankind.DevTools;
using System.Linq;
using System.Reflection;
using Amplitude.Framework.Simulation;
using Amplitude.Mercury.Sandbox;
using Amplitude.Mercury.Simulation;
using Amplitude.Serialization;
using Modding.Humankind.DevTools.Core;
using MajorEmpire = Amplitude.Mercury.Interop.AI.Entities.MajorEmpire;
using MinorEmpire = Amplitude.Mercury.Interop.AI.Entities.MinorEmpire;
using Settlement = Amplitude.Mercury.Interop.AI.Entities.Settlement;
using Territory = Amplitude.Mercury.Interop.AI.Entities.Territory;

namespace DevTools.Humankind.GUITools.UI
{
    public partial class PaintBrush
    {
        
        public static bool TryGetEntitiesAt(int tileIndex, out int empireIndex, out Settlement settlement, out Territory territory)
        {
            empireIndex = -1;
            settlement = null;
            territory = null;
            
            var empires = HumankindGame.GetAllEmpireEntities();

            for (var indexOfEmpire = 0; indexOfEmpire < empires.Length; indexOfEmpire++)
            {
                ref var empire = ref empires[indexOfEmpire];

                if (empire is MajorEmpire major)
                {
                    for (var settlementIndex = 0; settlementIndex < major.Settlements.Length; settlementIndex++)
                    {
                        foreach (var t in major.Settlements[settlementIndex].Territories)
                        {
                            if (t.TileIndexes.Contains(tileIndex))
                            {
                                territory = t;
                                settlement = major.Settlements[settlementIndex];
                                empireIndex = indexOfEmpire;
                                
                                return true;
                            }
                        }
                    }
                }
                else if (empire is MinorEmpire minor)
                {
                    for (var settlementIndex = 0; settlementIndex < minor.Settlements.Length; settlementIndex++)
                    {
                        foreach (var t in minor.Settlements[settlementIndex].Territories)
                        {
                            if (t.TileIndexes.Contains(tileIndex))
                            {
                                territory = t;
                                settlement = minor.Settlements[settlementIndex];
                                empireIndex = indexOfEmpire;
                                
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        private static readonly FieldInfo WorldTerritories =
            typeof(Amplitude.Mercury.Simulation.World).GetField("Territories", R.NonPublicInstance);
        
        private static readonly FieldInfo RegionSettlement =
            typeof(Amplitude.Mercury.Simulation.Region).GetField("Settlement", R.NonPublicInstance);

        private static readonly FieldInfo SettlementEmpire =
            typeof(Amplitude.Mercury.Simulation.Settlement).GetField("Empire", R.NonPublicInstance);

        private static readonly FieldInfo SettlementEmpireIndex =
            typeof(Amplitude.Mercury.Simulation.Empire).GetField("Index", R.NonPublicInstance);

        
        // public static bool TryGetSimulationTerritoryAt(int tileIndex, out Amplitude.Mercury.Simulation.Territory territory)
        
        public static bool TryGetSimulationTerritoryAt(int tileIndex, out ISerializable territory)
        {
            var territories = (Amplitude.Mercury.Simulation.Territory[]) WorldTerritories.GetValue(Sandbox.World);
            territory = null;

            for (var i = 0; i < territories.Length; i++)
            {
                if (territories[i].TileIndexes.Contains(tileIndex))
                {
                    territory = territories[i];
                    return true;
                }
            }
            
            return false;
        }
        
        public static bool TryGetSimulationEntitiesAt(
            int tileIndex, 
            out int empireIndex,
            out ISerializable territory,
            out ISerializable empire,
            out ISerializable settlement,
            out ISerializable district,
            out ISerializable army)
        {
            empireIndex = -1;
            settlement = null;
            empire = null;
            territory = null;
            army = null;
            district = null;

            if (TryGetSimulationTerritoryAt(tileIndex, out territory))
            {
                if (territory is Amplitude.Mercury.Simulation.Territory _territory)
                {
                    // ARMY on tileIndex
                    for (var i = 0; i < _territory.Armies.Count; i++)
                    {
                        if ((_territory.Armies[i] as IPathfindContextProvider).WorldPosition.ToTileIndex() == tileIndex)
                        {
                            army = _territory.Armies[i];
                            break;
                        }
                    }

                    // DISTRICT on tileIndex
                    for (var i = 0; i < _territory.Districts.Count; i++)
                    {
                        if (_territory.Districts[i].WorldPosition.ToTileIndex() == tileIndex)
                        {
                            district = _territory.Districts[i];
                            break;
                        }
                    }

                    Amplitude.Mercury.Simulation.Region region = _territory.Region.Entity;
                    if (region != null)
                    {
                        var regionSettlement =
                            (Reference<Amplitude.Mercury.Simulation.Settlement>) RegionSettlement.GetValue(region);
                        if (regionSettlement.Entity != null)
                        {
                            settlement = regionSettlement.Entity;
                            var settlementEmpire =
                                (Reference<Amplitude.Mercury.Simulation.Empire>) SettlementEmpire.GetValue(settlement);
                            if (settlementEmpire.Entity != null)
                            {
                                empire = settlementEmpire.Entity;
                                empireIndex = (int) SettlementEmpireIndex.GetValue(empire);
                            }
                        }
                    }

                    return true;
                }
            }
            
            return false;
        }
        
    }
}
