using System.Linq;
using StyledGUI;
using UnityEngine;

namespace DevTools.Humankind.GUITools.UI
{
    public class GameGrid
    {
        public VirtualGrid VirtualGrid { get; set; }
        public GameStatsSnapshot Snapshot { get; set; }
        public int[] DisplayOrder { get; set; } = null;
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
            if (DisplayOrder == null || DisplayOrder.Length != Snapshot.Empires.Length)
                DisplayOrder = Snapshot.Empires.Select((e, i) => i).ToArray();

            for (var i = 0; i < DisplayOrder.Length; i++)
            {
                int index = i;
                int empireIndex = DisplayOrder[i];

                EmpireSnapshot empire = Snapshot.Empires[DisplayOrder[i]];

                VirtualGrid.Columns[i] = new VirtualGrid.Column()
                {
                    Header = new VirtualGrid.ColumnHeader()
                    {
                        Text = empire.UserName
                    },
                    Sections = new []
                    {
                        new VirtualGrid.Section()
                        {
                            Rows = new []
                            {
                                new VirtualGrid.Row()
                                {
                                    Text = empire.MoneyStock
                                },
                                new VirtualGrid.Row()
                                {
                                    Text = empire.MoneyNet
                                }
                            }
                        },
                        new VirtualGrid.Section()
                        {
                            Rows = new []
                            {
                                new VirtualGrid.Row { Text = empire.InfluenceStock },
                                new VirtualGrid.Row { Text = empire.InfluenceNet },
                            }
                        }
                    }
                };
            }

        }

        

    }
}