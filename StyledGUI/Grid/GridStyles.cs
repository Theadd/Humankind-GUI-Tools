using System;
using UnityEngine;

namespace StyledGUI
{
    public interface IStyledGrid : ICellSpan
    {
        Color CellTintColor { get; set; }
        Color CellTintColorAlt { get; set; }
        Color IconTintColor { get; set; }
        Color CellButtonTintColor { get; set; }
    }
    
    public abstract class GridStyles : CellSpanGrid, IStyledGrid
    {
        public Color CellTintColor { get; set; } = new Color(0.9f, 0.9f, 0.85f, 1f);
        public Color CellTintColorAlt { get; set; } = new Color(0.6f, 0.6f, 0.6f, 1f);
        public Color IconTintColor { get; set; } = new Color(1f, 1f, 1f, 0.7f);
        public Color CellButtonTintColor { get; set; } = new Color32(85, 136, 254, 230);

    }
}
