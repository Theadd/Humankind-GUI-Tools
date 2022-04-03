using System;
using System.Linq;
using Amplitude;
using Amplitude.Framework;
using Amplitude.Mercury.Data.Simulation;
using Amplitude.Mercury.Interop;
using Amplitude.Mercury.Presentation;
using Amplitude.Mercury.Runtime;
using Modding.Humankind.DevTools.DeveloperTools.UI;
using UnityEngine;
using IRuntimeService = Amplitude.Framework.Runtime.IRuntimeService;

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

        private IRuntimeService runtimeService;
        private int affinityIndex = -1;
        private int extensionDistrictIndex = -1;
        private string[] affinities;
        private string[] extensionDistrictDefinitions;
        // private Vector2 extensionDistrictPosition;
        // private Vector2 affinityPosition;
        // private Vector2 initialAffinityPosition;
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
            using (new GUILayout.VerticalScope("Widget.ClientArea", GUILayout.ExpandWidth(true)))
            {
                if (runtimeService == null)
                {
                    GUILayout.Label("Waiting for the runtime service...");
                    if (Event.current.type != EventType.Repaint)
                        return;
                    runtimeService = Services.GetService<IRuntimeService>();
                }
                else if (runtimeService.Runtime == null || !runtimeService.Runtime.HasBeenLoaded)
                    GUILayout.Label("Waiting for the runtime...");
                else if (runtimeService.Runtime.FiniteStateMachine.CurrentState == null)
                    GUILayout.Label("Waiting for the runtime...");
                else if (runtimeService.Runtime.FiniteStateMachine.CurrentState.GetType() != typeof(RuntimeState_InGame))
                    GUILayout.Label("Waiting for the runtime state...");
                else if (!Presentation.HasBeenStarted)
                    GUILayout.Label("Waiting for the presentation...");
                else
                    DrawClientArea();
            }
        }

        private void DrawClientArea()
        {
            if (affinities == null)
            {
                affinities = Databases.GetDatabase<BuildingVisualAffinityDefinition>()
                    .Select(element => element.Name.ToString())
                    .ToArray();
                extensionDistrictDefinitions = Databases.GetDatabase<ConstructibleDefinition>()
                    .Where(element => element is ExtensionDistrictDefinition)
                    .Select(element => element.Name.ToString())
                    .ToArray();

                VisualAffinityGrid = new ScrollableGrid
                {
                    ItemsPerRow = 1,
                    Items = affinities.Select(name => new GUIContent(name)).ToArray(),
                };

                DistrictDefinitionGrid = new ScrollableGrid
                {
                    ItemsPerRow = 1,
                    Items = extensionDistrictDefinitions.Select(name => new GUIContent(
                            /*R.Text.GetLocalizedTitle(new StaticString(name)) + "  " +*/ name
                        )).ToArray(),
                };
            }
            DistrictPainterCursor currentCursor = Presentation.PresentationCursorController.CurrentCursor as DistrictPainterCursor;
            int num = currentCursor == null ? 1 : 0;
            activeCursor = GUILayout.Toggle(activeCursor, "Activate toggle cursor");
            if (num != 0)
            {
                if (activeCursor)
                {
                    Presentation.PresentationCursorController.ChangeToDistrictPainterCursor();
                    currentCursor = Presentation.PresentationCursorController.CurrentCursor as DistrictPainterCursor;
                }
                else
                    GUI.enabled = false;
            }
            else if (!activeCursor)
            {
                Presentation.PresentationCursorController.ChangeToDefaultCursor();
                GUI.enabled = false;
            }
            using (new GUILayout.HorizontalScope(Array.Empty<GUILayoutOption>()))
            {
                using (new GUILayout.VerticalScope(Array.Empty<GUILayoutOption>()))
                {
                    GUI.enabled = activeCursor;
                    using (new GUILayout.HorizontalScope(Array.Empty<GUILayoutOption>()))
                    {
                        useAffinity = GUILayout.Toggle(useAffinity, "Paint affinity");
                        useInitialAffinity = GUILayout.Toggle(useInitialAffinity, "Paint initial affinity");
                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.Space(10f);
                    GUI.enabled = true;

                    affinityIndex = VisualAffinityGrid.Draw<GUIContent>();
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
                    GUI.enabled = activeCursor;
                    using (new GUILayout.HorizontalScope(Array.Empty<GUILayoutOption>()))
                    {
                        useDistrictDefinition = GUILayout.Toggle(useDistrictDefinition, "Paint district definition");
                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.Space(10f);
                    GUI.enabled = true;

                    extensionDistrictIndex = DistrictDefinitionGrid.Draw<GUIContent>();
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
                    currentCursor.UseCurrentAffinity = useAffinity;
                    currentCursor.UseCurrentInitialAffinity = useInitialAffinity;
                    currentCursor.UseCurrentDistrictDefinition = useDistrictDefinition;
                    currentCursor.CurrentAffinity = affinityIndex < 0 ? StaticString.Empty : new StaticString(affinities[affinityIndex]);
                    currentCursor.CurrentInitialAffinity = affinityIndex < 0 ? StaticString.Empty : new StaticString(affinities[affinityIndex]);
                    currentCursor.CurrentDistrictDefinition = extensionDistrictIndex < 0 ? StaticString.Empty : new StaticString(extensionDistrictDefinitions[extensionDistrictIndex]);
                }
            }
            GUI.enabled = true;
        }
    }
}
