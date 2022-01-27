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
        
        public HexOverlay HexPainter { get; set; }

        protected bool IsToolboxVisible = false;

        public bool IsInGame { get; private set; } = false;

        public bool IsInLoadingScreen => Amplitude.Mercury.LoadingScreen.Visible;

        protected bool Initialized = false;
        protected bool InitializedInGame = false;
        

        protected virtual void Initialize()
        {
            ScreenOverlay.SetInnerRectAsVisible(false);
            OnCollapse();

            Initialized = true;
        }

        protected virtual void InitializeInGame()
        {
            HexPainter = new HexOverlay();
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
            RuntimeGameState.Refresh();
            
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

                    GUILayout.Label("LEFT ARROW", ScreenTag);
                    GUILayout.Label("INPUT STRING: " + Input.inputString, ScreenTag);
                    if (Amplitude.Mercury.Presentation.Presentation.IsPresentationRunning)
                        GUILayout.Label("IN GAME", ScreenTag);
                    if (Input.anyKey)
                        GUILayout.Label("ANY KEY", ScreenTag);
                    if (Input.anyKeyDown)
                        GUILayout.Label("ANY KEY DOWN", ScreenTag);
                    GUI.backgroundColor = Color.white;
                }
                GUILayout.EndHorizontal();
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();

            
            if (IsInGame)
            {
                HexPainter.Draw();

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
            
            
            GUILayout.EndVertical();
            
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
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
