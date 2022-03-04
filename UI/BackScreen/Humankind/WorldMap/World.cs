using System.Linq;
using Amplitude;
using Amplitude.Mercury.Data.World;
using Amplitude.Mercury.Simulation;
using Amplitude.Mercury.Interop.AI;
using Amplitude.Mercury.Interop.AI.Data;

using UniverseLib;
using UniverseLib.Utility;
using UniverseLib.Runtime.Mono;


namespace DevTools.Humankind.GUITools.UI
{
    public partial class World
    {
        #region WORLD
        // public static World World => Amplitude.Mercury.Sandbox.Sandbox.World;
        
        // public static River[] Rivers => World.Tables.Rivers;

        public static Amplitude.Mercury.Interop.WorldSnapshot.Data WorldSnapshotData => Amplitude.Mercury.Interop.Snapshots.WorldSnapshot.PresentationData;
        
        public static Hexagon.OffsetCoords[] SpawnLocations =>
            Amplitude.Mercury.Simulation.World.Tables.SpawnLocations;
        #endregion
        
        #region DEFINITIONS
        public static TerrainTypeDefinition[] TerrainTypeDefinitions =>
            Amplitude.Mercury.Simulation.World.Tables.TerrainTypeDefinitions;

        public static PointOfInterestDefinition[] PointOfInterestDefinitions =>
            Amplitude.Mercury.Simulation.World.Tables.PointOfInterestDefinitions;

        public static BiomeDefinition[] BiomeDefinitions =>
            Amplitude.Mercury.Simulation.World.Tables.BiomeDefinitions;

        public static NaturalWonderDefinition[] NaturalWonderDefinitions =>
            Amplitude.Mercury.Simulation.World.Tables.NaturalWonderDefinitions;
        #endregion
        
        #region SNAPSHOTS
        public static WorldSnapshot WorldSnapshot => Snapshots.World;
        public static GameSnapshot GameSnapshot => Snapshots.Game;
        
        public static DataSnapshot DataSnapshot => Snapshots.Data;
        public static TradeSnapshot TradeSnapshot => Snapshots.Trade;
        public static CultureSnapshot CultureSnapshot => Snapshots.Culture;
        public static ReligionSnapshot ReligionSnapshot => Snapshots.Religion;
        public static BattleSnapshot BattleSnapshot => Snapshots.Battle;
        public static SurrenderSnapshot SurrenderSnapshot => Snapshots.Surrender;
        public static PollutionSnapshot PollutionSnapshot => Snapshots.Pollution;
        #endregion




        public static ResourceDepositDefinition
            GetResourceDepositDefinition(int tileIndex) => (ResourceDepositDefinition)
            _findResourceDepositDefinitionAt.Invoke(Amplitude.Mercury.Sandbox.Sandbox.World, new object[] {GetTile(tileIndex).WorldPosition});

        public static Amplitude.Mercury.Interop.ResourceDepositInfo GetResourceDepositInfo(int tileIndex) =>
            WorldSnapshotData.ResourceDepositInfo.Data.FirstOrDefault(r => r.TileIndex == tileIndex);
            // WorldSnapshotData.ResourceDepositInfo[WorldSnapshotData.FindResourceDepositInfoAt(tileIndex)];
        
        public static Tile GetTile(int tileIndex) => Snapshots.World.Tiles[tileIndex];
        public static Amplitude.Mercury.Interop.TileInfo GetTileInfo(int tileIndex) => Snapshots.World.TileInfo[tileIndex];
        
        public static void Initialize()
        {
            // ReflectionUtility.GetTypeByName()
            
            // var data = Amplitude.Mercury.Interop.Snapshots.WorldSnapshot.PresentationData;
            // var resourceInfoIndex = data.FindResourceDepositInfoAt(TileIndex);
        }
    }

    public class WorldTile : IWorldTile
    {
        public int TileIndex { get; set; } = 0;
        
        /// public Amplitude.Mercury.Interop.ResourceDepositInfo ResourceDepositInfo => ...
        ///
        
    }

    public static class IWorldTileEx
    {
        public static Tile Tile(this IWorldTile self) =>
            World.GetTile(self.TileIndex);
        public static Amplitude.Mercury.Interop.TileInfo TileInfo(this IWorldTile self) =>
            World.GetTileInfo(self.TileIndex);
        
        public static Amplitude.Mercury.Interop.TileInfo GetTileInfoTest(this WorldTile self) =>
            World.GetTileInfo(self.TileIndex);
    }
}
