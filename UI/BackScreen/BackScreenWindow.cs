using System;
using DevTools.Humankind.GUITools.Collections;
using Modding.Humankind.DevTools;
using UnityEngine;
using StyledGUI;

namespace DevTools.Humankind.GUITools.UI
{
    public class BackScreenWindow : BackScreenWindowBase
    {
        public override bool ShouldBeVisible => true;
        public override bool ShouldRestoreLastWindowPosition => false;
        public override string UniqueName => "BackScreenWindow";
        public Rect MinWindowRect { get; set; } = new Rect (0, 0, Screen.width, 24f);
        public Rect MaxWindowRect { get; set; } = new Rect (0, 0, Screen.width, Screen.height);
        
        protected bool IsToolboxVisible = false;

        public bool IsInGame { get; private set; } = false;

        public static bool IsDirty { get; set; }

        public bool IsInLoadingScreen => Amplitude.Mercury.LoadingScreen.Visible;

        protected bool Initialized = false;
        protected bool InitializedInGame = false;
        private bool _wasLiveEditorEnabled;
        private int _lastScreenWidth;
        private int _lastScreenHeight;
        

        protected virtual void Initialize()
        {
            ViewController.Initialize();
            
            ScreenOverlay
                .SetInnerRectAsVisible(false)
                .SetUIMarkerVisibility(false);
            OnCollapse();

            Initialized = true;
        }

        protected virtual void InitializeInGame()
        {
            SyncToScreenSize();
            LiveEditorMode.Initialize();
            ScreenOverlay
                .SetInnerRectAsVisible(false)
                .SetUIMarkerVisibility(false);
            ToolboxController.Initialize(this);

            InitializedInGame = true;
            ToolboxController.SetDocked(true);
        }

        public GUIStyle ScreenTag { get; set; } = new GUIStyle(Styles.ColorableCellStyle)
        {
            margin = new RectOffset(2, 0, 0, 0),
            padding = new RectOffset(8, 8, 4, 4),
            fontSize = 10
        };
        
        public GUIStyle ScreenTagToggleButton { get; set; } = new GUIStyle(Styles.ColorableCellToggleStyle)
        {
            margin = new RectOffset(2, 0, 0, 0),
            padding = new RectOffset(8, 8, 4, 4),
            fontSize = 10
        };

        private bool _drawOnLiveEditorEnabled = false;
        private bool _drawInGame = false;
        private bool _hexPainterVisible = false;
        
        public override void OnDrawUI(int _)
        {
            try
            {
                GUILayout.BeginArea(new Rect(0f, 0f, WindowRect.width, WindowRect.height));
                GUILayout.BeginHorizontal();

                GUILayout.BeginVertical();

                GUILayout.Space(2f);

                GUILayout.BeginHorizontal();
                {
                    if (!_drawOnToolboxVisible)
                        GUILayout.FlexibleSpace();
                    else
                        GUILayout.Space(ToolboxController.PanelWidth);

                    GUILayout.BeginHorizontal();
                    {
                        GUI.backgroundColor = Color.black;
                        DrawStatusBar();
                        GUI.backgroundColor = Color.white;
                    }
                    GUILayout.EndHorizontal();
                    if (!_drawOnToolboxVisible)
                        GUILayout.FlexibleSpace();
                }
                GUILayout.EndHorizontal();


                if (_drawInGame)
                {
                    SyncInGameUI();
                    LiveEditorMode.HexPainter.IsVisible = _hexPainterVisible;
                    LiveEditorMode.HexPainter.Draw();

                    if (_drawOnLiveEditorEnabled)
                    {
                        if (Event.current.type == EventType.Repaint)
                            LiveEditorMode.Run();

                        if (IsToolboxVisible != ToolboxController.Draw())
                        {
                            IsToolboxVisible = !IsToolboxVisible;

                            ScreenOverlay
                                .SetInnerRect(
                                    new Rect(
                                        ToolboxController.ToolboxRect.x, 
                                        ToolboxController.ToolboxRect.y, 
                                        ToolboxController.ToolboxRect.width - 1f, 
                                        ToolboxController.ToolboxRect.height))
                                .SetInnerRectAsVisible(IsToolboxVisible);

                            if (IsToolboxVisible)
                            {
                                OnExpand();
                            }
                            else
                            {
                                OnCollapse();
                            }
                        }
                    }
                }

                GUILayout.EndVertical();

                GUILayout.EndHorizontal();
                GUILayout.EndArea();
            }
            catch (Exception e)
            {
                Loggr.Log(e);
            }
            finally
            {
                if (Event.current.type == EventType.Repaint)
                {
                    _drawOnLiveEditorEnabled =
                        IsInGame && ViewController.View == ViewType.InGame && LiveEditorMode.Enabled;
                    _drawInGame = IsInGame && ViewController.View == ViewType.InGame;
                    _hexPainterVisible = _drawOnLiveEditorEnabled &&
                                         LiveEditorMode.EditorMode == EditorModeType.TilePainter;
                }

                if (GUI.tooltip.Length > 0)
                    TooltipOverlay.SetTooltip(StringHandle.Parse(GUI.tooltip));
                
                TooltipOverlay.Run();
            }
        }

        private static string _normalModeTitle = string.Empty;
        private static string _freeCamModeTitle = string.Empty;
        private static string _liveEditorModeTitle = string.Empty;
        private static string _overviewModeTitle = string.Empty;
        
        private static string _onPaintTitle = string.Empty;
        private static string _onEraseTitle = string.Empty;
        private static string _brushTypeTitle = string.Empty;
        private static string _toolbarTitle = string.Empty;
        private void UpdateDirtyStuff()
        {
            _normalModeTitle = (KeyMappings.TryGetKeyDisplayValue(
                "BackToNormalModeInGameView", out var normalText) ? 
                normalText + " " : "") + "<b>DEFAULT MODE</b>";
            _freeCamModeTitle = (KeyMappings.TryGetKeyDisplayValue(
                "ToggleFreeCameraMode", out var freeCamText) ? 
                freeCamText + " " : "") + "<b>FREE CAMERA</b>";
            _liveEditorModeTitle = (KeyMappings.TryGetKeyDisplayValue(
                "ToggleLiveEditor", out var liveEditorText) ? 
                liveEditorText + " " : "") + "<b>LIVE EDITOR</b>";
            _overviewModeTitle = (KeyMappings.TryGetKeyDisplayValue(
                "ToggleGameOverviewWindow", out var overviewText) ? 
                overviewText + " " : "") + "<b>OVERVIEW</b>";
            
            
            _brushTypeTitle = (KeyMappings.TryGetKeyDisplayValue(
                LiveEditorMode.StickedToolboxActionName, out var brushKeyText) ? 
                brushKeyText + " " : "") + "BRUSH TYPE: ";
            _onPaintTitle = (KeyMappings.TryGetKeyDisplayValue(
                LiveEditorMode.CreateUnderCursorActionName, out var onPaintKeyText) ? 
                onPaintKeyText + " " : "") + "ON PAINT: ";
            _onEraseTitle = (KeyMappings.TryGetKeyDisplayValue(
                LiveEditorMode.DestroyUnderCursorActionName, out var onEraseKeyText) ? 
                onEraseKeyText + " " : "") + "ON ERASE: ";
            _toolbarTitle = (KeyMappings.TryGetKeyDisplayValue(
                "ToggleHideToolbarWindow", out var toolbarKeyText) ? 
                toolbarKeyText + " " : "") + "<b>TOOLBAR</b>";
            
            IsDirty = false;
        }
        
        private void ViewModeToggleButton(string actionName, string displayText, ViewModeType viewMode)
        {
            var activeColor = (Color) new Color32(42, 42, 192, 240);
            var active = ViewController.ViewMode == viewMode;
            GUI.backgroundColor = active ? activeColor : Color.black;
            if (GUILayout.Toggle(active, displayText, ScreenTagToggleButton) != active)
            {
                KeyMappings.Trigger(actionName);
            }
        }

        private bool _drawNotInGameTags = false;
        private bool _drawInGameTags = false;
        private bool _drawLiveEditorTags = false;
        private bool _drawDefaultModeTags = false;
        private bool _drawOnToolboxVisible = false;
        
        private void DrawStatusBar()
        {
            GUILayout.Space(8f);
            if (_drawNotInGameTags)
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("<b>" + ViewController.View.ToString().ToUpper() + "</b>", ScreenTag);
                }
                GUILayout.EndHorizontal();
            }
            if (_drawInGameTags)
            {
                GUILayout.BeginHorizontal();
                {
                    ViewModeToggleButton("BackToNormalModeInGameView", _normalModeTitle, ViewModeType.Normal);
                    ViewModeToggleButton("ToggleFreeCameraMode", _freeCamModeTitle, ViewModeType.FreeCamera);
                    ViewModeToggleButton("ToggleLiveEditor", _liveEditorModeTitle, ViewModeType.LiveEditor);
                    ViewModeToggleButton("ToggleGameOverviewWindow", _overviewModeTitle, ViewModeType.Overview);
                    GUI.backgroundColor = Color.black;
                }
                GUILayout.EndHorizontal();
                if (!_drawOnToolboxVisible)
                    GUILayout.Space(12f);
                
                if (_drawLiveEditorTags)
                {
                    if (_drawOnToolboxVisible)
                        GUILayout.FlexibleSpace();
                    
                    GUILayout.BeginHorizontal();
                    {
                        if (!_drawOnToolboxVisible)
                            GUILayout.Label(
                                _brushTypeTitle + "<b>" + LiveEditorMode.BrushType.ToString().ToUpper() + "</b>", ScreenTag);
                        var onPaintText = PaintBrush.ActionNameOnCreate != string.Empty
                            ? _onPaintTitle + "<b>" + PaintBrush.ActionNameOnCreate + "</b>"
                            : "NO EFFECT ON PAINT";
                        var onEraseText = PaintBrush.ActionNameOnDestroy != string.Empty
                            ? _onEraseTitle + "<b>" + PaintBrush.ActionNameOnDestroy + "</b>"
                            : "NO EFFECT ON ERASE";
                        GUILayout.Label(onPaintText, ScreenTag);
                        GUILayout.Label(onEraseText, ScreenTag);
                    }
                    GUILayout.EndHorizontal();
                }
                else if (_drawDefaultModeTags)
                {
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label(_toolbarTitle, ScreenTag);
                    }
                    GUILayout.EndHorizontal();
                }
            }

            if (Event.current.type == EventType.Repaint)
            {
                _drawNotInGameTags = ViewController.View != ViewType.InGame;
                _drawInGameTags = ViewController.View == ViewType.InGame;
                _drawLiveEditorTags = IsInGame && LiveEditorMode.Enabled;
                _drawDefaultModeTags = ViewController.ViewMode == ViewModeType.Normal;
                _drawOnToolboxVisible = IsToolboxVisible;
            }
            GUILayout.Space(8f);
        }

        private bool _wasToolboxSticked = true;
        
        private void SyncInGameUI()
        {
            if (_wasLiveEditorEnabled != LiveEditorMode.Enabled)
            {
                if (LiveEditorMode.Enabled)
                {
                    if (_wasToolboxSticked)
                        ToolboxController.IsSticked = true;
                    //ScreenLocker.LockFullScreenMouseEvents = true;
                    InGameUIController.SetVisibilityOfInGameOverlays(false);
                }
                else
                {
                    IsToolboxVisible = false;
                    ScreenOverlay.SetInnerRectAsVisible(false);
                    InGameUIController.SetVisibilityOfInGameOverlays(true);
                    _wasToolboxSticked = ToolboxController.IsSticked;
                    ToolboxController.IsSticked = false;
                    ScreenLocker.LockFullScreenMouseEvents = false;
                    OnCollapse();
                }
                _wasLiveEditorEnabled = LiveEditorMode.Enabled;
            }
        }
        
        protected virtual void SyncToScreenSize()
        {
            MinWindowRect = new Rect (0, 0, Screen.width, 24f);
            MaxWindowRect = new Rect (0, 0, Screen.width, Screen.height);
            _lastScreenWidth = Screen.width;
            _lastScreenHeight = Screen.height;
            InGameUIController.AdaptUIForBackScreenToFit(this);
        }

        public override void OnZeroGUI()
        {
            if (MainTools.IsDebugModeEnabled && MainTools.IsTestingPlaygroundEnabled)
                TestingPlayground.OnZeroUpdate();
            
            if (!Initialized)
                Initialize();
            
            if (IsDirty) 
                UpdateDirtyStuff();

            if (InitializedInGame && (_lastScreenWidth != Screen.width || _lastScreenHeight != Screen.height))
                SyncToScreenSize();
            
            bool isInGame = Amplitude.Mercury.Presentation.Presentation.IsPresentationRunning;

            if (isInGame != IsInGame)
            {
                if (isInGame && !InitializedInGame)
                    InitializeInGame();

                IsInGame = isInGame;
            }
        }

        public void OnCollapse()
        {
            GUI.BringWindowToBack(WindowID);
            WindowRect = MinWindowRect;
        }

        public void OnExpand()
        {
            GUI.BringWindowToFront(WindowID);
            WindowRect = MaxWindowRect;
        }

    }
}
