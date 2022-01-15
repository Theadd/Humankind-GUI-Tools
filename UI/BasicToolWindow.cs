using System;
using System.Reflection;
using UnityEngine;
using Modding.Humankind.DevTools;
using Modding.Humankind.DevTools.Core;
using Modding.Humankind.DevTools.DeveloperTools.UI;
using Amplitude.Mercury.Interop;

namespace DevTools.Humankind.GUITools.UI
{
    public class BasicToolWindow : FloatingToolWindow
    {
        public override bool ShouldBeVisible => true;
        public override bool ShouldRestoreLastWindowPosition => true;
        public override string WindowTitle { get; set; } = "BASIC TOOL WINDOW";
        public override Rect WindowRect { get; set; } = new Rect (300, 300, 900, 600);

        public static FieldInfo EmpireEndGameStatusField = R.GetField<Amplitude.Mercury.Simulation.MajorEmpire>("EmpireEndGameStatus", R.NonPublicInstance);

        private int loop = 0;
        public override void OnDrawUI()
        {
            var asMajorEmpire = HumankindGame.Empires[0].Simulation;
            EmpireEndGameStatus endGameStatus = (EmpireEndGameStatus)EmpireEndGameStatusField.GetValue(asMajorEmpire);

            if (loop < 20)
            {
                if (loop == 16 && endGameStatus == EmpireEndGameStatus.Resigned)
                {
                    Loggr.Log("SETTING EmpireEndGameStatusField TO InGame");
                    EmpireEndGameStatusField.SetValue(asMajorEmpire, EmpireEndGameStatus.InGame);
                }
                loop++;
            }

            GUILayout.BeginVertical();
                DrawValue("Name", Amplitude.Framework.Application.Name);
                DrawValue("User Name", Amplitude.Framework.Application.UserName);
                DrawValue("User Identifier", Amplitude.Framework.Application.UserIdentifier.ToString());
                DrawValue("User Directory", Amplitude.Framework.Application.UserDirectory);
                DrawValue("Game Directory", Amplitude.Framework.Application.GameDirectory);
                DrawValue("Game Save Directory", Amplitude.Framework.Application.GameSaveDirectory);
                DrawValue("Current Game Language", Amplitude.Framework.Application.CurrentGameLanguage);
                DrawValue("EmpireEndGameStatus", endGameStatus.ToString());

                Utils.DrawHorizontalLine(0.6f);

                GUILayout.BeginHorizontal();
                    GUI.enabled = HumankindGame.IsGameLoaded;
                    GUILayout.Label("<size=11><b>FOG OF WAR</b></size>");
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("<size=10><b>ENABLE</b></size>"))
                        EnableFogOfWar(true);
                    if (GUILayout.Button("<size=10><b>DISABLE</b></size>"))
                        EnableFogOfWar(false);
                    GUI.enabled = true;
                GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        private void DrawValue(string title, string value)
        {
            GUILayout.BeginHorizontal();
                GUILayout.Label("<size=11><b>" + title.ToUpper() + "</b></size>");
                GUILayout.FlexibleSpace();
                GUILayout.Label(value, "RightAlignedLabel");
            GUILayout.EndHorizontal();
        }

        public static void EnableFogOfWar(bool enable)
        {
            if (!HumankindGame.IsGameLoaded)
            {
                Loggr.Log("UNABLE TO CHANGE FOG OF WAR VALUE WHEN NO GAME IS RUNNING.", ConsoleColor.DarkRed);
                return;
            }

            HumankindGame.Empires[0].EnableFogOfWar(enable);
            HumankindGame.Update();
        }
    }
}
