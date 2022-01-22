using UnityEngine;
using Amplitude.Mercury.Presentation;
using Modding.Humankind.DevTools;
using Modding.Humankind.DevTools.DeveloperTools.UI;

namespace DevTools.Humankind.GUITools.UI
{
    public class FreeCameraController
    {
        public bool cameraBool;
        public float nearClip = 33.93487f; //orig values
        public float farClip = 113.942f;

        public GameObject cameraGo;
        public PresentationCameraMover presCamMover;
        public FreeCamera FreeCam;
        public Camera cameraCam;

        public void Setup()
        {
            cameraBool = false;
            cameraGo = GameObject.Find("Camera");
            if (cameraGo == null)
            {
                Loggr.Log("NullRef, No Camera GameObject found");
            }

            //this is the default script the game uses to control the camera
            presCamMover = cameraGo.GetComponent<PresentationCameraMover>();
            if (presCamMover == null)
            {
                Loggr.Log("NullRef, No PresentationCameraMover found");
            }

            cameraCam = cameraGo.GetComponent<Camera>();

            Quaternion localRotation = cameraGo.transform.localRotation;
            Vector3 position = cameraGo.transform.position;

            FreeCam = cameraGo.AddComponent<FreeCamera>();

            nearClip = cameraCam.nearClipPlane; //in case of different camera settings
            farClip = cameraCam.farClipPlane;

            FreeCam.transform.position = new Vector3(position.x, 25f, position.z);
            FreeCam.transform.localRotation = localRotation;
        }

        public void ToggleFreeCameraMode()
        {
            if (!cameraGo)
            {
                Setup();
            }

            cameraBool = !cameraBool;
            presCamMover.enabled = !cameraBool;
            FreeCam.enabled = cameraBool;
            //GUI.enabled = !GUI.enabled;

            if (cameraBool == true)
            {
                UnityEngine.Cursor.lockState = CursorLockMode.Locked;
                cameraCam.nearClipPlane = 0.01f;
                cameraCam.farClipPlane = farClip * 2;
                if (UIController.AreTooltipsVisible)
                    ActionController.ToggleAmplitudeUIVisibility();
            }
            else
            {
                UnityEngine.Cursor.lockState = CursorLockMode.None;
                cameraCam.nearClipPlane = nearClip;
                cameraCam.farClipPlane = farClip;
                if (!UIController.AreTooltipsVisible)
                    ActionController.ToggleAmplitudeUIVisibility();
            }
        }
    }
}
