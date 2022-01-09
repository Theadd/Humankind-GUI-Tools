using Amplitude.Framework.Overlay;
using Amplitude.Mercury.Data.AI;
using Amplitude.Mercury.Data.Simulation;
using Amplitude.Mercury.Interop;
using Amplitude.Mercury.Sandbox;
using Amplitude.Mercury.Simulation;
using System;
using UnityEngine;
using Modding.Humankind.DevTools.Core;
using Modding.Humankind.DevTools.DeveloperTools.UI;

namespace DevTools.Humankind.GUITools.UI
{
    public class DiplomacyToolWindow : FloatingToolWindow
    {
        public override string WindowTitle { get; set; } = "DIPLOMACY TOOL";

        public override string WindowGUIStyle { get; set; } = "PopupWindow";

        public override bool ShouldBeVisible => !GlobalSettings.ShouldHideTools;

        public override bool ShouldRestoreLastWindowPosition => true;

        public override Rect WindowRect { get; set; } = new Rect(130f, 260f, 720f, 500f);

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
        private Vector2 scrollPosition;

        protected override void OnBecomeVisible() => base.OnBecomeVisible();

        protected override void OnDrawWindowClientArea(int instanceId)
        {
            GUI.color = Color.white;
            GUI.backgroundColor = Color.white;
            GUILayout.BeginVertical((GUIStyle)"Widget.ClientArea", GUILayout.ExpandWidth(true));
            if (Snapshots.DiplomaticSnapshot == null)
                return;
            using (new GUILayout.VerticalScope((GUIStyle)"Widget.ClientArea", new GUILayoutOption[1]
            {
        GUILayout.Height(500f)
            }))
            {
                this.scrollPosition = GUILayout.BeginScrollView(this.scrollPosition);
                using (new GUILayout.HorizontalScope(Array.Empty<GUILayoutOption>()))
                {
                    GUILayout.Label(" ");
                    GUILayout.Label("Empire", GUILayout.Width(45f));
                    GUILayout.Label(" | ");
                    GUILayout.Label("State", GUILayout.Width(100f));
                    GUILayout.Label(" | ");
                    GUILayout.Label("TreatyState?", GUILayout.Width(100f));
                    GUILayout.Label(" | ");
                    GUILayout.Label("ForceWar?", GUILayout.Width(90f));
                    GUILayout.Label(" | ");
                    GUILayout.Label("Morale", GUILayout.Width(45f));
                    GUILayout.Label(" | ");
                    GUILayout.Label("Morale Delta", GUILayout.Width(45f));
                }
                DiplomaticRelationSummaryInfo[] relationSummaries = Snapshots.DiplomaticSnapshot.PresentationData.LocalEmpireDiplomaticSummary.RelationSummaries;
                for (int index1 = 0; index1 < relationSummaries.Length; ++index1)
                {
                    DiplomaticRelationSummaryInfo relationSummaryInfo = relationSummaries[index1];
                    using (new GUILayout.HorizontalScope(Array.Empty<GUILayoutOption>()))
                    {
                        if (index1 == (int)Snapshots.GameSnapshot.PresentationData.LocalEmpireInfo.EmpireIndex)
                        {
                            GUILayout.Label("     You don't have diplomatic relation with yourself.");
                            continue;
                        }
                        GUILayout.Label(" ");
                        GUILayout.Label(relationSummaryInfo.OtherEmpireIndex.ToString(), GUILayout.Width(45f));
                        GUILayout.Label(" | ");
                        GUILayout.Label(relationSummaryInfo.CurrentState.ToString(), GUILayout.Width(100f));
                        GUILayout.Label(" | ");
                        GUILayout.Label(relationSummaryInfo.TreatyInfo.PropositionInfo.Status.ToString(), GUILayout.Width(100f));
                        GUILayout.Label(" | ");
                        if (GUILayout.Button("ForceWar", GUILayout.Width(90f)))
                            SandboxManager.PostOrder((EditorOrder)new EditorOrderForceWar()
                            {
                                LeftEmpireIndex = relationSummaryInfo.OwnerEmpireIndex,
                                RightEmpireIndex = index1
                            });
                        GUILayout.Label(" | ");
                        GUILayout.Label(relationSummaryInfo.LocalEmpireMoral.Moral.ToString(), GUILayout.Width(45f));
                        GUILayout.Label(" | ");
                        GUILayout.Label(relationSummaryInfo.LocalEmpireMoral.MoralDelta.ToString(), GUILayout.Width(45f));
                    }
                    using (new GUILayout.HorizontalScope(Array.Empty<GUILayoutOption>()))
                    {
                        if (relationSummaryInfo.OwnerCurrentAIBehaviourState == AIBehaviourState.Unknown)
                        {
                            GUILayout.Label("The AI hasn't met the other empire yet");
                        }
                        else
                        {
                            GUILayout.Label(string.Format("The AI moved from {0} to {1} at turn {2}", (object)relationSummaryInfo.OwnerPreviousAIBehaviourState, (object)relationSummaryInfo.OwnerCurrentAIBehaviourState, (object)relationSummaryInfo.OwnerAIBehaviourStateChangeDate));
                            GUILayout.Label(string.Format("The AI has a rapport score of {0}, a superiority score of {1}", (object)(int)((double)relationSummaryInfo.OwnerAIRapportScore * 100.0), (object)(int)((double)relationSummaryInfo.OwnerAISuperiorityScore * 100.0)));
                            int transitioningStates = relationSummaryInfo.OwnerAITransitioningStates;
                            string text;
                            if (transitioningStates == 0)
                            {
                                text = "Cannot transition to any state currently";
                            }
                            else
                            {
                                text = "The AI could transition to ";
                                bool flag = false;
                                for (int index2 = 0; index2 < 24; ++index2)
                                {
                                    if ((transitioningStates & 1 << index2) > 0)
                                    {
                                        if (!flag)
                                        {
                                            text += string.Format("{0}", (object)(AIBehaviourState)index2);
                                            flag = true;
                                        }
                                        else
                                            text += string.Format(", {0}", (object)(AIBehaviourState)index2);
                                    }
                                }
                            }
                            GUILayout.Label(text);
                        }
                    }
                    using (new GUILayout.HorizontalScope(Array.Empty<GUILayoutOption>()))
                    {
                        if (relationSummaryInfo.OwnerCurrentAIBehaviourState != AIBehaviourState.Unknown)
                        {
                            if (relationSummaryInfo.OwnerPreviousAIBehaviourState != AIBehaviourState.Unknown)
                            {
                                string empty = string.Empty;
                                string text;
                                if (relationSummaryInfo.OwnFeedbackEntries == null)
                                {
                                    text = "Due to optimization, AIs only send feedbacks to human controlled empire. Switch empire, trigger a decision, then come back to this empire";
                                }
                                else
                                {
                                    text = "The AI changed state because ";
                                    for (int index3 = 0; index3 < relationSummaryInfo.OwnFeedbackEntries.Length; ++index3)
                                    {
                                        ref DiplomaticScoreModifierEntry local = ref relationSummaryInfo.OwnFeedbackEntries[index3];
                                        text = text + " " + (local.ModifierType == DiplomaticScoreModifierType.Log ? local.LogName.ToString() : local.ScoreModifier.ToString()) + ", ";
                                    }
                                }
                                GUILayout.Label(text);
                            }
                        }
                    }
                    if (relationSummaryInfo.CrisisInfo.CrisisStatus == CrisisInfo.Status.OnGoing && GUILayout.Button("Force Stall for time", (GUIStyle)"PopupWindow.Button"))
                        SandboxManager.PostOrder((Order)new OrderDiplomaticAction()
                        {
                            DiplomaticAction = DiplomaticAction.StallForTime,
                            OtherEmpireIndex = relationSummaryInfo.OtherEmpireIndex
                        });
                }
                GUILayout.EndScrollView();
            }
        }
    }
}
