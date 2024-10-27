using Amplitude.AI.Heuristics;
using Amplitude.Framework;
using Amplitude.Framework.Overlay;
using Amplitude.Graphics;
using Amplitude.Mercury.AI;
using Amplitude.Mercury.Data.AI;
using Amplitude.Mercury.Interop;
using Amplitude.Mercury.Interop.AI.Data;
using Amplitude.Mercury.Presentation;
using Amplitude.Mercury.Runtime;
using Amplitude.Mercury.Sandbox;
using Amplitude.Mercury.Simulation;
using Amplitude.Mercury.Terrain;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Net;
using Amplitude;
using Amplitude.Mercury;
using UnityEngine;
using Amplitude.Mercury.Overlay;
using Modding.Humankind.DevTools.Core;
using Modding.Humankind.DevTools.DeveloperTools.UI;

namespace DevTools.Humankind.GUITools.UI
{
    public class AIToolWindow : FloatingToolWindow
    {
        protected internal const float DelayBetweenRequestsInSeconds = 1f;
        private readonly string[] controlledByHumanBindingNames;
        private readonly string[] debuggerTypeNames;
        private readonly string[] territoryAnalysisTypeNames;
        private readonly string[] diplomaticAnalysisTypeNames;

        private readonly string[] empireNames = new string[10]
        {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9"
        };

        private bool toggleAiState = true;
        private float nextRefresh;

        public override string WindowTitle { get; set; } = "AI TOOLS";

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
            OnDrawWindowTitleBar();

            OnDrawWindowContent();
        }

        public AIToolWindow()
        {
            this.controlledByHumanBindingNames = System.Enum.GetNames(typeof(ControlledByHumanBinding));
            this.debuggerTypeNames = System.Enum.GetNames(typeof(DebugControl.DebuggerType));
            this.territoryAnalysisTypeNames = System.Enum.GetNames(typeof(DebugControl.TerritoryAnalysisType));
            this.diplomaticAnalysisTypeNames = System.Enum.GetNames(typeof(DebugControl.DiplomaticAnalysisType));
        }

        protected Amplitude.Framework.Runtime.IRuntimeService RuntimeService { get; private set; }

        protected void OnDrawWindowTitleBar()
        {
            if (Amplitude.Mercury.AI.Preferences.ControlledByHumanBindingAfterStartup ==
                ControlledByHumanBinding.DeactivateAIOnAllEmpires ||
                Amplitude.Mercury.AI.Preferences.ExecuteAIOnMainThread)
            {
                Color color = GUI.color;
                Color backgroundColor = GUI.backgroundColor;
                GUI.color = Color.red;
                GUI.backgroundColor = Color.red;
                WindowUtils.DrawWindowTitleBar(this);
                GUI.color = color;
                GUI.backgroundColor = backgroundColor;
            }
            else
                WindowUtils.DrawWindowTitleBar(this);
        }

        protected void OnDrawWindowContent()
        {
            GUI.color = Color.white;
            GUI.backgroundColor = Color.white;
            GUILayout.BeginVertical((GUIStyle) "Widget.ClientArea", GUILayout.ExpandWidth(true));
            if (this.RuntimeService != null)
            {
                if (this.RuntimeService.Runtime != null && this.RuntimeService.Runtime.HasBeenLoaded)
                {
                    if (this.RuntimeService.Runtime.FiniteStateMachine.CurrentState != null)
                    {
                        if (this.RuntimeService.Runtime.FiniteStateMachine.CurrentState.GetType() ==
                            typeof(RuntimeState_InGame))
                        {
                            if (Amplitude.Mercury.Presentation.Presentation.HasBeenStarted)
                                this.OnDrawAIWindow();
                            else
                                GUILayout.Label("Waiting for the presentation...");
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

        private void OnDrawAIWindow()
        {
            var stateNames = controlledByHumanBindingNames.Select(sn => R.Text.Bold(R.Text.Size(R.Text.SplitCamelCase(sn).ToUpperInvariant(), 11)));
            
            GUILayout.BeginHorizontal();
            GUILayout.Space(8f);
            this.toggleAiState = GUILayout.Toggle(this.toggleAiState, "<size=10><b>AI STATE</b></size>");
            GUILayout.FlexibleSpace();
            if (this.toggleAiState)
                Amplitude.Mercury.AI.Preferences.ControlledByHumanBindingAfterStartup =
                    (ControlledByHumanBinding) GUILayout.SelectionGrid(
                        (int) Amplitude.Mercury.AI.Preferences.ControlledByHumanBindingAfterStartup,
                        /*this.controlledByHumanBindingNames*/ stateNames.ToArray(), 1, (GUIStyle) "PopupWindow.ToolbarButton");
            GUILayout.Space(8f);
            GUILayout.EndHorizontal();
            Amplitude.Mercury.AI.Preferences.PreventAttackMissions = GUILayout.Toggle(
                Amplitude.Mercury.AI.Preferences.PreventAttackMissions, "<size=11><b>Prevent Attack Missions</b></size>".ToUpperInvariant(),
                (GUIStyle) "PopupWindow.Button");
            GUILayout.BeginHorizontal();
            GUILayout.Label("Debug");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("<size=11><b>Force decision</b></size>".ToUpperInvariant()))
            {
                string address = "http://localhost:1477/api/ai-controller";
                string data =
                    "{\"ReloadAIAssembly\" : false , \"PauseAIDecisions\" : false, \"ForceNewAIDecision\" : true }";
                using (WebClient webClient = new WebClient())
                    webClient.UploadString(address, data);
            }

            GUI.enabled = false;
            if (GUILayout.Button("<size=11><b>Open Inspector</b></size>".ToUpperInvariant()))
                UnityEngine.Application.OpenURL("http://localhost:1477/");
            GUI.enabled = true;
            GUILayout.Space(8f);
            GUILayout.EndHorizontal();
            GUILayout.Space(4f);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Overlay");
            GUILayout.FlexibleSpace();
            int num = GUILayout.SelectionGrid((int) DebugControl.WantedDebugger, this.debuggerTypeNames, 4,
                (GUIStyle) "PopupWindow.ToolbarButton");
            if ((DebugControl.DebuggerType) num != DebugControl.WantedDebugger)
            {
                DebugControl.UpdatePresentationDebugger = true;
                DebugControl.WantedDebugger = (DebugControl.DebuggerType) num;
            }
            GUILayout.Space(8f);
            GUILayout.EndHorizontal();
            ITerrainPickingService instance = RenderContextAccess.GetInstance<ITerrainPickingService>(0);
            if (instance != null && DebugControl.WantedDebugger != DebugControl.DebuggerType.None)
            {
                Hexagon.OffsetCoords offsetHexCoords = new Hexagon.OffsetCoords();
                if (instance.ScreenPositionToHexagonOffsetCoords((Vector2) Input.mousePosition, ref offsetHexCoords))
                {
                    WorldPosition cursorPosition = DebugControl.CursorPosition;
                    DebugControl.CursorPosition = new WorldPosition(offsetHexCoords);
                    if (DebugControl.CursorPosition != cursorPosition)
                        DebugControl.UpdatePresentationDebugger = true;
                }
                else
                {
                    DebugControl.CursorPosition = WorldPosition.Invalid;
                    DebugControl.UpdatePresentationDebugger = true;
                }
            }

            switch (DebugControl.WantedDebugger)
            {
                case DebugControl.DebuggerType.Territory:
                    this.OnDrawTerritoryDebugger();
                    break;
                case DebugControl.DebuggerType.Military:
                    this.OnDrawMilitaryDebugger();
                    break;
                case DebugControl.DebuggerType.Army:
                    this.OnDrawArmyDebugger();
                    break;
                case DebugControl.DebuggerType.Terrain:
                    this.OnDrawTerrainDebugger();
                    break;
                case DebugControl.DebuggerType.Adjacency:
                    this.OnDrawAdjacencyDebugger();
                    break;
                case DebugControl.DebuggerType.City:
                    this.OnDrawCityDebugger();
                    break;
                case DebugControl.DebuggerType.BattleMap:
                    this.OnDrawBattleMapDebugger();
                    break;
                case DebugControl.DebuggerType.Fortification:
                    this.OnDrawFortificationDebugger();
                    break;
                case DebugControl.DebuggerType.Diplomacy:
                    this.OnDrawDiplomacyDebugger();
                    break;
                case DebugControl.DebuggerType.Visibility:
                    this.OnDrawVisibilityDebugger();
                    break;
            }
            GUILayout.Space(12f);
        }

        private void OnDrawDiplomacyDebugger()
        {
            GUILayout.Space(12f);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Empire");
            GUILayout.FlexibleSpace();
            int num1 = GUILayout.SelectionGrid(DebugControl.DiplomaticAnalysisEmpireIndex, this.empireNames,
                this.empireNames.Length, (GUIStyle) "PopupWindow.ToolbarButton");
            if (num1 != DebugControl.DiplomaticAnalysisEmpireIndex ||
                (double) Time.realtimeSinceStartup > (double) this.nextRefresh)
            {
                if (num1 != DebugControl.DiplomaticAnalysisEmpireIndex)
                    DebugControl.TerritoryRequestResult = (JObject) null;
                HttpWebRequest httpWebRequest = (HttpWebRequest) WebRequest.Create(
                    "http://localhost:1477/api/empires/" + num1.ToString() +
                    "/brain/analysis-data/all-diplomatic-relations/");
                httpWebRequest.BeginGetResponse(new AsyncCallback(this.OnDiplomacyRequestEnd), (object) httpWebRequest);
                DebugControl.DiplomaticAnalysisEmpireIndex = num1;
                this.nextRefresh = Time.realtimeSinceStartup + 1f;
            }
            GUILayout.Space(8f);
            GUILayout.EndHorizontal();
            if (DebugControl.DiplomacyRequestResult == null)
                return;
            GUILayout.BeginHorizontal();
            GUILayout.Label("Analysis");
            GUILayout.FlexibleSpace();
            int num2 = GUILayout.SelectionGrid((int) DebugControl.WantedDiplomaticAnalaysis,
                this.diplomaticAnalysisTypeNames, 3, (GUIStyle) "PopupWindow.ToolbarButton");
            if ((DebugControl.DiplomaticAnalysisType) num2 != DebugControl.WantedDiplomaticAnalaysis)
            {
                DebugControl.UpdatePresentationDebugger = true;
                DebugControl.WantedDiplomaticAnalaysis = (DebugControl.DiplomaticAnalysisType) num2;
            }

            GUILayout.EndHorizontal();
            if (!DebugControl.CursorPosition.IsWorldPositionValid())
                return;
            int territoryIndex = (int) Amplitude.Mercury.Interop.AI.Snapshots.World
                .TileInfo[DebugControl.CursorPosition.ToTileIndex()].TerritoryIndex;
            Amplitude.Mercury.Interop.AI.Entities.Territory territory =
                Amplitude.Mercury.Interop.AI.Snapshots.World.Territories[territoryIndex];
            if (territory.Settlement == null)
                return;
            int empireIndex = territory.Settlement.Empire.EmpireIndex;
            if (empireIndex >= Amplitude.Mercury.Interop.AI.Snapshots.Game.MajorEmpires.Length ||
                empireIndex == DebugControl.DiplomaticAnalysisEmpireIndex)
                return;
            JToken jtoken = DebugControl.DiplomacyRequestResult["Data"][(object) empireIndex];
            GUILayout.Label("Attitude : " + System.Enum.GetName(typeof(AIBehaviourState),
                (object) jtoken[(object) "DiplomaticContext"][(object) "StateMachineContext"][(object) "CurrentState"]
                    [(object) "Id"].ToObject<AIBehaviourState>()));
            GUILayout.Label(string.Format("Rapport: {0}", (object) jtoken[(object) "RapportScore"][(object) "Value"]));
            GUILayout.Label(string.Format("Superiority: {0}",
                (object) jtoken[(object) "SuperiorityScore"][(object) "Value"]));
        }

        private void OnDiplomacyRequestEnd(IAsyncResult asynchronousResult)
        {
            try
            {
                using (HttpWebResponse response =
                    (HttpWebResponse) ((WebRequest) asynchronousResult.AsyncState).EndGetResponse(asynchronousResult))
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader streamReader = new StreamReader(responseStream))
                        {
                            DebugControl.DiplomacyRequestResult = JObject.Parse(streamReader.ReadToEnd());
                            DebugControl.UpdatePresentationDebugger = true;
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                Diagnostics.LogException((Exception) ex);
            }
        }

        private void OnDrawTerritoryDebugger()
        {
            GUILayout.Space(12f);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Empire");
            GUILayout.FlexibleSpace();
            int wantedEmpire = GUILayout.SelectionGrid(DebugControl.TerritoryAnalysisEmpireIndex, this.empireNames, 8,
                (GUIStyle) "PopupWindow.ToolbarButton");
            if (wantedEmpire != DebugControl.TerritoryAnalysisEmpireIndex ||
                (double) Time.realtimeSinceStartup > (double) this.nextRefresh)
            {
                this.RefreshTerritoryRequestIfNeeded(wantedEmpire);
                DebugControl.TerritoryAnalysisEmpireIndex = wantedEmpire;
                this.nextRefresh = Time.realtimeSinceStartup + 1f;
            }

            GUILayout.EndHorizontal();
            if (DebugControl.TerritoryRequestResult == null)
                return;
            GUILayout.BeginHorizontal();
            GUILayout.Label("Analysis");
            GUILayout.FlexibleSpace();
            int num = GUILayout.SelectionGrid((int) DebugControl.WantedTerritoryAnalysis,
                this.territoryAnalysisTypeNames, 3, (GUIStyle) "PopupWindow.ToolbarButton");
            if ((DebugControl.TerritoryAnalysisType) num != DebugControl.WantedTerritoryAnalysis)
            {
                DebugControl.UpdatePresentationDebugger = true;
                DebugControl.WantedTerritoryAnalysis = (DebugControl.TerritoryAnalysisType) num;
            }

            GUILayout.EndHorizontal();
            if (!DebugControl.CursorPosition.IsWorldPositionValid())
                return;
            int territoryIndex = (int) Amplitude.Mercury.Interop.AI.Snapshots.World
                .TileInfo[DebugControl.CursorPosition.ToTileIndex()].TerritoryIndex;
            GUILayout.Label(string.Format("Position {0} | Territory {1}", (object) DebugControl.CursorPosition,
                (object) territoryIndex));
            try
            {
                JToken jtoken = DebugControl.TerritoryRequestResult["Data"][(object) territoryIndex];
                switch (DebugControl.WantedTerritoryAnalysis)
                {
                    case DebugControl.TerritoryAnalysisType.Development:
                        GUILayout.Label("Score :" + jtoken[(object) "DevelopmentScore"][(object) "Value"]?.ToString());
                        break;
                    case DebugControl.TerritoryAnalysisType.Importance:
                        GUILayout.Label("Score :" + jtoken[(object) "Importance"][(object) "Value"]?.ToString());
                        break;
                    case DebugControl.TerritoryAnalysisType.Threat:
                        GUILayout.Label("Max ground enemy strength :" +
                                        jtoken[(object) "MaxGroundEnemyStrength"][(object) "Value"]?.ToString());
                        GUILayout.Label("Max naval enemy strength :" +
                                        jtoken[(object) "MaxNavalEnemyStrength"][(object) "Value"]?.ToString());
                        GUILayout.Label("Threat: " + jtoken[(object) "Threat"].ToObject<HeuristicFloat>().ToString());
                        break;
                    case DebugControl.TerritoryAnalysisType.WantedGroundArmies:
                        GUILayout.Label("WantedGroundArmies: " +
                                        jtoken[(object) "WantedGroundArmies"].ToObject<HeuristicFloat>().ToString());
                        break;
                    case DebugControl.TerritoryAnalysisType.WantedNavalArmies:
                        GUILayout.Label("WantedNavalArmies: " +
                                        jtoken[(object) "WantedNavalArmies"].ToObject<HeuristicFloat>().ToString());
                        break;
                    case DebugControl.TerritoryAnalysisType.Strategic:
                        GUILayout.Label("Strategic: " +
                                        jtoken[(object) "StrategicScore"].ToObject<HeuristicFloat>().ToString());
                        break;
                    case DebugControl.TerritoryAnalysisType.Exploration:
                        GUILayout.Label("ExplorationScore :" +
                                        jtoken[(object) "ExplorationScore"][(object) "Value"]?.ToString());
                        GUILayout.Label("Distance from settlement :" +
                                        jtoken[(object) "DistanceFromSettlements"]?.ToString());
                        for (int index = 0; index < jtoken[(object) "TerritoryTags"].Count<JToken>(); ++index)
                            GUILayout.Label(jtoken[(object) "TerritoryTags"][(object) index][(object) "PrettyName"]
                                .ToString());
                        break;
                    case DebugControl.TerritoryAnalysisType.PathFind:
                        DebugControl.LandUnit = GUILayout.Toggle(DebugControl.LandUnit, "LandUnit");
                        DebugControl.CanLostAtSea = GUILayout.Toggle(DebugControl.CanLostAtSea, "CanLostAtSea");
                        DebugControl.IncludeCloseBorder =
                            GUILayout.Toggle(DebugControl.IncludeCloseBorder, "IncludeCloseBorder");
                        break;
                }
            }
            catch (Exception ex)
            {
                GUILayout.Label(ex.Message);
            }
        }

        private void RefreshTerritoryRequestIfNeeded(int wantedEmpire)
        {
            if (wantedEmpire != DebugControl.TerritoryAnalysisEmpireIndex)
                DebugControl.TerritoryRequestResult = (JObject) null;
            HttpWebRequest httpWebRequest = (HttpWebRequest) WebRequest.Create("http://localhost:1477/api/empires/" +
                                                                               Amplitude.Mercury.Interop.Snapshots
                                                                                   .GameSnapshot.PresentationData
                                                                                   .EmpireInfo[wantedEmpire].EmpireIndex
                                                                                   .ToString() +
                                                                               "/brain/analysis-data/territories");
            httpWebRequest.BeginGetResponse(new AsyncCallback(this.OnTerritoryRequestEnd), (object) httpWebRequest);
        }

        private void OnTerritoryRequestEnd(IAsyncResult asynchronousResult)
        {
            try
            {
                using (HttpWebResponse response =
                    (HttpWebResponse) ((WebRequest) asynchronousResult.AsyncState).EndGetResponse(asynchronousResult))
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader streamReader = new StreamReader(responseStream))
                        {
                            DebugControl.TerritoryRequestResult = JObject.Parse(streamReader.ReadToEnd());
                            DebugControl.UpdatePresentationDebugger = true;
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                Diagnostics.LogException((Exception) ex);
            }
        }

        private void OnDrawMilitaryDebugger()
        {
            GUILayout.Space(12f);
            ref LocalEmpireInfo local =
                ref Amplitude.Mercury.Interop.Snapshots.GameSnapshot.PresentationData.LocalEmpireInfo;
            if ((double) Time.realtimeSinceStartup > (double) this.nextRefresh)
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest) WebRequest.Create(
                    "http://localhost:1477/api/empires/" + local.EmpireIndex.ToString() +
                    "/brain/analysis-data/military-missions");
                httpWebRequest.BeginGetResponse(new AsyncCallback(this.OnMilitaryRequestEnd), (object) httpWebRequest);
                this.nextRefresh = Time.realtimeSinceStartup + 1f;
            }

            if (DebugControl.MilitaryRequestResult == null || !DebugControl.CursorPosition.IsWorldPositionValid())
                return;
            JObject militaryRequestResult = DebugControl.MilitaryRequestResult;
            int num = militaryRequestResult["Data"].Count<JToken>();
            GUILayout.Label(string.Format("{0} military missions in desire set for empire {1}", (object) num,
                (object) local.EmpireIndex));
            int territoryIndex = (int) Amplitude.Mercury.Interop.AI.Snapshots.World
                .TileInfo[DebugControl.CursorPosition.ToTileIndex()].TerritoryIndex;
            GUILayout.Label(string.Format("Position {0} | Territory {1}", (object) DebugControl.CursorPosition,
                (object) territoryIndex));
            for (int index = 0; index < num; ++index)
            {
                JToken jtoken = militaryRequestResult["Data"][(object) index];
                if (jtoken.Value<int>((object) "TargetTerritoryIndex") == territoryIndex)
                {
                    GUILayout.Label(string.Format("- Task {0}: {1} - Motivation: {2}", (object) jtoken[(object) "Id"],
                        (object) (MilitaryObjective) jtoken.Value<int>((object) "Objective"),
                        (object) jtoken[(object) "Motivation"].Value<float>((object) "Value")));
                    GUILayout.Label(string.Format("  Army type: {0} - Status: {1} - Reinforcement: {2}",
                        (object) jtoken[(object) "WantedArmyType"], (object) jtoken[(object) "Status"],
                        (object) jtoken[(object) "ReinforcementIndex"]));
                }
            }
        }

        private void OnMilitaryRequestEnd(IAsyncResult asynchronousResult)
        {
            try
            {
                using (HttpWebResponse response =
                    (HttpWebResponse) ((WebRequest) asynchronousResult.AsyncState).EndGetResponse(asynchronousResult))
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader streamReader = new StreamReader(responseStream))
                        {
                            DebugControl.MilitaryRequestResult = JObject.Parse(streamReader.ReadToEnd());
                            DebugControl.UpdatePresentationDebugger = true;
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                Diagnostics.LogException((Exception) ex);
            }
        }

        private void OnDrawArmyDebugger()
        {
            GUILayout.Space(12f);
            if (!Amplitude.Mercury.Interop.Snapshots.ArmyCursorSnapshot.PresentationData.EntityGUID.IsValid)
            {
                DebugControl.ArmyGuid = 0UL;
            }
            else
            {
                int empireIndex = (int) Amplitude.Mercury.Interop.Snapshots.ArmyCursorSnapshot.PresentationData
                    .EmpireIndex;
                if ((SimulationEntityGUID) DebugControl.ArmyGuid != Amplitude.Mercury.Interop.Snapshots
                        .ArmyCursorSnapshot.PresentationData.EntityGUID ||
                    (double) Time.realtimeSinceStartup > (double) this.nextRefresh)
                {
                    DebugControl.ArmyGuid = (ulong) Amplitude.Mercury.Interop.Snapshots.ArmyCursorSnapshot
                        .PresentationData.EntityGUID;
                    HttpWebRequest httpWebRequest = (HttpWebRequest) WebRequest.Create(
                        "http://localhost:1477/api/empires/" + empireIndex.ToString() + "/brain/army-actuator/" +
                        Amplitude.Mercury.Interop.Snapshots.ArmyCursorSnapshot.PresentationData.EntityGUID.ToString());
                    httpWebRequest.BeginGetResponse(new AsyncCallback(this.OnArmyRequestEnd), (object) httpWebRequest);
                    if ((SimulationEntityGUID) DebugControl.ArmyGuid != Amplitude.Mercury.Interop.Snapshots
                        .ArmyCursorSnapshot.PresentationData.EntityGUID)
                        DebugControl.ArmyRequestResult = (JObject) null;
                    this.nextRefresh = Time.realtimeSinceStartup + 1f;
                }

                if (GUILayout.Button(
                    "Army " + DebugControl.ArmyGuid.ToString() + " of empire " + empireIndex.ToString()))
                    UnityEngine.Application.OpenURL(string.Format(
                        "http://localhost:1477/empires/{0}/execution/army-actuators?armyGuid={1}", (object) empireIndex,
                        (object) DebugControl.ArmyGuid));
                if (GUILayout.Button("Toggle auto-explore"))
                    SandboxManager.PostOrder((Order) new OrderToggleAutoExplore()
                    {
                        EntityGUID = (SimulationEntityGUID) DebugControl.ArmyGuid
                    });
                try
                {
                    JObject armyRequestResult = DebugControl.ArmyRequestResult;
                    JToken jtoken = armyRequestResult["Data"][(object) 0][(object) "TreeContext"];
                    GUILayout.Label("Behavior tree: " +
                                    armyRequestResult["Data"][(object) 0][(object) "BehaviorTreeName"]?.ToString());
                    GUILayout.Label("Move command: " + jtoken[(object) "CurrentMoveCommand"]?.ToString());
                    GUILayout.Label("Move state: " + jtoken[(object) "CurrentMoveState"]?.ToString());
                    GUILayout.Label("Mission type: " +
                                    jtoken[(object) "MissionInfo"][(object) "MissionType"]?.ToString());
                    GUILayout.Label("Mission flags: " +
                                    jtoken[(object) "MissionInfo"][(object) "MissionFlags"]?.ToString());
                }
                catch (Exception)
                {
                }
            }
        }

        private void OnArmyRequestEnd(IAsyncResult asynchronousResult)
        {
            try
            {
                using (HttpWebResponse response =
                    (HttpWebResponse) ((WebRequest) asynchronousResult.AsyncState).EndGetResponse(asynchronousResult))
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader streamReader = new StreamReader(responseStream))
                        {
                            DebugControl.ArmyRequestResult = JObject.Parse(streamReader.ReadToEnd());
                            DebugControl.UpdatePresentationDebugger = true;
                        }
                    }
                }
            }
            catch
            {
                DebugControl.ArmyRequestResult = (JObject) null;
            }
        }

        private void OnSettlementRequestEnd(IAsyncResult asynchronousResult)
        {
            try
            {
                using (HttpWebResponse response =
                    (HttpWebResponse) ((WebRequest) asynchronousResult.AsyncState).EndGetResponse(asynchronousResult))
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader streamReader = new StreamReader(responseStream))
                        {
                            DebugControl.SettlementRequestResult = JObject.Parse(streamReader.ReadToEnd());
                            DebugControl.UpdatePresentationDebugger = true;
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                Diagnostics.LogException((Exception) ex);
            }
        }

        private void OnDrawTerrainDebugger()
        {
            if (!DebugControl.CursorPosition.IsWorldPositionValid())
                return;
            ref Tile local =
                ref Amplitude.Mercury.Interop.AI.Snapshots.World.Tiles[DebugControl.CursorPosition.ToTileIndex()];
            GUILayout.Label(string.Format("Position {0} | {1}", (object) DebugControl.CursorPosition,
                (object) local.MovementType));
            GUILayout.Label(string.Format("{0} exploitable neighbors", (object) local.AdjacentExploitableTilesCount));
            GUILayout.Label(string.Format("Neighbor FIMS: {0}", (object) local.AdjacentExploitableTilesFims));
            GUILayout.Label(string.Format("Visible {0}",
                (object) Amplitude.Mercury.Presentation.Presentation.PresentationVisibilityController.IsTileVisible(
                    DebugControl.CursorPosition.ToTileIndex())));
            GUILayout.Label(string.Format("Explored {0}",
                (object) Amplitude.Mercury.Presentation.Presentation.PresentationVisibilityController.IsTileExplored(
                    DebugControl.CursorPosition.ToTileIndex())));
            if (!local.GreaterElevationThanAdjacentTiles)
                return;
            GUILayout.Label("None of the neighbors have a higher elevation.");
        }

        private void OnDrawAdjacencyDebugger()
        {
            if (!DebugControl.CursorPosition.IsWorldPositionValid())
                return;
            int tileIndex = DebugControl.CursorPosition.ToTileIndex();
            ref Tile local1 = ref Amplitude.Mercury.Interop.AI.Snapshots.World.Tiles[tileIndex];
            ref TileInfo local2 = ref Amplitude.Mercury.Interop.AI.Snapshots.World.TileInfo[tileIndex];
            Amplitude.Mercury.Interop.AI.Entities.Territory territory =
                Amplitude.Mercury.Interop.AI.Snapshots.World.Territories[(int) local2.TerritoryIndex];
            GUILayout.Label(string.Format("Position {0} | {1}", (object) DebugControl.CursorPosition,
                (object) local1.MovementType));
            GUILayout.Label(string.Format("Territory {0}", (object) local2.TerritoryIndex));
            for (int index = 0; index < territory.Adjacencies.Length; ++index)
                GUILayout.Label(string.Format("Adjacency with {0}: {1}",
                    (object) territory.Adjacencies[index].TerritoryIndex,
                    (object) territory.Adjacencies[index].Transition));
        }

        private void OnDrawCityDebugger()
        {
            if (!(Amplitude.Mercury.Presentation.Presentation.PresentationCursorController.CurrentCursor is
                SettlementCursor))
                return;
            GUILayout.Label("Settlement of empire " + Amplitude.Mercury.Interop.Snapshots.SettlementCursorSnapshot
                .PresentationData.EmpireIndex.ToString());
            if (GUILayout.Button("Ask Advice"))
                SandboxManager.PostOrder((Order) new OrderAskAdviseConstructible()
                {
                    SettlementGUID = Amplitude.Mercury.Interop.Snapshots.SettlementCursorSnapshot.PresentationData
                        .SettlementGUID
                });
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("<"))
            {
                DebugControl.SettlementComputeOrder = System.Math.Max(0, DebugControl.SettlementComputeOrder - 1);
                DebugControl.UpdatePresentationDebugger = true;
            }

            bool flag = GUILayout.Toggle(DebugControl.SettlementComputeOrderAtMax, "Max");
            if (flag != DebugControl.SettlementComputeOrderAtMax)
            {
                DebugControl.SettlementComputeOrderAtMax = flag;
                DebugControl.UpdatePresentationDebugger = true;
            }

            if (GUILayout.Button(">"))
            {
                ++DebugControl.SettlementComputeOrder;
                DebugControl.UpdatePresentationDebugger = true;
            }

            GUILayout.EndHorizontal();
            JObject settlementRequestResult = DebugControl.SettlementRequestResult;
            if (settlementRequestResult != null && settlementRequestResult["Data"].Any<JToken>())
            {
                JToken jtoken1 = settlementRequestResult["Data"][(object) 0];
                try
                {
                    JToken source = jtoken1[(object) "NeedBydistrictCountGain"];
                    JToken jtoken2 = jtoken1[(object) "DistrictCountByGain"];
                    string str1 = string.Empty;
                    for (int index = 0; index < source.Count<JToken>(); ++index)
                        str1 = str1 + "[" + jtoken2[(object) index]?.ToString() + "/" +
                               source[(object) index]?.ToString() + "]";
                    GUILayout.Label("FimsNeed:" + str1);
                    WorldPosition highlightedPosition = Amplitude.Mercury.Presentation.Presentation
                        .PresentationCursorController.CurrentHighlightedPosition;
                    for (int index1 = 0; index1 < jtoken1[(object) "WantedDistricts"].Count<JToken>(); ++index1)
                    {
                        if (jtoken1[(object) "WantedDistricts"][(object) index1].Value<int>((object) "TileIndex") ==
                            highlightedPosition.ToTileIndex())
                        {
                            string[] strArray = jtoken1[(object) "WantedDistricts"][(object) index1][(object) "Score"]
                                .ToString().Split('\n');
                            string str2 = string.Empty;
                            for (int index2 = 0; index2 < strArray.Length; ++index2)
                            {
                                string str3 = strArray[index2].Trim();
                                if (str3.Length > 3 && !str3.Contains("Children"))
                                    str2 = str2 + strArray[index2] + "\n";
                            }

                            GUILayout.Label(string.Format("{0}: {1}",
                                (object) jtoken1[(object) "WantedDistricts"][(object) index1][
                                    (object) "DistrictConstructionName"], (object) str2));
                        }
                    }
                }
                catch (Exception)
                {
                }
            }

            if (!((SimulationEntityGUID) DebugControl.SettlementGuid != Amplitude.Mercury.Interop.Snapshots
                    .SettlementCursorSnapshot.PresentationData.SettlementGUID) &&
                (double) Time.realtimeSinceStartup <= (double) this.nextRefresh)
                return;
            DebugControl.SettlementGuid = (ulong) Amplitude.Mercury.Interop.Snapshots.SettlementCursorSnapshot
                .PresentationData.SettlementGUID;
            string requestUriString = "http://localhost:1477/api/empires/" +
                                      Amplitude.Mercury.Interop.Snapshots.SettlementCursorSnapshot.PresentationData
                                          .EmpireIndex.ToString() + "/brain/analysis-data/entities/" + Amplitude.Mercury
                                          .Interop.Snapshots.SettlementCursorSnapshot.PresentationData.SettlementGUID
                                          .ToString();
            Diagnostics.Log("Send url {0}", (object) requestUriString);
            HttpWebRequest httpWebRequest = (HttpWebRequest) WebRequest.Create(requestUriString);
            httpWebRequest.BeginGetResponse(new AsyncCallback(this.OnSettlementRequestEnd), (object) httpWebRequest);
            if ((SimulationEntityGUID) DebugControl.SettlementGuid != Amplitude.Mercury.Interop.Snapshots
                .SettlementCursorSnapshot.PresentationData.SettlementGUID)
                DebugControl.SettlementRequestResult = (JObject) null;
            this.nextRefresh = Time.realtimeSinceStartup + 1f;
        }

        private void OnDrawBattleMapDebugger()
        {
            if (!DebugControl.CursorPosition.IsWorldPositionValid())
                return;
            int battle =
                Amplitude.Mercury.Interop.AI.Snapshots.Battle.BattleMap[DebugControl.CursorPosition.ToTileIndex()];
            if (battle < 0)
                return;
            GUILayout.Label(string.Format("Battle {0} (index: {1})",
                (object) Amplitude.Mercury.Interop.AI.Snapshots.Battle.Battles[battle].SimulationEntityGUID,
                (object) battle));
        }

        private void OnDrawFortificationDebugger()
        {
        }

        private void OnDrawVisibilityDebugger()
        {
            ref LocalEmpireInfo local =
                ref Amplitude.Mercury.Interop.Snapshots.GameSnapshot.PresentationData.LocalEmpireInfo;
            if ((double) Time.realtimeSinceStartup <= (double) this.nextRefresh)
                return;
            HttpWebRequest httpWebRequest = (HttpWebRequest) WebRequest.Create("http://localhost:1477/api/empires/" +
                                                                               local.EmpireIndex.ToString() +
                                                                               "/brain/visibility-dates");
            httpWebRequest.BeginGetResponse(new AsyncCallback(this.OnVisibilityRequestEnd), (object) httpWebRequest);
            this.nextRefresh = Time.realtimeSinceStartup + 1f;
        }

        private void OnVisibilityRequestEnd(IAsyncResult asynchronousResult)
        {
            try
            {
                using (HttpWebResponse response =
                    (HttpWebResponse) ((WebRequest) asynchronousResult.AsyncState).EndGetResponse(asynchronousResult))
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader streamReader = new StreamReader(responseStream))
                        {
                            DebugControl.VisibilityRequestResult =
                                JArray.FromObject((object) JObject.Parse(streamReader.ReadToEnd())["Data"]);
                            DebugControl.UpdatePresentationDebugger = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Diagnostics.LogException(ex);
            }
        }
    }
}