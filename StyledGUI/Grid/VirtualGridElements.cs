using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

namespace StyledGUI.VirtualGridElements
{
    public interface ICell
    {
        
    }
    
    public class Column
    {
        public ColumnHeader Header { get; set; } = ColumnHeader.Empty;
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

        public string Title { get; set; } = string.Empty;
        public Row[] Rows { get; set; }
    }
    
    public class Row
    {
        public static Row Empty = new Row();

        public string Title { get; set; } = string.Empty;
        public IEnumerable<ICell> Cells { get; set; }
    }

    public class CellGroup : ICell
    {
        public ICell[] Cells { get; set; }
    }
        
    public class ClickableCell : ICell
    {
        public Action Action { get; set; }
        public string Text { get; set; }
    }
        
    public class Cell : ICell
    {
        public string Text { get; set; }
        public GUILayoutOption Span { get; set; } = null;
    }
}
