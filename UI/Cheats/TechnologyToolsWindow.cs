using Amplitude.Framework;
using Amplitude.Mercury.Data.Simulation;
using Amplitude.Mercury.Interop;
using Amplitude.Mercury.Sandbox;
using Amplitude.Mercury.Simulation;
using Amplitude.Mercury.UI;
using Amplitude.Mercury.UI.Helpers;
using System;
using Modding.Humankind.DevTools.Core;
using Modding.Humankind.DevTools.DeveloperTools.UI;
using Modding.Humankind.DevTools;
using UnityEngine;

namespace DevTools.Humankind.GUITools.UI
{
    public class TechnologyToolsWindow : FloatingToolWindow
    {
        public override string WindowTitle { get; set; } = "TECHNOLOGY TOOLS";
        public override string WindowGUIStyle { get; set; } = "PopupWindow";
        public override bool ShouldBeVisible => HumankindGame.IsGameLoaded && !GlobalSettings.ShouldHideTools;
        public override bool ShouldRestoreLastWindowPosition => true;
        public override Rect WindowRect { get; set; } = new Rect(130f, 260f, 780f, 500f);
        private Vector2 scrollPosition;
        private string techFilter = "";
        private int listedTechsCount = 1;
        private Color DisabledTextColor = new Color(1f, 1f, 1f, 0.5f);
        private GUIStyle tooltipDown;

        // A Custom GUIStyle based on a copy of an existing one
        public GUIStyle TooltipDown
        {
            get
            {
                if (tooltipDown == null)
                {
                    tooltipDown =
                        new GUIStyle(
                            Modding.Humankind.DevTools.DeveloperTools.UI.UIController.DefaultSkin.FindStyle("Tooltip"));
                    tooltipDown.padding = new RectOffset(4, 4, 4, 0);
                    tooltipDown.margin = new RectOffset(6, 8, 0, 4);
                    tooltipDown.alignment = TextAnchor.UpperCenter;
                    tooltipDown.clipping = TextClipping.Overflow;
                    tooltipDown.wordWrap = true;
                    tooltipDown.stretchHeight = true;
                }

                return tooltipDown;
            }
        }

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
            GUILayout.BeginVertical("Widget.ClientArea");

            if (Snapshots.TechnologyScreenSnapshot != null &&
                !Snapshots.TechnologyScreenSnapshot.IsActive(TechnologyScreenSnapshot.ActivationFlags.FloatingWindow))
                Snapshots.TechnologyScreenSnapshot.Start(TechnologyScreenSnapshot.ActivationFlags.FloatingWindow);

            using (new GUILayout.VerticalScope((GUIStyle) "Widget.ClientArea", new GUILayoutOption[1]
            {
                GUILayout.Height(500f)
            }))
            {
                OnDrawWindowHeader();
                GUILayout.Space(4f);
                OnDrawTableHeader();
                this.scrollPosition = GUILayout.BeginScrollView(this.scrollPosition);
                OnDrawTable();
                GUILayout.EndScrollView();
                OnDrawWindowFooter();
            }

            GUILayout.Space(10f);
            GUILayout.EndVertical();
        }

        protected void OnDrawTable()
        {
            IDatabase<TechnologyDefinition> database = Databases.GetDatabase<TechnologyDefinition>();
            TechnologyInfo[] technologyInfo1 =
                Snapshots.TechnologyScreenSnapshot.PresentationData.TechnologyInfo;
            int length = technologyInfo1.Length;
            listedTechsCount = 0;
            for (int index = 0; index < length; ++index)
            {
                TechnologyInfo technologyInfo2 = technologyInfo1[index];
                string localizedTitle = R.Text.GetLocalizedTitle(technologyInfo2.TechnologyDefinitionName).ToUpper();
                string techDefinitionName = technologyInfo2.TechnologyDefinitionName.ToString().Substring(11).ToUpper();
                string techState = technologyInfo2.TechnologyState.ToString().ToUpper();

                if (techFilter.Length > 0)
                {
                    if (!localizedTitle.Contains(techFilter) && !techDefinitionName.Contains(techFilter) &&
                        !techState.Contains(techFilter))
                        continue;
                }

                listedTechsCount++;
                var localizedDescription = R.Text.GetLocalizedDescription(technologyInfo2.TechnologyDefinitionName);

                var isEnabled = technologyInfo2.TechnologyState != TechnologyStates.LockedByEra &&
                                technologyInfo2.TechnologyState != TechnologyStates.LockedByTechnology;
                if (database.GetValue(technologyInfo2.TechnologyDefinitionName).Visibility !=
                    TechnologyDefinition.VisibilityFlags.Never)
                {
                    GUI.enabled = true;
                    using (new GUILayout.HorizontalScope(new GUIContent("", localizedDescription),
                        listedTechsCount % 2 == 0 ? "PopupWindow.Row" : "PopupWindow.RowEven"))
                    {

                        GUI.color = isEnabled ? Color.white : DisabledTextColor;
                        if (GUILayout.Button(
                            new GUIContent("<size=10><b>" + localizedTitle + "</b></size>", localizedDescription),
                            "Link"))
                            MoveToTechnology(technologyInfo2.TechnologyDefinitionName);

                        GUI.color = Color.white;
                        GUI.enabled = isEnabled;
                        GUILayout.Label("<size=10><color=#999999AA>" + techDefinitionName + "</color></size>");

                        GUILayout.FlexibleSpace();

                        GUILayout.BeginHorizontal();
                        GUILayout.Label(
                            "<size=10><b>" + ((int) technologyInfo2.ResearchCost).ToString() + "</b></size>",
                            "RightAlignedLabel", GUILayout.Width(50f));
                        GUILayout.Label("<size=10><b>" +
                                        ((int) (technologyInfo2.ResearchInvestedInPercent * 100)).ToString() +
                                        "%</b></size>", "RightAlignedLabel", GUILayout.Width(70f));
                        GUILayout.Label("<size=10><b>" +
                                        techState + "</b></size>", "RightAlignedLabel", GUILayout.Width(150f));

                        if (technologyInfo2.IsInQueue)
                        {
                            GUI.enabled = true;
                            if (GUILayout.Button("<size=10><b>REMOVE</b></size>", (GUIStyle) "button",
                                GUILayout.Width(65f)))
                                SandboxManager.PostOrder((Order) new OrderRemoveTechnologyAt()
                                {
                                    TechnologyIndexInQueue = technologyInfo2.IndexInQueue
                                });
                        }
                        else
                        {
                            GUI.enabled = technologyInfo2.TechnologyState == TechnologyStates.Available;
                            if (GUILayout.Button("<size=10><b>QUEUE</b></size>", (GUIStyle) "button",
                                GUILayout.Width(65f)))
                                SandboxManager.PostOrder((Order) new OrderEnqueueTechnology()
                                {
                                    TechnologyName = technologyInfo2.TechnologyDefinitionName
                                });
                        }

                        GUI.enabled = technologyInfo2.TechnologyState != TechnologyStates.Completed;
                        if (GUILayout.Button("<size=10><b>UNLOCK</b></size>", (GUIStyle) "button",
                            GUILayout.Width(65f)))
                            SandboxManager.PostOrder((EditorOrder) new EditorOrderCompleteTechnology()
                            {
                                EmpireIndex = (int) Snapshots.GameSnapshot.PresentationData.LocalEmpireInfo
                                    .EmpireIndex,
                                TechnologyName = technologyInfo2.TechnologyDefinitionName
                            });

                        GUILayout.EndHorizontal();
                    }
                }
            }

            GUI.enabled = true;
        }

        protected void OnDrawWindowFooter()
        {
            GUILayout.Space(1f);
            GUILayout.Label("L O C A L I Z E D   D E S C R I P T I O N", "PopupWindow.SectionHeader");

            GUI.enabled = true;
            GUI.color = new Color(1f, 1f, 1f, 0.5f);
            GUILayout.Label(R.Text.Size(R.Text.Bold(GUI.tooltip.ToUpper()), 10), TooltipDown);
            GUILayout.Space(12f);
            GUI.color = Color.white;
        }

        public void MoveToTechnology(Amplitude.StaticString technologyDefinitionName)
        {
            TechnologyScreen window = WindowsUtils.GetWindow<TechnologyScreen>();
            if (window != null)
                window.MoveToTechnology(technologyDefinitionName);
        }


        protected void OnDrawTableHeader()
        {
            using (new GUILayout.HorizontalScope("PopupWindow.SectionHeader"))
            {
                GUILayout.Label("<size=11><b>TECHNOLOGY</b></size>");
                GUILayout.FlexibleSpace();
                GUILayout.BeginHorizontal();
                GUILayout.Label("<size=11><b>COST</b></size>", "RightAlignedLabel", GUILayout.Width(50f));
                GUILayout.Label("<size=11><b>PROGRESS</b></size>", "RightAlignedLabel", GUILayout.Width(70f));
                GUILayout.Label("<size=11><b>STATUS</b></size>", "RightAlignedLabel", GUILayout.Width(150f));
                GUILayout.Label("<size=11><b>ACTIONS</b></size>", "RightAlignedLabel", GUILayout.Width(134f));
                GUILayout.Space(8f);
                GUILayout.EndHorizontal();
            }
        }

        protected void OnDrawWindowHeader()
        {
            using (new GUILayout.HorizontalScope(Array.Empty<GUILayoutOption>()))
            {
                GUILayout.Label("INVEST RESEARCH: ", "PopupWindow.Title");
                GUI.enabled = true;
                if (GUILayout.Button(" <b>+25</b> "))
                    SandboxManager.PostOrder((Order) new OrderInvestResearch()
                    {
                        Gain = 25
                    });
                if (GUILayout.Button(" <b>+250</b> "))
                    SandboxManager.PostOrder((Order) new OrderInvestResearch()
                    {
                        Gain = 250
                    });
                if (GUILayout.Button(" <b>+5K</b> "))
                    SandboxManager.PostOrder((Order) new OrderInvestResearch()
                    {
                        Gain = 5000
                    });
                GUILayout.FlexibleSpace();
                GUILayout.BeginHorizontal();

                GUILayout.Label("<b>FILTER: </b>", "PopupWindow.Title");
                if (listedTechsCount == 0)
                    GUI.color = Color.red;
                techFilter = GUILayout.TextField(techFilter, GUILayout.Width(160f)).ToUpper();
                GUI.color = Color.white;

                if (GUILayout.Button("<size=11><b>UNLOCK ALL</b></size>", "PopupWindow.ToolbarButton"))
                {
                    SandboxManager.PostOrder((EditorOrder) new EditorOrderCompleteAllTechnology()
                    {
                        EmpireIndex = (int) Snapshots.GameSnapshot.PresentationData.LocalEmpireInfo.EmpireIndex
                    });
                }

                GUILayout.Space(8f);
                GUILayout.EndHorizontal();
            }
        }
    }
}
