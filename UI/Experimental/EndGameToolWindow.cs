using Amplitude.Framework.Overlay;
using Amplitude.Mercury.Interop;
using Amplitude.Mercury.Sandbox;
using Amplitude.Mercury;
using Amplitude;
using System.Text;
using UnityEngine;

using Modding.Humankind.DevTools.Core;
using Modding.Humankind.DevTools.DeveloperTools.UI;

namespace DevTools.Humankind.GUITools.UI
{
    public class EndGameToolWindow : FloatingToolWindow
    {
        public override string WindowTitle { get; set; } = "END GAME TOOL";

        public override string WindowGUIStyle { get; set; } = "PopupWindow";

        public override bool ShouldBeVisible => !GlobalSettings.ShouldHideTools;

        public override bool ShouldRestoreLastWindowPosition => true;

        public override Rect WindowRect { get; set; } = new Rect(130f, 260f, 750f, 500f);

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
        private bool showStatistics;
        private StringBuilder workingStringBuilder = new StringBuilder();

        /*protected override void OnBecomeVisible()
        {
            base.OnBecomeVisible();
            this.Width = 750f;
            this.AdjustWindowRect(this.WindowPosition);
        }*/

        protected override void OnDrawWindowClientArea(int instanceId)
        {
            GUILayout.BeginVertical((GUIStyle)"Widget.ClientArea", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            if (!Amplitude.Mercury.Presentation.Presentation.HasBeenStarted)
            {
                GUILayout.Label("Waiting for the presentation...");
            }
            else
            {
                if (GUILayout.Button("Launch end sequence"))
                    SandboxManager.PostOrder((Order)new OrderStartEndGameCameraSequence());
                if (GUILayout.Button("Ready to continue"))
                    SandboxManager.PostOrder((Order)new OrderContinueGameAfterEndGame());
                int num1 = this.showStatistics ? 1 : 0;
                this.showStatistics = GUILayout.Toggle(this.showStatistics, "ShowStatistics");
                int num2 = this.showStatistics ? 1 : 0;
                if (num1 != num2)
                {
                    if (this.showStatistics)
                        Snapshots.GameStatisticsSnapshot.SetActivationFlags(GameStatisticsSnapshot.ActivationFlags.DebugWindow);
                    else
                        Snapshots.GameStatisticsSnapshot.UnsetActivationFlags(GameStatisticsSnapshot.ActivationFlags.DebugWindow);
                }
                if (this.showStatistics)
                {
                    if (Snapshots.GameStatisticsSnapshot == null || Snapshots.GameStatisticsSnapshot.PresentationData == null)
                    {
                        GUILayout.BeginArea(new Rect(0.0f, 20f, this.WindowPosition.width, 20f), GUIContent.none, (GUIStyle)"Widget.ClientArea");
                        GUILayout.Label("Waiting for the snapshot to be valid...");
                        GUILayout.EndArea();
                        return;
                    }
                    ref FixedPoint[,] local1 = ref Snapshots.GameStatisticsSnapshot.PresentationData.FamePerEmpirePerEra;
                    if (local1 != null)
                    {
                        int length1 = local1.GetLength(0);
                        int length2 = local1.GetLength(1);
                        for (int index1 = 0; index1 < length1; ++index1)
                        {
                            this.workingStringBuilder.Clear();
                            this.workingStringBuilder.Length = 0;
                            this.workingStringBuilder.Append(string.Format("Empire {0} |\n ", (object)index1));
                            for (int index2 = 0; index2 < length2; ++index2)
                                this.workingStringBuilder.Append(string.Format("Era {0} Fame {1} | ", (object)index2, (object)local1[index1, index2]));
                            GUILayout.Label(this.workingStringBuilder.ToString());
                        }
                    }
                    int empireIndex = (int)Snapshots.GameSnapshot.PresentationData.LocalEmpireInfo.EmpireIndex + 1;
                    ref EmpireStatistics local2 = ref Snapshots.GameStatisticsSnapshot.PresentationData.EmpireStatistics[empireIndex];
                    this.workingStringBuilder.Clear();
                    this.workingStringBuilder.Append(string.Format("Current Empire Statistics {0} |\n ", (object)empireIndex));
                    this.workingStringBuilder.Append(string.Format("Total Life Lost {0} |\n ", (object)local2.AggregationData.LifeLost));
                    this.workingStringBuilder.Append(string.Format("Total Units Lost {0} |\n ", (object)local2.AggregationData.NumberOfMyUnitsKilledByOther));
                    this.workingStringBuilder.Append(string.Format("Total Units Killed {0} |\n ", (object)local2.AggregationData.NumberOfOtherUnitsKilledByMe));
                    GUILayout.Label(this.workingStringBuilder.ToString());
                }
                if (GUILayout.Button("Force end game at end of turn"))
                    SandboxManager.PostOrder((Order)new OrderForceEndGame());
                if (GUILayout.Button("Resign the empire and give the control back to the AI")) 
                    SandboxManager.PostOrder((Order)new OrderEmpireResign());
            }
            GUILayout.EndVertical();
        }
    }
}
