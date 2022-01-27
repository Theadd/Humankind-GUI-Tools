using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using StyledGUI.VirtualGridElements;

namespace StyledGUI
{
    public class VirtualGrid
    {
        public Column[] Columns { get; set; }
        public Section[] Sections { get; set; }
        public IStyledGrid Grid { get => _grid; set => SetStyledGrid(value); }
        public bool DrawSectionHeaders { get; set; } = true;
        public bool DrawRowHeaders { get; set; } = true;
        public GUILayoutOption RowHeaderCellSpan { get; set; } = null;
        public GUILayoutOption ColumnCellSpan { get; set; } = null;
        public float SectionHorizontalLineWidth { get; set; } = -1f;
        public float ColumnGap { get; set; } = 4f;
        public int[] Distribution { get => _distribution; set => SetDistribution(value, true); }

        public int ColumnIndex = 0;
        public int Index = 0;
        
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
            if (SectionHorizontalLineWidth < 0)
                SectionHorizontalLineWidth = _grid.GetCellWidth() * 6;
        }
        
        public void Render()
        {
            if (_distribution == null || (!isExplicitDistribution && _distribution.Length != Columns.Length))
                SetDistribution(Columns.Select((e, i) => i).ToArray());

            if (Columns.Length < 1) return;

            for (var sectionIndex = 0; sectionIndex < Sections.Length; sectionIndex++)
            {
                string sectionTitle = Sections[sectionIndex].Title;
                
                if (Sections[sectionIndex].SpaceBefore != 0)
                    Grid.Space(Sections[sectionIndex].SpaceBefore);
                
                if (DrawSectionHeaders && sectionTitle.Length > 0)
                {
                    if (DrawRowHeaders)
                    {
                        Grid.Row(Styles.StaticRowStyle)
                            .VerticalStack()
                            .RowHeader(sectionTitle, RowHeaderCellSpan)
                            .DrawHorizontalLine(0.5f, SectionHorizontalLineWidth)
                            .EndVerticalStack()
                            .EndRow();
                    }
                    else
                    {
                        Grid.Row(Styles.StaticRowStyle)
                            .RowHeader(sectionTitle, RowHeaderCellSpan)
                            .EndRow();
                    }
                }
                
                for (var rowIndex = 0; rowIndex < Sections[sectionIndex].Rows.Length; rowIndex++)
                {
                    var row = Sections[sectionIndex].Rows[rowIndex];

                    Grid.Row(row.Style ?? Styles.RowStyle);
                    if (DrawRowHeaders)
                    {
                        Grid.RowHeader(row.Title, RowHeaderCellSpan);
                    }

                    Distribution
                        .Select(index => row.Cells.ElementAt(index))
                        .Render(this, true);
                    Grid.EndRow();
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
