using System;
using Amplitude;
using Amplitude.Mercury.Interop;
using Amplitude.Mercury.Presentation;
using Amplitude.Mercury.Sandbox;
using Amplitude.Mercury.Simulation;
using DevTools.Humankind.GUITools.UI.PlacementCursors;
using Modding.Humankind.DevTools;

namespace DevTools.Humankind.GUITools.UI
{
    public partial class PlacementCursorController
    {
        public StaticString ConstructibleDefinitionName { get; set; } = StaticString.Empty;
        public SimulationEntityGUID SettlementGUID { get; set; } = SimulationEntityGUID.Zero;

        private static PlacementCursorController _instance;

        public static PlacementCursorController Instance => _instance != null
            ? _instance
            : (_instance = new PlacementCursorController());

        public static void Run(int tileIndex) => Instance.DoRun(tileIndex);

        // TODO: REMOVE
        private bool TryGetDistrictInfoAt(int position, out DistrictInfo districtInfo)
        {
            DistrictInfo[] districtInfo1 =
                (DistrictInfo[]) Snapshots.GameSnapshot.PresentationData.LocalEmpireInfo.DistrictInfo;
            for (int index = 0; index < districtInfo1.Length; ++index)
            {
                if (districtInfo1[index].TileIndex == position)
                {
                    districtInfo = districtInfo1[index];
                    return true;
                }
            }

            districtInfo = new DistrictInfo();
            return false;
        }
        //

        public static bool TryGetSettlementInfoAt(int tileIndex, out SettlementInfo settlementInfo)
        {
            try
            {
                ref var local = ref Snapshots.GameSnapshot.PresentationData.SettlementInfo.Data[
                    Snapshots.GameSnapshot.PresentationData.GetSettlementIndexAt(tileIndex)];

                settlementInfo = local;
            }
            catch (Exception e)
            {
                Loggr.Log(e);
                settlementInfo = new SettlementInfo();
                return false;
            }

            return true;
        }

        private void DoRun(int tileIndex)
        {
            var placement = PlacementType.None;
            var localEmpireIndex = Snapshots.GameSnapshot.PresentationData.LocalEmpireInfo.EmpireIndex;
            int empireIndex = localEmpireIndex;

            if (LiveEditorMode.BrushType == LiveBrushType.District)
            {
 
                /*if (TryGetSettlementInfoAt(tileIndex, out SettlementInfo settlementInfo))
                {
                    empireIndex = settlementInfo.EmpireIndex;

                    if (empireIndex < Sandbox.NumberOfMajorEmpires)
                    {
                        SettlementGUID = settlementInfo.SimulationEntityGUID;
                        ConstructibleDefinitionName = LiveEditorMode.ActivePaintBrush.Name;
                    }
                }*/

                if (PaintBrush.TryGetEntitiesAt(tileIndex, out empireIndex,
                        out Amplitude.Mercury.Interop.AI.Entities.Settlement settlement,
                        out Amplitude.Mercury.Interop.AI.Entities.Territory territory))
                {
                    if (empireIndex < Sandbox.NumberOfMajorEmpires)
                    {
                        placement = PlacementType.District;
                        SettlementGUID = settlement.SimulationEntityGUID;
                        ConstructibleDefinitionName = LiveEditorMode.ActivePaintBrush.Name;
                    }
                }
            }

            empireIndex = empireIndex >= 0 && empireIndex < Sandbox.NumberOfMajorEmpires
                ? empireIndex
                : localEmpireIndex;

            if (placement != PlacementType.None)
            {
                if (IsAnyVirtualCursorActive)
                {
                    if (!IsValidActiveCursor())
                        DeactivateVirtualPlacementCursor();
                    // ActivateVirtualPlacementCursor(placement, empireIndex);
                }
                else
                {
                    ActivateVirtualPlacementCursor(placement, empireIndex);
                }
            }
            else
            {
                if (IsAnyVirtualCursorActive)
                    DeactivateVirtualPlacementCursor();
            }
        }

        private void DeactivateVirtualPlacementCursor()
        {
            Presentation.PresentationCursorController.ChangeToDefaultCursor();
            // ActiveCursor.Deactivate();
        }

        private void ActivateVirtualPlacementCursor(PlacementType placementType, int empireIndex)
        {
            switch (placementType)
            {
                case PlacementType.District:
                    Presentation.PresentationCursorController.ChangeToVirtualDistrictPlacementCursor(
                        empireIndex, SettlementGUID, ConstructibleDefinitionName);
                    break;
                default:
                    Loggr.Log(new NotImplementedException());
                    break;
            }
        }

        public bool IsValidActiveCursor()
        {
            if (ActiveCursor is VirtualDistrictPlacementCursor districtCursor)
            {
                if (!districtCursor.ConstructibleDefinitionName.Equals(ConstructibleDefinitionName) ||
                    districtCursor.SettlementGUID != SettlementGUID)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
