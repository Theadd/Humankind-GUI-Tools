using Amplitude.Framework;
using Amplitude.Extensions;
using Amplitude.Framework.Overlay;
using Amplitude.Mercury.AI;
using Amplitude.Mercury.AI.Battle;
using Amplitude.Mercury.Interop;
using Amplitude.Mercury.Presentation;
using Amplitude.Mercury.Runtime;
using Amplitude.Mercury.Simulation;
using System.Collections.Generic;
using System.Net.Http;
using UnityEngine;
using Amplitude.Mercury.Overlay;
using Modding.Humankind.DevTools.Core;
using Modding.Humankind.DevTools.DeveloperTools.UI;

namespace DevTools.Humankind.GUITools.UI
{
    public class BattleAIToolWindow : FloatingToolWindow
    {
        private readonly HttpClient httpClient = new HttpClient();
        private Vector2 configurationScrollPosition;
        private string[] battleLabels;
        private string[] unitLabels;
        private List<SimulationEntityGUID> unitGUIDs = new List<SimulationEntityGUID>();
        private int currentBattleIndex;
        private int currentUnitIndex;
        private int currentActionIndex;

        protected Amplitude.Framework.Runtime.IRuntimeService RuntimeService { get; private set; }

        public override string WindowTitle { get; set; } = "BATTLE AI TOOL";

        public override string WindowGUIStyle { get; set; } = "PopupWindow";

        public override bool ShouldBeVisible => true;

        public override bool ShouldRestoreLastWindowPosition => true;

        public override Rect WindowRect { get; set; } = new Rect(130f, 260f, 520f, 500f);

        public override void OnGUIStyling()
        {
            base.OnGUIStyling();
            GUI.backgroundColor = new Color32(255, 255, 255, 230);
        }

        public override void OnDrawUI()
        {
            WindowUtils.DrawWindowTitleBar(this);

            OnDrawWindowContent();
        }

        protected void OnDrawWindowContent()
        {
            GUI.color = Color.white;
            GUI.backgroundColor = Color.white;
            if (Snapshots.GameSnapshot == null || Snapshots.SandboxSnapshot == null)
                return;
            GUILayout.BeginVertical((GUIStyle)"Widget.ClientArea", GUILayout.ExpandWidth(true));
            if (this.RuntimeService != null)
            {
                if (this.RuntimeService.Runtime != null && this.RuntimeService.Runtime.HasBeenLoaded)
                {
                    if (this.RuntimeService.Runtime.FiniteStateMachine.CurrentState != null)
                    {
                        if (this.RuntimeService.Runtime.FiniteStateMachine.CurrentState.GetType() == typeof(RuntimeState_InGame))
                        {
                            DrawBattleMenu();
                            /*if (Amplitude.Mercury.Presentation.Presentation.HasBeenStarted)
                                GUILayout.Label("No debug in release mode");
                            else
                                GUILayout.Label("Waiting for the presentation...");*/
                        }
                        else
                            GUILayout.Label("Waiting for the runtime state...");
                    }
                    else
                        GUILayout.Label("Waiting for the runtime...");
                }
                else
                    GUILayout.Label("Waiting for the runtime...");
            }
            else
            {
                GUILayout.Label("Waiting for the runtime service...");
                if (UnityEngine.Event.current.type == UnityEngine.EventType.Repaint)
                    this.RuntimeService = Services.GetService<Amplitude.Framework.Runtime.IRuntimeService>();
            }
            GUILayout.EndVertical();
        }

        private void DrawBattleMenu()
        {
            if (Amplitude.Mercury.Presentation.Presentation.PresentationCursorController.CurrentCursor is BattleUnitCursor currentCursor)
                GUILayout.Label(string.Format("Unit {0} selected", (object)currentCursor.BattleUnitGuid));
            else
                GUILayout.Label("No battle unit selected.");
            GUILayout.BeginHorizontal();
            DebugControl.AutoSyncBattleDebug = GUILayout.Toggle(DebugControl.AutoSyncBattleDebug, "Auto sync", (GUIStyle)"PopupWindow.Button");
            DebugControl.ShowBestBattleDebug = GUILayout.Toggle(DebugControl.ShowBestBattleDebug, "Show Best", (GUIStyle)"PopupWindow.Button");
            DebugControl.StepByStepBattles = GUILayout.Toggle(DebugControl.StepByStepBattles, "Step-by-step", (GUIStyle)"PopupWindow.Button");
            GUILayout.EndHorizontal();
            int brainCount = BattleControllerDebug.BrainCount;
            if (brainCount == 0)
            {
                GUILayout.Label("No battles available.");
                DebugControl.BattleActionToHighlight.Type = ActionType.None;
            }
            else
            {
                if (this.battleLabels == null || this.battleLabels.Length != brainCount)
                    this.battleLabels = new string[brainCount];
                for (int index = 0; index < brainCount; ++index)
                    this.battleLabels[index] = string.Format("{0}", (object)BattleControllerDebug.Brains[index].BattleGUID);
                if (this.currentBattleIndex >= brainCount)
                    this.currentBattleIndex = 0;
                GUILayout.BeginHorizontal();
                GUILayout.Label("Select a battle:");
                GUILayout.Space(2f);
                int num1 = GUILayout.Toolbar(this.currentBattleIndex, this.battleLabels, (GUIStyle)"PopupWindow.ToolbarButton", GUILayout.ExpandWidth(false));
                if (num1 != this.currentBattleIndex)
                {
                    DebugControl.UpdatePresentationDebugger = true;
                    this.currentBattleIndex = num1;
                }
                GUILayout.EndHorizontal();
                BattleBrainDebugInfo brain = BattleControllerDebug.Brains[this.currentBattleIndex];
                GUILayout.Label(string.Format("Battle {0}: {1}", (object)brain.BattleGUID, (object)brain.BattleState));
                GUILayout.Label(string.Format("Brain state: {0} (Empire {1})", (object)brain.BrainState, (object)brain.CurrentEmpireIndex));
                if ((Object)brain.CurrentBattleConfiguration != (Object)null)
                    GUILayout.Label(string.Format("Current battle configuration: {0}", (object)brain.CurrentBattleConfiguration.Name));
                else
                    GUILayout.Label("no battle configuration");
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Open brain inspector", (GUIStyle)"PopupWindow.Button"))
                    UnityEngine.Application.OpenURL(string.Format("http://localhost:1477/global/battle/{0}", (object)brain.BattleGUID));
                if (GUILayout.Button("Do next step", (GUIStyle)"PopupWindow.Button"))
                    this.httpClient.GetAsync(string.Format("http://localhost:1477/api/ai-controller/battle/{0}/step", (object)brain.BattleGUID));
                GUILayout.EndHorizontal();
                if (brain.ActionCount == 0)
                {
                    GUILayout.Label("No actions available.");
                    DebugControl.BattleActionToHighlight.Type = ActionType.None;
                }
                else
                {
                    this.unitGUIDs.Clear();
                    this.unitGUIDs.Add((SimulationEntityGUID)0UL);
                    for (int index = 0; index < brain.ActionCount; ++index)
                        this.unitGUIDs.AddOnce<SimulationEntityGUID>(brain.Actions[index].ChosenUnitGUID);
                    int count = this.unitGUIDs.Count;
                    if (this.unitLabels == null || this.unitLabels.Length != count)
                        this.unitLabels = new string[count];
                    this.unitLabels[0] = "All";
                    for (int index = 1; index < count; ++index)
                        this.unitLabels[index] = string.Format("{0}", (object)this.unitGUIDs[index]);
                    if (this.currentUnitIndex >= count)
                        this.currentUnitIndex = 0;
                    int num2 = brain.ActionCount;
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Select a unit:");
                    GUILayout.Space(2f);
                    int num3 = GUILayout.Toolbar(this.currentUnitIndex, this.unitLabels, (GUIStyle)"PopupWindow.ToolbarButton", GUILayout.ExpandWidth(false));
                    if (num3 != this.currentUnitIndex)
                    {
                        DebugControl.UpdatePresentationDebugger = true;
                        this.currentUnitIndex = num3;
                    }
                    if (this.currentUnitIndex != 0)
                    {
                        num2 = 0;
                        for (int index = 0; index < brain.ActionCount; ++index)
                        {
                            if (brain.Actions[index].ChosenUnitGUID == this.unitGUIDs[this.currentUnitIndex])
                                ++num2;
                        }
                    }
                    GUILayout.EndHorizontal();
                    if (this.currentActionIndex >= brain.ActionCount)
                        this.currentActionIndex = 0;
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("<", (GUIStyle)"PopupWindow.Button"))
                    {
                        DebugControl.UpdatePresentationDebugger = true;
                        DebugControl.ShowBestBattleDebug = false;
                        if (this.currentActionIndex > 0)
                            --this.currentActionIndex;
                        else
                            this.currentActionIndex = num2 - 1;
                    }
                    GUILayout.Label(string.Format("Action [{0}/{1}] (sorted)", (object)(this.currentActionIndex + 1), (object)num2));
                    if (GUILayout.Button(">", (GUIStyle)"PopupWindow.Button"))
                    {
                        DebugControl.UpdatePresentationDebugger = true;
                        DebugControl.ShowBestBattleDebug = false;
                        if (this.currentActionIndex < num2 - 1)
                            ++this.currentActionIndex;
                        else
                            this.currentActionIndex = 0;
                    }
                    GUILayout.EndHorizontal();
                    if (DebugControl.ShowBestBattleDebug && this.currentUnitIndex == 0)
                    {
                        for (int index = 0; index < brain.ActionCount; ++index)
                        {
                            if (brain.Actions[index].PoolAllocationIndex == brain.BestAction)
                            {
                                this.currentActionIndex = index;
                                break;
                            }
                        }
                    }
                    ActionDebugInfo action1 = brain.Actions[this.currentActionIndex];
                    ActionDebugInfo action2 = brain.Actions[(this.currentActionIndex + 1) % brain.ActionCount];
                    if (this.currentUnitIndex != 0)
                    {
                        int num4 = 0;
                        for (int index = 0; index < brain.ActionCount; ++index)
                        {
                            if (brain.Actions[index].ChosenUnitGUID == this.unitGUIDs[this.currentUnitIndex])
                            {
                                if (num4 == this.currentActionIndex)
                                    action1 = brain.Actions[index];
                                if (num4 == (this.currentActionIndex + 1) % num2)
                                    action1 = brain.Actions[index];
                                ++num4;
                            }
                        }
                    }
                    DebugControl.BattleActionToHighlight = action1;
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(string.Format("Action ID: {0}", (object)action1.PoolAllocationIndex));
                    if (brain.BestAction == action1.PoolAllocationIndex)
                        GUILayout.Label("Best One");
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    switch (action1.Type)
                    {
                        case ActionType.Wait:
                            GUILayout.Label(string.Format("Unit{0}:Wait", (object)action1.ChosenUnitGUID));
                            break;
                        case ActionType.Attack:
                            GUILayout.Label(string.Format("Unit{0}:Attack", (object)action1.ChosenUnitGUID));
                            GUI.color = new Color(0.4f, 1f, 0.43f);
                            GUILayout.Label(string.Format("From{0}", (object)action1.ChosenUnitPosition));
                            GUI.color = Color.red;
                            GUILayout.Label(string.Format("Attack{0} at {1}", (object)action1.TargetGUID, (object)action1.TargetPosition));
                            break;
                        case ActionType.Move:
                            GUILayout.Label(string.Format("Unit{0}:Move", (object)action1.ChosenUnitGUID));
                            GUI.color = new Color(0.4f, 1f, 0.43f);
                            GUILayout.Label(string.Format("From{0}", (object)action1.ChosenUnitPosition));
                            GUI.color = new Color(0.12f, 0.56f, 1f);
                            GUILayout.Label(string.Format("To{0}", (object)action1.Destination));
                            break;
                        case ActionType.MoveAndAttack:
                            GUILayout.Label(string.Format("Unit{0}: Move and attack", (object)action1.ChosenUnitGUID));
                            GUI.color = new Color(0.4f, 1f, 0.43f);
                            GUILayout.Label(string.Format("From{0}", (object)action1.ChosenUnitPosition));
                            GUI.color = new Color(0.12f, 0.56f, 1f);
                            GUILayout.Label(string.Format("To{0}", (object)action1.Destination));
                            GUI.color = Color.red;
                            GUILayout.Label(string.Format("Attack {0} at {1}", (object)action1.TargetGUID, (object)action1.TargetPosition));
                            break;
                    }
                    GUILayout.FlexibleSpace();
                    GUI.color = Color.white;
                    GUILayout.EndHorizontal();
                    if ((Object)brain.CurrentBattleConfiguration == (Object)null)
                        return;
                    BattleConfiguration.HeuristicLevel[] heuristicLevelArray = brain.BattleState == BattleState.Deployment ? brain.CurrentBattleConfiguration.DeploymentHeuristicLevels : brain.CurrentBattleConfiguration.CombatHeuristicLevels;
                    BattleConfiguration.ActionComparer actionComparer = brain.BattleState == BattleState.Deployment ? brain.CurrentBattleConfiguration.DeploymentActionComparer : brain.CurrentBattleConfiguration.CombatActionComparer;
                    string text = "->";
                    if (action1.ScoreByHeuristic == null)
                        return;
                    GUILayout.Label(string.Format(" (ε:{0})", (object)actionComparer.Step));
                    for (int index1 = 0; index1 < heuristicLevelArray.Length; ++index1)
                    {
                        BattleConfiguration.HeuristicLevel heuristicLevel = heuristicLevelArray[index1];
                        float num5 = 0.0f;
                        float num6 = 0.0f;
                        for (int index2 = 0; index2 < heuristicLevel.Heuristics.Length; ++index2)
                        {
                            BattleConfiguration.HeuristicConfiguration heuristic = heuristicLevel.Heuristics[index2];
                            float num7 = action1.ScoreByHeuristic[(int)heuristic.ConfigurationHeuristic].Value;
                            float num8 = action2.ScoreByHeuristic[(int)heuristic.ConfigurationHeuristic].Value;
                            num5 += num7;
                            num6 += num8;
                            GUILayout.BeginHorizontal();
                            GUI.color = Color.white;
                            GUILayout.Label(text);
                            GUI.color = Color.yellow;
                            GUILayout.Label(heuristic.ConfigurationHeuristic.ToString());
                            GUI.color = new Color(0.3f, 1f, 0.5f);
                            GUILayout.Label(num7.ToString("F3"));
                            GUI.color = (double)num7 - (double)num8 > 0.0001 ? Color.yellow : ((double)num7 - (double)num8 < -0.0001 ? Color.blue : Color.white);
                            GUILayout.Label((double)num7 > (double)num8 ? " > " : ((double)num7 < (double)num8 ? "< " : " = "));
                            GUI.color = new Color(0.3f, 1f, 0.5f);
                            GUILayout.Label(num8.ToString("F3"));
                            GUILayout.FlexibleSpace();
                            GUI.color = Color.white;
                            GUILayout.EndHorizontal();
                        }
                        float num9 = num5 / (float)heuristicLevel.Heuristics.Length;
                        float num10 = num6 / (float)heuristicLevel.Heuristics.Length;
                        GUILayout.BeginHorizontal();
                        GUI.color = Color.white;
                        GUILayout.Label(text);
                        GUI.color = new Color(0.12f, 0.56f, 1f);
                        GUILayout.Label(string.Format("Level {0}", (object)(index1 + 1)));
                        GUI.color = new Color(0.3f, 1f, 0.5f);
                        GUILayout.Label(num9.ToString("F3"));
                        GUI.color = (double)num9 - (double)num10 > (double)actionComparer.Step ? Color.red : ((double)num9 > (double)num10 ? Color.yellow : Color.white);
                        GUILayout.Label((double)num9 - (double)num10 > (double)actionComparer.Step ? " >> " : ((double)num9 > (double)num10 ? " > " : " ≈ "));
                        GUI.color = new Color(0.3f, 1f, 0.5f);
                        GUI.color = Color.white;
                        GUILayout.Label(num10.ToString("F3"));
                        GUI.color = Color.white;
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                        text = "---" + text;
                    }
                }
            }
        }
    }
}
