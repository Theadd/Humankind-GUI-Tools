using System.Linq;
using System;
using System.Collections.Generic;
using Modding.Humankind.DevTools.Core;
using Amplitude.Mercury.Interop.AI.Entities;
using Amplitude.Mercury.Interop;

namespace DevTools.Humankind.GUITools.UI.ArmyTools
{

    public static class ArmyUtils
    {
        public static string ArmyUnitNames(Army army) => string.Join(", ", army.Units.Select(UnitName)
            .GroupBy(name => name).Select(g => (g.Count() > 1 ? StyledUI.Tag.Hot(g.Count() + "x") : "") 
                + g.Key.ToUpper()).ToArray());

        public static string UnitName(Amplitude.Mercury.Interop.AI.Data.Unit unit) => (ToolSettings.Instance.LocalizedTitles.Value ? 
            R.Text.GetLocalizedTitle(unit.UnitDefinition.Name) : unit.UnitDefinition.Name.ToString().Split('_').LastOrDefault());

        public static string ArmyTags(Army army) {
            List<string> res = new List<string>();

            if ((float) army.PathfindContext.MovementRatio >= 0.5f && army.BattleIndex < 0)
                res.Add(StyledUI.Tag.Success("READY"));

            if (army.State != Amplitude.Mercury.Simulation.ArmyState.Idle)
                res.Add(StyledUI.Tag.State(army.State.ToString()));

            if (army.AutoExplore) res.Add(StyledUI.Tag.Common("AUTO"));

            if (army.SpawnType != Amplitude.Mercury.Data.Simulation.UnitSpawnType.Land)
                res.Add(StyledUI.Tag.Class(army.SpawnType.ToString()));

            if (army.BattleIndex >= 0) res.Add(StyledUI.Tag.Warn("BATTLE"));

            return res.Count > 0 ? string.Join(StyledUI.Tag.Separator, res) : "";
        }

        private static string row(string name, string value) => 
            name.ToUpper() + ": " + R.Text.Color(value, "ACAC77FF");

        public static string SummaryOfSelectedArmy(Army army)
        {
            List<string> res;

            try {
                var data = Snapshots.ArmyCursorSnapshot.PresentationData;
                Amplitude.Mercury.Simulation.PathfindContext ctx = army.PathfindContext;
                res = new List<string>() {
                    "<size=7>\n</size>" + row("Name", data.EntityName.ToString()),
                    row("State", data.ArmyState.ToString() + " / " + data.AwakeState.ToString()), 
                    row("Movement", ArmyMovementPoints(army)),
                    row("Vision Range", ((int)data.VisionRange).ToString()),
                    "<size=5>\n\n</size>" + row("Detection Range", ((int)data.DetectionRange).ToString()),
                    row("Attack Range", ((int)data.DetectionRange).ToString()),
                    row("Combat Strength", ((float)data.DetectionRange).ToString()),
                    "<size=5>\n\n</size>" + row("Units", data.NumberOfUnits + " / " + data.ArmyMaximumSize),
                    row("Health", ((int)((float)data.HealthRatio * (float)data.HitPoints)).ToString() + " / " + ((int)data.HitPoints).ToString()),
                    row("Health Regen", "+" + ((int)((float)data.HealthRegen * (float)data.HitPoints)).ToString() + " / Turn"),
                    row("Upkeep", ((int)data.Upkeep).ToString()),
                    "\n",
                };
            }
            catch (Exception e) {
                return "";
            }

            return string.Join(StyledUI.Tag.Separator, res);
        }

        public static string ArmyMovementPoints(Army army) =>
            "" + ((int)(ArmyMovementRatio(army) * army.MovementSpeed)) + " / " + ((int)army.MovementSpeed);

        public static float ArmyMovementRatio(Army army) => (float) army.PathfindContext.MovementRatio;

        public static bool IsRunning(Army army) => army.GoToActionStatus == Army.ActionStatus.Running;
    }


}
