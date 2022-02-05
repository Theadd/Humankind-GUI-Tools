using System;
using UnityEngine;

namespace StyledGUI
{

    public interface ICellSpan
    {
        GUILayoutOption CellSpan1 { get; set; }
        GUILayoutOption CellSpan2 { get; set; }
        GUILayoutOption CellSpan3 { get; set; }
        GUILayoutOption CellSpan4 { get; set; }
        GUILayoutOption CellSpan5 { get; set; }
        GUILayoutOption CellSpan6 { get; set; }
        GUILayoutOption CellSpan7 { get; set; }
        GUILayoutOption CellSpan8 { get; set; }
        GUILayoutOption CellSpan9 { get; set; }
        GUILayoutOption CellSpan10 { get; set; }
        float GetCellWidth();
        float GetCellSpace();
        float GetCellHeight();
    }
    
    public abstract class CellSpanGrid : ICellSpan
    {
        public float CellWidth;
        public float CellSpace;
        public GUILayoutOption CellSpan1 { get; set; }
        public GUILayoutOption CellSpan2 { get; set; }
        public GUILayoutOption CellSpan3 { get; set; }
        public GUILayoutOption CellSpan4 { get; set; }
        public GUILayoutOption CellSpan5 { get; set; }
        public GUILayoutOption CellSpan6 { get; set; }
        public GUILayoutOption CellSpan7 { get; set; }
        public GUILayoutOption CellSpan8 { get; set; }
        public GUILayoutOption CellSpan9 { get; set; }
        public GUILayoutOption CellSpan10 { get; set; }
        public GUILayoutOption CellSpan(int numCells) => GUILayout.Width(CellWidth * numCells + CellSpace * (numCells - 1));

        public CellSpanGrid()
        {
            Resize(34f, 1f);
        }

        public CellSpanGrid(float cellWidth, float cellSpace)
        {
            Resize(cellWidth, cellSpace);
        }

        public CellSpanGrid Resize(float cellWidth, float cellSpace)
        {
            CellWidth = cellWidth;
            CellSpace = cellSpace;
            CellSpan1 = CellSpan(1);
            CellSpan2 = CellSpan(2);
            CellSpan3 = CellSpan(3);
            CellSpan4 = CellSpan(4);
            CellSpan5 = CellSpan(5);
            CellSpan6 = CellSpan(6);
            CellSpan7 = CellSpan(7);
            CellSpan8 = CellSpan(8);
            CellSpan9 = CellSpan(9);
            CellSpan10 = CellSpan(10);

            return this;
        }

        public virtual float GetCellWidth() => CellWidth;
        public virtual float GetCellSpace() => CellSpace;
        public virtual float GetCellHeight() => 22f;
    }

    
}
