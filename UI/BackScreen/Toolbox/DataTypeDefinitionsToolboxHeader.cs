using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Modding.Humankind.DevTools;
using Modding.Humankind.DevTools.DeveloperTools;
using Modding.Humankind.DevTools.DeveloperTools.UI;
using StyledGUI;
using UnityEngine;

namespace DevTools.Humankind.GUITools.UI
{
    public partial class DataTypeDefinitionsToolbox
    {

        private static string[] tabNames =
        {
            "DISTRICTS",
            "UNITS", 
            "COLLECTIBLES", 
            "MAP MARKER", 
            "UNITS", 
            "COLLECTIBLES", 
            "MAP MARKER", 
        };
        
        private List<Vector2> _storedScrollViewPositions = tabNames.Select(n => Vector2.zero).ToList();

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
        
        private Rect _scrollViewTabsRect = Rect.zero;
        private bool _scrollToActiveTabRequested = false;

        private GUIStyle HScrollbarTabs { get; set; } = new GUIStyle(UIController.DefaultSkin.FindStyle("noscrollbar"))
        {
            name = "noscrollbar",
            overflow = new RectOffset(0, 0, 0, 0),
            margin = new RectOffset(0, 0, -1, 0),
        };
        
        private GUIStyle ImageLinkStyle { get; set; } = new GUIStyle(UIController.DefaultSkin.FindStyle("Link")) {
            alignment = TextAnchor.MiddleCenter,
            font = Utils.SourceCodeProRegularFont,
            fontSize = 23,
            margin = new RectOffset(0, 0, 4, 0),
            padding = new RectOffset(8, 8, 0, 0),
            fixedHeight = 26f,
            normal = new GUIStyleState()
            {
                background = UIController.DefaultSkin.FindStyle("Link").normal.background,
                // background = Utils.BlackTexture,
                textColor = Color.white
            }
        };

        private static Color TabNormalTextColor { get; set; } = new Color32(220, 220, 220, 240);
        private static Color TabOnNormalTextColor { get; set; } = new Color32(16, 123, 235, 245);
        private static Color DisabledLinkColor { get; set; } = new Color32(250, 250, 250, 100);
        private static GUIStyle CustomTabButtonStyle { get; set; } = new GUIStyle(UIController.DefaultSkin.FindStyle("TabButton"))
        {
            fontStyle = FontStyle.Normal,
            alignment = TextAnchor.MiddleCenter,
            overflow = new RectOffset(0, 0, 0, 0),
            margin = new RectOffset(0, 1, 5, 0),
            padding = new RectOffset(8, 8, 0, -2),
            fixedHeight = 26f,
            normal = new GUIStyleState()
            {
                background  = UIController.DefaultSkin.FindStyle("TabButton").hover.background,
                textColor = TabNormalTextColor
            },
            onNormal = new GUIStyleState()
            {
                background  = StyledGUI.Graphics.White215Texture,
                textColor = TabOnNormalTextColor
            },
            onHover = new GUIStyleState()
            {
                background  = StyledGUI.Graphics.White215Texture,
                textColor = TabOnNormalTextColor
            }
        };
        
        
            
        
        // private GUIContent ScrollLeftLinkContent { get; set; } = new GUIContent("<size=27>⇦⇦ ◁ ◀ ← ◀ « ← </size>");
        private GUIContent ScrollLeftLinkContent { get; set; } = new GUIContent("⇦");
        private GUIContent ScrollRightLinkContent { get; set; } = new GUIContent("⇨");


        private CustomToolbar Toolbar { get; set; } = new CustomToolbar()
        {
            ButtonSize = GUI.ToolbarButtonSize.FitToContents,
            Style = CustomTabButtonStyle
        };

        private SmoothScroller TabsScroller { get; set; }

        private GUIContent[] TabContents { get; set; } = tabNames
            .Select(t => new GUIContent("<size=13><b>" + t + "</b></size>"))
            .ToArray();

        private void DrawTabs()
        {
            if (TabsScroller == null)
                TabsScroller = new SmoothScroller();

            TabsScroller.Run();

            GUILayout.Space(6f);
            
            GUILayout.BeginHorizontal();
            GUILayout.Space(4f);

            GUI.enabled = ActiveTab > 0;
            GUI.color = ActiveTab > 0 ? Color.white : DisabledLinkColor;
            if (GUILayout.Button(ScrollLeftLinkContent, ImageLinkStyle, GUILayout.ExpandWidth(false)))
            {
                if (ActiveTab > 0)
                    SetActiveTab(ActiveTab - 1);
            }

            GUI.enabled = true;
            GUI.color = Color.white;

            int activeTab;
            
            GUILayout.BeginScrollView(
                TabsScroller.Value, 
                true, 
                false, 
                HScrollbarTabs,
                "noscrollbar",
                "scrollview",
                GUILayout.ExpandWidth(true));
            {
                activeTab = Toolbar.Draw(ActiveTab, TabContents);
            }
            GUILayout.EndScrollView();

            if (Event.current.type == EventType.Repaint)
            {
                _scrollViewTabsRect = GUILayoutUtility.GetLastRect();
                
                if (_scrollToActiveTabRequested)
                {
                    ScrollToActiveTab();
                    _scrollToActiveTabRequested = false;
                }
            }
            
            if (activeTab != ActiveTab)
            {
                SetActiveTab(activeTab);
            }
            
            GUI.enabled = ActiveTab + 1 < tabNames.Length;
            GUI.color = ActiveTab + 1 < tabNames.Length ? Color.white : DisabledLinkColor;
            if (GUILayout.Button(ScrollRightLinkContent, ImageLinkStyle, GUILayout.ExpandWidth(false)))
            {
                if (ActiveTab + 1 < tabNames.Length)
                    SetActiveTab(ActiveTab + 1);
            }
            GUI.enabled = true;
            GUI.color = Color.white;
            GUILayout.Space(4f);
            
            GUILayout.EndHorizontal();
            Utils.DrawHorizontalLine();
        }

        public void SetActiveTab(int index)
        {
            if (!(index >= 0 && index < tabNames.Length))
                return;
            
            _storedScrollViewPositions[ActiveTab] = ScrollViewPosition;
            ScrollViewPosition = _storedScrollViewPositions[index];
            ActiveTab = index;
            TypeDefinitionsGrid.VirtualGrid.VisibleViews = new[] { ActiveTab };
            _scrollToActiveTabRequested = true;
        }

        private void ScrollToActiveTab()
        {
            // _scrollViewTabsPos = (80.0, 0.0)
            // _scrollViewTabsRect = (x:36.00, y:55.00, width:445.00, height:31.00)
            // ActiveTabSize [xMin, xMax] = [159, 266], Length: 107
            // ToolbarSize = (616.0, 26.0) 
                
            var tabSize = Toolbar.Sizes[ActiveTab];

            int r = ((int)_scrollViewTabsRect.width - tabSize.length) / 2;
            int xMaxScrollRange = (int) (Toolbar.ToolbarSize.x - _scrollViewTabsRect.width);
            int scrollViewNextX = Mathf.Min(Mathf.Max(0, tabSize.start - r), xMaxScrollRange);

            // _scrollViewTabsPos.x = scrollViewNextX;
            TabsScroller?.ScrollTo(scrollViewNextX);
            
            // Loggr.Log("_scrollViewTabsPos = " + _scrollViewTabsPos.ToString(), ConsoleColor.DarkCyan);
            Loggr.Log("_scrollViewTabsRect = " + _scrollViewTabsRect.ToString(), ConsoleColor.DarkCyan);
            Loggr.Log("ActiveTabSize [xMin, xMax] = [" + tabSize.start + ", " + tabSize.end + "], Length: " + tabSize.length  
                      , ConsoleColor.DarkCyan);
            Loggr.Log("ToolbarSize = " + Toolbar.ToolbarSize.ToString(), ConsoleColor.DarkCyan);
        }
    }
}
