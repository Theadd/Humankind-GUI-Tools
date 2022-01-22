using Amplitude.Mercury.Interop;
using Amplitude.Mercury.Presentation;
using Amplitude.Mercury.Sandbox;
using Modding.Humankind.DevTools.DeveloperTools.UI;
using UnityEngine;
using Amplitude;
using Amplitude.Framework;
using Amplitude.Mercury;
using Amplitude.Mercury.Simulation;

namespace DevTools.Humankind.GUITools.UI
{
    public static partial class ActionController
    {
        public static bool IsPresentationFogOfWarEnabled =>
            Snapshots.GameSnapshot?.PresentationData?.IsPresentationFogOfWarEnabled ?? false;

        public static void TogglePresentationFogOfWar() =>
            Snapshots.GameSnapshot?.SetFogOfWarEnabled(!IsPresentationFogOfWarEnabled);

        public static void ToggleFreeCameraMode() => FreeCamera.ToggleFreeCameraMode();

        public static FreeCameraController FreeCamera { get; set; } = new FreeCameraController();

        public static PresentationCameraMover CameraMover => _cameraMover ? _cameraMover 
            : (_cameraMover = GameObject.Find("Camera")?.GetComponent<PresentationCameraMover>());

        public static Camera Camera => _camera ? _camera 
            : (_camera = GameObject.Find("Camera")?.GetComponent<Camera>());

        public static void SwitchCameraFieldOfView()
        {
            var camera = Camera;

            if (camera != null)
                camera.fieldOfView = camera.fieldOfView < 40f ? (camera.fieldOfView < 30f ? 65f : 15f) : 35f;
        }

        public static void ToggleTooltipsVisibility() =>
            UIController.AreTooltipsVisible = !UIController.AreTooltipsVisible;

        public static void ToggleAmplitudeUIVisibility() =>
            UIController.IsAmplitudeUIVisible = !UIController.IsAmplitudeUIVisible;

        public static void ToggleFrontiersVisibility() => Presentation.PresentationFrontiersController?
            .DisplayAllFrontiers(!Presentation.PresentationFrontiersController?.FrontiersDisplayed ?? false);

        public static void ToggleGodMode() => UIController.GodMode = !UIController.GodMode;

        public static bool TryGetDistrictInfoAt(WorldPosition position, out DistrictInfo districtInfo)
        {
            DistrictInfo[] array = Snapshots.GameSnapshot.PresentationData.LocalEmpireInfo.DistrictInfo;
            for (int i = 0; i < array.Length; i++)
            {
                if (new WorldPosition(array[i].TileIndex) == position)
                {
                    districtInfo = array[i];
                    return true;
                }
            }
            districtInfo = default(DistrictInfo);
            return false;
        }
        
        private static Camera _camera = null;
        private static PresentationCameraMover _cameraMover = null;
    }
}