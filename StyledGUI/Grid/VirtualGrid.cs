using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using StyledGUI.VirtualGridElements;

namespace StyledGUI
{
    public class VirtualGrid
    {
        public List<Column> Columns { get; set; } = new List<Column>();
        public Section[] Sections { get; set; }
        public IStyledGrid Grid { get => _grid; set => SetStyledGrid(value); }
        public bool DrawColumnHeaders { get; set; } = true;
        public bool DrawSectionHeaders { get; set; } = true;
        public bool DrawRowHeaders { get; set; } = true;
        public GUILayoutOption RowHeaderCellSpan { get; set; } = null;
        public GUILayoutOption ColumnCellSpan { get; set; } = null;
        public float ColumnGap { get; set; } = 4f;
        public int[] Distribution { get => _distribution; set => SetDistribution(value, true); }
        
        private int[] _distribution = null;
        private bool isExplicitDistribution = false;
        private IStyledGrid _grid;

        private void SetStyledGrid(IStyledGrid grid)
        {
            _grid = grid;
            if (ColumnCellSpan == null)
                ColumnCellSpan = _grid.CellSpan4;
            if (RowHeaderCellSpan == null)
                RowHeaderCellSpan = _grid.CellSpan6;
        }
        
        public void Render()
        {
            if (_distribution == null || (!isExplicitDistribution && _distribution.Length != Columns.Count))
                SetDistribution(Columns.Select((e, i) => i).ToArray());

            if (Columns.Count < 1) return;

            if (DrawColumnHeaders)
            {
                Grid.Row(Styles.StaticRowStyle);
                if (DrawRowHeaders)
                {
                    Grid.RowHeader(" ", RowHeaderCellSpan);
                }

                Distribution.Select(colIndex => Columns[colIndex].Header).Render(this);
            }

            for (var sectionIndex = 0; sectionIndex < Sections.Length; sectionIndex++)
            {

                for (var rowIndex = 0; rowIndex < Sections[sectionIndex].Rows.Length; rowIndex++)
                {
                    var row = Sections[sectionIndex].Rows[rowIndex];

                    Grid.Row( /* TODO: Row style */);
                    if (DrawRowHeaders)
                    {
                        Grid.RowHeader(row.Title, RowHeaderCellSpan);
                    }

                    row.Cells.Render(this, true);
                }
                
            }
            
            
        }
        
        private void SetDistribution(int[] distribution, bool isExplicit = false)
        {
            if (isExplicit)
                isExplicitDistribution = true;

            _distribution = distribution;
        }
    }
}
