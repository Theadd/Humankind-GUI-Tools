using Amplitude;
using UniverseLib;
using UniverseLib.Utility;
using UniverseLib.Runtime.Mono;

namespace DevTools.Humankind.GUITools.UI
{
    public static partial class WorldController
    {
        #region WORLD
        // public static Amplitude.Mercury.Simulation.World World => Amplitude.Mercury.Sandbox.Sandbox.World;
        
        // public static Amplitude.Mercury.Simulation.River[] Rivers => Amplitude.Mercury.Simulation.World.Tables.Rivers;

        public static Amplitude.Hexagon.OffsetCoords[] SpawnLocations =>
            Amplitude.Mercury.Simulation.World.Tables.SpawnLocations;
        #endregion
        
        #region DEFINITIONS
        public static Amplitude.Mercury.Data.World.TerrainTypeDefinition[] TerrainTypeDefinitions =>
            Amplitude.Mercury.Simulation.World.Tables.TerrainTypeDefinitions;

        public static Amplitude.Mercury.Data.World.PointOfInterestDefinition[] PointOfInterestDefinitions =>
            Amplitude.Mercury.Simulation.World.Tables.PointOfInterestDefinitions;

        public static Amplitude.Mercury.Data.World.BiomeDefinition[] BiomeDefinitions =>
            Amplitude.Mercury.Simulation.World.Tables.BiomeDefinitions;

        public static Amplitude.Mercury.Data.World.NaturalWonderDefinition[] NaturalWonderDefinitions =>
            Amplitude.Mercury.Simulation.World.Tables.NaturalWonderDefinitions;
        #endregion
        
        #region SNAPSHOTS
        public static Amplitude.Mercury.Interop.AI.WorldSnapshot WorldSnapshot => Amplitude.Mercury.Interop.AI.Snapshots.World;
        public static Amplitude.Mercury.Interop.AI.GameSnapshot GameSnapshot => Amplitude.Mercury.Interop.AI.Snapshots.Game;
        
        public static Amplitude.Mercury.Interop.AI.DataSnapshot DataSnapshot => Amplitude.Mercury.Interop.AI.Snapshots.Data;
        public static Amplitude.Mercury.Interop.AI.TradeSnapshot TradeSnapshot => Amplitude.Mercury.Interop.AI.Snapshots.Trade;
        public static Amplitude.Mercury.Interop.AI.CultureSnapshot CultureSnapshot => Amplitude.Mercury.Interop.AI.Snapshots.Culture;
        public static Amplitude.Mercury.Interop.AI.ReligionSnapshot ReligionSnapshot => Amplitude.Mercury.Interop.AI.Snapshots.Religion;
        public static Amplitude.Mercury.Interop.AI.BattleSnapshot BattleSnapshot => Amplitude.Mercury.Interop.AI.Snapshots.Battle;
        public static Amplitude.Mercury.Interop.AI.SurrenderSnapshot SurrenderSnapshot => Amplitude.Mercury.Interop.AI.Snapshots.Surrender;
        public static Amplitude.Mercury.Interop.AI.PollutionSnapshot PollutionSnapshot => Amplitude.Mercury.Interop.AI.Snapshots.Pollution;
        #endregion




        public static Amplitude.Mercury.Data.World.ResourceDepositDefinition
            GetResourceDepositDefinition(int tileIndex) => (Amplitude.Mercury.Data.World.ResourceDepositDefinition)
            _findResourceDepositDefinitionAt.Invoke(Amplitude.Mercury.Sandbox.Sandbox.World, new object[] {GetTile(tileIndex).WorldPosition});

        public static Amplitude.Mercury.Interop.AI.Data.Tile GetTile(int tileIndex) => Amplitude.Mercury.Interop.AI.Snapshots.World.Tiles[tileIndex];
        public static Amplitude.Mercury.Interop.TileInfo GetTileInfo(int tileIndex) => Amplitude.Mercury.Interop.AI.Snapshots.World.TileInfo[tileIndex];
        
        public static void Initialize()
        {
            // ReflectionUtility.GetTypeByName()
            
        }
    }

    public class WorldTile : IWorldTile
    {
        public int TileIndex { get; set; } = 0;
    }

    public static class IWorldTileEx
    {
        public static Amplitude.Mercury.Interop.AI.Data.Tile Tile(this IWorldTile self) =>
            WorldController.GetTile(self.TileIndex);
        public static Amplitude.Mercury.Interop.TileInfo TileInfo(this IWorldTile self) =>
            WorldController.GetTileInfo(self.TileIndex);
    }
}
