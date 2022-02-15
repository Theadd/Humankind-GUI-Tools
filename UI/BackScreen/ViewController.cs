using System;
using Amplitude.Framework;
using Amplitude.Framework.Presentation;
using Amplitude.Mercury.Presentation;
using Amplitude.Mercury.UI;
using DevTools.Humankind.GUITools.UI.PauseMenu;
using Modding.Humankind.DevTools;

namespace DevTools.Humankind.GUITools.UI
{
    public enum ViewType
    {
        Loading,    //EmptyView
        OutGame,
        InGame,
        MapEditor,
        ShuttingDown
    }
    
    [Flags]
    public enum ViewModeType
    {
        None = 0,
        Normal = 1 << 0,
        LiveEditor = 1 << 1,
        FreeCamera = 1 << 2,
        Overview = 1 << 3,
        KeyboardShortcuts = 1 << 4,
        GameMenu = 1 << 5,
        CameraSequence = 1 << 6,
        EndGame = 1 << 7,
        
        Auto = 1 << 8,
        All = ~(-1 << 9)
    }
    
    public static class ViewController
    {
        public static ViewType View { get; private set; } = ViewType.Loading;
        public static ViewModeType ViewMode
        {
            get => _viewMode;
            set => SetViewMode(value);
        }
        public static UIState State { get; private set; } = UIState.Undefined;
        
        private static ViewModeType _viewMode = ViewModeType.Normal;
        private static bool _registeredToViewChanges = false;
        public static IViewService ViewService { get; private set; }
        public static IUIService UIService { get; private set; }

        private static void OnUIStateChange(UIState newState)
        {
            if (!Modding.Humankind.DevTools.DevTools.QuietMode)
                Loggr.Log("IN OnUIStateChange(newState = " + newState.ToString() + ")", ConsoleColor.DarkCyan);

            if (newState == UIState.ShuttingDown || newState == UIState.Shutdown)
                View = ViewType.ShuttingDown;
            
            State = newState;
        }

        public static void Initialize(bool onErrorIgnore = false)
        {
            if (ViewService == null)
                ViewService = Services.GetService<IViewService>();

            if (ViewService == null && !onErrorIgnore)
                Loggr.Log(new NullReferenceException(
                    "VIEWSERVICE INVALID AFTER CALL TO INITIALIZE IN RUNTIMEGAMESTATE."));
            
            if (ViewService != null && !_registeredToViewChanges)
            {
                if (!_registeredToViewChanges)
                    RegisterToViewChangeEvents();
            }
            
            if (UIService == null)
            {
                UIService = Services.GetService<IUIService>();
                
                if (UIService == null && !onErrorIgnore)
                    Loggr.Log(new NullReferenceException(
                        "UIService not expected to be null after requesting it."));

                if (UIService != null)
                {
                    UIService.UIStateChange -= OnUIStateChange;
                    UIService.UIStateChange += OnUIStateChange;
                    OnUIStateChange(UIService.UIState);
                }
            }
        }

        public static void OnViewChange(Action action)
        {
            if (!_registeredToViewChanges)
            {
                Loggr.Log(new NullReferenceException(
                    "Unable to register for view change events, ViewController has not been initialized yet."));
                return;
            }
            ViewService.ViewChange += (object s, ViewChangedEventArgs v) =>
            {
                try
                {
                    action?.Invoke();
                }
                catch (Exception e)
                {
                    Loggr.Log(e);
                }
            };
        }

        private static void RegisterToViewChangeEvents()
        {
            ViewService.ViewChange += OnViewChanged;
            _registeredToViewChanges = true;
            ApplyViewType(ViewService.View);
        }

        public static void OnViewChanged(object sender, ViewChangedEventArgs eventArgs)
        {
            if (!Modding.Humankind.DevTools.DevTools.QuietMode)
                Loggr.Log("OnViewChanged => " + eventArgs.View.Name, ConsoleColor.DarkCyan);

            ApplyViewType(eventArgs.View);
        }

        private static void ApplyViewType(IView view)
        {
            if (view is InGameView) View = ViewType.InGame;
            if (view is OutGameView) View = ViewType.OutGame;
            if (view is MapEditorView) View = ViewType.MapEditor;
            if (view is EmptyView) View = ViewType.Loading;
        }
        
        private static void SetViewMode(ViewModeType value)
        {
            ViewModeType next = View == ViewType.InGame ? ValidateNextViewMode(value) : ViewModeType.Normal;
            
            if (next != ViewModeType.GameMenu && InGameMenuController.IsVisible)
                InGameMenuWindow.CloseInGameMenu();
            
            if (next != ViewModeType.FreeCamera && FreeCameraController.Enabled)
                ActionController.ToggleFreeCameraMode();
            
            if (next != ViewModeType.LiveEditor && LiveEditorMode.Enabled)
                ActionController.ToggleLiveEditorMode();
            
            if (next != ViewModeType.EndGame && MainTools.IsEndGameWindowEnabled)
                MainTools.CloseEndGameStatisticsWindow();
            
            if (next != ViewModeType.Overview && MainTools.IsGameOverviewEnabled)
                MainTools.ToggleGameOverviewWindow();

            _viewMode = next;
        }

        private static ViewModeType ValidateNextViewMode(ViewModeType value)
        {
            ViewModeType next = value;

            if (value == ViewModeType.Auto)
            {
                next = ViewModeType.Normal;
                
                if (InGameMenuController.IsVisible) next = ViewModeType.GameMenu;
                if (FreeCameraController.Enabled) next = ViewModeType.FreeCamera;
                if (LiveEditorMode.Enabled) next = ViewModeType.LiveEditor;
                if (MainTools.IsEndGameWindowEnabled) next = ViewModeType.EndGame;
                if (MainTools.IsGameOverviewEnabled) next = ViewModeType.Overview;
            }

            return next;
        }
    }
}
