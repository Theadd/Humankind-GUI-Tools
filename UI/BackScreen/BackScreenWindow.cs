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

        public bool IsInLoadingScreen => Amplitude.Mercury.LoadingScreen.Visible;

        protected bool Initialized = false;
        protected bool InitializedInGame = false;
        private bool _wasLiveEditorEnabled;
        

        protected virtual void Initialize()
        {
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

        public override void OnDrawUI(int _)
        {
            // TODO: RuntimeGameState.Refresh();
            
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

                    if (IsInGame && LiveEditorMode.Enabled)
                    {
                        GUILayout.Label("LIVE EDITOR MODE", ScreenTag);
                        if (LiveEditorMode.BrushType != LiveBrushType.None)
                            GUILayout.Label("BRUSH TYPE: <b>" + LiveEditorMode.BrushType.ToString().ToUpper() + "</b>", ScreenTag);
                        if (PaintBrush.ActionNameOnCreate != string.Empty)
                            GUILayout.Label("ON PAINT: <b>" + PaintBrush.ActionNameOnCreate + "</b>", ScreenTag);      
                        if (PaintBrush.ActionNameOnCreate != string.Empty)
                            GUILayout.Label("ON ERASE: <b>" + PaintBrush.ActionNameOnDestroy + "</b>", ScreenTag);
                    }

                    GUI.backgroundColor = Color.white;
                }
                GUILayout.EndHorizontal();
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();

            
            if (IsInGame)
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
            if (!Initialized)
                Initialize();
            
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
