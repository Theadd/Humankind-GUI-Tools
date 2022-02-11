using System;
using System.Linq;
using Modding.Humankind.DevTools;
using StyledGUI.VirtualGridElements;
using UnityEngine;

namespace StyledGUI
{
    public class VirtualGridCell
    {
        public int RowIndex { get; set; }
        public int CellIndex { get; set; }
        public int CellSubIndex { get; set; }
        public int ColumnIndex { get; set; }
        public ICell Cell { get; set; }
    }
    
    public class VirtualGridCursor
    {
        public event Action OnSelectionChange;

        public VirtualGridSelectionType SelectionType { get; set; } = VirtualGridSelectionType.None;

        public VirtualGridCell DefaultSelectedCell { get; set; } = new VirtualGridCell();

        public VirtualGrid Owner { get; private set; }

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
        public int CellSubIndex { get; set; }
        
        /// <summary>
        ///     RowIndex within it's section
        /// </summary>
        public int SectionRowIndex { get; set; }

        public bool IsSelectionActive { get; private set; } = false;

        public float X { get; set; }
        
        public float Y { get; set; }
        
        public float LastWidth { get; set; }
        
        public float LastHeight { get; set; }

        private VirtualGridCell SelectedGridCell { get; set; }
        private bool _initialized = false;
        
        private ICell GetCurrentCell()
        {
            ICell currentCell = null;
            try
            {
                currentCell = Owner
                    .Sections[SectionIndex]
                    .Rows[SectionRowIndex]
                    .Cells.ElementAt(CellIndex);

                if (CellSubIndex != -1 && currentCell is CellGroup cellGroup)
                {
                    currentCell = cellGroup.Cells.ElementAt(CellSubIndex);
                }

                /*if (currentCell is Clickable4xCell cell)
                {
                    Loggr.Log(cell.Title, ConsoleColor.Green);
                }
                else if (currentCell is ClickableImageCell imageCell)
                {
                    Loggr.Log(imageCell.Title, ConsoleColor.Blue);
                }*/
            }
            catch (Exception e)
            {
                Loggr.Log(e);
                Loggr.Log(this);
                Loggr.Log(Owner.Sections[SectionIndex]);
                Loggr.Log(Owner.Sections[SectionIndex].Rows.Length.ToString());
            }

            return currentCell;
        }

        public void Reset()
        {
            SectionIndex = 0;
            ColumnIndex = 0;
            RowIndex = 0;
            CellIndex = 0;
            VisibleSectionIndex = 0;
            VisibleRowIndex = 0;
            CellSubIndex = 0;
            Y = 0;
            X = 0;
        }

        public void Initialize(VirtualGrid owner)
        {
            if (_initialized)
                throw new NotSupportedException("VirtualGridCursor already initialized.");

            Owner = owner;
            
            if (SelectionType == VirtualGridSelectionType.SingleCell)
            {
                SelectedGridCell = DefaultSelectedCell;
                IsSelectionActive = true;
            }

            _initialized = true;
        }

        public void SyncXY()
        {
            if (Event.current.type != EventType.Repaint)
                return;
            
            var r = GUILayoutUtility.GetLastRect();
            
            X = r.x;
            Y = r.y;
            LastWidth = r.width;
            LastHeight = r.height;
        }

        public VirtualGridCell SelectedCell => SelectedGridCell;

        public void AddToSelection()
        {
            switch (SelectionType)
            {
                case VirtualGridSelectionType.SingleCell:
                case VirtualGridSelectionType.OptionalSingleCell:
                    IsSelectionActive = true;
                    SelectedGridCell = new VirtualGridCell()
                    {
                        RowIndex = RowIndex,
                        ColumnIndex = ColumnIndex,
                        CellIndex = CellIndex,
                        CellSubIndex = CellSubIndex,
                        Cell = GetCurrentCell()
                    };
                    InvokeOnSelectionChange();
                    
                    break;
                case VirtualGridSelectionType.None:
                    IsSelectionActive = false;
                    break;
            }
        }

        public void ClearSelection()
        {
            switch (SelectionType)
            {
                case VirtualGridSelectionType.OptionalSingleCell:
                    IsSelectionActive = false;
                    SelectedGridCell = null;
                    break;
                case VirtualGridSelectionType.SingleCell:
                    IsSelectionActive = true;
                    SelectedGridCell = DefaultSelectedCell;
                    break;
            }
            
            InvokeOnSelectionChange();
        }


        public bool IsCurrentCellSelected =>
            IsSelectionActive && 
            SelectedGridCell.RowIndex == RowIndex && 
            SelectedGridCell.ColumnIndex == ColumnIndex &&
            SelectedGridCell.CellIndex == CellIndex && 
            (
                SelectedGridCell.CellSubIndex == -1 || 
                (
                    SelectedGridCell.CellSubIndex != -1 && 
                    SelectedGridCell.CellSubIndex == CellSubIndex
                )
            );
        
        private void InvokeOnSelectionChange()
        {
            OnSelectionChange += Dummy;
            if (!BepInEx.Utility.TryDo(OnSelectionChange, out Exception ex))
                Loggr.Log(ex);
            OnSelectionChange -= Dummy;
        }
        
        private static void Dummy() { }
    }
}