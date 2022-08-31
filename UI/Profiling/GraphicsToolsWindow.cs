using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Amplitude.Graphics;
using Amplitude.Graphics.Fx;
using Amplitude.Interop;
using Modding.Humankind.DevTools.DeveloperTools.UI;
using UnityEngine;
using UnityEngine.Profiling;
using Application = Amplitude.Framework.Application;
using Object = UnityEngine.Object;

namespace DevTools.Humankind.GUITools.UI
{
    public class GraphicsToolsWindow : FloatingToolWindow
    {
        public override string WindowTitle { get; set; } = "GRAPHICS TOOLS";

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

            OnDrawWindowContent();
        }

        protected void OnDrawWindowContent()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("GC collect"))
                GC.Collect();
            if (GUILayout.Button("Unload unused assets"))
                Resources.UnloadUnusedAssets();
            if (GUILayout.Button("Empty working set"))
                VirtualMemory.EmptyWorkingSet();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Texture"))
                ExportNames<Texture>(TextureFirstLine,
                    TextureAssetLine);
            if (GUILayout.Button("+", GUILayout.MaxWidth(20f)))
                ExportNames<Texture>(TextureFirstLine,
                    TextureAssetLine, "Compare");
            if (GUILayout.Button("Material"))
                ExportNames<Material>(DefaultFirstLine,
                    DefaultAssetLine);
            if (GUILayout.Button("+", GUILayout.MaxWidth(20f)))
                ExportNames<Material>(DefaultFirstLine,
                    DefaultAssetLine, "Compare");
            if (GUILayout.Button("Mesh"))
                ExportNames<Mesh>(DefaultFirstLine,
                    DefaultAssetLine);
            if (GUILayout.Button("+", GUILayout.MaxWidth(20f)))
                ExportNames<Mesh>(DefaultFirstLine,
                    DefaultAssetLine, "Compare");
            if (GUILayout.Button("AnimationClip"))
                ExportNames<AnimationClip>(DefaultFirstLine,
                    DefaultAssetLine);
            if (GUILayout.Button("+", GUILayout.MaxWidth(20f)))
                ExportNames<AnimationClip>(DefaultFirstLine,
                    DefaultAssetLine, "Compare");
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("FxOutputLayer"))
                ExportNames<FxOutputLayer>(DefaultFirstLine,
                    DefaultAssetLine);
            if (GUILayout.Button("+", GUILayout.MaxWidth(20f)))
                ExportNames<FxOutputLayer>(DefaultFirstLine,
                    DefaultAssetLine, "Compare");
            GUILayout.EndHorizontal();
            if (!GUILayout.Button("Open report folder"))
                return;
            Process.Start(new ProcessStartInfo
            {
                FileName = Application.DumpDirectory,
                UseShellExecute = true,
                Verb = "open"
            });
        }

        private void ExportNames<T>(
            FirstLine firstLine,
            GetAssetLine getAssetLine,
            string suffix = null)
            where T : Object
        {
            List<string> names = new List<string>();
            FillAssetNames<T>(names, getAssetLine);
            using (StreamWriter streamWriter = new StreamWriter(Path.Combine(
                Application.DumpDirectory,
                string.Format("All{0}{1}.csv", typeof(T),
                    suffix != null ? suffix : (object) string.Empty))))
            {
                streamWriter.WriteLine(firstLine());
                for (int index = 0; index < names.Count; ++index)
                    streamWriter.WriteLine(names[index]);
            }
        }

        private void FillAssetNames<T>(
            List<string> names,
            GetAssetLine getAssetLine)
            where T : Object
        {
            foreach (Object unityObject in Resources.FindObjectsOfTypeAll(typeof(T)))
                names.Add(getAssetLine(unityObject));
            names.Sort();
        }

        private string DefaultFirstLine() => "Name;Size(kb)";

        private string DefaultAssetLine(Object unityObject) => string.Format("{0};{1:000 000}",
            unityObject != null ? unityObject.name : (object) "null",
            Profiler.GetRuntimeMemorySizeLong(unityObject) / 1024f);

        private string TextureFirstLine() => "Name;Type;Size(kb);width;height;depth;format";

        private string TextureAssetLine(Object unityObject)
        {
            string name = unityObject.name;
            string str = string.Empty;
            float num1 = Profiler.GetRuntimeMemorySizeLong(unityObject) / 1024f;
            string empty = string.Empty;
            int num2 = 0;
            int num3 = 0;
            int num4 = 0;
            Texture2D texture2D = unityObject as Texture2D;
            if (texture2D != null)
            {
                str = "2D";
                num2 = texture2D.width;
                num3 = texture2D.height;
                num4 = 1;
                empty = texture2D.format.ToString();
            }

            Cubemap cubemap = unityObject as Cubemap;
            if (cubemap != null)
            {
                str = "CUBE";
                num2 = cubemap.width;
                num3 = cubemap.height;
                num4 = 6;
                empty = cubemap.format.ToString();
            }

            RenderTexture renderTexture = unityObject as RenderTexture;
            if (renderTexture != null)
            {
                str = "RT";
                num2 = renderTexture.width;
                num3 = renderTexture.height;
                num4 = 1;
                empty = renderTexture.format.ToString();
            }

            Texture2DArray texture2Darray = unityObject as Texture2DArray;
            TextureFormat format;
            if (texture2Darray != null)
            {
                str = "Array";
                num2 = texture2Darray.width;
                num3 = texture2Darray.height;
                num4 = texture2Darray.depth;
                format = texture2Darray.format;
                empty = format.ToString();
                float sizeInBytePerPixel;
                TextureFormatHelper.GetTextureFormatInfo(texture2Darray, out bool a, out bool b, out bool c,
                    out bool d, out sizeInBytePerPixel);
                int num5 = (int) (num4 * num2 * num3 * (double) sizeInBytePerPixel);
                int num6 = num2;
                int num7 = num3;
                for (int index = 0; index < texture2Darray.mipmapCount; ++index)
                {
                    num6 /= 2;
                    num7 /= 2;
                    num5 += (int) (num4 * num6 * num7 * (double) sizeInBytePerPixel);
                }

                num1 = num5 / 1024;
            }

            Texture3D texture3D = unityObject as Texture3D;
            if (texture3D != null)
            {
                str = "3D";
                num2 = texture3D.width;
                num3 = texture3D.height;
                num4 = texture3D.depth;
                format = texture3D.format;
                empty = format.ToString();
            }

            return string.Format("{0};{1};{2};{3};{4};{5};{6}", name, str,
                num1.ToString("0"), num2, num3, num4, empty);
        }

        public delegate string FirstLine();

        public delegate string GetAssetLine(Object unityObject);
    }
}
