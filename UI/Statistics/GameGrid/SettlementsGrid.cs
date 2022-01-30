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
    
    public class EmpireSettlementsGrid : GridStyles { }
    
    public class SettlementsGrid
    {
        public VirtualGrid VirtualGrid { get; set; }
        public GameStatsSnapshot Snapshot { get; set; }
        public bool IsDirty { get; set; } = true;
        public const string PerTurn = "<color=#000000FF><size=8>/</size><size=7> TURN</size></color>";

        protected IStyledGrid Grid;

        public SettlementsGrid() { }
        
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
            VirtualGrid.Columns = Snapshot.Empires.Select(empire => new Column()
            {
                Name = empire.UserName
            }).ToArray();

            VirtualGrid.Sections = new[]
            {
                new Section()
                {
                    Rows = new []
                    {
                        new Row() { Cells = Snapshot.Empires.Select(empire => new Cell()
                        {
                            Text = (empire.Index == Snapshot.LocalEmpireIndex) ? 
                                "<size=16><color=#FFFFFFFF><b>A C T I V E</b></color></size>" : " ",
                            Style = Styles.CenteredStaticRowStyle
                        }), Style = Styles.StaticRowStyle },
                        new Row() { Cells = Snapshot.Empires.Select(empire => new TintableCell()
                        {
                            Text = "<size=10><b>" + empire.UserName + "</b></size>",
                            BackgroundColor = empire.Color,
                            Color = empire.SecondaryColor
                        }), Style = Styles.StaticRowStyle }
                    },
                    SpaceBefore = 0
                },
                new Section()
                {
                    SpaceBefore = 4f,
                    Rows = new[]
                    {
                        new Row() { Title = "TERRITORIES, CITIES / MAX", 
                            Cells = Snapshot.Empires.Select(e => new CellGroup() { Cells = new ICell[] {
                                    new Cell() { Text = e.TerritoryCount, Span = Grid.CellSpan1},
                                    new Cell() { Text = e.CityCount + " / " + e.CityCap, Span = Grid.CellSpan3},
                        }})},
                        new Row() { Title = "STABILITY", 
                            Cells = Snapshot.Empires.Select(e => new CompositeCell() { Elements = new IElement[] {
                                new TextElement() { Text = e.Stability },
                                new ImageElement() { Image = Utils.StabilityTexture },
                            }})},
                        new Row() { Title = "ERA LEVEL, SUM OF ERA STARS", 
                            Cells = Snapshot.Empires.Select(e => new CellGroup() { Cells = new ICell[] {
                                new Cell() { Text = e.EraLevel, Span = Grid.CellSpan1},
                                new Cell() { Text = e.SumOfEraStars, Span = Grid.CellSpan3},
                            }})},
                        new Row() { Title = "CITIZENS, EMPIRE POPULATION", 
                            Cells = Snapshot.Empires.Select(e => new CellGroup() { Cells = new ICell[] {
                                new Cell() { Text = e.SettlementsPopulation, Span = Grid.CellSpan2},
                                new Cell() { Text = e.EmpirePopulation, Span = Grid.CellSpan2},
                            }})},
                        new Row() { Title = "POLLUTION", 
                            Cells = Snapshot.Empires.Select(e => new CellGroup() { Cells = new ICell[] {
                                new Cell() { Text = e.PollutionStock, Span = Grid.CellSpan2},
                                new Cell() { Text = (int.Parse(e.PollutionNet) >= 0 ? "+" : "") + 
                                                    e.PollutionNet + " " + PerTurn, Span = Grid.CellSpan2},
                            }})}
                    }
                }
            };


        }

        private void Trigger(Action action)
        {
            // some actions tracking stuff here
            action.Invoke();
            // Trigger update
            GameStatsWindow.ResetLoop();
        }

        private void OnAdd5kMoney(int empireIndex) =>
            Trigger(() => HumankindGame.Empires[empireIndex].MoneyStock += 5000);

        private Row QuickTextRow(string title, Func<EmpireSnapshot, string> builder) =>
            new Row()
            {
                Title = title,
                Cells = Snapshot.Empires.Select(empire => new Cell()
                {
                    Text = builder.Invoke(empire)
                })
            };
    }
}