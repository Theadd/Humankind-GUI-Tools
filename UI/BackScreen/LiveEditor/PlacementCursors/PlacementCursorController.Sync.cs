using System;
using Amplitude.Mercury;
using Amplitude.Mercury.Presentation;
using DevTools.Humankind.GUITools.Collections;
using Modding.Humankind.DevTools;

namespace DevTools.Humankind.GUITools.UI
{
    public partial class PlacementCursorController
    {
        private static Cursor _activeCursor;
        public static bool IsAnyVirtualCursorActive { get; private set; }

        public static Cursor ActiveCursor
        {
            get => IsAnyVirtualCursorActive ? _activeCursor : default;
            private set => _activeCursor = value;
        }

        private PlacementCursorController()
        {
            Presentation.PresentationCursorController.CursorChange -= OnCursorChange;
            Presentation.PresentationCursorController.CursorChange += OnCursorChange;
            ViewController.ViewModeChange -= OnViewModeChange;
            ViewController.ViewModeChange += OnViewModeChange;
        }

        public void OnCursorChange(Cursor newCursor)
        {
            IsAnyVirtualCursorActive = newCursor is IVirtualPlacementCursor virtualCursor;
            ActiveCursor = newCursor;
        }

        public void OnViewModeChange()
        {
            if (ViewController.ViewMode != ViewModeType.LiveEditor && IsAnyVirtualCursorActive)
            {
                DeactivateVirtualPlacementCursor();
            }
        }

        public static void Unload() => Instance.OnDestroy();

        private void OnDestroy()
        {
            SafeInvoke.All(
                (() => Presentation.PresentationCursorController.CursorChange -= OnCursorChange),
                (() => ViewController.ViewModeChange -= OnViewModeChange),
                (() =>
                {
                    if (IsAnyVirtualCursorActive)
                        DeactivateVirtualPlacementCursor();
                })
            );
        }
    }
}
