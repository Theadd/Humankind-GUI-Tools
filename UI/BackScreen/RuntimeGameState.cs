using System;
using Amplitude.Framework;
using Amplitude.Framework.Presentation;
using Modding.Humankind.DevTools;

namespace DevTools.Humankind.GUITools.UI
{
    public static class RuntimeGameState
    {
        private static string _currentState = "UNKNOWN STATE";
        private static string _currentView = "UNKNOWN VIEW";
        private static bool _registeredToViewChanges = false;

        public static string CurrentState
        {
            get => _currentState;
            private set
            {
                if (_currentState == value)
                    return;

                _currentState = value;
                Log();
            }
        }
        
        public static string CurrentView
        {
            get => _currentView;
            private set
            {
                if (_currentView == value)
                    return;

                _currentView = value;
                Log();
            }
        }

        public static Amplitude.Framework.Runtime.IRuntimeService RuntimeService { get; private set; }
        public static Amplitude.Framework.Presentation.IViewService ViewService { get; private set; }

        public static void Log() => Loggr.Log("VIEW = " + _currentView + ", STATE = " + _currentState, ConsoleColor.Magenta);

        public static void Refresh()
        {
            if (RuntimeService != null)
            {
                if (RuntimeService.Runtime?.HasBeenLoaded ?? false)
                {
                    CurrentState = RuntimeService.Runtime.FiniteStateMachine?.CurrentState?.GetType().Name ??
                        "FiniteStateMachine is NULL";
                }
                else
                    CurrentState = "!Runtime.HasBeenLoaded";
            }
            else
            {
                CurrentState = "Requesting RuntimeService";
                if (UnityEngine.Event.current.type == UnityEngine.EventType.Repaint)
                    RuntimeService = Services.GetService<Amplitude.Framework.Runtime.IRuntimeService>();
            }

            if (ViewService == null)
            {
                ViewService = Services.GetService<Amplitude.Framework.Presentation.IViewService>();
            }
            else
            {
                if (!_registeredToViewChanges)
                    RegisterToViewChangeEvents();
                
                CurrentView = ViewService.View?.Name ?? "NULL";
            }
        }

        private static void RegisterToViewChangeEvents()
        {
            ViewService.ViewChange += OnViewChanged;
            _registeredToViewChanges = true;
        }

        public static void OnViewChanged(object sender, ViewChangedEventArgs eventArgs)
        {
            Loggr.Log("OnViewChanged => " + eventArgs.View.Name, ConsoleColor.Yellow);
        }
    }
}
