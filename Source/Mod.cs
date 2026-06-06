using HarmonyLib;
using System.Linq;
using System.Reflection;
using Verse;

namespace DeepAndDeeperPatch_BiomesCaverns
{
    [StaticConstructorOnStartup]
    internal class Mod : Verse.Mod
    {
        static Mod()
        {
            var harmony = new Harmony("postoronnim.deepanddeeperpatch");

            UnpatchDeepAndDeeperTemperaturePrefix(
                harmony,
                AccessTools.PropertyGetter(typeof(MapTemperature), nameof(MapTemperature.OutdoorTemp)),
                "OutdoorTemp"
            );

            UnpatchDeepAndDeeperTemperaturePrefix(
                harmony,
                AccessTools.PropertyGetter(typeof(MapTemperature), nameof(MapTemperature.SeasonalTemp)),
                "SeasonalTemp"
            );

            harmony.PatchAll();

            Log.Message("[CaveAnimals] Startup finished.");
        }

        public Mod(ModContentPack content) : base(content) { }

        private static void UnpatchDeepAndDeeperTemperaturePrefix(
            Harmony harmony,
            MethodInfo original,
            string label)
        {
            const string deepAndDeeperHarmonyId = "DeepAndDeeper";

            var infoBefore = Harmony.GetPatchInfo(original);
            int before = infoBefore?.Prefixes?.Count(p => p.owner == deepAndDeeperHarmonyId) ?? 0;

            harmony.Unpatch(original, HarmonyPatchType.Prefix, deepAndDeeperHarmonyId);

            var infoAfter = Harmony.GetPatchInfo(original);
            int after = infoAfter?.Prefixes?.Count(p => p.owner == deepAndDeeperHarmonyId) ?? 0;

            Log.Message($"[CaveAnimals] {label}: DeepAndDeeper temperature prefixes before={before}, after={after}.");
        }
    }
}
