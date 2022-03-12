using System;
using Amplitude.Extensions;
using DevTools.Humankind.GUITools.UI;
using Modding.Humankind.DevTools;
using UnityEngine;

public class WireRenderer : MonoBehaviour
{
    private static WireRenderer _instance = null;
    
    void OnPreRender()
    {
        GL.wireframe = true;
    }

    void OnPostRender()
    {
        GL.wireframe = false;
    }

    public static void Attach(Camera camera = null)
    {
        if (camera == null)
            camera = ActionController.ImpostorCamera;

        if (camera != null)
        {
            Detach();
            _instance = camera.gameObject.AddComponent<WireRenderer>();
            Loggr.Log(_instance);
            Loggr.Log(_instance.gameObject);
            Loggr.Log(_instance.gameObject.GetPath());
            Loggr.Log("ATTACHED", ConsoleColor.Blue);
        }
    }

    public static void Detach()
    {
        if (_instance != null)
        {
            Destroy(_instance);
            _instance = null;
            Loggr.Log("DETACHED", ConsoleColor.Red);
        }
    }
}
