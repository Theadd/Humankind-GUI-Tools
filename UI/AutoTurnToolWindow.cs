using Amplitude;
using Amplitude.Framework;
using Amplitude.Framework.Overlay;
using Amplitude.Mercury.AI;
using Amplitude.Mercury.Runtime;
using System;
using UnityEngine;
using Modding.Humankind.DevTools.Core;
using Modding.Humankind.DevTools.DeveloperTools.UI;
using Modding.Humankind.DevTools;
using Math = System.Math;

namespace DevTools.Humankind.GUITools.UI
{
    public class AutoTurnToolWindow : FloatingToolWindow
    {
        private static readonly string[] TurnSelectionGridEntries = new string[6]
            { "5", "10", "25", "75", "150", "Custom" };
        private static readonly string[] DurationSelectionGridEntries = new string[4]
            { "0", "2", "5", "15" };

        private bool advancedOptions = true;
        private bool stopOnError;
        private int lastTurnEntryIndex = 1;
        private int numberOfTurns = 10;
        private int lastDurationEntryIndex;
        private ControlledByHumanBinding lastControlledByHumanBinding;
        private EmpireReadyOrderPolicy lastEmpireReadyOrderPolicy;
        private int autoTurnStartTurn;
        private int lastCheckedTurn = -1;
        private string lastCustomNumberOfTurns = string.Empty;

        protected Amplitude.Framework.Runtime.IRuntimeService RuntimeService { get; private set; }

        protected bool AutoTurnActivated => Preferences.ControlledByHumanBindingAfterStartup == ControlledByHumanBinding.ActivateAIOnAllEmpires && Preferences.EmpireReadyOrderPolicy == EmpireReadyOrderPolicy.AlwaysSendEmpireReadyOrder;

        public override string WindowTitle { get; set; } = "AUTO TURN TOOL";
        
        public override string WindowGUIStyle { get; set; } = "PopupWindow";

        public override bool ShouldBeVisible => HumankindGame.IsGameLoaded;

        public override bool ShouldRestoreLastWindowPosition => true;
        
        public override Rect WindowRect { get; set; } = new Rect(130f, 260f, 360f, 300f);

        public override void OnGUIStyling()
        {
            base.OnGUIStyling();
            GUI.backgroundColor = new Color32(255, 255, 255, 230);
        }
        
        public override void OnDrawUI()
        {
            //Loggr.Log(GetWindowRect().ToString());
            if (advancedOptions)
                WindowUtils.DrawWindowTitleBar(this);
            
            GUILayout.BeginVertical((GUIStyle)"Widget.ClientArea", GUILayout.ExpandWidth(true));

                DrawClientArea();

            GUILayout.EndVertical();
        }

        private void DrawClientArea()
        {
            RefreshDurationIndex();
            if (DoesAutoTurnNeedsToStop())
                StopAutoturn();
            
            GUILayout.Space(advancedOptions ? 4f : 12f);
            
            GUILayout.BeginHorizontal();
            int numberOfTurns = this.numberOfTurns;
            if (AutoTurnActivated)
                numberOfTurns -= lastCheckedTurn - autoTurnStartTurn;
            GUILayout.Space(8f);
            GUILayout.Label("<b>AUTO TURN [" + numberOfTurns.ToString() + "]</b>");
            GUILayout.Space(12f);
            GUILayout.FlexibleSpace();
            stopOnError = GUILayout.Toggle(stopOnError, "<size=11><b> STOP ON ERROR </b></size>", (GUIStyle)"PopupWindow.ToolbarButton");
            if (AutoTurnActivated)
            {
                if (GUILayout.Button("<size=11><b> STOP </b></size>", (GUIStyle)"PopupWindow.ToolbarButton"))
                    StopAutoturn();
            }
            else if (GUILayout.Button("<size=11><b> START </b></size>", (GUIStyle)"PopupWindow.ToolbarButton"))
                StartAutoTurn();



            advancedOptions = !GUILayout.Toggle(!advancedOptions, advancedOptions ? " - " : " + ", (GUIStyle)"PopupWindow.ToolbarButton", GUILayout.Width(32f));
            GUILayout.Space(8f);
            GUILayout.EndHorizontal();
            GUILayout.Space(advancedOptions ? 4f : 8f);
            if (!advancedOptions)
                return;
            
            
            GUILayout.Label("CONFIGURATION", "PopupWindow.SectionHeader");
            GUILayout.Space(8f);
            GUILayout.BeginHorizontal();
            GUILayout.Space(8f);
            GUILayout.Label("<size=10><b>Number of turns</b></size>".ToUpper());
            GUILayout.Space(1f);
            // GUILayout.FlexibleSpace();
            int index1 = GUILayout.SelectionGrid(this.lastTurnEntryIndex, AutoTurnToolWindow.TurnSelectionGridEntries, 
                3, (GUIStyle)"PopupWindow.ToolbarButton", GUILayout.ExpandWidth(false));
            if (index1 != this.lastTurnEntryIndex)
            {
                this.lastTurnEntryIndex = index1;
                if (this.lastTurnEntryIndex < AutoTurnToolWindow.TurnSelectionGridEntries.Length - 1)
                {
                    this.numberOfTurns = int.Parse(AutoTurnToolWindow.TurnSelectionGridEntries[index1]);
                }
                else
                {
                    int result;
                    if (int.TryParse(this.lastCustomNumberOfTurns, out result))
                        this.numberOfTurns = result;
                }
            }
            GUILayout.Space(8f);
            GUILayout.EndHorizontal();
            GUILayout.Space(8f);
            GUILayout.BeginHorizontal();
            GUI.enabled = this.lastTurnEntryIndex == AutoTurnToolWindow.TurnSelectionGridEntries.Length - 1;
            GUILayout.Space(12f);
            GUILayout.Label("<size=10><b>CUSTOM:</b></size>", GUILayout.Width(56f));
            GUILayout.Space(8f);
            string s = GUILayout.TextField(lastCustomNumberOfTurns, GUILayout.ExpandWidth(true));
            s = s.Substring(0, Math.Min(s.Length, 4));
            if (s != lastCustomNumberOfTurns)
            {
                int result;
                if (int.TryParse(s, out result))
                {
                    if (result > 0)
                        this.numberOfTurns = result;

                    lastCustomNumberOfTurns = s;  // result.ToString();
                }

                if (s.Length == 0)
                    lastCustomNumberOfTurns = "";
            }
            GUI.enabled = true;
            GUILayout.Space(4f);
            GUILayout.EndHorizontal();
            GUILayout.Space(12f);
            GUILayout.BeginHorizontal();
            GUILayout.Space(8f);
            GUILayout.Label("<size=10><b>Min turn duration</b></size>".ToUpper());
            GUILayout.Space(12f);
            GUILayout.FlexibleSpace();
            int index2 = GUILayout.SelectionGrid(this.lastDurationEntryIndex, AutoTurnToolWindow.DurationSelectionGridEntries, 4, (GUIStyle)"PopupWindow.ToolbarButton", GUILayout.ExpandWidth(false));
            if (index2 != this.lastDurationEntryIndex)
            {
                this.lastDurationEntryIndex = index2;
                Preferences.NumberOfTicksBeforeSendEmpireReadyOrder = TimeSpan.FromSeconds((double)float.Parse(AutoTurnToolWindow.DurationSelectionGridEntries[index2])).Ticks;
            }
            GUILayout.Space(8f);
            GUILayout.EndHorizontal();
            GUILayout.Space(14f);
            
        }

        private void RefreshDurationIndex()
        {
            for (int index = 0; index < AutoTurnToolWindow.DurationSelectionGridEntries.Length; ++index)
            {
                int num = int.Parse(AutoTurnToolWindow.DurationSelectionGridEntries[index]);
                if (Mathf.RoundToInt((float)TimeSpan.FromTicks(Preferences.NumberOfTicksBeforeSendEmpireReadyOrder).TotalSeconds) == num)
                {
                    this.lastDurationEntryIndex = index;
                    return;
                }
            }
            Diagnostics.LogWarning("Invalid duration.");
        }

        private bool DoesAutoTurnNeedsToStop()
        {
            this.lastCheckedTurn = Amplitude.Mercury.Interop.AI.Snapshots.Game.Turn;
            return this.lastCheckedTurn - this.autoTurnStartTurn >= this.numberOfTurns;
        }

        private void StartAutoTurn()
        {
            if (this.AutoTurnActivated)
                return;
            Diagnostics.MessageLogged += new Diagnostics.MessageLoggedEventHandler(this.Diagnostics_MessageLogged);
            this.lastControlledByHumanBinding = Preferences.ControlledByHumanBindingAfterStartup;
            this.lastEmpireReadyOrderPolicy = Preferences.EmpireReadyOrderPolicy;
            Preferences.EmpireReadyOrderPolicy = EmpireReadyOrderPolicy.AlwaysSendEmpireReadyOrder;
            Preferences.ControlledByHumanBindingAfterStartup = ControlledByHumanBinding.ActivateAIOnAllEmpires;
            this.autoTurnStartTurn = Amplitude.Mercury.Interop.AI.Snapshots.Game.Turn;
            this.lastCheckedTurn = Amplitude.Mercury.Interop.AI.Snapshots.Game.Turn;
        }

        private void StopAutoturn()
        {
            if (!this.AutoTurnActivated)
                return;
            Diagnostics.MessageLogged -= new Diagnostics.MessageLoggedEventHandler(this.Diagnostics_MessageLogged);
            Preferences.EmpireReadyOrderPolicy = this.lastEmpireReadyOrderPolicy;
            Preferences.ControlledByHumanBindingAfterStartup = this.lastControlledByHumanBinding;
        }

        private void Diagnostics_MessageLogged(LogMessage message)
        {
            if (!this.stopOnError || message.LogLevel != Amplitude.LogLevel.Critical && message.LogLevel != Amplitude.LogLevel.Error)
                return;
            this.StopAutoturn();
        }

        
    }
}
