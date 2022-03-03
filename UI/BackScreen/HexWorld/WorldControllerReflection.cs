using System;
using System.Reflection;
using Amplitude;
using Amplitude.Mercury;
using UniverseLib;
using UniverseLib.Utility;
using UniverseLib.Runtime.Mono;

namespace DevTools.Humankind.GUITools.UI
{
    public static partial class WorldController
    {

        private static MethodInfo AsWorldMethod(string name, Type[] parameters = null) =>
            typeof(Amplitude.Mercury.Simulation.World).GetMethod(name, ReflectionUtility.FLAGS, null,
                parameters ?? new Type[] { }, null);

        private static MethodInfo _destroyTerrainAt =
            AsWorldMethod("DestroyTerrainAt", new[] {typeof(int), typeof(bool)});
        
        private static MethodInfo _getTerrainTypeIndex =
            AsWorldMethod("GetTerrainTypeIndex", new[] {typeof(StaticString)});
        
        private static MethodInfo _findResourceDepositDefinitionAt =
            AsWorldMethod("FindResourceDepositDefinitionAt", new[] {typeof(WorldPosition)});

    }
}
// new[] {typeof(FixedPoint)}
