using System;
using System.Collections.Generic;
using DevTools.Humankind.GUITools.UI.SceneInspector;
using Modding.Humankind.DevTools;
using StyledGUI.VirtualGridElements;
using UnityEngine;

namespace DevTools.Humankind.GUITools.UI
{
    public partial class DataTypeDefinitionsToolbox
    {
        public BackScreenWindow Window { get; set; }
        public DataTypeDefinitionsGrid TypeDefinitionsGrid { get; set; }
        public Vector2 ScrollViewPosition { get; set; } = Vector2.zero;
        public Rect ScrollViewRect { get; private set; } = Rect.zero;
        public Vector2 MousePosition { get; private set; } = Vector2.zero;
        public bool IsMouseHover { get; private set; } = false;
        public bool IsMouseHoverCell { get; private set; } = false;
        public IClickableImageCell CellWithMouseHover { get; private set; } = null;
        public float VScrollbarWidth { get; set; } = 10f;
        public RectOffset ScrollViewPadding { get; set; } = new RectOffset(8, 8, 0, 0);
        public int ActiveTab { get; private set; } = 0;

        private bool _waitingForClick = false;
        
        public void Draw(Rect targetRect)
        {
            GUILayout.BeginArea(targetRect);
            {
                if ((int)TypeDefinitionsGrid.FixedWidth != (int)(targetRect.width - VScrollbarWidth - ScrollViewPadding.left - ScrollViewPadding.right))
                    TypeDefinitionsGrid.FixedWidth = (int)(targetRect.width - VScrollbarWidth - ScrollViewPadding.left - ScrollViewPadding.right);
                
                Draw();
            }
            GUILayout.EndArea();
        }

        private bool _drawInspectorAdditionalContent = false;

        public void Draw()
        {
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();

            DrawToolboxHeader();
            
            DrawTabs();
            
            ScrollViewPosition = GUILayout.BeginScrollView(
                ScrollViewPosition, 
                false, 
                false, 
                "horizontalscrollbar",
                "verticalscrollbar",
                "scrollview");
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(ScrollViewPadding.left);
                GUILayout.BeginVertical(GUILayout.Width(TypeDefinitionsGrid.FixedWidth));
                {
                    // ADDITIONAL CONTENT PER ACTIVE TAB, IF ANY
                    if (_drawInspectorAdditionalContent)
                    {
                        SceneInspectorController.Screen.Draw();
                    }
                    //
                    
                    if (Event.current.type == EventType.MouseDown)
                    {
                        _waitingForClick = true;
                        TypeDefinitionsGrid.VirtualGrid.FindCellAtPosition(Event.current.mousePosition, OnClickHandler,
                            OnClickFallbackHandler);
                    }
                    else
                    {
                        if (!_waitingForClick && IsMouseHover && Event.current.type == EventType.Repaint)
                        {
                            TypeDefinitionsGrid.VirtualGrid.FindCellAtPosition(Event.current.mousePosition,
                                OnHoverHandler, OnHoverFallbackHandler);
                        }
                    }
                    
                    TypeDefinitionsGrid.Render();

                    GUILayout.Space(8f);
                }
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            if (Event.current.type == EventType.Repaint)
            {
                ScrollViewRect = GUILayoutUtility.GetLastRect();
                MousePosition = new Vector2(Event.current.mousePosition.x, Event.current.mousePosition.y);
                IsMouseHover = ScrollViewRect.Contains(MousePosition);
                _drawInspectorAdditionalContent = tabNames[ActiveTab] == InspectorTab;
            }
            GUILayout.FlexibleSpace();
            DrawSelectionPreview();
            GUILayout.Space(1f);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
        
        public void OnClickHandler(ICell cell)
        {
            TypeDefinitionsGrid.VirtualGrid.Cursor.AddToSelection();
            _waitingForClick = false;
        }
        
        public void OnClickFallbackHandler()
        {
            _waitingForClick = false;
        }
        
        public void OnHoverHandler(ICell cell)
        {
            IsMouseHoverCell = false;
            if (cell is IClickableImageCell icell)
            {
                CellWithMouseHover = icell;
                IsMouseHoverCell = true;
            }
        }
        
        public void OnHoverFallbackHandler()
        {
            IsMouseHoverCell = false;
        }
    }
}
