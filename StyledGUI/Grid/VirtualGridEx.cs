using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace StyledGUI.VirtualGridElements
{
    public static class VirtualGridEx
    {
        public static IEnumerable<ColumnHeader> Render(this IEnumerable<ColumnHeader> sequence, VirtualGrid grid)
        {
            var columnHeaders = sequence as ColumnHeader[] ?? sequence.ToArray();
            int count = columnHeaders.Count();
            
            for (var i = 0; i < count; i++)
            {
                if (grid.ColumnGap != 0 && i != 0)
                    GUILayout.Space(grid.ColumnGap);
                
                ColumnHeader header = columnHeaders[i];
                GUI.backgroundColor = header.BackgroundColor;
                grid.Grid.Cell(header.Text, Styles.ColorableCellStyle, header.Color, grid.Grid.CellSpan4);
            }

            GUI.backgroundColor = Color.white;
            
            return columnHeaders;
        }
        
        public static IEnumerable<ICell> Render(this IEnumerable<ICell> sequence, VirtualGrid grid, bool addColumnGap = false)
        {
            bool drawGap = false;
            foreach (ICell element in sequence)
            {
                if (drawGap && grid.ColumnGap != 0)
                    GUILayout.Space(grid.ColumnGap);
                
                if (element is Cell cell)
                {
                    cell.Render(grid);
                }

                drawGap = addColumnGap;
            }
            
            return sequence;
        }

        public static void Render(this Cell self, VirtualGrid grid)
        {
            grid.Grid.Cell(self.Text, self.Span ?? grid.ColumnCellSpan);
        }
    }
}