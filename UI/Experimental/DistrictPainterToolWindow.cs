using Amplitude.Framework;
using Amplitude.Framework.Overlay;
using Amplitude.Mercury.Data.Simulation;
using Amplitude.Mercury.Interop;
using Amplitude.Mercury.Presentation;
using Amplitude.Mercury.Runtime;
using Amplitude.Mercury.Overlay;
using Amplitude;
using System;
using System.Linq;
using UnityEngine;

using Modding.Humankind.DevTools.Core;
using Modding.Humankind.DevTools.DeveloperTools.UI;

namespace DevTools.Humankind.GUITools.UI
{
    public class DistrictPainterToolWindow : FloatingToolWindow
    {
        public override string WindowTitle { get; set; } = "DISTRICT PAINTER TOOL";

        public override string WindowGUIStyle { get; set; } = "PopupWindow";

        public override bool ShouldBeVisible => !GlobalSettings.ShouldHideTools;

        public override bool ShouldRestoreLastWindowPosition => true;

        public override Rect WindowRect { get; set; } = new Rect(130f, 260f, 680f, 500f);

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

        private ScrollableGrid VisualAffinityGrid;
        private ScrollableGrid DistrictDefinitionGrid;

        private Amplitude.Framework.Runtime.IRuntimeService runtimeService;
        private int affinityIndex = -1;
        private int extensionDistrictIndex = -1;
        private string[] affinities;
        private string[] extensionDistrictDefinitions;
        private Vector2 extensionDistrictPosition;
        private Vector2 affinityPosition;
        private Vector2 initialAffinityPosition;
        private bool useAffinity;
        private bool useDistrictDefinition;
        private bool useInitialAffinity;
        private bool activeCursor;

        protected override void OnDrawWindowClientArea(int instanceId)
        {
            GUI.color = Color.white;
            GUI.backgroundColor = Color.white;
            if (Snapshots.GameSnapshot == null || Snapshots.SandboxSnapshot == null)
                return;
            using (new GUILayout.VerticalScope((GUIStyle)"Widget.ClientArea", new GUILayoutOption[1]
            {
                GUILayout.ExpandWidth(true)
            }))
            {
                if (this.runtimeService == null)
                {
                    GUILayout.Label("Waiting for the runtime service...");
                    if (Event.current.type != UnityEngine.EventType.Repaint)
                        return;
                    this.runtimeService = Services.GetService<Amplitude.Framework.Runtime.IRuntimeService>();
                }
                else if (this.runtimeService.Runtime == null || !this.runtimeService.Runtime.HasBeenLoaded)
                    GUILayout.Label("Waiting for the runtime...");
                else if (this.runtimeService.Runtime.FiniteStateMachine.CurrentState == null)
                    GUILayout.Label("Waiting for the runtime...");
                else if (this.runtimeService.Runtime.FiniteStateMachine.CurrentState.GetType() != typeof(RuntimeState_InGame))
                    GUILayout.Label("Waiting for the runtime state...");
                else if (!Amplitude.Mercury.Presentation.Presentation.HasBeenStarted)
                    GUILayout.Label("Waiting for the presentation...");
                else
                    this.DrawClientArea();
            }
        }

        private void DrawClientArea()
        {
            if (this.affinities == null)
            {
                this.affinities = Databases.GetDatabase<BuildingVisualAffinityDefinition>().Select<BuildingVisualAffinityDefinition, string>((Func<BuildingVisualAffinityDefinition, string>)(element => element.Name.ToString())).ToArray<string>();
                this.extensionDistrictDefinitions = Databases.GetDatabase<ConstructibleDefinition>().Where<ConstructibleDefinition>((Func<ConstructibleDefinition, bool>)(element => element is ExtensionDistrictDefinition)).Select<ConstructibleDefinition, string>((Func<ConstructibleDefinition, string>)(element => element.Name.ToString())).ToArray<string>();

                VisualAffinityGrid = new ScrollableGrid() {
                    ItemsPerRow = 1,
                    Items = this.affinities.Select(name => new GUIContent(name)).ToArray(),
                };

                DistrictDefinitionGrid = new ScrollableGrid() {
                    ItemsPerRow = 1,
                    Items = this.extensionDistrictDefinitions.Select(name => new GUIContent(
                            /*R.Text.GetLocalizedTitle(new StaticString(name)) + "  " +*/ name
                        )).ToArray(),
                };
            }
            DistrictPainterCursor currentCursor = Amplitude.Mercury.Presentation.Presentation.PresentationCursorController.CurrentCursor as DistrictPainterCursor;
            int num = currentCursor == null ? 1 : 0;
            this.activeCursor = GUILayout.Toggle(this.activeCursor, "Activate toggle cursor");
            if (num != 0)
            {
                if (this.activeCursor)
                {
                    Amplitude.Mercury.Presentation.Presentation.PresentationCursorController.ChangeToDistrictPainterCursor();
                    currentCursor = Amplitude.Mercury.Presentation.Presentation.PresentationCursorController.CurrentCursor as DistrictPainterCursor;
                }
                else
                    GUI.enabled = false;
            }
            else if (!this.activeCursor)
            {
                Amplitude.Mercury.Presentation.Presentation.PresentationCursorController.ChangeToDefaultCursor();
                GUI.enabled = false;
            }
            using (new GUILayout.HorizontalScope(Array.Empty<GUILayoutOption>()))
            {
                using (new GUILayout.VerticalScope(Array.Empty<GUILayoutOption>()))
                {
                    GUI.enabled = this.activeCursor;
                    using (new GUILayout.HorizontalScope(Array.Empty<GUILayoutOption>()))
                    {
                        this.useAffinity = GUILayout.Toggle(this.useAffinity, "Paint affinity");
                        this.useInitialAffinity = GUILayout.Toggle(this.useInitialAffinity, "Paint initial affinity");
                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.Space(10f);
                    GUI.enabled = true;

                    this.affinityIndex = VisualAffinityGrid.Draw<GUIContent>();
                    /*this.affinityPosition = GUILayout.BeginScrollView(
                        this.affinityPosition,
                        false,
                        true,
                        "horizontalscrollbar",
                        "verticalscrollbar",
                        "scrollview",
                        GUILayout.Height(320f)
                    );
                    // this.affinityPosition = GUILayout.BeginScrollView(this.affinityPosition);
                    this.affinityIndex = GUILayout.SelectionGrid(this.affinityIndex, this.affinities, 1);
                    GUILayout.EndScrollView();*/
                }
                using (new GUILayout.VerticalScope(Array.Empty<GUILayoutOption>()))
                {
                    GUI.enabled = this.activeCursor;
                    using (new GUILayout.HorizontalScope(Array.Empty<GUILayoutOption>()))
                    {
                        this.useDistrictDefinition = GUILayout.Toggle(this.useDistrictDefinition, "Paint district definition");
                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.Space(10f);
                    GUI.enabled = true;

                    this.extensionDistrictIndex = DistrictDefinitionGrid.Draw<GUIContent>();
                    /*this.extensionDistrictPosition = GUILayout.BeginScrollView(
                        this.extensionDistrictPosition,
                        false,
                        true,
                        "horizontalscrollbar",
                        "verticalscrollbar",
                        "scrollview",
                        GUILayout.Height(320f)
                    );
                    this.extensionDistrictIndex = GUILayout.SelectionGrid(this.extensionDistrictIndex, this.extensionDistrictDefinitions, 1);
                    GUILayout.EndScrollView();*/
                }
                if (currentCursor != null)
                {
                    currentCursor.UseCurrentAffinity = this.useAffinity;
                    currentCursor.UseCurrentInitialAffinity = this.useInitialAffinity;
                    currentCursor.UseCurrentDistrictDefinition = this.useDistrictDefinition;
                    currentCursor.CurrentAffinity = this.affinityIndex < 0 ? StaticString.Empty : new StaticString(this.affinities[this.affinityIndex]);
                    currentCursor.CurrentInitialAffinity = this.affinityIndex < 0 ? StaticString.Empty : new StaticString(this.affinities[this.affinityIndex]);
                    currentCursor.CurrentDistrictDefinition = this.extensionDistrictIndex < 0 ? StaticString.Empty : new StaticString(this.extensionDistrictDefinitions[this.extensionDistrictIndex]);
                }
            }
            GUI.enabled = true;
        }
    }
}
