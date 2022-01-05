/*using Amplitude.Framework.Overlay;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using Amplitude.Graphics.Profiling;
using Modding.Humankind.DevTools.Core;
using Modding.Humankind.DevTools.DeveloperTools.UI;

namespace DevTools.Humankind.GUITools.UI
{
    public class GPUProfilerToolWindow : FloatingToolWindow
    {
        public List<GPUProfiler.SampleResult> sampleResults = new List<GPUProfiler.SampleResult>();
        public List<GPUProfiler.SampleResult> indentStack = new List<GPUProfiler.SampleResult>();
        public List<GPUProfilerToolWindow.SampleHistoryDrawCommand> sampleHistoryDrawCommands = new List<GPUProfilerToolWindow.SampleHistoryDrawCommand>();
        public Dictionary<GPUProfilerToolWindow.SampleHistoryKey, GPUProfilerToolWindow.SampleHistory> sampleHistories = new Dictionary<GPUProfilerToolWindow.SampleHistoryKey, GPUProfilerToolWindow.SampleHistory>();
        public bool recording;
        public ulong processorFrequency;
        public int lastRetrievedFrameIndex;

        public override string WindowTitle { get; set; } = "GPU PROFILER";

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

            gpuProfilerIsSupportedField.SetValue(null, true);

            OnDrawWindowContent();
        }

        public static readonly FieldInfo gpuProfilerIsSupportedField = 
            typeof(GPUProfiler).GetField("gpuProfilerIsSupported", R.NonPublicStatic);

        protected void OnDrawWindowContent()
        {
            GUI.color = Color.white;
            GUI.backgroundColor = Color.white;
            GUILayout.BeginVertical((GUIStyle)"Widget.ClientArea", GUILayout.ExpandWidth(true));
            if (!GPUProfiler.IsSupported || !GPUProfiler.Loaded)
            {
                if (!GPUProfiler.IsSupported)
                    GUILayout.Label("GPU profiler is not supported");
                if (GPUProfiler.Loaded)
                    return;
                GUILayout.Label("GPU profiler is not loaded");
            }
            else
            {
                this.DrawToolbar();
                using (new GUILayout.VerticalScope())
                {
                    foreach (GPUProfilerToolWindow.SampleHistoryDrawCommand historyDrawCommand in this.sampleHistoryDrawCommands.Distinct<GPUProfilerToolWindow.SampleHistoryDrawCommand>())
                    {
                        GPUProfilerToolWindow.SampleHistory sampleHistory = historyDrawCommand.SampleHistory;
                        float num1 = sampleHistory.Average();
                        float captured = sampleHistory.Captured;
                        string eventName = sampleHistory.EventName;
                        string text1 = string.Format("{0:000.000} | {1:000.000}", (object)(float)((double)captured * 1000.0), (object)(float)((double)num1 * 1000.0));
                        float num2 = num1 / captured;
                        string text2;
                        if ((double)captured != 0.0)
                        {
                            text2 = string.Format(" (x{0:0.00})", (object)num2);
                        }
                        else
                        {
                            text2 = "  (N/A)";
                            num2 = 1f;
                        }
                        GUILayoutOption guiLayoutOption = GUILayout.Height(20f);
                        using (new GUILayout.HorizontalScope(new GUILayoutOption[1]
                        {
              guiLayoutOption
                        }))
                        {
                            GUILayout.Space((float)(historyDrawCommand.Indent * 12));
                            if (GUILayout.Button(sampleHistory.Foldout ? " ▼ " : " ▶ ", (GUIStyle)"PopupMenu.Item", guiLayoutOption))
                                sampleHistory.Foldout = !sampleHistory.Foldout;
                            GUILayout.Label(eventName, guiLayoutOption);
                            GUILayout.FlexibleSpace();
                            GUILayout.Label(text1, guiLayoutOption);
                            if ((double)num2 > 1.04999995231628)
                                GUI.color = Color.red;
                            else if ((double)num2 < 0.949999988079071)
                                GUI.color = Color.green;
                            GUILayout.Label(text2, guiLayoutOption);
                            GUI.color = Color.white;
                        }
                    }
                }
                GUILayout.EndVertical();
            }
        }

        protected override void OnBecomeVisible()
        {
            base.OnBecomeVisible();
            GPUProfiler.Load();
        }

        public void DrawToolbar()
        {
            using (new GUILayout.HorizontalScope(Array.Empty<GUILayoutOption>()))
            {
                if (GUILayout.Button(this.recording ? "Stop" : "Record", (GUIStyle)"PopupWindow.ToolbarButton"))
                {
                    this.recording = !this.recording;
                    if (this.recording)
                        GPUProfiler.RequestRecording();
                    else
                        GPUProfiler.ReleaseRequestRecording();
                }
                if (GUILayout.Button("Capture", (GUIStyle)"PopupWindow.ToolbarButton"))
                {
                    foreach (KeyValuePair<GPUProfilerToolWindow.SampleHistoryKey, GPUProfilerToolWindow.SampleHistory> sampleHistory in this.sampleHistories)
                        sampleHistory.Value.Capture();
                }
                GUILayout.Label(GPUProfiler.CurrentLogLevel.ToString(), GUILayout.Width(80f));
                if (GUILayout.Button("+", (GUIStyle)"PopupWindow.ToolbarButton"))
                    ++GPUProfiler.CurrentLogLevel;
                if (GUILayout.Button("-", (GUIStyle)"PopupWindow.ToolbarButton"))
                    --GPUProfiler.CurrentLogLevel;
                GUILayout.FlexibleSpace();
            }
        }

        public void Update()
        {
            this.processorFrequency = GPUProfiler.ProcessorFrequency();
            GPUProfiler.UpdateProfilerSampleResults();
            int frameIndex = this.lastRetrievedFrameIndex;
            if (this.recording)
            {
                this.sampleResults.Clear();
                frameIndex = GPUProfiler.GetProfilerSampleResults(this.sampleResults);
                this.sampleResults.Sort((IComparer<GPUProfiler.SampleResult>)new GPUProfilerToolWindow.SampleResultSorter());
            }
            bool flag = frameIndex != this.lastRetrievedFrameIndex && this.recording;
            this.lastRetrievedFrameIndex = frameIndex;
            this.indentStack.Clear();
            this.sampleHistoryDrawCommands.Clear();
            int num = int.MaxValue;
            foreach (GPUProfiler.SampleResult sampleResult in this.sampleResults)
            {
                float duration = (float)(sampleResult.End - sampleResult.Start) / (float)this.processorFrequency;
                while (this.indentStack.Count != 0)
                {
                    GPUProfiler.SampleResult indent = this.indentStack[this.indentStack.Count - 1];
                    if (sampleResult.Start >= indent.End)
                        this.indentStack.RemoveAt(this.indentStack.Count - 1);
                    else
                        break;
                }
                List<int> callstackEventIds = new List<int>();
                foreach (GPUProfiler.SampleResult indent in this.indentStack)
                    callstackEventIds.Add((int)indent.ClassId);
                callstackEventIds.Add((int)sampleResult.ClassId);
                GPUProfilerToolWindow.SampleHistoryKey key = new GPUProfilerToolWindow.SampleHistoryKey(callstackEventIds);
                GPUProfilerToolWindow.SampleHistory sampleHistory;
                if (!this.sampleHistories.TryGetValue(key, out sampleHistory))
                {
                    sampleHistory = new GPUProfilerToolWindow.SampleHistory(GPUProfiler.SampleEventName(sampleResult));
                    this.sampleHistories.Add(key, sampleHistory);
                }
                if (flag)
                    sampleHistory.AddSample(duration, frameIndex);
                int count = this.indentStack.Count;
                if (count < num)
                {
                    this.sampleHistoryDrawCommands.Add(new GPUProfilerToolWindow.SampleHistoryDrawCommand(sampleHistory, count));
                    num = sampleHistory.Foldout ? int.MaxValue : count + 1;
                }
                this.indentStack.Add(sampleResult);
            }
        }

        [StructLayout(LayoutKind.Sequential, Size = 1)]
        public struct SampleResultSorter : IComparer<GPUProfiler.SampleResult>
        {
            public int Compare(GPUProfiler.SampleResult x, GPUProfiler.SampleResult y)
            {
                int num = x.Start.CompareTo(y.Start);
                return num == 0 ? y.End.CompareTo(x.End) : num;
            }
        }

        public struct SampleHistoryKey : IEquatable<GPUProfilerToolWindow.SampleHistoryKey>
        {
            public readonly int[] callstackEventIds;

            public SampleHistoryKey(List<int> callstackEventIds) => this.callstackEventIds = callstackEventIds.ToArray();

            public override int GetHashCode()
            {
                int num = 0;
                foreach (int callstackEventId in this.callstackEventIds)
                    num ^= callstackEventId;
                return num;
            }

            public bool Equals(GPUProfilerToolWindow.SampleHistoryKey other)
            {
                if (this.callstackEventIds.Length != other.callstackEventIds.Length)
                    return false;
                int length = this.callstackEventIds.Length;
                for (int index = 0; index < length; ++index)
                {
                    if (this.callstackEventIds[index] != other.callstackEventIds[index])
                        return false;
                }
                return true;
            }
        }

        public struct SampleHistoryDrawCommand : IEquatable<GPUProfilerToolWindow.SampleHistoryDrawCommand>
        {
            public readonly GPUProfilerToolWindow.SampleHistory SampleHistory;
            public readonly int Indent;

            public SampleHistoryDrawCommand(GPUProfilerToolWindow.SampleHistory sampleHistory, int indent)
            {
                this.SampleHistory = sampleHistory;
                this.Indent = indent;
            }

            public bool Equals(GPUProfilerToolWindow.SampleHistoryDrawCommand other) => this.SampleHistory == other.SampleHistory && this.Indent == other.Indent;
        }

        public class SampleHistory
        {
            public const int Capacity = 180;
            public readonly string eventName;
            public float[] durations = new float[180];
            public int nextIndex;
            public int lastFrameIndex;
            public float lastDuration;
            public float capturedDuration;
            public bool foldout;

            public SampleHistory(string eventName) => this.eventName = eventName;

            public string EventName => this.eventName;

            public float Last => this.lastDuration;

            public float Captured => this.capturedDuration;

            public bool Foldout
            {
                get => this.foldout;
                set => this.foldout = value;
            }

            public void AddSample(float duration, int frameIndex)
            {
                if (frameIndex > this.lastFrameIndex)
                {
                    this.lastDuration = duration;
                    ++this.nextIndex;
                    this.durations[this.nextIndex % this.durations.Length] = duration;
                    this.lastFrameIndex = frameIndex;
                }
                else
                {
                    this.lastDuration += duration;
                    this.durations[this.nextIndex % this.durations.Length] += duration;
                }
            }

            public float Average()
            {
                int num1 = Math.Min(this.nextIndex, this.durations.Length);
                if (num1 <= 0)
                    return 0.0f;
                float num2 = 0.0f;
                for (int index = 0; index < num1; ++index)
                    num2 += this.durations[index];
                return num2 / (float)num1;
            }

            public void Capture() => this.capturedDuration = this.Average();
        }
    }
}
*/