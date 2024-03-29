﻿using Amplitude.Mercury.UI;
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

        public static UITransform EndGameWindowUITransform { get; private set; }

        public static void Resync()
        {
            var container = GameObject.Find("/WindowsRoot/InGameOverlays/PauseMenuModalWindow");
            var endGameContainer = GameObject.Find("WindowsRoot/InGameFullscreen/EndGameWindow");

            if (container != null && endGameContainer != null)
            {
                Container = container;
                PauseMenuWindow = Container.transform.GetComponent<PauseMenuModalWindow>();
                Background = Container.transform.Find("Background")?.GetComponent<UIImage>();

                EndGameWindowUITransform = endGameContainer.transform.GetComponent<UITransform>();

                RegisterOnVisibilityChangeEvents();
            }
        }

        public static bool IsVisible => PauseMenuWindow?.Shown ?? false;
        public static bool IsEndGameWindowVisible => EndGameWindowUITransform?.VisibleSelf ?? false;

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
            else if (newState == UIAbstractShowable.VisibilityState.Invisible)
                ViewController.ViewMode = ViewModeType.Auto;
            else if (newState == UIAbstractShowable.VisibilityState.Visible)
                ViewController.ViewMode = ViewModeType.GameMenu;
            // else
            //    Loggr.Log("Next VisibilityState unknown yet: " + newState + ", coming from: " + oldState);
        }

        private static void OnShowingEvent()
        {
            if (Background != null)
                PreviousBackgroundColor = Background.Color;

            ViewController.ViewMode = ViewModeType.GameMenu;
            // Do something here
            //SetBackgroundColor(new Color(0.7f, 0.65f, 0.1f, 0.45f));

            BackgroundFader.StartAnimation();
        }

        private static void OnHidingEvent()
        {
            // Do something here

            if (Background != null)
                Background.Color = PreviousBackgroundColor;
        }

        public static void SetBackgroundColor(Color backgroundColor)
        {
            if (Background != null)
                Background.Color = backgroundColor;
        }
    }
}
