using System;
using UnityEngine;
using StyledGUI;

namespace DevTools.Humankind.GUITools.UI
{
    public static class GameOverviewGridEx
    {
        /*public static IStyledGrid Iterate<TDType>(
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
        
        */

        public static IStyledGrid Iterate<TDType>(
            this IGameStatisticsGrid self, 
            Action<EmpireSnapshot, int, int> action) where TDType : IEmpireSnapshotDataType
        {
            for (var i = 0; i < self.DisplayOrder.Length; i++)
            {
                if (self.SpaceEmpireColumnsBy != 0 && i != 0)
                    GUILayout.Space(self.SpaceEmpireColumnsBy);
                self.CurrentIndex = i;
                self.CurrentEmpireIndex = self.DisplayOrder[i];
                action.Invoke(self.CurrentSnapshot.Empires[self.DisplayOrder[i]], i, self.CurrentEmpireIndex);
            }

            return self;
        }

        public static IStyledGrid Iterate<T>(
            this IGameStatisticsGrid self, 
            Action<EmpireSnapshot> action) where T : IEmpireSnapshotDataType
        {
            for (var i = 0; i < self.DisplayOrder.Length; i++)
            {
                if (self.SpaceEmpireColumnsBy != 0 && i != 0)
                    GUILayout.Space(self.SpaceEmpireColumnsBy);
                self.CurrentIndex = i;
                self.CurrentEmpireIndex = self.DisplayOrder[i];
                action.Invoke(self.CurrentSnapshot.Empires[self.DisplayOrder[i]]);
            }
            
            return self;
        }

        /*public static IStyledGrid CellButton<T>(
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
        }*/
        
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
                Modding.Humankind.DevTools.HumankindGame.Update();
                // CurrentSnapshot.Snapshot();
                GameStatsWindow.ResetLoop();
            }
            GUI.backgroundColor = prevBgTint;

            return self;
        }
    }
}