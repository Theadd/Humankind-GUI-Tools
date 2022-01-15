using Amplitude.Framework;
using Amplitude.Framework.Overlay;
using Amplitude.Mercury.Data.Simulation;
using Amplitude.Mercury.Interop;
using Amplitude.Mercury.Runtime;
using Amplitude.Mercury.Sandbox;
using Amplitude;
using UnityEngine;

using Modding.Humankind.DevTools.Core;
using Modding.Humankind.DevTools.DeveloperTools.UI;

namespace DevTools.Humankind.GUITools.UI
{
    public class SettlementToolsWindow : FloatingToolWindow
    {
        public override string WindowTitle { get; set; } = "SETTLEMENT TOOLS";

        public override string WindowGUIStyle { get; set; } = "PopupWindow";

        public override bool ShouldBeVisible => !GlobalSettings.ShouldHideTools;

        public override bool ShouldRestoreLastWindowPosition => true;

        public override Rect WindowRect { get; set; } = new Rect(130f, 260f, 520f, 500f);

        private Color bgColor = new Color32(255, 255, 255, 230);
        private Color bgColorOpaque = new Color32(255, 255, 255, 255);

        public override void OnGUIStyling()
        {
            base.OnGUIStyling();
            GUI.backgroundColor = GlobalSettings.WindowTransparency.Value ? bgColor : bgColorOpaque;
        }

        public override void OnDrawUI()
        {
            if (GlobalSettings.WindowTitleBar.Value)
                WindowUtils.DrawWindowTitleBar(this);

            OnDrawWindowClientArea(0);
        }
        private static StaticString campRelocationConstructibleName = new StaticString("ConstructibleAction_CampRelocationCenter");
        private Vector2 scrollPosition = Vector2.zero;

        protected Amplitude.Framework.Runtime.IRuntimeService RuntimeService { get; private set; }

        protected override void OnDrawWindowClientArea(int instanceId)
        {
            GUILayout.BeginVertical((GUIStyle)"Widget.ClientArea", GUILayout.ExpandWidth(true));
            this.Draw();
            GUILayout.EndVertical();
        }

        private void Draw()
        {
            if (this.RuntimeService == null)
            {
                GUILayout.Label("Waiting for the runtime service...");
                if (UnityEngine.Event.current.type != UnityEngine.EventType.Repaint)
                    return;
                this.RuntimeService = Services.GetService<Amplitude.Framework.Runtime.IRuntimeService>();
            }
            else if (this.RuntimeService.Runtime == null || !this.RuntimeService.Runtime.HasBeenLoaded)
                GUILayout.Label("Waiting for the runtime...");
            else if (this.RuntimeService.Runtime.FiniteStateMachine.CurrentState == null)
                GUILayout.Label("Waiting for the runtime...");
            else if (this.RuntimeService.Runtime.FiniteStateMachine.CurrentState.GetType() != typeof(RuntimeState_InGame))
                GUILayout.Label("Waiting for the runtime state...");
            else if (!Amplitude.Mercury.Presentation.Presentation.HasBeenStarted)
                GUILayout.Label("Waiting for the presentation...");
            else if (Snapshots.GameSnapshot == null || Snapshots.SettlementCursorSnapshot == null || !Snapshots.GameSnapshot.Enabled)
                GUILayout.Label("Waiting for the snapshots...");
            else if (!Snapshots.SettlementCursorSnapshot.Enabled || !Snapshots.SettlementCursorSnapshot.PresentationData.SettlementGUID.IsValid)
            {
                GUILayout.Label("No settlement selected...");
            }
            else
            {
                GameSnapshot.Data presentationData1 = Snapshots.GameSnapshot.PresentationData;
                SettlementCursorSnapshot.Data presentationData2 = Snapshots.SettlementCursorSnapshot.PresentationData;
                GUILayout.Label("GUID : " + presentationData2.SettlementGUID.ToString());
                GUILayout.Label("Status : " + presentationData2.SettlementStatus.ToString());
                GUILayout.Label("Name : " + presentationData2.EntityName.ToString());
                GUILayout.Label("Territory Count : " + presentationData2.TerritoryCount.ToString());
                if (presentationData2.SettlementStatus == SettlementStatuses.Camp && presentationData2.EmpireIndex == (int)presentationData1.LocalEmpireInfo.EmpireIndex)
                {
                    GUI.enabled = true;
                    if (GUILayout.Button("CampRelocationCursor", (GUIStyle)"PopupWindow.ToolbarButton", GUILayout.Width(150f)))
                        Amplitude.Mercury.Presentation.Presentation.PresentationCursorController.ChangeToCampRelocationCursor(presentationData2.SettlementGUID, SettlementToolsWindow.campRelocationConstructibleName);
                    ref SettlementActionInfo local = ref presentationData2.SettlementActionInfos[20];
                    GUI.enabled = local.FailureFlags == Amplitude.Mercury.Simulation.FailureFlags.None;
                    if (GUILayout.Button("Evolve to City", (GUIStyle)"PopupWindow.ToolbarButton"))
                        SandboxManager.PostOrder((Order)new OrderSettlementEvolveToCity()
                        {
                            SettlementGUID = presentationData2.SettlementGUID
                        });
                    if (local.FailureFlags == Amplitude.Mercury.Simulation.FailureFlags.None)
                        return;
                    GUILayout.Label(string.Format("Evolve failure flags : {0}.", (object)local.FailureFlags));
                }
                else
                {
                    if (presentationData2.SettlementStatus != SettlementStatuses.City)
                        return;
                    GUILayout.BeginHorizontal();
                    ref SettlementActionInfo local1 = ref presentationData2.SettlementActionInfos[19];
                    GUI.enabled = local1.FailureFlags == Amplitude.Mercury.Simulation.FailureFlags.None;
                    GUILayout.Label(string.Format("Warning: {0}", (object)local1.SettlementActionWarning));
                    if (GUILayout.Button("Liberate settlement"))
                        SandboxManager.PostOrder((Order)new OrderLiberateSettlement()
                        {
                            SettlementGUID = presentationData2.SettlementGUID
                        });
                    GUILayout.EndHorizontal();
                    if (presentationData2.CurrentConstructionInfos.Length != 0)
                    {
                        if (presentationData2.CurrentConstructionInfos[0].CanBeBoughtOutWithPopulation)
                        {
                            GUILayout.Label(string.Format("Population buyout\n Convert '{0}' population into '{1}' industry.", (object)presentationData2.CurrentConstructionInfos[0].PopulationBuyoutCost, (object)presentationData2.CurrentConstructionInfos[0].PopulationBuyoutIndustryProduced));
                            if (presentationData2.CurrentConstructionInfos[0].CanAffordPopulationBuyout)
                            {
                                if (GUILayout.Button("Buyout with population"))
                                    SandboxManager.PostOrder((Order)new OrderBuyoutConstructionWithPopulationAt()
                                    {
                                        SettlementGUID = presentationData2.SettlementGUID,
                                        ConstructionIndex = 0
                                    });
                            }
                            else
                                GUILayout.Label("Can't afford buyout with population.");
                        }
                        else
                            GUILayout.Label("Buyout with population locked.");
                    }
                    ref SettlementActionInfo local2 = ref presentationData2.SettlementActionInfos[8];
                    if ((local2.FailureFlags & Amplitude.Mercury.Simulation.FailureFlags.ActionLocked) == Amplitude.Mercury.Simulation.FailureFlags.None)
                    {
                        GUILayout.Label("Money Mode");
                        int cooldownDuration = local2.CooldownDuration;
                        int currentTurn = presentationData1.CurrentTurn;
                        int remainingCooldown = local2.RemainingCooldown;
                        if (local2.IsToggledOn && remainingCooldown < cooldownDuration)
                            GUILayout.Label(string.Format("Toggle money mode cooldown : {0} turns.", (object)(cooldownDuration - remainingCooldown)));
                        if (GUILayout.Button(!local2.IsToggledOn ? "Enter money mode" : "Exit money mode."))
                            SandboxManager.PostOrder((Order)new OrderToggleMoneyModeAt()
                            {
                                SettlementGUID = presentationData2.SettlementGUID
                            });
                    }
                    else
                        GUILayout.Label("Money Mode locked.");
                    byte empireIndex = Snapshots.GameSnapshot.PresentationData.LocalEmpireInfo.EmpireIndex;
                    ref EmpireInfo local3 = ref Snapshots.GameSnapshot.PresentationData.EmpireInfo[(int)empireIndex];
                    GUILayout.Space(10f);
                    GUILayout.Label("Expansionist affinity");
                    if ((local3.ExoticAbilityFlags & EmpireExoticAbilityFlags.CanBuyNeighbourTerritoryWithMoney) == EmpireExoticAbilityFlags.None)
                    {
                        GUILayout.Label("Bribe neighbour settlement action locked.");
                        GUI.enabled = false;
                    }
                    if (GUILayout.Button("Bribe neighbour settlement with money"))
                        Amplitude.Mercury.Presentation.Presentation.PresentationCursorController.ChangeToRallyTerritoryCursor(presentationData2.SettlementGUID);
                    GUI.enabled = true;
                    ref SettlementActionInfo local4 = ref presentationData2.SettlementActionInfos[14];
                    GUI.enabled = local4.FailureFlags == Amplitude.Mercury.Simulation.FailureFlags.None;
                    if (GUILayout.Button("Launch Inquisition", (GUIStyle)"PopupWindow.ToolbarButton"))
                        SandboxManager.PostOrder((Order)new OrderSettlementLaunchInquisition()
                        {
                            SettlementGUID = presentationData2.SettlementGUID
                        });
                    if (local4.FailureFlags != Amplitude.Mercury.Simulation.FailureFlags.None)
                        GUILayout.Label(string.Format("Inquisition failure flags : {0}.", (object)local4.FailureFlags));
                    ref SettlementActionInfo local5 = ref presentationData2.SettlementActionInfos[12];
                    GUI.enabled = local5.FailureFlags == Amplitude.Mercury.Simulation.FailureFlags.None;
                    if (GUILayout.Button("Start procession", (GUIStyle)"PopupWindow.ToolbarButton"))
                        SandboxManager.PostOrder((Order)new OrderSettlementStartProcession()
                        {
                            SettlementGUID = presentationData2.SettlementGUID
                        });
                    if (local5.FailureFlags != Amplitude.Mercury.Simulation.FailureFlags.None)
                        GUILayout.Label(string.Format("Procession failure flags : {0}.", (object)local5.FailureFlags));
                    ref SettlementActionInfo local6 = ref presentationData2.SettlementActionInfos[13];
                    GUI.enabled = local6.FailureFlags == Amplitude.Mercury.Simulation.FailureFlags.None;
                    if (GUILayout.Button("Raise reservist army"))
                        SandboxManager.PostOrder((Order)new OrderRaiseSettlementReservistArmy()
                        {
                            SettlementGUID = presentationData2.SettlementGUID
                        });
                    if (local6.AffinityActionFailureFlags != AffinityActionFailureFlags.None)
                        GUILayout.Label(string.Format("Raise reserviste failure flags : {0}.", (object)local6.AffinityActionFailureFlags));
                    GUI.enabled = presentationData2.SettlementActionInfos[19].FailureFlags == Amplitude.Mercury.Simulation.FailureFlags.None;
                    if (GUILayout.Button("Liberate settlement"))
                        SandboxManager.PostOrder((Order)new OrderLiberateSettlement()
                        {
                            SettlementGUID = presentationData2.SettlementGUID
                        });
                    GUI.enabled = true;
                    this.scrollPosition = GUILayout.BeginScrollView(this.scrollPosition, GUILayout.MinHeight(150f));
                    if (presentationData2.EmpireIndex >= presentationData1.NumberOfMajorEmpires)
                    {
                        if (!Snapshots.MinorPatronageSnapshot.Enabled)
                            Snapshots.MinorPatronageSnapshot.Start(presentationData2.EmpireIndex, MinorPatronageSnapshot.ActivationFlags.DebugWindow);
                        MinorPatronageSnapshot.Data presentationData3 = Snapshots.MinorPatronageSnapshot.PresentationData;
                        ref PatronageRelationWithMajor local7 = ref presentationData3.RelationWithMajors[(int)presentationData1.LocalEmpireInfo.EmpireIndex];
                        GUILayout.Label(string.Format("Patronage level with minor {0}", (object)local7.GaugeLevel));
                        GUILayout.Label("Ranked empire patronage");
                        int length = presentationData3.RankedMajorEmpirePatron.Length;
                        for (int index1 = length - 1; index1 >= 0; --index1)
                        {
                            int index2 = presentationData3.RankedMajorEmpirePatron[index1];
                            ref PatronageRelationWithMajor local8 = ref presentationData3.RelationWithMajors[index2];
                            GUILayout.Label(string.Format(" - {0} - Empire {1} with a score of {2}, net {3}", (object)(length - index1), (object)index2, (object)local8.PatronageStock, (object)local8.PatronageNet));
                        }
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Patronage stock");
                        int num = 0;
                        if (GUILayout.Button("-5"))
                            num -= 5;
                        if (GUILayout.Button("+5"))
                            num += 5;
                        if (num != 0)
                            SandboxManager.PostOrder((Order)new OrderForcePatronageStock()
                            {
                                DesiredPatronageStock = ((int)local7.PatronageStock + num),
                                MajorEmpireIndex = empireIndex,
                                MinorEmpireIndex = (byte)presentationData2.EmpireIndex
                            });
                        GUILayout.EndHorizontal();
                    }
                    else if (Snapshots.MinorPatronageSnapshot.Enabled)
                        Snapshots.MinorPatronageSnapshot.Stop(MinorPatronageSnapshot.ActivationFlags.DebugWindow);
                    GUILayout.EndScrollView();
                }
            }
        }
    }
}
