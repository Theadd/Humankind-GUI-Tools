using UnityEngine;
using System.Linq;
using System;
using System.Collections.Generic;
using Modding.Humankind.DevTools;
using Amplitude.Mercury.Interop.AI.Entities;
using Amplitude.Mercury.Interop;
using Amplitude.Mercury.Presentation;

namespace DevTools.Humankind.GUITools.UI.ArmyTools
{
    public class ArmyController
    {
        public int SelectedEmpireIndex { get; set; } = 0;
        public int SelectedArmyIndex { get; set; } = 0;

        // Currently selected Army's EntityGUID by the tool
        public ulong SelectedArmyEntityGUID { get; set; } = 0;

        // Currently selected Army's EntityGUID in game
        public ulong ArmyCursorEntityGUID { get; private set; } = 0;

        // EmpireIndex of currently selected Army in game
        public int ArmyCursorEmpireIndex { get; private set; } = 0;
        public Army[] Armies;
        public StaticGrid EmpireGrid;
        public ScrollableGrid ArmyGrid;
        public PresentationArmy SelectedPresentationArmy { get; private set; }
        public ToolSettings Settings { get; private set; }
        public int GameID => _gameId;
        private bool _isValidGame = false;
        private int _gameId;

        public ArmyController(/*ToolSettings settings = null*/)
        {
            Settings = new ToolSettings();
            // Settings = settings == null ? new ToolSettings() : settings;
        }

        public bool IsValidGame()
        {
            if (HumankindGame.IsGameLoaded)
            {
                if (!_isValidGame || _gameId != HumankindGame.GameID)
                    Initialize();
                _gameId = HumankindGame.GameID;
                
                return (_isValidGame = true);
            }

            return (_isValidGame = false);
        }

        protected void Initialize()
        {
            SelectedEmpireIndex = 0;
            Armies = HumankindGame.Empires[SelectedEmpireIndex].Armies.ToArray();
            SelectedArmyIndex = 0;

            EmpireGrid = new StaticGrid() {
                ItemsPerRow = HumankindGame.Empires.Length > 3 ? 3 : HumankindGame.Empires.Length,
                Items = GetEmpiresGridItems(),
                SelectedIndex = 0
            };
            ArmyGrid = new ScrollableGrid() {
                ItemsPerRow = 1,
                Items = ArmiesAsGridItems(Armies.AsEnumerable()).ToArray(),
            };
        }

        public void SyncArmyCursor()
        {
            // TODO: Use TryGetArmyCursor
            if (Snapshots.ArmyCursorSnapshot == null)
                return;
        
            if (Presentation.PresentationCursorController.CurrentCursor is ArmyCursor armyCursor)
            {
                if (ArmyCursorEntityGUID == 0 && armyCursor.EntityGUID == SelectedArmyEntityGUID)
                    ArmyCursorEntityGUID = armyCursor.EntityGUID;

                if (armyCursor.EntityGUID == SelectedArmyEntityGUID)
                {
                    if (SelectedPresentationArmy == null || SelectedPresentationArmy.SimulationEntityGuid != (ulong) SelectedArmyEntityGUID)
                    {
                        SetPresentationArmy((ulong) armyCursor.EntityGUID);
                        ArmyCursorEntityGUID = armyCursor.EntityGUID;
                        ArmyCursorEmpireIndex = SelectedEmpireIndex;
                    }

                    return;
                }
                // Select ArmyCursorEntityGUID when controlled by major empire.
                if (ArmyCursorEntityGUID != armyCursor.EntityGUID)
                {
                    ArmyCursorEntityGUID = armyCursor.EntityGUID;
                    SetPresentationArmy((ulong) armyCursor.EntityGUID);
                    ArmyCursorEmpireIndex = SelectedPresentationArmy.EmpireIndex;
                    // Set selection to ArmyCursor
                    SetSelectedEmpire(ArmyCursorEmpireIndex, ArmyCursorEntityGUID);
                }
            }
        }

        private void SetPresentationArmy(ulong entityGUID)
        {
            SelectedPresentationArmy = Presentation.PresentationEntityFactoryController.GetArmy(entityGUID);
        }

        public void SetSelectedEmpire(int selectedEmpireIndex, ulong selectedEntityGUID = 0)
        {
            SelectedEmpireIndex = selectedEmpireIndex;
            EmpireGrid.SelectedIndex = selectedEmpireIndex;
            UpdateArmies(-1, selectedEntityGUID);
        }

        public void UpdateArmies(int selectedArmyIndex, ulong selectedEntityGUID = 0)
        {
            Armies = GetArmiesOfEmpireAt(SelectedEmpireIndex);
            SelectedArmyIndex = selectedEntityGUID != 0 ? Array.FindIndex(Armies, army => army.EntityGUID == selectedEntityGUID) : selectedArmyIndex;
            ArmyGrid.Items = ArmiesAsGridItems(Armies.AsEnumerable()).ToArray();
            SelectedArmyEntityGUID = SelectedArmyIndex >= 0 ? Armies[SelectedArmyIndex].EntityGUID : 0;
            ArmyGrid.SelectedIndex = SelectedArmyIndex;
            MakeSelectedArmyVisibleInScrollView();
        }

        private void MakeSelectedArmyVisibleInScrollView()
        {
            var targetY = ArmyGrid.SelectedIndex * 25;
            var view = ArmyGrid.ScrollViewPosition;

            if (view.y <= targetY && targetY + 12 < view.y + ArmyGrid.Height)
                return;

            ArmyGrid.ScrollViewPosition = new Vector2(
                view.x,
                Math.Min(Math.Max(targetY - 25, 0), Math.Max((ArmyGrid.Items.Length * 25) - ArmyGrid.Height, 0))
            );
        }

        protected static Army[] GetArmiesOfEmpireAt(int empireIndex) => empireIndex < HumankindGame.Empires.Length ? 
            HumankindGame.Empires[empireIndex].Armies.ToArray() : HumankindGame.GetEmpireEntityAt(empireIndex).Armies;

        protected static GUIContent[] GetEmpiresGridItems() => HumankindGame.Empires.Select(e => 
                new GUIContent(StyledUI.GridItem(e))).ToArray();

        public static GUIContent[] ArmiesAsGridItems(IEnumerable<Army> sequence) => sequence.Select(
            (army, armyIndex) => new GUIContent(StyledUI.GridItem(army))).ToArray();

        // ACTIONS

        protected List<EndlessMovingArmy> MovingArmies = new List<EndlessMovingArmy>();

        public void SyncBackgroundTasks()
        {
            if (MovingArmies.Count() > 0)
                SyncMovingArmies();
        }

        private void SyncMovingArmies()
        {
            for (var i = MovingArmies.Count() - 1; i >= 0; i--)
            {
                var movingArmy = MovingArmies[i];

                if (ArmyUtils.ArmyMovementRatio(movingArmy.Army) <= 0.5f)
                    Amplitude.Mercury.Sandbox.SandboxManager.PostOrder(movingArmy.Order, movingArmy.EmpireIndex);
                else if (!ArmyUtils.IsRunning(movingArmy.Army))
                {
                    movingArmy.SkipOneTurn();
                    MovingArmies.Remove(movingArmy);
                }
            }
        }

        public void AddToEndlessMovingArmies(Army army)
        {
            if (!MovingArmies.Any(item => item.Army.EntityGUID == army.EntityGUID))
            {
                if (IsArmySelectedInGame(army) && TryGetArmyCursor(out ArmyCursor armyCursor))
                {
                    if (army.EntityGUID == armyCursor.EntityGUID && ArmyUtils.IsRunning(army))
                    {
                        if (armyCursor.SelectedUnitCount != army.Units.Length)
                            armyCursor.SelectAll();

                        EndlessMovingArmy movingArmy = new EndlessMovingArmy() {
                            Order = new OrderChangeMovementRatio(
                                armyCursor.EntityGUID, 
                                armyCursor.SelectedUnits.Select(guid => guid).ToArray(), 
                                1.0f
                            ),
                            EmpireIndex = (int) Snapshots.ArmyCursorSnapshot.PresentationData.EmpireIndex,
                            Army = army
                        };
                        MovingArmies.Add(movingArmy);
                    }
                }
            }
        }

        public bool IsArmySelectedInGame(Army army) => army.EntityGUID == ArmyCursorEntityGUID;

        public static bool TryGetArmyCursor(out ArmyCursor armyCursor) => (
            (armyCursor = Presentation.PresentationCursorController.CurrentCursor as ArmyCursor) != null 
            && Snapshots.ArmyCursorSnapshot != null);
    }

    public class EndlessMovingArmy
    {
        public OrderChangeMovementRatio Order { get; set; }
        public int EmpireIndex { get; set; }
        public Army Army { get; set; }
        public EndlessMovingArmy() {}

    }

    public static class EndlessMovingArmyEx
    {
        public static void SkipOneTurn(this EndlessMovingArmy self) => 
            Amplitude.Mercury.Sandbox.SandboxManager.PostOrder((Order) new OrderChangeEntityAwakeState() {
                EntityGuid = self.Order.UnitCollectionSimulationEntityGUID, AwakeState = AwakeState.SkipOneTurn
            }, self.EmpireIndex);
    }
}
