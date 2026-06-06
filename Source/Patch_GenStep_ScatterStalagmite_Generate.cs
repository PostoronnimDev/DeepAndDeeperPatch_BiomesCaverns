using BiomesCaverns.GenSteps;
using HarmonyLib;
using System;
using System.Runtime.CompilerServices;
using Verse;

namespace DeepAndDeeperPatch_BiomesCaverns
{
    [HarmonyPatch(typeof(GenStep_ScatterStalagmite), "Generate")]
    public static class Patch_GenStep_ScatterStalagmite
    {
        private const string extraBiome = "ShashlichnikUnderground";

        public static bool Prefix(GenStep_ScatterStalagmite __instance, Map map, GenStepParams parms)
        {
            if (map.Biome?.defName != extraBiome)
                return true;

            GenerateScatterGroup(__instance, map, parms);
            return false;
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(GenStep_ScatterGroup), "Generate")]
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void GenerateScatterGroup(GenStep_ScatterGroup instance, Map map, GenStepParams parms)
        {
            throw new NotImplementedException("Harmony reverse patch stub");
        }
    }
}
