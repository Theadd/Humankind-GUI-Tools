using System.Linq;
using StyledGUI;
using StyledGUI.VirtualGridElements;
using System;
using Modding.Humankind.DevTools;
using Amplitude.Mercury.Interop;
using Amplitude.Mercury.Sandbox;
using Amplitude.Framework;
using Amplitude.Framework.Networking;
using UnityEngine;


namespace DevTools.Humankind.GUITools.UI
{
    public class StaticHeaderGrid : GridStyles { }
    
    public class CommonHeadersGrid
    {
        public VirtualGrid VirtualGrid { get; set; }
        public GameStatsSnapshot Snapshot { get; set; }
        public bool IsDirty { get; set; } = true;
        public const string PerTurn = "<color=#000000FF><size=8>/</size><size=7> TURN</size></color>";

        protected IStyledGrid Grid;

        public CommonHeadersGrid() { }
        
        public void Render()
        {
            if (IsDirty)
                Update();

            VirtualGrid.Render();
        }

        public void Update()
        {
            Grid = VirtualGrid.Grid;
            ComputeVirtualGrid();

            IsDirty = false;
        }

        private void ComputeVirtualGrid()
        {
            VirtualGrid.Columns = new[]
            {
                new Column() { Name = "GLOBAL"}
            };

            VirtualGrid.Sections = new[]
            {
                new Section()
                {
                    Rows = new []
                    {
                        new Row()
                        {
                            Style = Styles.StaticRowStyle,
                            Cells = new ICell[]
                            {
                                new CellGroup()
                                {
                                    Cells = new ICell[]
                                    {
                                        new Cell() { Text = "<size=11>TURN</size>", Style = Styles.RowHeaderStyle, Span = Grid.CellSpan2 },
                                        new Cell() { Text = "<size=11>" + Snapshot.Turn + "</size>", Span = Grid.CellSpan1  },
                                        new Cell() { Text = "<size=11>ATMOSPHERE POLLUTION</size>", Style = Styles.RowHeaderStyle, Span = Grid.CellSpan5 },
                                        new Cell() { Text = "<size=11>LEVEL " + Snapshot.AtmospherePollutionLevel + "</size>", Span = Grid.CellSpan2  },
                                        new Cell() { Span = Grid.CellSpan4, Text = "<size=11><b>" + Snapshot.AtmospherePollutionStock + "</b>" + 
                                            (Snapshot.AtmospherePollutionNet >= 0 ? "  ( +" : "  ( ") + 
                                            Snapshot.AtmospherePollutionNet + " " + PerTurn + " )</size>" },
                                        new ClickableCell() { Span = Grid.CellSpan1, Text = "<size=11>-2K</size>", Action = OnRemove2kPollution },
                                        new ClickableCell() { Span = Grid.CellSpan1, Text = "<size=11>-250</size>", Action = OnRemove250Pollution },
                                        new ClickableCell() { Span = Grid.CellSpan1, Text = "<size=11>+250</size>", Action = OnAdd250Pollution },
                                        new ClickableCell() { Span = Grid.CellSpan1, Text = "<size=11>+2K</size>", Action = OnAdd2kPollution },
                                    }
                                }
                            }
                        },
                    },
                    SpaceBefore = 0
                },
            };
        }

        private void Trigger(Action action)
        {
            // some actions tracking stuff here
            action.Invoke();
            // Trigger update
            GameStatsWindow.ResetLoop();
        }

        private void OnRemove2kPollution(int _) => 
            Trigger(() => SandboxManager.PostOrder((Order)new EditorOrderAddOrRemoveAtmospherePollution()
            {
                Delta = -2000
            }, (int)Snapshots.GameSnapshot.PresentationData.LocalEmpireInfo.EmpireIndex));

        private void OnRemove250Pollution(int _) => 
            Trigger(() => SandboxManager.PostOrder((Order)new EditorOrderAddOrRemoveAtmospherePollution()
            {
                Delta = -250
            }, (int)Snapshots.GameSnapshot.PresentationData.LocalEmpireInfo.EmpireIndex));
        
        private void OnAdd250Pollution(int _) => 
            Trigger(() => SandboxManager.PostOrder((Order)new EditorOrderAddOrRemoveAtmospherePollution()
            {
                Delta = 250
            }, (int)Snapshots.GameSnapshot.PresentationData.LocalEmpireInfo.EmpireIndex));
        
        private void OnAdd2kPollution(int _) => 
            Trigger(() => SandboxManager.PostOrder((Order)new EditorOrderAddOrRemoveAtmospherePollution()
            {
                Delta = 2000
            }, (int)Snapshots.GameSnapshot.PresentationData.LocalEmpireInfo.EmpireIndex));
    }
}