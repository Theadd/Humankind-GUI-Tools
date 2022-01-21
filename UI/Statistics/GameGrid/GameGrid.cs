using System.Linq;
using StyledGUI;
using StyledGUI.VirtualGridElements;
using System;
using UnityEngine;

namespace DevTools.Humankind.GUITools.UI
{
    public class GameGrid
    {
        public VirtualGrid VirtualGrid { get; set; }
        public GameStatsSnapshot Snapshot { get; set; }
        public bool IsDirty { get; set; } = true;

        public GameGrid() { }
        
        public void Render()
        {
            if (IsDirty)
                Update();

            VirtualGrid.Render();
        }

        public void Update()
        {
            ComputeVirtualGrid();

            IsDirty = false;
        }

        private void ComputeVirtualGrid()
        {
            // EmpireSnapshot empire = Snapshot.Empires[DisplayOrder[i]];

            VirtualGrid.Columns = Snapshot.Empires.Select(empire => new Column()
            {
                Header = new ColumnHeader()
                {
                    Text = empire.UserName
                },
            }).ToList();

            VirtualGrid.Sections = new[]
            {
                new Section()
                {
                    Rows = new[]
                    {
                        new Row()
                        {
                            Title = "MoneyStock",
                            Cells = Snapshot.Empires.Select(empire => new Cell()
                            {
                                Text = empire.MoneyStock
                            })
                        },
                        new Row()
                        {
                            Title = "MoneyNet",
                            Cells = Snapshot.Empires.Select(empire => new Cell()
                            {
                                Text = empire.MoneyNet
                            })
                        }
                    }
                },
                new Section()
                {
                    Rows = new[]
                    {
                        QuickTextRow("InfluenceStock", e => e.InfluenceStock),
                        QuickTextRow("InfluenceNet", e => e.InfluenceNet),
                    }
                }
            };


        }

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