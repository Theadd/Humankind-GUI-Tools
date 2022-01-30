using System.Collections.Generic;
using System.Linq;
using System;
using Modding.Humankind.DevTools;
using UnityEngine;
using StyledGUI.VirtualGridElements;

namespace StyledGUI
{
    public enum VirtualGridAlternateType
    {
        None,
        Columns,
        Rows
    }
    
    public enum VirtualGridSelectionType
    {
        None,
        SingleCell,
        OptionalSingleCell
    }
    
    public class VirtualGrid
    {
        public VirtualGridAlternateType AlternateType { get; set; } = VirtualGridAlternateType.Columns;
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
        public GUILayoutOption DefaultCellSpan { get; private set; }

        public VirtualGridCursor Cursor
        {
            get => _cursor;
            set
            {
                _cursor = value;
                _cursor.Initialize(this);
            }
        }

        private static GUILayoutOption LooseCellSpan { get; } = GUILayout.ExpandWidth(true);
        private int[] _distribution = null;
        private bool _isExplicitDistribution = false;
        private IStyledGrid _grid;
        private bool _isLookingForCell = false;
        private Action<ICell> OnTargetCellFound { get; set; }
        private Action OnTargetCellNotFound { get; set; }
        private bool _isAnyCellFound = false;
        private VirtualGridCursor _cursor = new VirtualGridCursor();

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
                    
                    Cursor.SectionRowIndex = rowIndex;
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
                    Cursor.RowIndex++;
                    Cursor.VisibleRowIndex++;
                }
                
                Cursor.VisibleSectionIndex++;
                Cursor.SectionIndex++;
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
            try
            {
                grid.OnTargetCellFound?.Invoke(cell);
            }
            catch (Exception ex)
            {
                Loggr.Log(ex);
            }
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

            if (Columns.Length == 1 && ExpandWidthOnSingleColumnGrid)
                RowHeaderCellSpan = LooseCellSpan;
        }
        
        private void SetDistribution(int[] distribution, bool isExplicit = false)
        {
            if (isExplicit)
                _isExplicitDistribution = true;

            _distribution = distribution;
        }
    }
}
