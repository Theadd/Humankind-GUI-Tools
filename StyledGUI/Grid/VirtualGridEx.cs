using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Modding.Humankind.DevTools;

namespace StyledGUI.VirtualGridElements
{
    public static class VirtualGridEx
    {
        public static IEnumerable<ICell> Render(this IEnumerable<ICell> sequence, VirtualGrid grid, bool addColumnGap = false)
        {
            bool drawGap = false;
            int count = sequence.Count();

            grid.Cursor.CellIndex = 0;
            
            for (var i = 0; i < count; i++)
            {
                if (addColumnGap)
                {
                    grid.Cursor.VisibleColumnIndex = i;
                    grid.Cursor.ColumnIndex = grid.Distribution[i];
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
                else if (element is Clickable4xCell clickable4XCell)
                {
                    clickable4XCell.Render(grid);
                }

                drawGap = addColumnGap;
                grid.Cursor.SyncXY();
                grid.Cursor.X += grid.Cursor.LastWidth;
                grid.Cursor.CellIndex++;
            }
            
            return sequence;
        }

        

        public static void Render(this Cell self, VirtualGrid grid)
        {
            if (grid.IsLookingForCell)
                DoCellLookup(self, grid, self.Span ?? grid.DefaultCellSpan);
            
            var prevTint = GUI.backgroundColor;
            if (grid.Cursor.IsCurrentCellSelected)
                GUI.backgroundColor = grid.Grid.SelectedCellTintColor;
            else
                AlternateBackgroundColor(grid);
            
            grid.Grid.Cell(self.Text, self.Style ?? Styles.CellStyle, self.Span ?? grid.DefaultCellSpan);

            GUI.backgroundColor = prevTint;
        }
        
        public static void Render(this CellGroup self, VirtualGrid grid)
        {
            self.Cells.Render(grid, false);
        }
        
        public static void Render(this TintableCell self, VirtualGrid grid)
        {
            if (grid.IsLookingForCell)
                DoCellLookup(self, grid, self.Span ?? grid.DefaultCellSpan);
            
            var prev = GUI.backgroundColor;
            GUI.backgroundColor = self.BackgroundColor;
            grid.Grid.Cell(
                "<color=" + self.Color + ">" + self.Text + "</color>", 
                self.Style ?? Styles.ColorableCellStyle, 
                self.BackgroundColor, 
                self.Span ?? grid.DefaultCellSpan);
            GUI.backgroundColor = prev;
        }
        
        public static void Render(this ClickableCell self, VirtualGrid grid)
        {
            if (grid.IsLookingForCell)
                DoCellLookup(self, grid, self.Span ?? grid.DefaultCellSpan);
            
            var prev = GUI.backgroundColor;
            GUI.backgroundColor = self.Color;
            GUI.enabled = self.Enabled;
            if (GUILayout.Button(self.Text, self.Style ?? Styles.CellButtonStyle, self.Span))
            {
                self.Action.Invoke(self.Index ?? grid.Cursor.ColumnIndex);
            }

            GUI.enabled = true;
            GUI.backgroundColor = prev;
        }
        
        public static void Render(this KeyboardShortcutCell self, VirtualGrid grid)
        {
            if (grid.IsLookingForCell)
                DoCellLookup(self, grid, self.Span ?? grid.DefaultCellSpan);
            
            var prev = GUI.backgroundColor;
            GUI.backgroundColor = self.Color;
            GUI.enabled = self.Enabled;

            if (self.Style != null && self.Style != self.Field.Style)
                self.Field.Style = self.Style;
            
            if (self.Field.Draw(self.Style, self.Span) && self.Action != null)
                self.Action.Invoke(self.Index ?? grid.Cursor.ColumnIndex);

            GUI.enabled = true;
            GUI.backgroundColor = prev;
        }

        public static void Render(this Clickable4xCell self, VirtualGrid grid)
        {
            GUI.enabled = self.Enabled;

            if (grid.IsLookingForCell)
                DoCellLookup(self, grid, self.Span ?? grid.DefaultCellSpan);
            
            var prevTint = GUI.backgroundColor;
            
            if (grid.Cursor.IsCurrentCellSelected)
                GUI.backgroundColor = grid.Grid.SelectedCellTintColor;
            else
                AlternateBackgroundColor(grid);

            using (var cellScope = new GUILayout.HorizontalScope(self.Style ?? Styles.ColorableCellStyle, self.Span ?? grid.DefaultCellSpan))
            {
                var r = GUILayoutUtility.GetRect(46f, 40f);
                GUI.DrawTexture(
                    new Rect(r.x + 6, r.y, 40f, 40f), 
                    self.Image ?? Texture2D.grayTexture, 
                    ScaleMode.ScaleToFit, 
                    true, 
                    1f,
                    Color.white, 
                    0, 
                    0
                );
                GUILayout.BeginVertical();
                {
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label(self.Title, Styles.Fixed20pxHeightTextStyle);
                        GUILayout.FlexibleSpace();
                        GUI.contentColor = Color.yellow;// Styles.GoldTextColor;
                        GUILayout.Label(self.Category, Styles.Fixed20pxHeightTextStyle);
                        GUI.contentColor = Color.white;
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    {
                        GUI.contentColor = Color.grey;
                        GUILayout.Label(self.Subtitle, Styles.Fixed20pxHeightTextStyle);
                        GUI.contentColor = Color.white;
                        GUILayout.FlexibleSpace();
                        GUILayout.Label(self.Tags, Styles.Fixed20pxHeightTextStyle);
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
                
                
            }

            GUI.backgroundColor = prevTint;
            GUI.enabled = true;
        }
        
        public static void Render(this CompositeCell self, VirtualGrid grid)
        {
            if (grid.IsLookingForCell)
                DoCellLookup(self, grid, self.Span ?? grid.DefaultCellSpan);
            
            var prevTint = GUI.backgroundColor;
            if (grid.Cursor.IsCurrentCellSelected)
                GUI.backgroundColor = grid.Grid.SelectedCellTintColor;
            else
                AlternateBackgroundColor(grid);
            
            GUILayout.BeginHorizontal(self.Style ?? Styles.CellStyle, self.Span ?? grid.DefaultCellSpan);
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
        
        public static readonly FieldInfo GUILayoutOptionValue = 
            typeof(GUILayoutOption).GetField("value", BindingFlags.NonPublic | BindingFlags.Instance);

        private static bool HitTest(Vector2 pos, VirtualGrid grid, GUILayoutOption span)
        {
            bool hit = false;
            
            if (grid.Cursor.Y <= pos.y && grid.Cursor.X <= pos.x && pos.y <= grid.Cursor.Y + grid.Grid.GetCellHeight())
            {
                hit = (grid.Columns.Length == 1 && grid.ExpandWidthOnSingleColumnGrid) ||
                      (pos.x <= grid.Cursor.X + (float) ((object) GUILayoutOptionValue.GetValue(span)));
            }

            return hit;
        }
        
        private static void DoCellLookup(ICell self, VirtualGrid grid, GUILayoutOption span)
        {
            if (HitTest(grid.TargetCellPosition, grid, span))
            {
                VirtualGrid.TriggerHitOnCell(self, grid);
            }
        }
        
        private static void AlternateBackgroundColor(VirtualGrid grid)
        {
            switch (grid.AlternateType)
            {
                case VirtualGridAlternateType.Columns:
                    GUI.backgroundColor = grid.Cursor.VisibleColumnIndex % 2 != 0
                        ? grid.Grid.CellTintColorAlt
                        : grid.Grid.CellTintColor;
                    break;
                case VirtualGridAlternateType.Rows:
                    GUI.backgroundColor = grid.Cursor.SectionRowIndex % 2 != 0
                        ? grid.Grid.CellTintColorAlt
                        : grid.Grid.CellTintColor;
                    break;
                case VirtualGridAlternateType.None:
                    GUI.backgroundColor = Color.white;
                    break;
            }
        }
    }
}