using System;
using BepInEx.Configuration;
using UnityEngine;
using Modding.Humankind.DevTools;
using Modding.Humankind.DevTools.DeveloperTools.UI;

namespace DevTools.Humankind.GUITools.UI
{
    public class BasicToolWindow : FloatingToolWindow
    {
        public override bool ShouldBeVisible => true;
        public override bool ShouldRestoreLastWindowPosition => true;
        public override string WindowTitle { get; set; } = "BASIC TOOL WINDOW";

        private void FixedUpdate()
        {
            if (KeyboardShortcut.Empty.IsDown())
            {
                Loggr.Log("KeyboardShortcut.Empty.IsDown()", ConsoleColor.Magenta);
            }
            
            if (new KeyboardShortcut(KeyCode.None).IsDown())
            {
                Loggr.Log("KeyboardShortcut.Empty.IsDown()", ConsoleColor.Magenta);
            }
            
            if (new KeyboardShortcut(KeyCode.Mouse0).IsDown())
            {
                Loggr.Log("KeyboardShortcut.Mouse0.IsDown()", ConsoleColor.Magenta);
            }
            
            if (new KeyboardShortcut(KeyCode.Mouse1).IsDown())
            {
                Loggr.Log("KeyboardShortcut.Mouse1.IsDown()", ConsoleColor.Magenta);
            }
            
            if (new KeyboardShortcut(KeyCode.Home).IsDown())
            {
                Loggr.Log("new KeyboardShortcut(KeyCode.Home).IsDown()", ConsoleColor.Magenta);
            }
        }

        public override void OnDrawUI()
        {
            GUILayout.BeginHorizontal();
                GUILayout.Label("1. HELLO WORLD!");
                if (GUILayout.Button("I'M A BUTTON!"))
                    Loggr.Log("BUTTON CLICKED! AND AGAIN YEAH!");
                
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUI.color = Color.black;
                OnDrawDebugPanel();
                GUI.color = Color.white;
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
                if (GUILayout.Button("RESYNC PAUSE MENU"))
                    PauseMenu.InGameMenuController.Resync();
            GUILayout.EndHorizontal();

            if (Amplitude.Framework.Application.isQuitting || Amplitude.Framework.Application.isShuttingDown)
                Loggr.Log("APPLICATION IS QUITTING OR SHUTTING DOWN!", System.ConsoleColor.Magenta);
        }


        public Amplitude.Framework.Runtime.IRuntimeService RuntimeService { get; private set; }

        protected void OnDrawDebugPanel() 
        {
            if (RuntimeService == null)
            {
                if (UnityEngine.Event.current.type == UnityEngine.EventType.Repaint)
                    RuntimeService = Amplitude.Framework.Services.GetService<Amplitude.Framework.Runtime.IRuntimeService>();
            }
            else if (RuntimeService.Runtime != null && RuntimeService.Runtime.HasBeenLoaded)
            {
                if (RuntimeService.Runtime.FiniteStateMachine.CurrentState != null)
                {
                    var currentStateName = RuntimeService.Runtime.FiniteStateMachine.CurrentState.GetType().Name;

                    GUILayout.Label("<size=16><b>" + currentStateName + "</b></size>");
                }
            }
        }
    }
}
