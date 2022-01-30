using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

namespace StyledGUI.VirtualGridElements
{
    public interface ICell
    {
        
    }
    
    public interface IElement
    {
        
    }
    
    public class Column
    {
        public string Name { get; set; }
    }

    public class Section
    {
        public static Section Empty = new Section() { Rows = new []{ Row.Empty } };

        public string Title { get; set; } = string.Empty;
        public Row[] Rows { get; set; }
        public float SpaceBefore { get; set; } = 16f;
        public int View { get; set; } = 0;
    }
    
    public class Row
    {
        public static Row Empty = new Row();

        public string Title { get; set; } = string.Empty;
        public IEnumerable<ICell> Cells { get; set; }
        public GUIStyle Style { get; set; } = null;
    }

    public class CellGroup : ICell
    {
        public ICell[] Cells { get; set; }
    }
        
    public class ClickableCell : ICell
    {
        public Action<int> Action { get; set; }
        public string Text { get; set; }
        public Color Color { get; set; } = Color.white;
        public int? Index { get; set; } = null;
        public GUILayoutOption Span { get; set; } = null;
        public GUIStyle Style { get; set; } = null;
        public bool Enabled { get; set; } = true;
    }
    
    public class KeyboardShortcutCell : ICell
    {
        public KeyboardShortcutField Field { get; set; }
        public Action<int> Action { get; set; }
        public int? Index { get; set; } = null;
        public Color Color { get; set; } = Color.white;
        public GUILayoutOption Span { get; set; } = null;
        public GUIStyle Style { get; set; } = null;
        public bool Enabled { get; set; } = true;
    }
        
    public class Cell : ICell
    {
        public string Text { get; set; }
        public GUILayoutOption Span { get; set; } = null;
        public GUIStyle Style { get; set; } = null;
    }
    
    public class CompositeCell : ICell
    {
        public IElement[] Elements { get; set; }
        public GUILayoutOption Span { get; set; } = null;
        public GUIStyle Style { get; set; } = null;
    }
    
    public class TintableCell : ICell
    {
        public string Text { get; set; }
        public GUILayoutOption Span { get; set; } = null;
        public Color BackgroundColor { get; set; } = UnityEngine.Color.clear;
        public string Color { get; set; } = "#FFFFFFFF";
        public GUIStyle Style { get; set; } = null;
    }
    
    public class Clickable4xCell : ICell
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Tags { get; set; }
        public string Category { get; set; }
        public string UniqueName { get; set; }
        public Texture Image { get; set; }
        public Action<int> Action { get; set; }
        public GUILayoutOption Span { get; set; } = null;
        public GUIStyle Style { get; set; } = null;
        public bool Enabled { get; set; } = true;
    }

    public class TextElement : IElement
    {
        public string Text { get; set; }
    }
    
    public class ImageElement : IElement
    {
        public Texture Image { get; set; }
        public float Size { get; set; } = 14f;
    }
}
