using System;
using System.Linq;
using Amplitude.Mercury.Interop;
using Amplitude.Mercury.Presentation;
using Amplitude.Mercury.Sandbox;
using Modding.Humankind.DevTools.DeveloperTools.UI;
using UnityEngine;
using Amplitude;
using Amplitude.Framework;
using Amplitude.Mercury;
using Amplitude.Mercury.Simulation;
using Modding.Humankind.DevTools;

namespace DevTools.Humankind.GUITools.UI
{
    public static partial class ActionController
    {
        public static bool IsPresentationFogOfWarEnabled =>
            Snapshots.GameSnapshot?.PresentationData?.IsPresentationFogOfWarEnabled ?? false;

        public static void TogglePresentationFogOfWar()
        {
            if (ViewController.IsGloballyDisabled) return;
            Snapshots.GameSnapshot?.SetFogOfWarEnabled(!IsPresentationFogOfWarEnabled);
        }

        public static void ToggleFreeCameraMode()
        {
            if (ViewController.IsGloballyDisabled) return;
            FreeCamera.ToggleFreeCameraMode();
        }

        public static FreeCameraController FreeCamera { get; set; } = new FreeCameraController();

        public static PresentationCameraMover CameraMover => _cameraMover
            ? _cameraMover
            : (_cameraMover = GameObject.Find("Camera")?.GetComponent<PresentationCameraMover>());

        public static Camera Camera => _camera
            ? _camera
            : (_camera = GameObject.Find("Camera")?.GetComponent<Camera>());

        public static Camera ImpostorCamera => GameObject.Find("ImpostorCamera")?.GetComponent<Camera>();

        public static void SwitchCameraFieldOfView()
        {
            if (ViewController.IsGloballyDisabled) return;
            var camera = Camera;

            if (camera != null)
                camera.fieldOfView = camera.fieldOfView < 40f ? (camera.fieldOfView < 30f ? 65f : 15f) : 35f;
        }

        public static void ToggleTooltipsVisibility()
        {
            if (ViewController.IsGloballyDisabled) return;
            UIController.AreTooltipsVisible = !UIController.AreTooltipsVisible;
        }

        public static void ToggleAmplitudeUIVisibility()
        {
            if (ViewController.IsGloballyDisabled) return;
            UIController.IsAmplitudeUIVisible = !UIController.IsAmplitudeUIVisible;
        }

        public static void ToggleFrontiersVisibility()
        {
            if (ViewController.IsGloballyDisabled) return;
            Presentation.PresentationFrontiersController?
                .SetUIAllowsFrontierRendering(
                    !Presentation.PresentationFrontiersController?.FrontiersDisplayed ??
                    false);
        }

        public static void ToggleGodMode()
        {
            if (ViewController.IsGloballyDisabled) return;
            UIController.GodMode = !UIController.GodMode;
        }

        public static void ToggleLiveEditorMode()
        {
            if (ViewController.IsGloballyDisabled) return;
            LiveEditorMode.Enabled = !LiveEditorMode.Enabled;
        }
        
        public static void IncreaseToolWindowsWidth()
        {
            var floatingTools = Modding.Humankind.DevTools.DevTools
                .GetGameObject()?
                .GetComponents<FloatingToolWindow>()?
                .ToList();
            if (floatingTools == null) return;
            foreach (var tool in floatingTools)
            {
                if (!tool || tool is IFixedSizeWindow || !tool.IsVisible) continue;
                var r = tool.GetWindowRect();
                r.width = r.width + 100f;
                tool.SetWindowRect(r);
            }
        }
        
        public static void DecreaseToolWindowsWidth()
        {
            var floatingTools = Modding.Humankind.DevTools.DevTools
                .GetGameObject()?
                .GetComponents<FloatingToolWindow>()?
                .ToList();
            if (floatingTools == null) return;
            foreach (var tool in floatingTools)
            {
                if (!tool || tool is IFixedSizeWindow || !tool.IsVisible) continue;
                var r = tool.GetWindowRect();
                r.width = r.width - 20f;
                tool.SetWindowRect(r);
            }
        }
        
        public static void IncreaseToolWindowsHeight()
        {
            var floatingTools = Modding.Humankind.DevTools.DevTools
                .GetGameObject()?
                .GetComponents<FloatingToolWindow>()?
                .ToList();
            if (floatingTools == null) return;
            foreach (var tool in floatingTools)
            {
                if (!tool || tool is IFixedSizeWindow || !tool.IsVisible) continue;
                var r = tool.GetWindowRect();
                r.height = r.height + 100f;
                tool.SetWindowRect(r);
            }
        }
        
        public static void DecreaseToolWindowsHeight()
        {
            var floatingTools = Modding.Humankind.DevTools.DevTools
                .GetGameObject()?
                .GetComponents<FloatingToolWindow>()?
                .ToList();
            if (floatingTools == null) return;
            foreach (var tool in floatingTools)
            {
                if (!tool || tool is IFixedSizeWindow || !tool.IsVisible) continue;
                var r = tool.GetWindowRect();
                r.height = r.height - 20f;
                tool.SetWindowRect(r);
            }
        }
        
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
