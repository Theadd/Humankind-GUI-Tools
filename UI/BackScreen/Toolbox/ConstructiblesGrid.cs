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
    public class ConstructiblesStyledGrid : GridStyles
    {
        public override Color CellTintColor { get; set; } = new Color(0.26f, 0.26f, 0.26f, 1f);
        public override Color CellTintColorAlt { get; set; } = new Color(0, 0, 0, 1f);
        public override Color IconTintColor { get; set; } = new Color(1f, 1f, 1f, 0.7f);
        public override float GetCellHeight() => 48f;
    }

    public class ConstructiblesGrid
    {
        public VirtualGrid VirtualGrid { get; set; }
        public ConstructibleStoreSnapshot Snapshot { get; set; }
        public bool IsDirty { get; set; } = true;

        protected IStyledGrid Grid;

        public ConstructiblesGrid()
        {
        }

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
            VirtualGrid.Columns = new Column[]
            {
                new Column() {Name = "Constructibles"}
            };

            VirtualGrid.Sections = Snapshot.Units
                .Select(group => new Section()
                {
                    Title = group.Title,
                    View = 0,
                    Rows = group.Values.Select(c => new Row()
                    {
                        Cells = new[]
                        {
                            new Clickable4xCell()
                            {
                                Title = c.Title,
                                Subtitle = c.Name,
                                UniqueName = c.DefinitionName.ToString(),
                                Category = c.Category,
                                Tags = "ERA " + c.Era,
                                Image = c.Image
                                // Span = Grid.CellSpan8
                            }
                        }
                    }).ToArray()
                })
                .Concat(
                Snapshot.Districts.Select(group => new Section()
                {
                    Title = group.Title,
                    View = 1,
                    Rows = group.Values.Select(c => new Row()
                    {
                        Cells = new[]
                        {
                            new Clickable4xCell()
                            {
                                Title = c.Title,
                                Subtitle = c.Name,
                                UniqueName = c.DefinitionName.ToString(),
                                // Category = c.Category,
                                Tags = "ERA " + c.Era,
                                Image = c.Image
                                // Span = Grid.CellSpan8
                            }
                        }
                    }).ToArray()
                }))
                .ToArray();
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
    }
}