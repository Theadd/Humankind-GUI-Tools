
using Amplitude.Framework.Overlay;
using System;
using UnityEngine;
using Modding.Humankind.DevTools.Core;
using Modding.Humankind.DevTools;
using Modding.Humankind.DevTools.DeveloperTools.UI;
using Amplitude.Framework.Profiling;

namespace DevTools.Humankind.GUITools.UI
{
    public class ProfilerToolWindow : FloatingToolWindow
    {
        private const string FrameTimePath = "General|Frame time (ms)";
        private const string MemoryAllocationPath = "Memory|Allocation (kB)";
        [SerializeField]
        private ProfilerView view = new ProfilerView();
        [SerializeField]
        private bool requestRecording;
        [NonSerialized]
        private int garbageCollectionCountLastFrame;
        [NonSerialized]
        private long heapUsedSizeLastFrame;

        public override string WindowTitle { get; set; } = "PROFILER TOOL";

        public override string WindowGUIStyle { get; set; } = "PopupWindow";

        public override bool ShouldBeVisible => true;

        public override bool ShouldRestoreLastWindowPosition => true;

        public override Rect WindowRect { get; set; } = new Rect(130f, 260f, 520f, 500f);

        public override void OnGUIStyling()
        {
            base.OnGUIStyling();
            GUI.backgroundColor = new Color32(255, 255, 255, 230);
        }

        public override void OnDrawUI()
        {
            WindowUtils.DrawWindowTitleBar(this);

            OnDrawWindowContent();
        }

        protected void OnDrawWindowContent()
        {
            GUILayout.BeginHorizontal((GUIStyle)"Widget.ClientArea");



            bool flag = GUILayout.Toggle(this.requestRecording, "Record", (GUIStyle)"PopupWindow.Button");
            if (flag != this.requestRecording)
            {
                if (flag)
                    ProfilerDataRecorder.RequestRecording();
                else
                    ProfilerDataRecorder.ReleaseRequestRecording();
                this.requestRecording = flag;
                this.view.Updating = this.requestRecording;
            }
            if (GUILayout.Button("Capture", (GUIStyle)"PopupWindow.Button"))
                this.view.Capture();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical(GUILayout.Height(600f));
            this.view.OnGUI(WindowRect.width);
            GUILayout.EndVertical();
        }

        private void Update()
        {
            this.LogMemoryCounters();
            this.LogPerformanceCounters();
            ProfilerDataRecorder.Update();
        }

        private void LogMemoryCounters()
        {
            long monoHeapSizeLong = UnityEngine.Profiling.Profiler.GetMonoHeapSizeLong();
            long monoUsedSizeLong = UnityEngine.Profiling.Profiler.GetMonoUsedSizeLong();
            double num1 = (double)monoUsedSizeLong / (double)monoHeapSizeLong;
            int num2 = GC.CollectionCount(0);
            int collectionCountLastFrame = this.garbageCollectionCountLastFrame;
            double num3 = (double)Mathf.Max((float)(monoUsedSizeLong - this.heapUsedSizeLastFrame) / 1024f, 0.0f);
            this.garbageCollectionCountLastFrame = num2;
            this.heapUsedSizeLastFrame = monoUsedSizeLong;
        }

        private void LogPerformanceCounters()
        {
        }

        private void OnEnable()
        {
            Loggr.Log("OnEnable_01", ConsoleColor.Green);
            if (this.requestRecording)
                ProfilerDataRecorder.RequestRecording();
            Loggr.Log("OnEnable_02", ConsoleColor.Green);

            this.view.Updating = this.requestRecording;
            Loggr.Log("OnEnable_03", ConsoleColor.Green);

            foreach (ProfilerDataRecorder allRecorder in ProfilerDataRecorder.AllRecorders)
            {
                Loggr.Log("\tOnEnable_04 IN LOOP", ConsoleColor.Green);
                this.view.Watch(allRecorder);
            }
            Loggr.Log("OnEnable_05", ConsoleColor.Green);

            ProfilerDataRecorder.RecorderCreated -= new Action<ProfilerDataRecorder>(this.RecorderCreated);
            ProfilerDataRecorder.RecorderCreated += new Action<ProfilerDataRecorder>(this.RecorderCreated);
            Loggr.Log("OnEnable_06", ConsoleColor.Green);

        }

        private void RecorderCreated(ProfilerDataRecorder recorder) => this.view.Watch(recorder);

        private void OnDisable()
        {
            ProfilerDataRecorder.RecorderCreated -= new Action<ProfilerDataRecorder>(this.RecorderCreated);
            if (!this.requestRecording)
                return;
            ProfilerDataRecorder.ReleaseRequestRecording();
        }
    }
}
