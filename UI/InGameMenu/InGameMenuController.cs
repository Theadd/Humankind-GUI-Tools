using Amplitude.Mercury.UI;
using Amplitude.UI;
using Amplitude.UI.Renderers;
using Modding.Humankind.DevTools;
using UnityEngine;

namespace DevTools.Humankind.GUITools.UI.PauseMenu
{
    public static class InGameMenuController
    {
        public static GameObject Container { get; private set; } = null;

        public static PauseMenuModalWindow PauseMenuWindow { get; private set; }

        public static UIImage Background { get; private set; }

        public static Color PreviousBackgroundColor { get; private set; } = Color.clear;

        public static void Resync()
        {
            var container = GameObject.Find("/WindowsRoot/InGameOverlays/PauseMenuModalWindow");

            if (container != null)
            {
                Container = container;
                PauseMenuWindow = Container.transform.GetComponent<PauseMenuModalWindow>();
                Background = Container.transform.Find("BackgroundColor")?.GetComponent<UIImage>();

                RegisterOnVisibilityChangeEvents();
            }
        }

        public static bool IsVisible => PauseMenuWindow?.Shown ?? false;

        private static void RegisterOnVisibilityChangeEvents()
        {
            if (PauseMenuWindow != null)
            {
                PauseMenuWindow.VisibilityChange -= OnVisibilityChange;
                PauseMenuWindow.VisibilityChange += OnVisibilityChange;
            }
        }
        
        private static void OnVisibilityChange(
            UIAbstractShowable showable,
            UIAbstractShowable.VisibilityState oldState,
            UIAbstractShowable.VisibilityState newState)
        {
            if (newState == UIAbstractShowable.VisibilityState.Showing)
                OnShowingEvent();
            else if (newState == UIAbstractShowable.VisibilityState.Hiding)
                OnHidingEvent();
            // else
            //     Loggr.Log("Next VisibilityState unknown yet: " + newState + ", coming from: " + oldState);
        }

        private static void OnShowingEvent()
        {
            PreviousBackgroundColor = Background.Color;
            
            // Do something here
            //SetBackgroundColor(new Color(0.7f, 0.65f, 0.1f, 0.45f));
            
            BackgroundFader.StartAnimation();
        }
        
        private static void OnHidingEvent()
        {
            // Do something here

            Background.Color = PreviousBackgroundColor;
        }

        public static void SetBackgroundColor(Color backgroundColor)
        {
            Background.Color = backgroundColor;
        }
    }
}
