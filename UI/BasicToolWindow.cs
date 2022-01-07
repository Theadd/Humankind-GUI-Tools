using System;
using System.Reflection;
using System.Linq;
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

        public LoggrVisibilityChecker VisibilityChecker { get; set; } = new LoggrVisibilityChecker();
        private bool firstDraw = true;

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
            if (firstDraw)
            {
                /*Type t = VisibilityChecker.GetType();
                // Loggr.Log((object)t);
                // Loggr.Log(VisibilityChecker.GetType()); 
                // Loggr.Log(VisibilityChecker);

                FieldInfo[] fields = VisibilityChecker.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic |
                                                     BindingFlags.Static | BindingFlags.Instance);

                var namedFields = fields.AsEnumerable().Select(f => f.Name + ": IsSpecialName = " + 
                    (f.IsSpecialName.ToString()) + " IsPinvokeImpl = " + f.IsPinvokeImpl.ToString()).ToArray();
                Loggr.Log(string.Join("\n", namedFields));
                // Loggr.Log(fields[0]);
                
                var methods = t.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);

                Loggr.Log(methods);

                var namedMethods = methods.AsEnumerable()
                    .Where(m => !(m.Name.Length > 3 && m.Name[3] == '_'))
                    .Select(m => "\t" + m.Name + "(" + 
                        (string.Join(", ", m.GetParameters().Select(p => p.ParameterType.Name + " " + p.Name).ToArray())) + 
                        ") => " + m.ReturnType?.Name);

                Loggr.Log(string.Join("\n", namedMethods.ToArray()), ConsoleColor.DarkCyan);
                foreach (var prop in VisibilityChecker.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic |
                                                     BindingFlags.Static | BindingFlags.Instance))
                {
                    Loggr.Log(prop);
                }
                Loggr.Log("type IsClass: " + BindingFlags.NonPublic.GetType().IsClass.ToString());
                Loggr.Log(BindingFlags.NonPublic);

                Loggr.Log(((Checker)VisibilityChecker).GetType().Name, ConsoleColor.DarkRed);
                */

                // Loggr.Log(HumankindGame.Empires[0].Armies.Select(army => army.Units).ToArray());
            }

            GUILayout.BeginHorizontal();
                GUILayout.Label("1. HELLO WORLD!");
                if (GUILayout.Button("I'M A BUTTON!")){
                    VisibilityChecker.SetName("VisibilityChecker Playground Test");
                    Loggr.Log(VisibilityChecker);
                }
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

            firstDraw = false;
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


    public abstract class Checker
    {
        private string _Name = string.Empty;

        public string Name { get => _Name; private set => _Name = value; }

        protected bool Enabled { get; set; } = true;

        public abstract int AbstractProperty { get; set; }
        public virtual int VirtualProperty => 43;

        private int internalNumber = 72;

        protected int protectedNumber = 84;

        public void SetName(string newName) => Name = newName;
    }

    public class LoggrVisibilityChecker : Checker
    {
        public KeyboardShortcut Shortcut { get; set; } = new KeyboardShortcut(KeyCode.Space, KeyCode.LeftControl);
        public Vector2 VectorProperty { get; set; } = new Vector2(0.2f, 0.6f);
        private Vector2 PrivateVectorProperty { get; set; } = new Vector2(1.2f, 3.6f);
        public override int AbstractProperty { get => protectedNumber; set => protectedNumber = value; }

        private string hello = "World!";
        public string HelloEveryone = "Every World!";

        private string propertyHello => "Property World!";
        private string propertyGetSetHello { get; set; } = "Property GET SET private World!";

        public static Vector2 Position { get; set; } = new Vector2(20f, 30f);
        private static Vector2 PrivatePosition { get; set; } = new Vector2(2f, 3f);
    }

}
