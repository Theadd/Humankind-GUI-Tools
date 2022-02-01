using System;
using Amplitude.Framework.Overlay;
using Amplitude.Framework.Runtime;
using Amplitude.Mercury.Overlay;
using Amplitude.UI;
using BepInEx.Configuration;
using Modding.Humankind.DevTools;
using UnityEngine;
using Modding.Humankind.DevTools.DeveloperTools.UI;
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
        

        protected virtual void Initialize()
        {
            if (!Modding.Humankind.DevTools.DevTools.QuietMode)
                Loggr.Log("Initializing BackScreenWindow...", ConsoleColor.Green);
            
            ViewController.Initialize();
            
            ScreenOverlay.SetInnerRectAsVisible(false);
            OnCollapse();

            Initialized = true;
        }

        protected virtual void InitializeInGame()
        {
            LiveEditorMode.Initialize();
            ScreenOverlay.SetInnerRectAsVisible(false);
            ToolboxController.Initialize(this);

            InitializedInGame = true;
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

        public override void OnDrawUI(int _)
        {
            
            GUILayout.BeginArea(new Rect(0f, 0f, WindowRect.width, WindowRect.height));
            GUILayout.BeginHorizontal();
            
            GUILayout.BeginVertical();
            
            GUILayout.Space(2f);
            
            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                GUILayout.BeginHorizontal();
                {
                    GUI.backgroundColor = Color.black;
                    DrawStatusBar();
                    GUI.backgroundColor = Color.white;
                }
                GUILayout.EndHorizontal();
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();

            
            if (IsInGame && ViewController.View == ViewType.InGame)
            {
                SyncInGameUI();
                LiveEditorMode.HexPainter.IsVisible =
                    LiveEditorMode.Enabled && LiveEditorMode.EditorMode == EditorModeType.TilePainter;
                LiveEditorMode.HexPainter.Draw();
                
                if (LiveEditorMode.Enabled)
                {
                    if (Event.current.type == EventType.Repaint)
                        LiveEditorMode.Run();

                    if (IsToolboxVisible != ToolboxController.Draw())
                    {
                        IsToolboxVisible = !IsToolboxVisible;

                        ScreenOverlay
                            .SetInnerRect(ToolboxController.ToolboxRect)
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

        private static string _normalModeTitle = string.Empty;
        private static string _freeCamModeTitle = string.Empty;
        private static string _liveEditorModeTitle = string.Empty;
        
        private static string _onPaintTitle = string.Empty;
        private static string _onEraseTitle = string.Empty;
        private static string _brushTypeTitle = string.Empty;
        private void UpdateDirtyStuff()
        {
            _normalModeTitle = (KeyMappings.TryGetKeyDisplayValue(
                "BackToNormalModeInGameView", out var normalText) ? 
                normalText + " " : "") + "<b>DEFAULT MODE</b>";
            _freeCamModeTitle = (KeyMappings.TryGetKeyDisplayValue(
                "ToggleFreeCameraMode", out var freeCamText) ? 
                freeCamText + " " : "") + "<b>FREE CAMERA MODE</b>";
            _liveEditorModeTitle = (KeyMappings.TryGetKeyDisplayValue(
                "ToggleLiveEditor", out var liveEditorText) ? 
                liveEditorText + " " : "") + "<b>LIVE EDITOR MODE</b>";
            
            
            _brushTypeTitle = (KeyMappings.TryGetKeyDisplayValue(
                LiveEditorMode.StickedToolboxActionName, out var brushKeyText) ? 
                brushKeyText + " " : "") + "BRUSH TYPE: ";
            _onPaintTitle = (KeyMappings.TryGetKeyDisplayValue(
                LiveEditorMode.CreateUnderCursorActionName, out var onPaintKeyText) ? 
                onPaintKeyText + " " : "") + "ON PAINT: ";
            _onEraseTitle = (KeyMappings.TryGetKeyDisplayValue(
                LiveEditorMode.DestroyUnderCursorActionName, out var onEraseKeyText) ? 
                onEraseKeyText + " " : "") + "ON ERASE: ";
            
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

        private void DrawStatusBar()
        {
            if (ViewController.View != ViewType.InGame)
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("<b>" + ViewController.View.ToString().ToUpper() + "</b>", ScreenTag);
                }
                GUILayout.EndHorizontal();
            }
            if (ViewController.View == ViewType.InGame)
            {
                GUILayout.BeginHorizontal();
                {
                    ViewModeToggleButton("BackToNormalModeInGameView", _normalModeTitle, ViewModeType.Normal);
                    ViewModeToggleButton("ToggleFreeCameraMode", _freeCamModeTitle, ViewModeType.FreeCamera);
                    ViewModeToggleButton("ToggleLiveEditor", _liveEditorModeTitle, ViewModeType.LiveEditor);
                    GUI.backgroundColor = Color.black;
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(12f);
                
                if (IsInGame && LiveEditorMode.Enabled)
                {
                    GUILayout.BeginHorizontal();
                    {
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
            }
        }

        private void SyncInGameUI()
        {
            if (_wasLiveEditorEnabled != LiveEditorMode.Enabled)
            {
                if (LiveEditorMode.Enabled)
                {
                    //ScreenLocker.LockFullScreenMouseEvents = true;
                }
                else
                {
                    IsToolboxVisible = false;
                    ScreenOverlay.SetInnerRectAsVisible(false);
                    ToolboxController.IsSticked = false;
                    ScreenLocker.LockFullScreenMouseEvents = false;
                    OnCollapse();
                }
                _wasLiveEditorEnabled = LiveEditorMode.Enabled;
            }
        }

        public override void OnZeroGUI()
        {
            // ViewController.Initialize();
            
            if (!Initialized)
                Initialize();
            
            if (IsDirty) 
                UpdateDirtyStuff();
            
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
