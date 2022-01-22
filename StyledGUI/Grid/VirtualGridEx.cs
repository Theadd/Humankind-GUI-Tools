using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace StyledGUI.VirtualGridElements
{
    public static class VirtualGridEx
    {
        public static IEnumerable<ICell> Render(this IEnumerable<ICell> sequence, VirtualGrid grid, bool addColumnGap = false)
        {
            bool drawGap = false;
            int count = sequence.Count();
            
            for (var i = 0; i < count; i++)
            {
                if (addColumnGap)
                {
                    grid.Index = i;
                    grid.ColumnIndex = grid.Distribution[i];
                }

                ICell element = sequence.ElementAt(i);
                
                if (drawGap && grid.ColumnGap != 0)
                    GUILayout.Space(grid.ColumnGap);
                
                if (element is Cell cell)
                {
                    cell.Render(grid);
                } 
                else if (element is TintableCell tintableCell)
                {
                    tintableCell.Render(grid);
                }
                else if (element is CellGroup cellGroup)
                {
                    cellGroup.Render(grid);
                }
                else if (element is ClickableCell clickableCell)
                {
                    clickableCell.Render(grid);
                }
                else if (element is KeyboardShortcutCell shortcutCell)
                {
                    shortcutCell.Render(grid);
                }
                else if (element is CompositeCell compositeCell)
                {
                    compositeCell.Render(grid);
                }

                drawGap = addColumnGap;
            }
            
            return sequence;
        }

        public static void Render(this Cell self, VirtualGrid grid)
        {
            var prevTint = GUI.backgroundColor;
            GUI.backgroundColor = grid.Index % 2 != 0 ? grid.Grid.CellTintColorAlt : grid.Grid.CellTintColor;

            grid.Grid.Cell(self.Text, self.Style ?? Styles.CellStyle, self.Span ?? grid.ColumnCellSpan);

            GUI.backgroundColor = prevTint;
        }
        
        public static void Render(this CellGroup self, VirtualGrid grid)
        {
            self.Cells.Render(grid, false);
        }
        
        public static void Render(this TintableCell self, VirtualGrid grid)
        {
            var prev = GUI.backgroundColor;
            GUI.backgroundColor = self.BackgroundColor;
            grid.Grid.Cell(
                "<color=" + self.Color + ">" + self.Text + "</color>", 
                self.Style ?? Styles.ColorableCellStyle, 
                self.BackgroundColor, 
                self.Span ?? grid.ColumnCellSpan);
            GUI.backgroundColor = prev;
        }
        
        public static void Render(this ClickableCell self, VirtualGrid grid)
        {
            var prev = GUI.backgroundColor;
            GUI.backgroundColor = self.Color;
            GUI.enabled = self.Enabled;
            if (GUILayout.Button(self.Text, self.Style ?? Styles.CellButtonStyle, self.Span))
            {
                self.Action.Invoke(self.Index ?? grid.ColumnIndex);
            }

            GUI.enabled = true;
            GUI.backgroundColor = prev;
        }
        
        public static void Render(this KeyboardShortcutCell self, VirtualGrid grid)
        {
            var prev = GUI.backgroundColor;
            GUI.backgroundColor = self.Color;
            GUI.enabled = self.Enabled;

            if (self.Style != null && self.Style != self.Field.Style)
                self.Field.Style = self.Style;
            
            if (self.Field.Draw(self.Style, self.Span) && self.Action != null)
                self.Action.Invoke(self.Index ?? grid.ColumnIndex);

            GUI.enabled = true;
            GUI.backgroundColor = prev;
        }
        
        public static void Render(this CompositeCell self, VirtualGrid grid)
        {
            var prevTint = GUI.backgroundColor;
            GUI.backgroundColor = grid.Index % 2 != 0 ? grid.Grid.CellTintColorAlt : grid.Grid.CellTintColor;

            GUILayout.BeginHorizontal(self.Style ?? Styles.CellStyle, self.Span ?? grid.ColumnCellSpan);
            GUILayout.FlexibleSpace();

            self.Elements.Render(grid);
            
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUI.backgroundColor = prevTint;
        }
        
        public static IEnumerable<IElement> Render(this IEnumerable<IElement> sequence, VirtualGrid grid)
        {
            int count = sequence.Count();
            
            for (var i = 0; i < count; i++)
            {
                IElement element = sequence.ElementAt(i);

                if (element is TextElement textElement)
                {
                    textElement.Render(grid);
                }
                else if (element is ImageElement imageElement)
                {
                    imageElement.Render(grid);
                }
            }
            
            return sequence;
        }
        
        public static void Render(this TextElement self, VirtualGrid grid)
        {
            GUILayout.Label(self.Text, Styles.InlineCellContentStyle);
        }
        
        public static void Render(this ImageElement self, VirtualGrid grid)
        {
            GUI.DrawTexture(GUILayoutUtility.GetRect(self.Size, self.Size),
                self.Image, ScaleMode.StretchToFill, true,
                1f, grid.Grid.IconTintColor, 0, 0);
        }
    }
}