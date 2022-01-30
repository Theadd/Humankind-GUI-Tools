using Amplitude.Mercury.Presentation;
using UnityEngine;

namespace DevTools.Humankind.GUITools.UI
{
    public class FreeCamera : MonoBehaviour
    {
        public float cameraSensitivity = 15;
        public float climbSpeed = 8;
        public float normalMoveSpeed = 12;

        public float slowMoveFactor;
        public float fastMoveFactor;

        public float rotationX = 5f;
        public float rotationY = -26f;

        public Camera LinkedCamera { get; set; } = null;
        public PresentationCameraMover CameraMover { get; set; } = null;

        public Transform GetTransform() => transform;
        void Update()
        {
            rotationX += Input.GetAxis("Mouse X") * cameraSensitivity * Time.deltaTime;
            rotationY += Input.GetAxis("Mouse Y") * cameraSensitivity * Time.deltaTime;
            
            
            rotationY = Mathf.Clamp(rotationY, -90f, -5f); 

            transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
            transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);

            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                fastMoveFactor = 3;
            }
            else
            {
                fastMoveFactor = 1;
            }

            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                slowMoveFactor = 0.25f;
            }
            else
            {
                slowMoveFactor = 1;
            }

            transform.position += transform.forward * normalMoveSpeed * slowMoveFactor * fastMoveFactor *
                                  Input.GetAxis("Vertical") * Time.deltaTime;
            transform.position += transform.right * normalMoveSpeed * slowMoveFactor * fastMoveFactor *
                                  Input.GetAxis("Horizontal") * Time.deltaTime;


            if (Input.GetKey(KeyCode.Q))
            {
                transform.position += transform.up * climbSpeed * slowMoveFactor * fastMoveFactor * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.E))
            {
                transform.position -= transform.up * climbSpeed * slowMoveFactor * fastMoveFactor * Time.deltaTime;
            }
        }
    }
}