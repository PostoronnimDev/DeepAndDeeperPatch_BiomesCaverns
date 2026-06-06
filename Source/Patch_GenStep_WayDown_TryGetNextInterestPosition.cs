using HarmonyLib;
using Verse;

namespace DeepAndDeeperPatch_BiomesCaverns
{
    [HarmonyPatch(typeof(Shashlichnik.GenStep_WayDown), "CellValidator")]
    public static class Patch_GenStep_WayDown_TryGetNextInterestPosition
    {
        public static bool Prefix(
            Map map,
            IntVec3 c,
            ref bool __result)
        {
            if (map == null || !c.Standable(map))
            {
                __result = false;
                return false;
            }

            return true;
        }
    }
}
