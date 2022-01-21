using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace StyledGUI
{
    public static class VirtualGridEx
    {
        public static IEnumerable<VirtualGrid.ColumnHeader> Render(this IEnumerable<VirtualGrid.ColumnHeader> sequence, VirtualGrid grid)
        {
            var columnHeaders = sequence as VirtualGrid.ColumnHeader[] ?? sequence.ToArray();
            int count = columnHeaders.Count();
            
            for (var i = 0; i < count; i++)
            {
                if (grid.ColumnGap != 0 && i != 0)
                    GUILayout.Space(grid.ColumnGap);
                
                VirtualGrid.ColumnHeader header = columnHeaders[i];
                GUI.backgroundColor = header.BackgroundColor;
                grid.Grid.Cell(header.Text, Styles.ColorableCellStyle, header.Color, grid.Grid.CellSpan4);
            }

            GUI.backgroundColor = Color.white;
            
            return columnHeaders;
        }
    }
}