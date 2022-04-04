using System;
using Amplitude;
using Amplitude.Mercury.Audio;
using Amplitude.Mercury.Fx.HgFx;
using Amplitude.Mercury.Interop;
using Amplitude.Mercury.Presentation;
using Amplitude.Mercury.Simulation;
using Modding.Humankind.DevTools;
using Action = System.Action;
using Object = UnityEngine.Object;

namespace DevTools.Humankind.GUITools.UI
{
    public class VirtualDistrictPlacementCursor : DistrictPlacementCursor, IVirtualPlacementCursor
    {
        public int TargetEmpireIndex { get; set; }

        public override void Activate(SimulationEntityGUID settlementGuid)
        {
            // int empireIndex = Snapshots.GameSnapshot.PresentationData.LocalEmpireInfo.EmpireIndex;
            int empireIndex =
                TargetEmpireIndex; // Snapshots.GameSnapshot.PresentationData.LocalEmpireInfo.EmpireIndex;
            Snapshots.SettlementCursorSnapshot.Start(settlementGuid);
            Snapshots.AffinityPanelSnapshot.SelectSettlement(settlementGuid);
            var highlightedPosition = Presentation.PresentationCursorController.CurrentHighlightedPosition;
            Snapshots.DistrictPlacementCursorSnapshot.Start(empireIndex, settlementGuid,
                DistrictToEvaluate, highlightedPosition);
            var placementCursorSnapshot = Snapshots.DistrictPlacementCursorSnapshot;
            placementCursorSnapshot.PresentationDataChanged += OnPresentationDataChanged;
            Presentation.PresentationFimsController.StartDistrictPlacementFims(settlementGuid);
            Presentation.PresentationFrontiersController.OnSettlementLockedByCursor(settlementGuid);
            settlementGUID = settlementGuid;
            ActivateFromBaseTypes(settlementGuid);
        }

        private void ActivateFromBaseTypes(SimulationEntityGUID simulationEntityGUID)
        {
            this.settlementGUID = simulationEntityGUID;
            this.isCurrentPositionValid = false;
            Amplitude.Mercury.Presentation.Presentation.PresentationEntityFactoryController
                .SetSelectedSettlement(simulationEntityGUID);
            if ((UnityEngine.Object) this.mouseGhostLevelBuildComponent == (Object) null)
            {
                this.mouseGhostLevelBuildComponent =
                    Amplitude.Mercury.Presentation.Presentation.PresentationEntityMeshController
                        .CreatePresentationLevelBuildComponent("ConstructiblePlacementCursor.Ghost");
                this.mouseGhostLevelBuildComponent.gameObject.SetActive(true);
            }

            this.mouseGhostLevelBuildComponent.SetRandomSeed(0);
            StaticString constructibleDefinitionName = this.GetConstructibleDefinitionName();
            if (!StaticString.IsNullOrEmpty(constructibleDefinitionName))
            {
                StaticString affinityDefinitionName = Snapshots.GameSnapshot.PresentationData.LocalEmpireInfo
                    .BuildingVisualAffinityDefinitionName;
                int eraIndex = Snapshots.GameSnapshot.PresentationData.LocalEmpireInfo.EraIndex;
                this.ghostRequest =
                    AssetReferenceRepositoryRequestHelper.CreateAssetReferenceRepositoryRequest(
                        ref constructibleDefinitionName, ref affinityDefinitionName, eraIndex);
                this.mouseGhostLevelBuildComponent.SetChannel(0, ref this.ghostRequest,
                    this.isCurrentPositionValid
                        ? HgFxAnchorComponent.RenderModeEnum.GhostOk
                        : HgFxAnchorComponent.RenderModeEnum.GhostNOk, true);
            }
            else
                this.mouseGhostLevelBuildComponent.ClearChannel(0);

            this.worldPosition = this.lastWorldPosition = Amplitude.Mercury.Presentation.Presentation
                .PresentationCursorController.CurrentHighlightedPosition;
            this.ActivateTutorialDomain();
        }

        public override void Deactivate()
        {
            if (!FeatureFlags.QuietMode)
                Loggr.Log("\tDeactivating VisualDistrictPlacementCursor");
            /*Presentation.PresentationFrontiersController.OnSettlementUnlockedByCursor();
            Presentation.PresentationFimsController.StopDistrictPlacementFIMS();
            Snapshots.AffinityPanelSnapshot.SelectSettlement(SimulationEntityGUID.Zero);
            Snapshots.SettlementCursorSnapshot.Stop();
            Snapshots.DistrictPlacementCursorSnapshot.Stop();
            var placementCursorSnapshot = Snapshots.DistrictPlacementCursorSnapshot;
            placementCursorSnapshot.PresentationDataChanged -= OnPresentationDataChanged;
            // base.Deactivate already handles this // this.tileFeedbacks.Clear();
            // Skip call to DistrictPlacementCursor.Deactivate
            // base.Deactivate();
            var ptr = typeof(BaseConstructiblePlacementCursor).GetMethod("Deactivate").MethodHandle
                .GetFunctionPointer();
            var baseDeactivate = (Action) Activator.CreateInstance(typeof(Action), this, ptr);
            baseDeactivate();*/

            base.Deactivate();
        }

        public override void OnClick(MouseButton mouseButton)
        {
            // Loggr.Log($"Overriding {mouseButton.ToString()} Mouse CLICK event IN VirtualDistrictPlacementCursor.");
            // SendAudioEvent(AudioEvents.DistrictPlacementCursorClick);
        }

        public override void Back()
        {
            if (!FeatureFlags.QuietMode)
                Loggr.Log("\tBack() OVERRIDEN in VisualDistrictPlacementCursor", ConsoleColor.DarkRed);
            Presentation.PresentationCursorController.ChangeToDefaultCursor();
        }
    }
}
