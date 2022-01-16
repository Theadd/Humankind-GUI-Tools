using Amplitude.Framework;
using Amplitude.Framework.Overlay;
using Amplitude.Mercury.Data.Simulation;
using Amplitude.Mercury.Interop;
using Amplitude.Mercury.Runtime;
using Amplitude.Mercury.Sandbox;
using Amplitude.Mercury;
using Amplitude;
using System;
using System.Text;
using UnityEngine;
using Modding.Humankind.DevTools;
using Modding.Humankind.DevTools.Core;
using Modding.Humankind.DevTools.DeveloperTools.UI;

namespace DevTools.Humankind.GUITools.UI
{
    public class FameToolWindow : FloatingToolWindow
    {
        public override string WindowTitle { get; set; } = "FAME TOOL";

        public override string WindowGUIStyle { get; set; } = "PopupWindow";

        public override bool ShouldBeVisible => !GlobalSettings.ShouldHideTools;

        public override bool ShouldRestoreLastWindowPosition => true;

        public override Rect WindowRect { get; set; } = new Rect(130f, 260f, 780f, 500f);

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

        private GUIStyle ScrollViewStyle = new GUIStyle(UIController.DefaultSkin.FindStyle("PopupWindow.ListGrid")) {
            margin = new RectOffset(0, 3, 0, 3),
            hover = new GUIStyleState() {
                background = null,
                textColor = Color.white
            }
        };

        private StringBuilder stringBuilder = new StringBuilder();
        private IDatabase<DeedDefinition> deedDefinitionDatabase;
        private IDatabase<EraStarDefinition> eraStarDefinitionDatabase;
        private IDatabase<CompetitiveDeedDefinition> competitiveDeedDefinitionDatabase;
        private Vector2 eraStarsPosition = Vector2.zero;
        private Vector2 competitiveDeedPosition = Vector2.zero;

        protected Amplitude.Framework.Runtime.IRuntimeService RuntimeService { get; private set; }

        protected override void OnDrawWindowClientArea(int instanceId)
        {
            using (new GUILayout.VerticalScope((GUIStyle)"Widget.ClientArea", Array.Empty<GUILayoutOption>()))
            {
                if (this.RuntimeService != null)
                {
                    if (this.RuntimeService.Runtime != null && this.RuntimeService.Runtime.HasBeenLoaded)
                    {
                        if (this.RuntimeService.Runtime.FiniteStateMachine.CurrentState != null)
                        {
                            if (this.RuntimeService.Runtime.FiniteStateMachine.CurrentState.GetType() == typeof(RuntimeState_InGame))
                            {
                                if (Amplitude.Mercury.Presentation.Presentation.HasBeenStarted)
                                {
                                    this.deedDefinitionDatabase = this.deedDefinitionDatabase ?? Databases.GetDatabase<DeedDefinition>();
                                    this.eraStarDefinitionDatabase = this.eraStarDefinitionDatabase ?? Databases.GetDatabase<EraStarDefinition>();
                                    this.competitiveDeedDefinitionDatabase = this.competitiveDeedDefinitionDatabase ?? Databases.GetDatabase<CompetitiveDeedDefinition>();
                                    this.DrawFame();
                                    this.DrawEraStars();
                                    this.DrawCompetitiveDeeds();
                                }
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
                    if (Event.current.type != UnityEngine.EventType.Repaint)
                        return;
                    this.RuntimeService = Services.GetService<Amplitude.Framework.Runtime.IRuntimeService>();
                }
            }
        }

        private void DrawFame()
        {
            int empireIndex = (int)Snapshots.GameSnapshot.PresentationData.LocalEmpireInfo.EmpireIndex;
            FixedPoint fameStock = Snapshots.GameSnapshot.PresentationData.EmpireInfo[empireIndex].FameStock;
            using (new GUILayout.HorizontalScope(Array.Empty<GUILayoutOption>()))
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label("======================= Fame =======================", (GUIStyle)"PopupWindow.MonospacedLabel");
                GUILayout.FlexibleSpace();
            }
            this.stringBuilder.Length = 0;
            this.stringBuilder.AppendFormat("Score = {0}", (object)fameStock.Format(0, false, false));
            using (new GUILayout.HorizontalScope(Array.Empty<GUILayoutOption>()))
            {
                GUILayout.Space(8f);
                GUILayout.Label(this.stringBuilder.ToString(), GUILayout.Width(256f));
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("+10"))
                    SandboxManager.PostOrder((Order)new OrderForceGainFame()
                    {
                        Gain = 10
                    });
                if (GUILayout.Button("+500"))
                    SandboxManager.PostOrder((Order)new OrderForceGainFame()
                    {
                        Gain = 500
                    });

                GUILayout.Space(12f);
            }
        }

        private void DrawEraStars()
        {
            int empireIndex = (int)Snapshots.GameSnapshot.PresentationData.LocalEmpireInfo.EmpireIndex;
            FixedPoint eraStarsCount = Snapshots.GameSnapshot.PresentationData.EmpireInfo[empireIndex].EraStarsCount;
            FixedPoint starsRequirement = Snapshots.GameSnapshot.PresentationData.EmpireInfo[empireIndex].EraStarsRequirement;
            using (new GUILayout.HorizontalScope(Array.Empty<GUILayoutOption>()))
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label(string.Format("==================== Era Stars ({0}/{1}) ====================", (object)eraStarsCount.Format(), (object)starsRequirement.Format()), (GUIStyle)"PopupWindow.MonospacedLabel");
                GUILayout.FlexibleSpace();
            }
            //GUI.backgroundColor = Color.white;
            using (GUILayout.ScrollViewScope scrollViewScope = new GUILayout.ScrollViewScope(this.eraStarsPosition, ScrollViewStyle, GUILayout.Height(256f)))
            {
                ArrayWithFrame<EraStarInfo> eraStarInfo1 = Snapshots.GameSnapshot.PresentationData.EmpireInfo[empireIndex].EraStarInfo;
                int length = eraStarInfo1.Length;
                for (int index1 = 0; index1 < length; ++index1)
                {
                    EraStarInfo eraStarInfo2 = eraStarInfo1[index1];
                    if (eraStarInfo2.PoolAllocationIndex >= 0)
                    {
                        EraStarDefinition eraStarDefinition = (EraStarDefinition)null;
                        if (!this.eraStarDefinitionDatabase.TryGetValue(eraStarInfo2.EraStarDefinitionName, out eraStarDefinition))
                            return;
                        using (new GUILayout.HorizontalScope(Array.Empty<GUILayoutOption>()))
                        {
                            this.stringBuilder.Length = 0;
                            this.stringBuilder.AppendLine(eraStarDefinition.Name.ToString());
                            this.stringBuilder.AppendFormat("Based on Deed '{0}'", (object)eraStarDefinition.DeedDefinitionReference.ElementName);
                            GUILayout.Label(this.stringBuilder.ToString(), GUILayout.Width(384f));
                            this.stringBuilder.Length = 0;
                            this.stringBuilder.AppendFormat("Score = {0} - Reward = {1}", (object)eraStarInfo2.Score, (object)eraStarInfo2.CurrentReward);
                            int num = eraStarDefinition.Levels != null ? eraStarDefinition.Levels.Length : 0;
                            for (int index2 = 0; index2 < num; ++index2)
                            {
                                this.stringBuilder.AppendLine();
                                this.stringBuilder.AppendFormat("  - [{2}] Level {0}: {1}", (object)(index2 + 1), (object)eraStarInfo2.Thresholds[index2].Format(), index2 < eraStarInfo2.Level ? (object)"X" : (object)" ");
                            }
                            GUILayout.Label(this.stringBuilder.ToString(), GUILayout.Width(256f));
                            GUILayout.FlexibleSpace();
                            if (GUILayout.Button("+1"))
                                SandboxManager.PostOrder((Order)new OrderForceGainEraStarScore()
                                {
                                    EraStarName = eraStarDefinition.Name,
                                    Gain = 1
                                });
                            if (eraStarInfo2.Level < num)
                            {
                                if (GUILayout.Button("Turbo!"))
                                    SandboxManager.PostOrder((Order)new OrderForceGainEraStarScore()
                                    {
                                        EraStarName = eraStarInfo2.EraStarDefinitionName,
                                        Gain = (int)(eraStarInfo2.Thresholds[eraStarInfo2.Level] - eraStarInfo2.Score)
                                    });
                            }
                        }
                    }
                }
                this.eraStarsPosition = scrollViewScope.scrollPosition;
            }
            EraStarCatchupInfo eraStarCatchupInfo = Snapshots.GameSnapshot.PresentationData.EmpireInfo[empireIndex].EraStarCatchupInfo;
            this.stringBuilder.Length = 0;
            this.stringBuilder.AppendFormat("Catchup: {0} (+{1} per Turn) / {2} - Level = {3}", (object)eraStarCatchupInfo.Score.Format(), (object)eraStarCatchupInfo.PointsPerTurn.Format(), (object)eraStarCatchupInfo.Threshold.Format(), (object)eraStarCatchupInfo.Level);
            this.stringBuilder.AppendLine();
            GUILayout.Label(this.stringBuilder.ToString());
        }

        private void DrawCompetitiveDeeds()
        {
            using (new GUILayout.HorizontalScope(Array.Empty<GUILayoutOption>()))
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label("==================== Competitive Deeds ====================", (GUIStyle)"PopupWindow.MonospacedLabel");
                GUILayout.FlexibleSpace();
            }
            using (GUILayout.ScrollViewScope scrollViewScope = new GUILayout.ScrollViewScope(this.competitiveDeedPosition, ScrollViewStyle, GUILayout.Height(256f)))
            {
                int length = Snapshots.FameSnapshot.PresentationData.CompetitiveDeedInfo.Length;
                for (int index1 = 0; index1 < length; ++index1)
                {
                    ref CompetitiveDeedInfo local = ref Snapshots.FameSnapshot.PresentationData.CompetitiveDeedInfo.Data[index1];
                    using (new GUILayout.HorizontalScope(Array.Empty<GUILayoutOption>()))
                    {
                        this.stringBuilder.Length = 0;
                        this.stringBuilder.AppendLine(local.CompetitiveDeedDefinitionName.ToString());
                        this.stringBuilder.AppendFormat("Based on Deed '{0}'", (object)local.DeedDefinitionName);
                        if (!StaticString.IsNullOrEmpty(local.ElementName))
                        {
                            this.stringBuilder.AppendLine();
                            this.stringBuilder.AppendFormat("  - Name = {0}", (object)local.ElementName.ToString());
                        }
                        if (local.ElementIndex >= 0)
                        {
                            this.stringBuilder.AppendLine();
                            if (local.Type == CompetitiveDeedInfo.DeedType.PerNaturalWonder)
                                this.stringBuilder.AppendFormat("  - {0}", (object)Snapshots.GameSnapshot.PresentationData.NaturalWonderInfo[local.ElementIndex].NaturalWonderDefinitionName);
                            else
                                this.stringBuilder.AppendFormat("  - Index = {0}", (object)local.ElementIndex);
                        }
                        GUILayout.Label(this.stringBuilder.ToString());
                        GUILayout.FlexibleSpace();
                        CompetitiveDeedDefinition competitiveDeedDefinition = this.competitiveDeedDefinitionDatabase.GetValue(local.CompetitiveDeedDefinitionName);
                        string str = !StaticString.IsNullOrEmpty(competitiveDeedDefinition.FameRewardReference.DefinitionReference.ElementName) ? competitiveDeedDefinition.FameRewardReference.DefinitionReference.ElementName.ToString() : "Custom";
                        this.stringBuilder.Length = 0;
                        this.stringBuilder.AppendFormat("Reward: {0} ({1})", (object)local.FameReward, (object)str);
                        this.stringBuilder.AppendLine();
                        this.stringBuilder.Append("Winner: ");
                        int winningMajorEmpires = local.NumberOfWinningMajorEmpires;
                        if (winningMajorEmpires == 0)
                        {
                            this.stringBuilder.Append("None.");
                        }
                        else
                        {
                            this.stringBuilder.Append(local.WinningMajorEmpireIndices[0]);
                            for (int index2 = 1; index2 < winningMajorEmpires; ++index2)
                                this.stringBuilder.AppendFormat(", {0}", (object)local.WinningMajorEmpireIndices[index2]);
                        }
                        this.stringBuilder.AppendLine();
                        this.stringBuilder.Append(local.IsActive ? "Active!" : "Inactive.");
                        GUILayout.Label(this.stringBuilder.ToString(), GUILayout.Width(256f));
                    }
                }
                this.competitiveDeedPosition = scrollViewScope.scrollPosition;
            }
        }
    }
}
