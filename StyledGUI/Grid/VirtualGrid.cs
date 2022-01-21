using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StyledGUI
{
    public class VirtualGrid
    {
        public List<Column> Columns { get; set; } = new List<Column>();
        public IStyledGrid Grid { get; set; }
        public bool DrawColumnHeaders { get; set; } = true;
        public bool DrawSectionHeaders { get; set; } = true;
        public bool DrawRowHeaders { get; set; } = true;
        public GUILayoutOption RowHeaderCellSpan { get; set; } = null;
        public float ColumnGap { get; set; } = 4f;
        
        public int[] Distribution { get => _distribution; set => SetDistribution(value, true); }
        private int[] _distribution = null;
        private bool isExplicitDistribution = false;
        
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
                    Grid.RowHeader(" ", RowHeaderCellSpan ?? Grid.CellSpan6);
                }

                Distribution.Select(colIndex => Columns[colIndex].Header).Render(this);
            }
            
            
        }
        
        private void SetDistribution(int[] distribution, bool isExplicit = false)
        {
            if (isExplicit)
                isExplicitDistribution = true;

            _distribution = distribution;
        }
        
        public class Column
        {
            public ColumnHeader Header { get; set; } = ColumnHeader.Empty;
        
            public Section[] Sections { get; set; }
        }

        public class ColumnHeader
        {
            public static ColumnHeader Empty = new ColumnHeader();
        
            public string Text { get; set; } = string.Empty;
            public Color BackgroundColor { get; set; } = Color.clear;
            public Color Color { get; set; } = Color.white;
        }
    
        public class Section
        {
            public static Section Empty = new Section() { Rows = new []{ Row.Empty } };

            public Row[] Rows { get; set; }
        }
    
        public class Row
        {
            public static Row Empty = new Row();

            public string Text { get; set; } = string.Empty;
        }
    }

    
}