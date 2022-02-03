// using Amplitude.Mercury.Simulation;
using Modding.Humankind.DevTools;
using Amplitude.Mercury.Interop.AI.Entities;
using System.Linq;

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
        
    }
}
