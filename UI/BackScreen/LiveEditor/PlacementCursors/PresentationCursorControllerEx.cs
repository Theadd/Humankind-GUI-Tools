using System;
using System.Reflection;
using Amplitude;
using Amplitude.Mercury.Presentation;
using Amplitude.Mercury.Simulation;
using HarmonyLib;
using Modding.Humankind.DevTools;

namespace DevTools.Humankind.GUITools.UI
{
    public static class PresentationCursorControllerEx
    {
        private static MethodInfo GetMethod<T>(string name, Type[] parameters) =>
            typeof(T).GetMethod(name, AccessTools.all, null,
                parameters ?? new Type[] { }, null);

        private static MethodInfo GetMethod<T>(string name) =>
            typeof(T).GetMethod(name, AccessTools.all);

        private static MethodInfo ResetCursorInfoPerMouseButton =
            GetMethod<PresentationCursorController>("ResetCursorInfoPerMouseButton");

        private static MethodInfo GetOrCreateCursor =
            GetMethod<PresentationCursorController>("GetOrCreateCursor", new Type[] { typeof(Type) });

        private static MethodInfo ChangeCursor = GetMethod<PresentationCursorController>
            ("ChangeCursor", new Type[] { typeof(Type), typeof(SimulationEntityGUID) });

        public static void ChangeToVirtualDistrictPlacementCursor(
            this PresentationCursorController self,
            int empireIndex,
            SimulationEntityGUID settlementGUID,
            StaticString districtDefinitionName,
            Amplitude.Mercury.Data.Simulation.ActionType buyoutActionType =
                Amplitude.Mercury.Data.Simulation.ActionType.Count)
        {
            self.CurrentCursor?.Deactivate();
            var currentCursor = (VirtualDistrictPlacementCursor) GetOrCreateCursor.Invoke(self,
                new object[] { typeof(VirtualDistrictPlacementCursor) });
            currentCursor.ConstructibleDefinitionName = districtDefinitionName;
            currentCursor.BuyoutActionType = buyoutActionType;
            currentCursor.TargetEmpireIndex = empireIndex;

            ChangeCursor.Invoke(self,
                new object[] { typeof(VirtualDistrictPlacementCursor), settlementGUID });
            if (currentCursor != self.CurrentCursor)
                Loggr.Log(new Exception(
                    "PresentationCursorController.CurrentCursor DIFFERS FROM EXPECTED currentCursor."));
        }
    }
}
