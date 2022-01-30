using System;
using System.Linq;
using Amplitude.Mercury.Interop.AI.Data;
using Amplitude.Mercury.Interop;
using Amplitude.Mercury.Sandbox;
using Modding.Humankind.DevTools;
using Snapshots = Amplitude.Mercury.Interop.AI.Snapshots;
using Amplitude.Mercury.Interop.AI.Entities;
using Newtonsoft.Json.Utilities;

namespace DevTools.Humankind.GUITools.UI
{
    public interface ITileEx
    {
        int TileIndex { get; set; }
        Tile Tile { get; set; }
    }
    
    [Flags]
    public enum HexTileType
    {
        None = 0,
        Settlement = 1,
        CityCenter = 2,
        District = 4,
        Army = 8
    }

    /*public static class ITileExtensions
    {
        
        public static bool TryGetSettlement(
            this ITileEx self,
            out Settlement owner) 
        {
            owner = null;
            
            if (ActionController.TryGetDistrictInfoAt(self.Tile.WorldPosition, out DistrictInfo district))
            {
                var empire = HumankindGame.GetEmpireEntityAt(district.EmpireIndex);

                if (empire is MajorEmpire majorEmpire)
                {
                    foreach (var settlement in majorEmpire.Settlements)
                    {
                        if (settlement.Territories.Any(t => t.TileIndexes.Contains(self.TileIndex)))
                        {
                            owner = settlement;
                            return true;
                        }
                    }
                }
                else if (empire is MinorEmpire minorEmpire)
                {
                    if (minorEmpire.Settlement.Territories.Any(t => t.TileIndexes.Contains(self.TileIndex)))
                    {
                        Loggr.Log("Settlement found within MINOR EMPIRE");
                        owner = minorEmpire.Settlement;
                        
                        return true;
                    }
                }
                else if (empire is LesserEmpire lesserEmpire)
                {
                    Loggr.Log(lesserEmpire);
                    Loggr.Log("LesserEmpires does not OWN any settlement/city/territory.", ConsoleColor.Red);
                }
            }

            // Snapshots.World.Territories

            // Snapshots.GameSnapshot.PresentationData.LocalEmpireInfo.DistrictInfo

            return false;
            // return Sandbox.World.TileInfo.Data[settlementTileIndex];
        }
    }*/
}
