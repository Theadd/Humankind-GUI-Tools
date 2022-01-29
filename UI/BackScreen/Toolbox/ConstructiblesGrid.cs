using System.Linq;
using StyledGUI;
using StyledGUI.VirtualGridElements;
using UnityEngine;

namespace DevTools.Humankind.GUITools.UI
{
    public class ConstructiblesStyledGrid : GridStyles
    {
        public override Color CellTintColor { get; set; } = new Color(0, 0, 0, 0.6f);
        public override Color CellTintColorAlt { get; set; } = new Color(0, 0, 0, 0.2f);
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
                    Title = "<size=13><b>" + group.Title.ToUpper() + "</b></size>",
                    View = 0,
                    Rows = group.Values.Select(c => new Row()
                    {
                        Cells = new[]
                        {
                            new Clickable4xCell()
                            {
                                Title = "<size=11><b>" + c.Title.ToUpper() + "</b></size>",
                                Subtitle = c.Name,
                                UniqueName = c.DefinitionName.ToString(),
                                Category = c.Category == "None" ? "" : "<size=9>" + c.Category.ToUpper() + "</size>", 
                                Tags = c.Era > 0 ? "<size=10>ERA " + c.Era + "</size>" : "",
                                Image = c.Image
                                // Span = Grid.CellSpan8
                            }
                        }
                    }).ToArray()
                })
                .Concat(
                Snapshot.Districts.Select(group => new Section()
                {
                    Title = "<size=13><b>" + group.Title.ToUpper() + "</b></size>",
                    View = 1,
                    Rows = group.Values.Select(c => new Row()
                    {
                        Cells = new[]
                        {
                            new Clickable4xCell()
                            {
                                Title = "<size=11><b>" + c.Title.ToUpper() + "</b></size>",
                                Subtitle = c.Name,
                                UniqueName = c.DefinitionName.ToString(),
                                Category = c.Category == "None" ? "" : "<size=9>" + c.Category.ToUpper() + "</size>",
                                Tags = c.Era > 0 ? "<size=10>ERA " + c.Era + "</size>" : "",
                                Image = c.Image
                                // Span = Grid.CellSpan8
                            }
                        }
                    }).ToArray()
                }))
                .ToArray();
        }
    }
}
