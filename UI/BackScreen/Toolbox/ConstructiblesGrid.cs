﻿using System;
using System.Collections.Generic;
using System.Linq;
using Modding.Humankind.DevTools;
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
        public override float GetCellHeight() => CellHeight;
        public override VirtualGridDisplayMode DisplayMode { get; set; } = VirtualGridDisplayMode.Grid;
        public float CellHeight = 48f;
        public override RectOffset CellPadding { get; set; } = new RectOffset(8, 8, 2, 2);
        public override Texture2D MissingTexture => _missingTex ? _missingTex : (_missingTex = Utils.LoadTexture("Missing"));
        private Texture2D _missingTex;
    }

    public class ConstructiblesGrid
    {
        public VirtualGrid VirtualGrid { get; set; }
        public ConstructibleStoreSnapshot Snapshot { get; set; }
        public bool IsDirty { get; set; } = true;
        public string FilterBy
        {
            get => _filterBy;
            set
            {
                if (_filterBy != value)
                {
                    _filterBy = value;
                    IsDirty = true;
                }
            }
        }

        private string _filterBy = string.Empty;
        public int GridModeChunkSize
        {
            get => Grid.GridModeChunkSize;
            set
            {
                if (Grid == null)
                    Grid = VirtualGrid.Grid;
                
                Grid.GridModeChunkSize = value;
                IsDirty = true;
                var totalCellWidth = (int)
                    ((_fixedWidth - (Grid.GetCellSpace() * (Grid.GridModeChunkSize - 1))) 
                     / Grid.GridModeChunkSize);
                var cellWidth = totalCellWidth - Grid.CellPadding.left - Grid.CellPadding.right;
                ((ConstructiblesStyledGrid) Grid).CellHeight = (float) cellWidth + Grid.CellPadding.top + Grid.CellPadding.bottom;
                ((ConstructiblesStyledGrid) Grid).Resize(totalCellWidth, ((ConstructiblesStyledGrid) Grid).GetCellSpace());
            }
        }

        public float FixedWidth
        {
            get => _fixedWidth;
            set
            {
                _fixedWidth = value;
                // Trigger layout update
                GridModeChunkSize = GridModeChunkSize;
            }
        }

        private float _fixedWidth = 400f;

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

        
        
        private Row[] GetRowsInListMode(Constructible[] values)
        {
            return values.Select(c => new Row()
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
            }).ToArray();
        }
        
        private Row[] GetRowsInGridMode(Constructible[] values)
        {
            int i = 0;
            IEnumerable<IEnumerable<Constructible>> groups = values
                .GroupBy(c => i++ / GridModeChunkSize)
                .Select(g => g);

            return groups.Select(group => new Row()
            {
                Style = Styles.StaticRowStyle,
                Cells = new[]
                {
                    new CellGroup()
                    {
                        Cells = group.Select(c => new ClickableImageCell()
                        {
                            Title = c.Title.ToUpper(),
                            Subtitle = c.Name,
                            UniqueName = c.DefinitionName.ToString(),
                            Category = c.Category == "" ? "" : "" + c.Category.ToUpper(),
                            Tags = c.Era > 0 ? "<size=10>ERA " + c.Era + "</size>" : "",
                            Image = c.Image,
                            // Style = Styles.CellImageStyle,
                            Span = Grid.CellSpan1
                        }).ToArray<ICell>()
                    }
                    
                }
            }).ToArray();
        }
        
        private void ComputeVirtualGrid()
        {
            VirtualGrid.Columns = new Column[]
            {
                new Column() { Name = "Constructibles" }
            };

            VirtualGrid.Sections = Snapshot.Districts
                .Select(ApplyFilterBy)
                .Where(group => group.Values.Length > 0)
                .Select(group => new Section()
                {
                    Title = "<size=13><b>" + group.Title.ToUpper() + "</b></size>",
                    View = 0,
                    Rows = Grid.DisplayMode == VirtualGridDisplayMode.Grid 
                        ? GetRowsInGridMode(group.Values) 
                        : GetRowsInListMode(group.Values)
                })
                .Concat(
                    Snapshot.Units
                        .Select(ApplyFilterBy)
                        .Where(group => group.Values.Length > 0)
                        .Select(group => new Section() 
                        {
                            Title = "<size=13><b>" + group.Title.ToUpper() + "</b></size>", 
                            View = 1, 
                            Rows = Grid.DisplayMode == VirtualGridDisplayMode.Grid 
                                ? GetRowsInGridMode(group.Values) 
                                : GetRowsInListMode(group.Values)
                        }))
                .ToArray();
        }

        private DefinitionsGroup ApplyFilterBy(DefinitionsGroup group)
        {
            if (_filterBy.Length == 0)
                return group;

            return new DefinitionsGroup()
            {
                Title = group.Title,
                Values = group.Values
                    .Where(c => 
                        c.Category.IndexOf(_filterBy, StringComparison.OrdinalIgnoreCase) >= 0 
                        || c.Title.IndexOf(_filterBy, StringComparison.OrdinalIgnoreCase) >= 0 
                        || c.DefinitionName.ToString().IndexOf(_filterBy, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToArray()
            };
        }
    }
}
