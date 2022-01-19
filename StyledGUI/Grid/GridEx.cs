using System;
using DevTools.Humankind.GUITools.UI;
using Modding.Humankind.DevTools;
using UnityEngine;

namespace StyledGUI
{
    public static class StyledGridEx
    {
        public static IStyledGrid Row(this IStyledGrid self, GUIStyle style, params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal(style, options);

            return self;
        }
        
        public static IStyledGrid Row(this IStyledGrid self, params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal(Styles.RowStyle, options);

            return self;
        }

        public static IStyledGrid EndRow(this IStyledGrid self)
        {
            GUILayout.EndHorizontal();

            return self;
        }

        public static IStyledGrid EmptyRow(this IStyledGrid self, params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal(Styles.StaticRowStyle, options);
            GUILayout.Label(" ", Styles.RowHeaderStyle);
            GUILayout.EndHorizontal();

            return self;
        }
        
        public static IStyledGrid VerticalStack(this IStyledGrid self, params GUILayoutOption[] options)
        {
            GUILayout.BeginVertical(Styles.StaticRowStyle, options);

            return self;
        }

        public static IStyledGrid EndVerticalStack(this IStyledGrid self, params GUILayoutOption[] options)
        {
            GUILayout.EndVertical();

            return self;
        }

        public static IStyledGrid Cell(this IStyledGrid self, string text, GUIStyle style, params GUILayoutOption[] options)
        {
            GUILayout.Label(text, style, options);

            return self;
        }

        public static IStyledGrid Cell(this IStyledGrid self, string text, params GUILayoutOption[] options)
        {
            GUILayout.Label(text, Styles.CellStyle, options);

            return self;
        }
        
        public static IStyledGrid Cell(this IStyledGrid self, string text, Color color, params GUILayoutOption[] options)
        {
            var prevColor = GUI.backgroundColor;
            GUI.backgroundColor = color;

            GUILayout.Label("<size=11>" + text + "</size>", Styles.CellStyle, options);

            GUI.backgroundColor = prevColor;

            return self;
        }

        public static IStyledGrid Cell(this IStyledGrid self, string text, GUIStyle style, Color color, params GUILayoutOption[] options)
        {
            var prevColor = GUI.backgroundColor;
            GUI.backgroundColor = color;

            GUILayout.Label("<size=11>" + text + "</size>", style, options);

            GUI.backgroundColor = prevColor;

            return self;
        }

        public static IStyledGrid RowHeader(this IStyledGrid self, string text, GUIStyle style, params GUILayoutOption[] options)
        {
            GUILayout.Label("<size=10><b>" + text + "</b></size>", style, options);

            return self;
        }
        
        public static IStyledGrid RowHeader(this IStyledGrid self, string text, params GUILayoutOption[] options)
        {
            GUILayout.Label("<size=10>" + text + "</size>", Styles.RowHeaderStyle, options);

            return self;
        }

        
        public static IStyledGrid DrawHorizontalLine(this IStyledGrid self, float alpha = 0.3f)
        {   
            Graphics.DrawHorizontalLine(alpha);

            return self;
        }

        public static IStyledGrid DrawHorizontalLine(this IStyledGrid self, float alpha, float width)
        {   
            Graphics.DrawHorizontalLine(alpha, width);

            return self;
        }

        public static IStyledGrid Space(this IStyledGrid self, float size)
        {   
            GUILayout.Space(size);

            return self;
        }
    }

    public static class GameStatisticsGridEx
    {
        public static IStyledGrid Iterate<TDType>(
            this IStyledGrid self, 
            Action<TDType, int, int> action) where TDType : IDataType
        {
            switch (self)
            {
                case IGameStatisticsGrid gameStatisticsGrid:
                    return ((IGameStatisticsGrid)gameStatisticsGrid).Iterate<EmpireSnapshot>(action as Action<IEmpireSnapshotDataType, int, int>);
                default:
                    throw new NotImplementedException();
            }
        }
        
        public static IStyledGrid Iterate<TDType>(
            this IGameStatisticsGrid self, 
            Action<IEmpireSnapshotDataType, int, int> action) where TDType : IEmpireSnapshotDataType
        {
            for (var i = 0; i < self.DisplayOrder.Length; i++)
            {
                if (self.SpaceEmpireColumsBy != 0 && i != 0)
                    GUILayout.Space(self.SpaceEmpireColumsBy);
                self.CurrentIndex = i;
                self.CurrentEmpireIndex = self.DisplayOrder[i];
                action.Invoke(self.CurrentSnapshot.Empires[self.DisplayOrder[i]], i, self.CurrentEmpireIndex);
            }

            return self;
        }
        
        public static IStyledGrid Iterate<TDType>(
            this IStyledGrid self, 
            Action<TDType> action) where TDType : IDataType
        {
            switch (self)
            {
                case IGameStatisticsGrid gameStatisticsGrid:
                    return ((IGameStatisticsGrid)gameStatisticsGrid).Iterate<EmpireSnapshot>(action as Action<IEmpireSnapshotDataType>);
                default:
                    throw new NotImplementedException();
            }
        }
        
        public static IStyledGrid Iterate<T>(
            this IGameStatisticsGrid self, 
            Action<IEmpireSnapshotDataType> action) where T : IEmpireSnapshotDataType
        {
            for (var i = 0; i < self.DisplayOrder.Length; i++)
            {
                if (self.SpaceEmpireColumsBy != 0 && i != 0)
                    GUILayout.Space(self.SpaceEmpireColumsBy);
                self.CurrentIndex = i;
                self.CurrentEmpireIndex = self.DisplayOrder[i];
                action.Invoke(self.CurrentSnapshot.Empires[self.DisplayOrder[i]]);
            }
            
            return self;
        }

        public static IStyledGrid CellButton<T>(
            this IStyledGrid self, 
            string text, 
            Action<T, int, int> action, 
            params GUILayoutOption[] options) where T : IDataType
        {
            switch (self)
            {
                case IGameStatisticsGrid gameStatisticsGrid:
                    return ((IGameStatisticsGrid)gameStatisticsGrid).CellButton<EmpireSnapshot>(text, action as Action<IEmpireSnapshotDataType, int, int>, options);
                default:
                    throw new NotImplementedException();
            }
        }
        
        public static IStyledGrid CellButton<TDType>(
            this IGameStatisticsGrid self, 
            string text, 
            Action<IEmpireSnapshotDataType, int, int> action, 
            params GUILayoutOption[] options) where TDType : IEmpireSnapshotDataType
        {
            var empireIndex = self.CurrentEmpireIndex;
            var index = self.CurrentIndex;
            var prevBgTint = GUI.backgroundColor;
            // GUI.backgroundColor = CellButtonTintColor; 
            GUI.backgroundColor = Color.white;
            if (GUILayout.Button(text, Styles.CellButtonStyle, options))
            {
                action.Invoke(self.CurrentSnapshot.Empires[empireIndex], index, empireIndex);
                HumankindGame.Update();
                // CurrentSnapshot.Snapshot();
                GameStatsWindow.ResetLoop();
            }
            GUI.backgroundColor = prevBgTint;

            return self;
        }
    }
}