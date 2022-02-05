using Modding.Humankind.DevTools.DeveloperTools.UI;

namespace DevTools.Humankind.GUITools.UI
{
    public static class MappedActions
    {
        public static void ToggleGameOverviewWindow()
        {
            if (ViewController.View == ViewType.InGame)
            {
                bool wasEnabled = MainTools.IsGameOverviewEnabled;
                MainTools.ToggleGameOverviewWindow();
                ViewController.ViewMode = wasEnabled ? ViewModeType.Auto : ViewModeType.Overview;
            }
        }
        
        public static void BackToNormalModeInGameView()
        {
            if (ViewController.View == ViewType.InGame)
            {
                ViewController.ViewMode = ViewModeType.Normal;
            }
        }
        
        public static void ToggleHideToolbarWindow()
        {
            if (ViewController.View == ViewType.InGame && ViewController.ViewMode == ViewModeType.Normal)
            {
                MainTools.ToggleHideToolbarWindow();
            }
        }
        
        public static void TogglePresentationFogOfWar()
        {
            if (ViewController.View == ViewType.InGame 
                && (ViewModeType.Normal | ViewModeType.LiveEditor | ViewModeType.FreeCamera)
                .HasFlag(ViewController.ViewMode))
            {
                ActionController.TogglePresentationFogOfWar();
            }
        }
        
        public static void ToggleFreeCameraMode()
        {
            if (ViewController.View == ViewType.InGame)
            {
                bool wasEnabled = FreeCameraController.Enabled;
                ActionController.ToggleFreeCameraMode();
                ViewController.ViewMode = wasEnabled ? ViewModeType.Auto : ViewModeType.FreeCamera;
                if (FreeCameraController.Enabled)
                    UIController.IsAmplitudeUIVisible = false;
            }
        }
        
        public static void ToggleLiveEditorMode()
        {
            if (ViewController.View == ViewType.InGame)
            {
                bool wasEnabled = LiveEditorMode.Enabled;
                ActionController.ToggleLiveEditorMode();
                ViewController.ViewMode = wasEnabled ? ViewModeType.Auto : ViewModeType.LiveEditor;
            }
        }
    }
}
