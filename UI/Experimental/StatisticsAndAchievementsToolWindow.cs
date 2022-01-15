using Amplitude.Framework;
using Amplitude.Framework.Achievements;
using Amplitude.Framework.Overlay;
using Amplitude;
using System;
using UnityEngine;
using Modding.Humankind.DevTools.Core;
using Modding.Humankind.DevTools.DeveloperTools.UI;

namespace DevTools.Humankind.GUITools.UI
{
    public class StatisticsAndAchievementsToolWindow : FloatingToolWindow
    {
        public override string WindowTitle { get; set; } = "STATISTICS AND ACHIEVEMENTS TOOL";

        public override string WindowGUIStyle { get; set; } = "PopupWindow";

        public override bool ShouldBeVisible => !GlobalSettings.ShouldHideTools;

        public override bool ShouldRestoreLastWindowPosition => true;

        public override Rect WindowRect { get; set; } = new Rect(130f, 260f, 720f, 500f);

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

            OnDrawWindowClientArea(0);
        }
    private static float statisticNameLabelWidth = 150f;
    private static float valueLabelWidth = 70f;
    private static float statisticTypeLabelWidth = 70f;
    private static string[] commitModeNames = Enum.GetNames(typeof (CommitMode));
    private bool displayStatistics;
    private bool displayAchievements;
    private Vector2 statisticScrollPosition;
    private Vector2 achievementScrollPosition;

    protected override void OnDrawWindowClientArea(int instanceId)
    {
      GUI.color = Color.white;
      GUI.backgroundColor = Color.white;
      using (new GUILayout.VerticalScope((GUIStyle) "Widget.ClientArea", new GUILayoutOption[1]
      {
        GUILayout.ExpandWidth(true)
      }))
      {
        IStatisticsAndAchievementsService service = Services.GetService<IStatisticsAndAchievementsService>();
        if (service == null)
        {
          GUILayout.Label("Waiting for statistics and achievements service.");
        }
        else
        {
          using (new GUILayout.HorizontalScope(Array.Empty<GUILayoutOption>()))
          {
            CommitMode commitMode = service.CommitMode;
            int num = GUILayout.SelectionGrid((int) commitMode, StatisticsAndAchievementsToolWindow.commitModeNames, StatisticsAndAchievementsToolWindow.commitModeNames.Length);
            if ((CommitMode) num != commitMode)
              service.SetCommitMode((CommitMode) num);
            GUI.enabled = commitMode == CommitMode.Manual;
            if (GUILayout.Button("Commit", GUILayout.Width(StatisticsAndAchievementsToolWindow.valueLabelWidth)))
              service.Commit();
            GUI.enabled = true;
          }
          this.DisplayStatistics(service);
          this.DisplayAchievement(service);
          GUILayout.FlexibleSpace();
        }
      }
    }

    private void DisplayAchievement(
      IStatisticsAndAchievementsService statisticsAndAchievementsService)
    {
      using (new GUILayout.HorizontalScope(Array.Empty<GUILayoutOption>()))
      {
        this.displayAchievements = GUILayout.Toggle((this.displayAchievements ? 1 : 0) != 0, "Display achievements?", GUILayout.Width(StatisticsAndAchievementsToolWindow.statisticNameLabelWidth));
        GUI.enabled = statisticsAndAchievementsService.CanResetAchievements;
        if (GUILayout.Button("Reset all"))
          statisticsAndAchievementsService.ResetAllAchievements();
        GUI.enabled = true;
        GUILayout.FlexibleSpace();
      }
      if (!this.displayAchievements)
        return;
      using (new GUILayout.HorizontalScope(Array.Empty<GUILayoutOption>()))
      {
        GUILayout.Label("Achievement", GUILayout.Width(StatisticsAndAchievementsToolWindow.statisticNameLabelWidth));
        GUILayout.Label("Unlocked", GUILayout.Width(StatisticsAndAchievementsToolWindow.valueLabelWidth));
        GUILayout.Label("Statistic", GUILayout.Width(StatisticsAndAchievementsToolWindow.statisticNameLabelWidth));
        GUILayout.Label("Threshold", GUILayout.Width(StatisticsAndAchievementsToolWindow.valueLabelWidth));
        GUILayout.Label("Value", GUILayout.Width(StatisticsAndAchievementsToolWindow.valueLabelWidth));
        GUILayout.FlexibleSpace();
      }
      this.achievementScrollPosition = GUILayout.BeginScrollView(this.achievementScrollPosition);
      foreach (Tuple<AchievementDefinition, bool> enumerateAchievement in statisticsAndAchievementsService.EnumerateAchievements())
      {
        AchievementDefinition achievementDefinition = enumerateAchievement.Item1;
        bool flag = enumerateAchievement.Item2;
        using (new GUILayout.HorizontalScope(Array.Empty<GUILayoutOption>()))
        {
          GUILayout.Label(achievementDefinition.AchievementName, GUILayout.Width(StatisticsAndAchievementsToolWindow.statisticNameLabelWidth));
          GUILayout.Label(flag.ToString(), GUILayout.Width(StatisticsAndAchievementsToolWindow.valueLabelWidth));
          GUILayout.Label(achievementDefinition.StatisticName, GUILayout.Width(StatisticsAndAchievementsToolWindow.statisticNameLabelWidth));
          GUILayout.Label(achievementDefinition.Threshold.ToString(), GUILayout.Width(StatisticsAndAchievementsToolWindow.valueLabelWidth));
          GUILayout.Label(statisticsAndAchievementsService.GetStatistic(new StaticString(achievementDefinition.StatisticName)).ToString(), GUILayout.Width(StatisticsAndAchievementsToolWindow.valueLabelWidth));
          GUI.enabled = statisticsAndAchievementsService.CanResetAchievements;
          if (GUILayout.Button("Reset", GUILayout.Width(StatisticsAndAchievementsToolWindow.valueLabelWidth)))
            statisticsAndAchievementsService.ResetAchievement(achievementDefinition.Name);
          GUI.enabled = true;
          if (GUILayout.Button("=Threshold", GUILayout.Width(StatisticsAndAchievementsToolWindow.valueLabelWidth)))
            statisticsAndAchievementsService.SetStatistic(new StaticString(achievementDefinition.StatisticName), achievementDefinition.Threshold, SetStatisticMode.Set);
          GUILayout.FlexibleSpace();
        }
      }
      GUILayout.EndScrollView();
    }

    private void DisplayStatistics(
      IStatisticsAndAchievementsService statisticsAndAchievementsService)
    {
      using (new GUILayout.HorizontalScope(Array.Empty<GUILayoutOption>()))
      {
        this.displayStatistics = GUILayout.Toggle((this.displayStatistics ? 1 : 0) != 0, "Display statistics?", GUILayout.Width(StatisticsAndAchievementsToolWindow.statisticNameLabelWidth));
        GUI.enabled = statisticsAndAchievementsService.CanResetStatistics;
        if (GUILayout.Button("Reset all"))
          statisticsAndAchievementsService.ResetAllStatistics();
        GUI.enabled = true;
        GUILayout.FlexibleSpace();
      }
      if (!this.displayStatistics)
        return;
      using (new GUILayout.HorizontalScope(Array.Empty<GUILayoutOption>()))
      {
        GUILayout.Label("Statistic", GUILayout.Width(StatisticsAndAchievementsToolWindow.statisticNameLabelWidth));
        GUILayout.Label("Value", GUILayout.Width(StatisticsAndAchievementsToolWindow.valueLabelWidth));
        GUILayout.Label("Type", GUILayout.Width(StatisticsAndAchievementsToolWindow.statisticTypeLabelWidth));
        GUILayout.FlexibleSpace();
      }
      this.statisticScrollPosition = GUILayout.BeginScrollView(this.statisticScrollPosition);
      foreach (Tuple<StatisticDefinition, double> enumerateStatistic in statisticsAndAchievementsService.EnumerateStatistics())
      {
        StatisticDefinition statisticDefinition = enumerateStatistic.Item1;
        double num = enumerateStatistic.Item2;
        string text = statisticDefinition.StatisticType.ToString();
        using (new GUILayout.HorizontalScope(Array.Empty<GUILayoutOption>()))
        {
          GUILayout.Label(statisticDefinition.StatisticName, GUILayout.Width(StatisticsAndAchievementsToolWindow.statisticNameLabelWidth));
          GUILayout.Label(num.ToString(), GUILayout.Width(StatisticsAndAchievementsToolWindow.valueLabelWidth));
          GUILayout.Label(text, GUILayout.Width(StatisticsAndAchievementsToolWindow.statisticTypeLabelWidth));
          if (GUILayout.Button("Reset", GUILayout.Width(StatisticsAndAchievementsToolWindow.valueLabelWidth)))
            statisticsAndAchievementsService.SetStatistic(statisticDefinition.Name, 0.0, SetStatisticMode.Set);
          if (GUILayout.Button("+1", GUILayout.Width(StatisticsAndAchievementsToolWindow.valueLabelWidth)))
            statisticsAndAchievementsService.SetStatistic(statisticDefinition.Name, 1.0);
          if (GUILayout.Button("+10", GUILayout.Width(StatisticsAndAchievementsToolWindow.valueLabelWidth)))
            statisticsAndAchievementsService.SetStatistic(statisticDefinition.Name, 10.0);
          if (GUILayout.Button("+1000", GUILayout.Width(StatisticsAndAchievementsToolWindow.valueLabelWidth)))
            statisticsAndAchievementsService.SetStatistic(statisticDefinition.Name, 1000.0);
          if (GUILayout.Button("=1000", GUILayout.Width(StatisticsAndAchievementsToolWindow.valueLabelWidth)))
            statisticsAndAchievementsService.SetStatistic(statisticDefinition.Name, 1000.0, SetStatisticMode.Set);
          GUILayout.FlexibleSpace();
        }
      }
      GUILayout.EndScrollView();
    }
  }
}
