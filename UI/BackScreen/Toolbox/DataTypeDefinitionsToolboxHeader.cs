using System;
using System.Linq;
using System.Reflection;
using Modding.Humankind.DevTools;
using Modding.Humankind.DevTools.DeveloperTools.UI;
using UnityEngine;

namespace DevTools.Humankind.GUITools.UI
{
    public partial class DataTypeDefinitionsToolbox
    {
        public string TextFilter { get; set; } = string.Empty;

        /*private string[] tabNames =
        {
            "<size=14><b> DISTRICTS </b></size>",
            "<size=15><b> UNITS </b></size>", 
            "<size=16><b> COLLECTIBLES </b></size>", 
            "<size=15><b> MARKER </b></size>", 
            "<size=14><b> MARKER </b></size>", 
            "<size=13><b> MARKER </b></size>", 
            "<size=12><b> MARKER </b></size>", 
            "<size=11><b> MARKER </b></size>", 
        };*/
        
        private string[] tabNames =
        {
            "DISTRICTS",
            "UNITS", 
            "COLLECTIBLES", 
            "MARKER", 
            "MARKER", 
            "MARKER", 
            "MARKER", 
            "MARKER", 
        };
        
        private string[] displayModeNames =
        {
            "<size=10><b>LIST</b></size>", 
            "<size=10><b>GRID</b></size>"
        };
        
        private void DrawToolboxHeader()
        {
            GUILayout.Space(12f);
            
                

            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(10f);
                
                if (GUILayout.Button("<size=10><b>DESELECT</b></size>", GUILayout.ExpandWidth(false)))
                    ToolboxController.Toolbox.TypeDefinitionsGrid.VirtualGrid.Cursor.ClearSelection();
                
                // GUILayout.FlexibleSpace();
                
                if (GUILayout.Button("<size=10><b>RELOAD</b></size>", GUILayout.ExpandWidth(false)))
                {
                    DataTypeStore.Rebuild();
                    TypeDefinitionsGrid.Snapshot = new DataTypeStoreSnapshot()
                    {
                        Districts = DataTypeStore.Districts,
                        Units = DataTypeStore.Units,
                        Curiosities = DataTypeStore.Curiosities
                    };
                    TypeDefinitionsGrid.IsDirty = true;
                    Loggr.Log(TextFilterStyle);
                }
                
                GUILayout.Space(6f);
                
                DrawToolboxSearchBar();
                
                GUILayout.Space(6f);

                GUI.enabled = ToolboxController.Toolbox.TypeDefinitionsGrid.GridModeChunkSize > 1 && ToolboxController.IsDisplayModeGrid;
                if (GUILayout.Button("<size=15><b> ＋</b></size>", GUILayout.Width(22f), GUILayout.Height(21f), GUILayout.ExpandWidth(false)))
                    ToolboxController.Toolbox.TypeDefinitionsGrid.GridModeChunkSize -= 1;
                
                GUI.enabled = ToolboxController.Toolbox.TypeDefinitionsGrid.GridModeChunkSize < 12 && ToolboxController.IsDisplayModeGrid;
                if (GUILayout.Button("<size=13><b>—</b></size>", GUILayout.Width(22f), GUILayout.Height(21f), GUILayout.ExpandWidth(false)))
                    ToolboxController.Toolbox.TypeDefinitionsGrid.GridModeChunkSize += 1;

                GUI.enabled = true;
                GUILayout.Space(6f);

                var shouldDisplayAsGrid =
                    (GUILayout.Toolbar(ToolboxController.IsDisplayModeGrid ? 1 : 0, displayModeNames, GUILayout.ExpandWidth(false)) == 1);

                if (shouldDisplayAsGrid != ToolboxController.IsDisplayModeGrid)
                {
                    ToolboxController.IsDisplayModeGrid = shouldDisplayAsGrid;
                }

                GUILayout.Space(8f);
            }
            GUILayout.EndHorizontal();
            
            GUILayout.Space(12f);
        }

        private GUIStyle TextFilterStyle { get; set; } = new GUIStyle(UIController.DefaultSkin.textField)
        {
            fontSize = 10,
            fontStyle = FontStyle.Bold,
            stretchWidth = true,
            padding = new RectOffset(8, 8, 0, 0),
            margin = UIController.DefaultSkin.button.margin,
            overflow = new RectOffset(0, /*-3*/ 0, 0, 0),
        };
        
        private bool _isInputFilterDirty = false;
        private bool _didInputFilterGetFocus = false;
        private bool _justFocused = false;
        private string _focusedControlName = string.Empty;
        private const string InputFilterControlName = "ToolboxInputFilter";
        private TextEditor _textEditor;
        
        private void DrawToolboxSearchBar()
        {
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
            {
                var textFilter = TypeDefinitionsGrid.FilterBy;

                GUI.SetNextControlName(InputFilterControlName);
                var text = GUILayout.TextField(textFilter, TextFilterStyle, GUILayout.Height(21f),
                    GUILayout.ExpandWidth(true));
                _focusedControlName = GUI.GetNameOfFocusedControl();
                    
                if (_focusedControlName == InputFilterControlName)
                    _textEditor = (TextEditor) GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);

                if (_justFocused && _textEditor != null)
                {
                    _textEditor.ReplaceSelection(_textEditor.SelectedText);
                    _justFocused = false;
                }
                
                if (text != textFilter || (!_didInputFilterGetFocus && Event.current.keyCode == KeyCode.Backspace))
                {
                    textFilter = text.ToUpper();
                    TypeDefinitionsGrid.FilterBy = textFilter;

                    if (!_didInputFilterGetFocus)
                    {
                        _didInputFilterGetFocus = true;
                        _isInputFilterDirty = true;
                    }
                }

                if (_isInputFilterDirty && _focusedControlName != InputFilterControlName)
                {
                    FocusFilterTextField(false);
                    _isInputFilterDirty = false;
                }
            }
            GUILayout.EndHorizontal();
        }

        public void FocusFilterTextField(bool selectAll = true)
        {
            GUI.FocusControl(InputFilterControlName);
            _justFocused = !selectAll;
        }
        
        private void DrawTabs()
        {
            GUILayout.Space(6f);
            GUILayout.BeginHorizontal();
            GUILayout.Space(12f);

            var activeTab = GUILayout.Toolbar(ActiveTab,
                tabNames.Select((t, i) => i == ActiveTab 
                    ? "<b>" + t + "</b>" 
                    : t).ToArray(), 
                "TabButton", GUI.ToolbarButtonSize.FitToContents);

            if (activeTab != ActiveTab)
            {
                Loggr.Log("ACTIVE TAB = " + activeTab, ConsoleColor.Magenta);
                // Loggr.Log(UIController.DefaultSkin.textField);
                _storedScrollViewPositions[ActiveTab] = ScrollViewPosition;
                ScrollViewPosition = _storedScrollViewPositions[activeTab];
                ActiveTab = activeTab;
                TypeDefinitionsGrid.VirtualGrid.VisibleViews = new[] { ActiveTab };
            }
            GUILayout.EndHorizontal();
            Utils.DrawHorizontalLine();
        }
    }
}
