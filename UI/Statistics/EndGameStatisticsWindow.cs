using System;
using System.Reflection;
using UnityEngine;
using Modding.Humankind.DevTools;
using Modding.Humankind.DevTools.Core;
using Modding.Humankind.DevTools.DeveloperTools.UI;
using UIToolsManager = Modding.Humankind.DevTools.DeveloperTools.UI.UIController;
using Amplitude.Mercury.Interop;
using Amplitude.Mercury.Sandbox;
using Amplitude.Mercury.UI.Helpers;
using Amplitude.Mercury.UI;
using Amplitude.Framework;
using Amplitude.Framework.Networking;
using Amplitude.UI;

namespace DevTools.Humankind.GUITools.UI
{
    public class EndGameStatisticsWindow : FloatingToolWindow
    {
        public override bool ShouldBeVisible => ViewController.View == ViewType.InGame;   // PauseMenu.InGameMenuController.IsEndGameWindowVisible;
        public override bool ShouldRestoreLastWindowPosition => false;
        public override string WindowTitle { get; set; } = "STATISTICS WINDOW";
        public override Rect WindowRect { get; set; } = new Rect (24f, 24f, 230f, 40f);

        public static FieldInfo EmpireEndGameStatusField = R.GetField<Amplitude.Mercury.Simulation.MajorEmpire>("EmpireEndGameStatus", R.NonPublicInstance);

        private int localEmpireIndex { get; set; }
        private bool isClosing = false;
        private bool initialized = false;
        private bool isEndGameStatusRestored = false;
        private bool wasQuitButtonVisible = false;

        public UITransform QuitButtonUITransform { get; private set; }

        private GUIStyle bgStyle;
        private GUIStyle backButtonStyle;

        public static EndGameWindow EndGameWindow => endGameWindow ?? (endGameWindow = WindowsUtils.GetWindow<EndGameWindow>());
        private static EndGameWindow endGameWindow;

        protected override void Awake()
        {
            base.Awake();
            
            bgStyle = new GUIStyle(UIToolsManager.DefaultSkin.FindStyle("PopupWindow.Sidebar.Highlight")) {

            };
            backButtonStyle = new GUIStyle(UIToolsManager.DefaultSkin.toggle) {
                margin = new RectOffset(1, 1, 1, 1)
            };
        }

        public override void OnDrawUI()
        {
            if (!isClosing)
            {
                localEmpireIndex = (int)Snapshots.GameSnapshot.PresentationData.LocalEmpireInfo.EmpireIndex;

                if (!initialized)
                {
                    initialized = true;
                    SandboxManager.PostOrder((Order)new OrderEmpireResign());

                    QuitButtonUITransform = GameObject
                        .Find("WindowsRoot/InGameFullscreen/EndGameWindow/LeftButtonsGroup/QuitButton")
                        .transform
                        .GetComponent<UITransform>();
                    

                }
                else
                {
                    if (!isEndGameStatusRestored && PauseMenu.InGameMenuController.IsEndGameWindowVisible)
                    {
                        if ((EmpireEndGameStatus)EmpireEndGameStatusField.GetValue(HumankindGame.Empires[localEmpireIndex].Simulation) == EmpireEndGameStatus.Resigned)
                        {
                            EmpireEndGameStatusField.SetValue(HumankindGame.Empires[localEmpireIndex].Simulation, EmpireEndGameStatus.InGame);
                            isEndGameStatusRestored = true;
                        }
                    }
                }
            }

            if (!wasQuitButtonVisible)
            {
                if (QuitButtonUITransform.VisibleSelf)
                {
                    wasQuitButtonVisible = true;
                    QuitButtonUITransform.VisibleSelf = false;
                }
                GUILayout.BeginVertical();
                GUILayout.Space(4f);
                GUILayout.Label("    ");
                GUILayout.Space(4f);
                GUILayout.EndVertical();

                return;
            }

            if (QuitButtonUITransform.VisibleSelf)
            {
                wasQuitButtonVisible = true;
                QuitButtonUITransform.VisibleSelf = false;
            }

            GUILayout.BeginVertical(bgStyle);
                GUI.backgroundColor = Color.white;
                if (GUILayout.Button("<b>BACK TO THE GAME</b>", backButtonStyle,
                    GUILayout.Width(228f), GUILayout.Height(38f)))
                {
                    MainTools.CloseEndGameStatisticsWindow();
                    ViewController.ViewMode = ViewModeType.Auto;
                }
                GUI.backgroundColor = Color.white;

            GUILayout.EndVertical();
        }

        public override void Close(bool saveVisibilityStateBeforeClosing = false)
        {
            isClosing = true;

            // redundancy on purpose
            if ((EmpireEndGameStatus)EmpireEndGameStatusField.GetValue(HumankindGame.Empires[localEmpireIndex].Simulation) == EmpireEndGameStatus.Resigned)
            {
                EmpireEndGameStatusField.SetValue(HumankindGame.Empires[localEmpireIndex].Simulation, EmpireEndGameStatus.InGame);
                isEndGameStatusRestored = true;
            }

            Services.GetService<INetworkingService>()?.CreateMessageSender().SendLocalMessage(
                (LocalMessage) new SandboxControlMessage(
                    (ISandboxControlInstruction) new ChangeLocalEmpireInstruction(localEmpireIndex)
                )
            );
            
            base.Close(false);
        }
    }
}
