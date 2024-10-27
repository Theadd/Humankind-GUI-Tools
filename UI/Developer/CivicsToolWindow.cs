using Amplitude.Framework;
using Amplitude;
using Amplitude.Framework.Overlay;
using Amplitude.Mercury.Data.Simulation;
using Amplitude.Mercury.Interop;
using Amplitude.Mercury.Runtime;
using Amplitude.Mercury.Sandbox;
using System;
using UnityEngine;
using Amplitude.Mercury.Overlay;
using Modding.Humankind.DevTools.Core;
using Modding.Humankind.DevTools;
using Modding.Humankind.DevTools.DeveloperTools.UI;

namespace DevTools.Humankind.GUITools.UI
{
    public class CivicsToolWindow : FloatingToolWindow
    {
        private byte currentEmpireIndex = byte.MaxValue;
        private Vector2 scrollPosition;

        protected Amplitude.Framework.Runtime.IRuntimeService RuntimeService { get; private set; }

        public override string WindowTitle { get; set; } = "CIVICS TOOL";

        public override string WindowGUIStyle { get; set; } = "PopupWindow";

        public override bool ShouldBeVisible => !GlobalSettings.ShouldHideTools;

        public override bool ShouldRestoreLastWindowPosition => true;

        public override Rect WindowRect { get; set; } = new Rect(130f, 260f, 620f, 500f);

        private Color bgColor = new Color32(255, 255, 255, 230);
        private Color bgColorOpaque = new Color32(255, 255, 255, 255);

        private static GUIStyle CivicHeader { get; set; }
        
        protected override void Awake()
        {
            base.Awake();
            
            CivicHeader = new GUIStyle(UIController.DefaultSkin.FindStyle("PopupWindow.SectionHeader")) {
                margin = new RectOffset(0, 0, 0, 0)
            };
        }
        
        public override void OnGUIStyling()
        {
            base.OnGUIStyling();
            GUI.backgroundColor = GlobalSettings.WindowTransparency.Value ? bgColor : bgColorOpaque;
        }

        public override void OnDrawUI()
        {
            if (GlobalSettings.WindowTitleBar.Value)
                WindowUtils.DrawWindowTitleBar(this);

            OnDrawWindowContent();

        }
        
        protected override void OnBecomeInvisible()
        {
            base.OnBecomeInvisible();
            this.currentEmpireIndex = byte.MaxValue;
        }

        protected void OnDrawWindowContent()
        {
            if (!this.IsVisible)
                return;
            GUI.color = Color.white;
            GUI.backgroundColor = Color.white;

            if (this.RuntimeService == null)
            {
                using (new GUILayout.VerticalScope((GUIStyle)"Widget.ClientArea", Array.Empty<GUILayoutOption>()))
                    GUILayout.Label("Waiting for the runtime service...");
                if (Event.current.type != UnityEngine.EventType.Repaint)
                    return;
                this.RuntimeService = Services.GetService<Amplitude.Framework.Runtime.IRuntimeService>();
            }
            else if (this.RuntimeService.Runtime == null || !this.RuntimeService.Runtime.HasBeenLoaded || this.RuntimeService.Runtime.FiniteStateMachine.CurrentState == null)
            {
                using (new GUILayout.VerticalScope((GUIStyle)"Widget.ClientArea", Array.Empty<GUILayoutOption>()))
                    GUILayout.Label("Waiting for the runtime...");
            }
            else if (this.RuntimeService.Runtime.FiniteStateMachine.CurrentState.GetType() != typeof(RuntimeState_InGame))
            {
                using (new GUILayout.VerticalScope((GUIStyle)"Widget.ClientArea", Array.Empty<GUILayoutOption>()))
                    GUILayout.Label("Waiting for the runtime state...");
            }
            else if (!Amplitude.Mercury.Presentation.Presentation.HasBeenStarted)
            {
                using (new GUILayout.VerticalScope((GUIStyle)"Widget.ClientArea", Array.Empty<GUILayoutOption>()))
                    GUILayout.Label("Waiting for the presentation...");
            }
            else if (Snapshots.GameSnapshot == null || Snapshots.CivicSnapshot == null)
            {
                using (new GUILayout.VerticalScope((GUIStyle)"Widget.ClientArea", Array.Empty<GUILayoutOption>()))
                    GUILayout.Label("Waiting for snapshots...");
            }
            else
            {
                if ((int)this.currentEmpireIndex != (int)Snapshots.GameSnapshot.PresentationData.LocalEmpireInfo.EmpireIndex)
                {
                    this.currentEmpireIndex = Snapshots.GameSnapshot.PresentationData.LocalEmpireInfo.EmpireIndex;
                }
                using (new GUILayout.VerticalScope((GUIStyle)"Widget.ClientArea", new GUILayoutOption[1]
                {
                    GUILayout.Height(500f)
                }))
                {
                    EmpireInfo empireInfo = Snapshots.GameSnapshot.PresentationData.EmpireInfo[(int)this.currentEmpireIndex];
                    using (new GUILayout.HorizontalScope(Array.Empty<GUILayoutOption>()))
                    {
                        string str = !StaticString.IsNullOrEmpty(empireInfo.EmpireStabilityDefinitionName) ? R.Text.GetLocalizedTitle(empireInfo.EmpireStabilityDefinitionName) : "no city";
                        GUILayout.Label(string.Format("<size=11><b>EMPIRE STABILITY: {0}% ({1})</b></size>", (object)((int)empireInfo.EmpireStability), (object)str.ToUpperInvariant()));
                        GUILayout.FlexibleSpace();
                    }

                    IDatabase<CivicDefinition> database = Databases.GetDatabase<CivicDefinition>();
                    CivicInfo[] civicInfo1 = Snapshots.CivicSnapshot.PresentationData.CivicInfo;
                    int length1 = civicInfo1.Length;
                    using (new GUILayout.HorizontalScope(Array.Empty<GUILayoutOption>()))
                    {
                        GUILayout.Label(string.Format("<size=11><b>NUMBER OF VOTABLE CIVICS: {0}</b></size>", (object)Snapshots.CivicSnapshot.PresentationData.NumberOfVotableCivicsPerMajorEmpire[(int)this.currentEmpireIndex]));
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("<size=11><b>UNLOCK ALL</b></size>", (GUIStyle)"PopupWindow.ToolbarButton", GUILayout.Width(90f)))
                        {
                            for (int index = 0; index < length1; ++index)
                            {
                                CivicInfo civicInfo2 = civicInfo1[index];
                                if (civicInfo2.CivicStatusPerMajorEmpire[(int)this.currentEmpireIndex] == CivicStatuses.Unknown)
                                    SandboxManager.PostOrder((EditorOrder)new EditorOrderUnlockCivic()
                                    {
                                        EmpireIndex = (int)Snapshots.GameSnapshot.PresentationData.LocalEmpireInfo.EmpireIndex,
                                        CivicName = civicInfo2.CivicDefinitionName
                                    });
                            }
                        }
                        GUILayout.Space(12f);
                    }
                    GUILayout.Space(8f);

                    Utils.DrawHorizontalLine(0.65f);

                    this.scrollPosition = GUILayout.BeginScrollView(this.scrollPosition);
                    for (int index1 = 0; index1 < length1; ++index1)
                    {
                        CivicInfo civicInfo3 = civicInfo1[index1];
                        CivicStatuses civicStatuses = civicInfo3.CivicStatusPerMajorEmpire[(int)this.currentEmpireIndex];
                        CivicDefinition civicDefinition = database.GetValue(civicInfo3.CivicDefinitionName);
                        string civicStatusText = civicStatuses.ToString().ToUpperInvariant();

                        switch (civicStatusText)
                        {
                            case "ENACTED":
                                break;
                            case "AVAILABLE":
                                civicStatusText = "<color=green>" + civicStatusText + "</color>";
                                break;
                            case "UNKNOWN":
                                civicStatusText = "<color=#11111188>" + civicStatusText + "</color>";
                                break;
                        }

                        using (new GUILayout.HorizontalScope(CivicHeader))
                        {
                            int num;
                            GUI.enabled = (num = civicStatuses == CivicStatuses.Available ? 1 : (civicStatuses == CivicStatuses.Enacted ? 1 : 0)) != 0;
                            string title = (string)null;
                            if (!R.Text.DataUtils.TryGetLocalizedTitle(civicDefinition.Name, out title))
                                title = "Missing UIMapper";
                            GUILayout.Label("<size=10><b>" + title.ToUpperInvariant() + "</b></size>");
                            GUILayout.Label("<size=10><color=#EAEAEA77>" + civicDefinition.Name + "</color></size>");
                            // GUILayout.Label(string.Format("{0}", (object)civicDefinition.Name));
                            GUILayout.FlexibleSpace();
                            GUILayout.Label("<size=10><b>" + civicStatusText + "</b></size>", "RightAlignedLabel");
                            GUI.enabled = num == 0;
                            if (GUILayout.Button("<size=10><b>UNLOCK</b></size>", (GUIStyle)"PopupWindow.ToolbarButton", GUILayout.Width(65f)))
                                SandboxManager.PostOrder((EditorOrder)new EditorOrderUnlockCivic()
                                {
                                    EmpireIndex = (int)Snapshots.GameSnapshot.PresentationData.LocalEmpireInfo.EmpireIndex,
                                    CivicName = civicInfo3.CivicDefinitionName
                                });
                            GUI.enabled = civicStatuses == CivicStatuses.Enacted;
                            if (GUILayout.Button("<size=10><b>CANCEL</b></size>", (GUIStyle)"PopupWindow.ToolbarButton", GUILayout.Width(65f)))
                                SandboxManager.PostOrder((Order)new OrderCancelCivic()
                                {
                                    CivicName = civicInfo3.CivicDefinitionName
                                });
                        }
                        int length2 = civicDefinition.Choices.Length;
                        for (int index2 = 0; index2 < length2; ++index2)
                        {
                            using (new GUILayout.HorizontalScope(Array.Empty<GUILayoutOption>()))
                            {
                                CivicChoice choice = civicDefinition.Choices[index2];
                                bool flag = index2 == civicInfo3.ActiveChoiceIndexPerMajorEmpire[(int)this.currentEmpireIndex];
                                GUI.enabled = flag;
                                string str = flag ? "  <color=yellow><size=10><b>></b></size></color>" : "   <size=10><b> </b></size>";
                                string title = (string)null;
                                
                                if (!R.Text.DataUtils.TryGetLocalizedTitle(choice.Name, out title))
                                    title = "Missing UIMapper";
                                
                                GUILayout.Label(str + " <size=10><b>" + title.ToUpperInvariant() + "</b></size>");
                                GUILayout.Label("<size=10><color=#EAEAEA77>" + choice.Name + "</color></size>");
                                // GUILayout.Label(string.Format("{0} {1} ({2})", (object)str, (object)choice.Name, (object)title));
                                GUILayout.FlexibleSpace();
                                bool affordEnactCivic = Snapshots.GameSnapshot.PresentationData.EmpireInfo[(int)this.currentEmpireIndex].CanAffordEnactCivic;
                                GUI.enabled = civicStatuses == CivicStatuses.Available & affordEnactCivic;
                                if (GUILayout.Button("<size=10><b>ACTIVATE</b></size>", (GUIStyle)"PopupWindow.ToolbarButton", GUILayout.Width(65f)))
                                    SandboxManager.PostOrder((Order)new OrderActivateCivic()
                                    {
                                        CivicName = civicInfo3.CivicDefinitionName,
                                        ChoiceIndex = index2
                                    });
                                GUI.enabled = !flag;
                                if (GUILayout.Button("<size=10><b>FORCE</b></size>", (GUIStyle)"PopupWindow.ToolbarButton", GUILayout.Width(65f)))
                                    SandboxManager.PostOrder((EditorOrder)new EditorOrderActivateCivic()
                                    {
                                        EmpireIndex = (int)Snapshots.GameSnapshot.PresentationData.LocalEmpireInfo.EmpireIndex,
                                        CivicName = civicInfo3.CivicDefinitionName,
                                        ChoiceIndex = index2
                                    });
                            }
                        }
                        GUILayout.Space(5f);
                    }
                    GUILayout.EndScrollView();
                    Utils.DrawHorizontalLine(0.65f);

                    GUI.enabled = true;
                    if (Snapshots.GameSnapshot == null)
                        return;

                    // GUILayout.BeginVertical();
                    GUILayout.Space(5f);
                    // GUILayout.Label("Revolution option");
                    using (new GUILayout.HorizontalScope(/*GUILayout.Height(32f)*/))
                    {
                        if ((Snapshots.GameSnapshot.PresentationData.EmpireInfo[(int)this.currentEmpireIndex].MiscFlags & EmpireMiscFlags.HasActivatedFirstCivics) > EmpireMiscFlags.None)
                        {
                            FixedPoint revolutionPointStock = Snapshots.GameSnapshot.PresentationData.EmpireInfo[(int)this.currentEmpireIndex].RevolutionPointStock;
                            FixedPoint revolutionTimer = Snapshots.GameSnapshot.PresentationData.EmpireInfo[(int)this.currentEmpireIndex].RevolutionTimer;
                            if (revolutionTimer >= FixedPoint.MaxValue)
                            {
                                GUILayout.Label("<size=11><b>REVOLUTION POINTS: " + revolutionPointStock.ToString() + "</b></size>");
                                // GUILayout.Label(string.Format("Revolution points : {0}.", (object)revolutionPointStock, (object)revolutionTimer));
                                if (GUILayout.Button("<size=10><b>+5 REVOLUTION</b></size>", (GUIStyle)"PopupWindow.ToolbarButton", GUILayout.Width(95f)))
                                    SandboxManager.PostOrder((Order)new OrderGainRevolutionPoints()
                                    {
                                        RevolutionPointsAmount = 5
                                    });
                                if (GUILayout.Button("<size=10><b>-5 REVOLUTION</b></size>", (GUIStyle)"PopupWindow.ToolbarButton", GUILayout.Width(95f)))
                                    SandboxManager.PostOrder((Order)new OrderGainRevolutionPoints()
                                    {
                                        RevolutionPointsAmount = -5
                                    });
                                if (GUILayout.Button("<size=10><b>START REVOLUTION</b></size>", (GUIStyle)"PopupWindow.ToolbarButton", GUILayout.Width(130f)))
                                    SandboxManager.PostOrder((Order)new OrderChangeRevolutionState()
                                    {
                                        State = RevolutionState.InRevolution
                                    });
                            }
                            else
                            {
                                GUILayout.Label(string.Format("In revolution since {0} turn(s).", (object)revolutionTimer));
                                if (GUILayout.Button("<size=10><b>STOP REVOLUTION</b></size>", (GUIStyle)"PopupWindow.ToolbarButton", GUILayout.Width(120f)))
                                    SandboxManager.PostOrder((Order)new OrderChangeRevolutionState()
                                    {
                                        State = RevolutionState.None
                                    });
                            }
                        }
                        else
                            GUILayout.Label("Revolution system not active (Choose a civics first)");
                        
                        GUILayout.Space(12f);
                    }
                    GUILayout.Space(16f);
                    // GUILayout.EndVertical();

                }
            }
        }
    }
}
