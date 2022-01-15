using Amplitude.Framework;
using Amplitude.Framework.Networking;
using Amplitude.Framework.Overlay;
using Amplitude.Mercury.Interop;
using Amplitude.Mercury.Runtime;
using Amplitude.Mercury.Sandbox;
using UnityEngine;

using Modding.Humankind.DevTools.Core;
using Modding.Humankind.DevTools.DeveloperTools.UI;

namespace DevTools.Humankind.GUITools.UI
{
    public class GameInfoToolWindow : FloatingToolWindow
    {
        public override string WindowTitle { get; set; } = "GAME INFO TOOL";

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
        private static string[] toolbar_EmpireIndices = new string[0];

        protected Amplitude.Framework.Runtime.IRuntimeService RuntimeService { get; private set; }

        protected override void OnDrawWindowClientArea(int instanceId)
        {
            GUI.color = Color.white;
            GUI.backgroundColor = Color.white;
            GUILayout.BeginVertical((GUIStyle)"Widget.ClientArea", GUILayout.ExpandWidth(true));
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
                                if (Snapshots.GameSnapshot != null && Snapshots.SandboxSnapshot != null)
                                {
                                    int numberOfMajorEmpires = Snapshots.GameSnapshot.PresentationData.NumberOfMajorEmpires;
                                    if (GameInfoToolWindow.toolbar_EmpireIndices.Length != numberOfMajorEmpires)
                                    {
                                        GameInfoToolWindow.toolbar_EmpireIndices = new string[numberOfMajorEmpires];
                                        for (int index = 0; index < numberOfMajorEmpires; ++index)
                                            GameInfoToolWindow.toolbar_EmpireIndices[index] = index.ToString();
                                    }
                                    GUILayout.BeginHorizontal();
                                    GUILayout.Label(Snapshots.SandboxSnapshot.PresentationData.CurrentSandboxStateName, (GUIStyle)"PopupWindow.MonospacedLabel");
                                    GUILayout.FlexibleSpace();
                                    GUILayout.Label(Snapshots.SandboxSnapshot.PresentationData.Frame.ToString());
                                    GUILayout.EndHorizontal();
                                    GUILayout.Space(1f);
                                    GUILayout.BeginHorizontal();
                                    bool isPresentationFogOfWarEnabled = GUILayout.Toggle(Snapshots.GameSnapshot.PresentationData.IsPresentationFogOfWarEnabled, "Enable fog of war (Presentation)");
                                    if (Snapshots.GameSnapshot.PresentationData.IsPresentationFogOfWarEnabled != isPresentationFogOfWarEnabled)
                                        Snapshots.GameSnapshot.SetFogOfWarEnabled(isPresentationFogOfWarEnabled);
                                    GUILayout.EndHorizontal();
                                    GUILayout.BeginHorizontal();
                                    bool flag = GUILayout.Toggle(Snapshots.GameSnapshot.PresentationData.IsGameplayFogOfWarEnabled, "Enable fog of war (Gameplay)");
                                    if (Snapshots.GameSnapshot.PresentationData.IsGameplayFogOfWarEnabled != flag)
                                        SandboxManager.PostOrder((Order)new OrderEnableFogOfWar()
                                        {
                                            Enable = flag
                                        });
                                    GUILayout.EndHorizontal();
                                    if (GUILayout.Button("Discover world."))
                                        SandboxManager.PostOrder((Order)new OrderDiscoverWorld());
                                    if (GUILayout.Button("Change to vision debug cursor."))
                                        Amplitude.Mercury.Presentation.Presentation.PresentationCursorController.ChangeToVisionDebugCursor();
                                    GUILayout.Space(1f);
                                    GUILayout.BeginHorizontal();
                                    if (GUILayout.Button("Create dump."))
                                        SandboxManager.PostRequest((Request)new RequestDump());
                                    GUILayout.EndHorizontal();
                                    GUILayout.Space(5f);
                                    GUILayout.BeginHorizontal();
                                    GUILayout.Label("Switch empire");
                                    GUILayout.Space(12f);
                                    GUILayout.FlexibleSpace();
                                    int localEmpireIndex = GUILayout.Toolbar((int)Snapshots.GameSnapshot.PresentationData.LocalEmpireInfo.EmpireIndex, GameInfoToolWindow.toolbar_EmpireIndices, (GUIStyle)"PopupWindow.ToolbarButton", GUILayout.ExpandWidth(false));
                                    if (localEmpireIndex != (int)Snapshots.GameSnapshot.PresentationData.LocalEmpireInfo.EmpireIndex && localEmpireIndex < Snapshots.GameSnapshot.PresentationData.NumberOfMajorEmpires)
                                        Services.GetService<INetworkingService>()?.CreateMessageSender().SendLocalMessage((LocalMessage)new SandboxControlMessage((ISandboxControlInstruction)new ChangeLocalEmpireInstruction(localEmpireIndex)));
                                    GUILayout.EndHorizontal();

                                    GUILayout.BeginHorizontal();

                                    GUI.color = Color.white;
                                    GUI.backgroundColor = Color.white;
                                    GUILayout.BeginVertical((GUIStyle) "Widget.ClientArea", GUILayout.ExpandWidth(true));
                                    if ((Object) Amplitude.Mercury.Presentation.Presentation.PresentationCursorController != (Object) null)
                                        Amplitude.Mercury.Presentation.Presentation.PresentationCursorController.OnInspectorGUI();
                                    else
                                        GUILayout.Label("PresentationCursorController is null.");
                                    GUILayout.EndVertical();

                                    GUILayout.EndHorizontal();
                                }
                                else
                                    GUILayout.Label("Waiting for the snapshots...");
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
                if (Event.current.type == UnityEngine.EventType.Repaint)
                    this.RuntimeService = Services.GetService<Amplitude.Framework.Runtime.IRuntimeService>();
            }
            GUILayout.EndVertical();
        }
    }
}
