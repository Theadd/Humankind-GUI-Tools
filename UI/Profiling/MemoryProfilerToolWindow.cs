using Amplitude.Framework.Overlay;
using System;
using System.Diagnostics;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using Amplitude.Framework.Profiling;
using Modding.Humankind.DevTools.Core;
using Modding.Humankind.DevTools.DeveloperTools.UI;

namespace DevTools.Humankind.GUITools.UI
{
    public class MemoryProfilerToolWindow : FloatingToolWindow
    {
        public override string WindowTitle { get; set; } = "MEMORY PROFILER";

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


        private ManagedMemoryAnalyzer memoryAnalyzer;
        private StringBuilder stringBuilder = new StringBuilder();
        private string lastSavedReportFilename = string.Empty;
        private bool capturing;

        protected void OnDrawWindowContent()
        {
            GUILayout.BeginHorizontal((GUIStyle)"Widget.ClientArea");
            if (GUILayout.Button("Capture", (GUIStyle)"PopupWindow.Button"))
            {
                this.CaptureMemoryDump();
                this.lastSavedReportFilename = string.Empty;
            }
            int num = GUI.enabled ? 1 : 0;
            GUI.enabled = !this.capturing && this.memoryAnalyzer != null;
            if (GUILayout.Button("Save Reports", (GUIStyle)"PopupWindow.Button"))
            {
                this.SaveMemoryDumpReport();
                this.SaveMemoryDumpReportCSV();
            }
            GUI.enabled = !this.capturing && !string.IsNullOrEmpty(this.lastSavedReportFilename);
            if (GUILayout.Button("Open Folder", (GUIStyle)"PopupWindow.Button"))
                this.OpenMemoryDumpFolder();
            GUI.enabled = num != 0;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.BeginVertical((GUIStyle)"Widget.ClientArea");
            this.stringBuilder.Length = 0;
            if (this.memoryAnalyzer != null)
            {
                this.stringBuilder.AppendFormat("Instances = {0}", (object)this.memoryAnalyzer.TotalInstanceCount);
                this.stringBuilder.AppendFormat("\nReferences = {0}", (object)this.memoryAnalyzer.TotalReferenceCount);
                this.stringBuilder.AppendLine();
            }
            else
                this.stringBuilder.Append("Data not available. Please capture. This task can take several minutes to complete.");
            GUILayout.Label(this.stringBuilder.ToString());
            GUILayout.EndVertical();
        }

        private void OpenMemoryDumpFolder() => Process.Start(new ProcessStartInfo()
        {
            FileName = Amplitude.Framework.Application.DumpDirectory,
            UseShellExecute = true,
            Verb = "open"
        });

        private void SaveMemoryDumpReport()
        {
            if (this.memoryAnalyzer == null)
                return;
            this.lastSavedReportFilename = System.IO.Path.Combine(Amplitude.Framework.Application.DumpDirectory, string.Format("{0:yyyy-MM-dd_hh-mm-ss}_memreport.txt", (object)DateTime.Now));
            this.memoryAnalyzer.WriteReportToFile(this.lastSavedReportFilename);
        }

        private void SaveMemoryDumpReportCSV()
        {
            if (this.memoryAnalyzer == null)
                return;
            this.lastSavedReportFilename = System.IO.Path.Combine(Amplitude.Framework.Application.DumpDirectory, string.Format("{0:yyyy-MM-dd_hh-mm-ss}_memreport.csv", (object)DateTime.Now));
            this.memoryAnalyzer.WriteReportToFile(this.lastSavedReportFilename);
        }

        private void CaptureMemoryDump()
        {
            this.memoryAnalyzer = new ManagedMemoryAnalyzer();
            GC.Collect();
            int sceneCount = SceneManager.sceneCount;
            for (int index = 0; index < sceneCount; ++index)
                this.memoryAnalyzer.StartAnalyze((object)SceneManager.GetSceneAt(index));
            WeakReference[] objects;
            Profiler.GetObjectsForMemoryAnalysis(out objects);
            foreach (WeakReference weakReference in objects)
            {
                if (weakReference.IsAlive)
                    this.memoryAnalyzer.StartAnalyze(weakReference.Target);
            }
            this.capturing = true;
            GC.Collect();
        }

        private void Update()
        {
            if (this.memoryAnalyzer == null)
                return;
            this.capturing = !this.memoryAnalyzer.UpdateAnalyze();
        }
    }
}
