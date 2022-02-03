﻿using Amplitude;
using Amplitude.Mercury;
using Amplitude.Mercury.Interop;
using Amplitude.Mercury.Interop.AI.Data;
using Amplitude.Mercury.Presentation;
using Amplitude.Mercury.Sandbox;
using Amplitude.Mercury.Simulation;
using Modding.Humankind.DevTools;
using Action = System.Action;
using Snapshots = Amplitude.Mercury.Interop.AI.Snapshots;

namespace DevTools.Humankind.GUITools.UI
{
    public partial class PaintBrush
    {
        // STORED ACTIONS
        private Action OnCreate { get; set; }
        private Action OnDestroy { get; set; }

        public static string ActionNameOnCreate { get; private set; } = string.Empty;
        public static string ActionNameOnDestroy { get; private set; } = string.Empty;

        // ACTION NAME IDENTIFIERS
        public static readonly string RemoveUnitAction = "REMOVE UNIT FROM ARMY";
        public static readonly string DestroySettlementAction = "DESTROY SETTLEMENT";
        public static readonly string DestroyDistrictAction = "DESTROY DISTRICT";
        public static readonly string EvolveOutpostAction = "EVOLVE OUTPOST";
        public static readonly string CreateDistrictAction = "CREATE DISTRICT";
        public static readonly string CreateCampAction = "CREATE OUTPOST";
        public static readonly string DetachTerritoryAction = "DETACH TERRITORY";
        public static readonly string RemoveMinorEmpireAction = "REMOVE MINOR EMPIRE";
        public static readonly string CreateArmyAction = "REMOVE MINOR EMPIRE";
        public static readonly string AddUnitToArmyAction = "REMOVE MINOR EMPIRE";

        // DESTROY ACTIONS
        public static void DestroySettlementAt(int tileIndex) => SandboxManager
            .PostOrder(new EditorOrderDestroySettlement {SettlementTileIndex = tileIndex});
        
        public static void DestroyDistrictAt(int tileIndex) => SandboxManager
            .PostOrder(new EditorOrderRemoveDistrictAt {TileIndex = tileIndex});
        
        public static void RemoveUnitFromArmyAt(int tileIndex) => SandboxManager
            .PostOrder(new EditorOrderRemoveUnitFromArmy { ArmyTileIndex = tileIndex, UnitIndex = 0 });
        
        // CREATE ACTIONS
        public static void CreateDistrictAt(int tileIndex, StaticString districtDefinitionName) => SandboxManager
            .PostOrder(new EditorOrderCreateExtensionDistrictAt
            {
                TileIndex = tileIndex,
                DistrictDefinitionName = districtDefinitionName
            });
        public static void CreateCampAt(int tileIndex) => SandboxManager
            .PostOrder(new EditorOrderCreateCampAt()
            {
                CampTileIndex = tileIndex,
                EmpireIndex = Amplitude.Mercury.Interop.Snapshots.GameSnapshot.PresentationData.LocalEmpireInfo.EmpireIndex
            });
        
        public static void CreateArmyAt(int tileIndex, StaticString unitDefinitionName) => SandboxManager
            .PostOrder(new OrderSpawnArmy
            {
                WorldPosition = Snapshots.World.Tiles[tileIndex].WorldPosition,
                UnitDefinitions = new []{ unitDefinitionName }
            });
        
        public static void AddUnitToArmy(int tileIndex, StaticString unitDefinitionName) => SandboxManager
            .PostOrder(new OrderAddUnitToArmy()
            {
                ArmyGUID = (SimulationEntityGUID) Snapshots.World.Tiles[tileIndex].Army.EntityGUID,
                UnitDefinition = unitDefinitionName
            });
        
        
        
        
        
        // TRANSFORM ACTIONS
        public static void EvolveOutpostToCityAt(int tileIndex) => SandboxManager
            .PostOrder(new EditorOrderEvolveCampToCity {CampTileIndex = tileIndex});

        // OTHER ACTIONS
        public static void RemoveMinorEmpire(int minorEmpireIndex) => SandboxManager
            .PostOrder(new EditorOrderRemoveMinorEmpire { MinorEmpireIndex = minorEmpireIndex });

        public static void DetachTerritoryFromCity(int cityTileIndex, int administrativeCenterTileIndex) =>
            SandboxManager.PostOrder(new EditorOrderDetachTerritoryFromCity
            {
                CityTileIndex = cityTileIndex,
                AdiministrativeCenterToDetachTileIndex = administrativeCenterTileIndex
            });


        private bool Destroy(Action action, string actionName, bool isRepeatingSameActionAllowed = false)
        {
            
            ActionNameOnDestroy = actionName;
            
            OnDestroy = () =>
            {
                if (LastTileIndex == TileIndex && LastHexTile == HexTile && !isRepeatingSameActionAllowed)
                    return;

                LastTileIndex = TileIndex;
                LastHexTile = HexTile;
                AudioPlayer.Play(AudioPlayer.CutForest);
                action.Invoke();
            };

            return true;
        }
        
        private bool Create(Action action, string actionName, bool isRepeatingSameActionAllowed = false)
        {
            ActionNameOnCreate = actionName;
            
            OnCreate = () =>
            {
                if (LastTileIndex == TileIndex && LastHexTile == HexTile && !isRepeatingSameActionAllowed)
                    return;

                LastTileIndex = TileIndex;
                LastHexTile = HexTile;
                AudioPlayer.Play(AudioPlayer.DistrictPlacement);
                action.Invoke();
            };

            return true;
        }
    }
}
