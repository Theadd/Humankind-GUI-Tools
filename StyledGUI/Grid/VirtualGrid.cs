using System.Collections.Generic;
using System.Linq;
using System;
using Modding.Humankind.DevTools;
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
        /// <summary>
        ///     Horizontal visibility distribution
        /// </summary>
        public int[] Distribution { get => _distribution; set => SetDistribution(value, true); }
        /// <summary>
        ///     Visibility of sections
        /// </summary>
        public int[] VisibleViews { get; set; } = null;
        public bool ExpandWidthOnSingleColumnGrid { get; set; } = true;
        public Vector2 TargetCellPosition { get; private set; } = Vector2.zero;
        public bool IsLookingForCell => _isLookingForCell && Event.current.type == EventType.Repaint;
        public VirtualGridCursor Cursor { get; } = new VirtualGridCursor();
        public GUILayoutOption DefaultCellSpan { get; private set; }

        private static GUILayoutOption LooseCellSpan { get; } = GUILayout.ExpandWidth(true);
        private int[] _distribution = null;
        private bool _isExplicitDistribution = false;
        private IStyledGrid _grid;
        private bool _isLookingForCell = false;
        private Action<ICell> OnTargetCellFound { get; set; }
        private Action OnTargetCellNotFound { get; set; }
        private bool _isAnyCellFound = false;

        public void FindCellAtPosition(Vector2 position, Action<ICell> callbackOnSuccess, Action callbackOnFailure = null)
        {
            _isLookingForCell = true;
            TargetCellPosition = position;
            OnTargetCellFound = callbackOnSuccess;
            OnTargetCellNotFound = callbackOnFailure;
            _isAnyCellFound = false;
        }

        public void Render()
        {
            if (_distribution == null || (!_isExplicitDistribution && _distribution.Length != Columns.Length))
                SetDistribution(Columns.Select((e, i) => i).ToArray());
            
            if (Columns.Length < 1) return;
            
            Cursor.Reset();
            UpdateColumnCellSpan();

            for (var sectionIndex = 0; sectionIndex < Sections.Length; sectionIndex++)
            {
                ref var section = ref Sections[sectionIndex];
                
                if (VisibleViews != null && !VisibleViews.Contains(section.View))
                {
                    Cursor.RowIndex += section.Rows?.Length ?? 0;
                    Cursor.SectionIndex++;
                    continue;
                }

                Cursor.VisibleSectionIndex++;
                Cursor.SectionIndex++;

                // START drawing Section header
                if (section.SpaceBefore != 0)
                {
                    Grid.Space(section.SpaceBefore);
                    Cursor.SyncXY();
                    Cursor.Y += Cursor.LastHeight;
                }
                if (DrawSectionHeaders && section.Title.Length > 0)
                {
                    if (DrawRowHeaders)
                    {
                        Grid.Row(Styles.StaticRowStyle)
                            .VerticalStack()
                            .RowHeader(section.Title, RowHeaderCellSpan)
                            .DrawHorizontalLine(0.5f, SectionHorizontalLineWidth)
                            .EndVerticalStack()
                            .EndRow();
                    }
                    else
                    {
                        Grid.Row(Styles.StaticRowStyle)
                            .RowHeader(section.Title, RowHeaderCellSpan)
                            .EndRow();
                    }
                    Cursor.SyncXY();
                    Cursor.Y += Cursor.LastHeight;
                }
                //
                
                for (var rowIndex = 0; rowIndex < section.Rows.Length; rowIndex++)
                {
                    var row = section.Rows[rowIndex];
                    Cursor.RowIndex++;
                    Cursor.VisibleRowIndex++;
                    Cursor.X = 0;

                    // START drawing Row
                    Grid.Row(row.Style ?? Styles.RowStyle);
                    if (DrawRowHeaders)
                    {
                        Grid.RowHeader(row.Title, RowHeaderCellSpan);
                        Cursor.SyncXY();
                        Cursor.X += Cursor.LastWidth;
                    }

                    Distribution
                        .Select(index => row.Cells.ElementAt(index))
                        .Render(this, true);
                    Grid.EndRow();
                    //

                    Cursor.X = 0;
                    Cursor.Y += Cursor.LastHeight;
                }
                
            }

            if (IsLookingForCell)
            {
                if (!_isAnyCellFound)
                    OnTargetCellNotFound?.Invoke();
                
                _isLookingForCell = false;
                _isAnyCellFound = false;
                TargetCellPosition = Vector2.zero;
                OnTargetCellFound = null;
                OnTargetCellNotFound = null;
            }
        }
        
        public static void TriggerHitOnCell(ICell cell, VirtualGrid grid)
        {
            if (!grid.IsLookingForCell)
                return;

            grid._isAnyCellFound = true;
            grid.OnTargetCellFound?.Invoke(cell);
        }
        
        private void SetStyledGrid(IStyledGrid grid)
        {
            _grid = grid;
            if (RowHeaderCellSpan == null)
                RowHeaderCellSpan = _grid.CellSpan6;
            if (SectionHorizontalLineWidth < 0)
                SectionHorizontalLineWidth = _grid.GetCellWidth() * 6;
        }

        private void UpdateColumnCellSpan()
        {
            DefaultCellSpan = Columns.Length == 1 && ExpandWidthOnSingleColumnGrid
                ? ColumnCellSpan ?? LooseCellSpan
                : ColumnCellSpan ?? _grid.CellSpan4;
        }
        
        private void SetDistribution(int[] distribution, bool isExplicit = false)
        {
            if (isExplicit)
                _isExplicitDistribution = true;

            _distribution = distribution;
        }
    }

    public class VirtualGridCursor
    {
        /// <summary>
        ///     Index of Section[] currently being drawn or the one in which the action is triggered.
        /// </summary>
        public int SectionIndex { get; set; }
        /// <summary>
        ///     Section[]'s current index, not taking into account those sections filtered out using the VisibleViews property.
        /// </summary>
        public int VisibleSectionIndex { get; set; }
        /// <summary>
        ///     The same as SectionIndex but for Rows instead.
        /// </summary>
        public int RowIndex { get; set; }
        /// <summary>
        ///     The same as VisibleSectionIndex but for Rows instead.
        /// </summary>
        public int VisibleRowIndex { get; set; }
        public int ColumnIndex { get; set; }
        public int VisibleColumnIndex { get; set; }
        /// <summary>
        ///     Index of Cell within the within a CellGroup, or 0 if there's a single Cell within the Column in that row.
        /// </summary>
        public int CellIndex { get; set; }
        /// <summary>
        ///     RowIndex within it's section
        /// </summary>
        public int SectionRowIndex { get; set; }
        public int SelectedRowIndex { get; set; }
        public int SelectedCellIndex { get; set; }
        public int SelectedColumnIndex { get; set; }

        public float X { get; set; }
        public float Y { get; set; }
        public float LastWidth { get; set; }
        public float LastHeight { get; set; }

        public void Reset()
        {
            SectionIndex = 0;
            ColumnIndex = 0;
            RowIndex = 0;
            CellIndex = 0;
            VisibleSectionIndex = 0;
            VisibleRowIndex = 0;
            Y = 0;
            X = 0;
        }

        public void SyncXY()
        {
            if (Event.current.type != EventType.Repaint)
                return;
            
            var r = GUILayoutUtility.GetLastRect();
            // Loggr.Log(Event.current.type.ToString() + r.ToString(), ConsoleColor.Cyan);
            X = r.x;
            Y = r.y;
            LastWidth = r.width;
            LastHeight = r.height;
        }
    }
}
