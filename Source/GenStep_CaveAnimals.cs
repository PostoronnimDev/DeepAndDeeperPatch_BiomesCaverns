using System;
using System.Linq;
using UnityEngine;
using Verse;

namespace DeepAndDeeperPatch_BiomesCaverns
{
    public class GenStep_CaveAnimals : GenStep
    {
        public int minCount = -1;
        public int maxCount = -1;
        public bool allowFoggedCells = false;

        public override int SeedPart => 9343982;

        public override void Generate(Map map, GenStepParams parms)
        {
            int cellCount = map.Size.x * map.Size.z;
            if (minCount < 0)
            {
                Log.Message("[DeepAndDeeperPatch - Biomes!Caverns] Assigning default animal minCount");
                minCount = cellCount / 1000;
            }

            if (maxCount < 0 || maxCount < minCount)
            {
                Log.Message("[DeepAndDeeperPatch - Biomes!Caverns] Assigning default animal maxCount");
                maxCount = Math.Max(minCount * 2, 1);
            }

            if (map.Biome == null)
            {
                Log.Warning("[DeepAndDeeperPatch - Biomes!Caverns] Map has no biome.");
                return;
            }

            if (map.Biome.AllWildAnimals == null || map.Biome.AllWildAnimals.Count() == 0)
            {
                Log.Warning("[DeepAndDeeperPatch - Biomes!Caverns] Biome " + map.Biome.defName + " has no wildAnimals.");
                return;
            }

            int count = Rand.RangeInclusive(minCount, maxCount);

            int spawned = 0;

            for (int i = 0; i < count; i++)
            {
                PawnKindDef animalKind = ChooseAnimalFromBiome(map);

                if (animalKind == null)
                {
                    Log.Warning("[DeepAndDeeperPatch - Biomes!Caverns] Could not choose animal from biome " + map.Biome.defName + ".");
                    return;
                }

                if (!TryFindSpawnCell(map, out IntVec3 loc))
                {
                    Log.Warning("[DeepAndDeeperPatch - Biomes!Caverns] Could not find valid cave animal spawn cell.");
                    return;
                }

                int randomInRange = animalKind.wildGroupSize.RandomInRange;

                int radius = Mathf.CeilToInt(Mathf.Sqrt(animalKind.wildGroupSize.max));
                IntVec3 intVec = CellFinder.RandomClosewalkCellNear(loc, map, radius);
                for (int j = 0; j < randomInRange; j++)
                {
                    Pawn pawn = PawnGenerator.GeneratePawn(animalKind, null);
                    GenSpawn.Spawn(pawn, loc, map);
                    spawned++;
                }

                i += randomInRange - 1;
            }

            Log.Message("[DeepAndDeeperPatch - Biomes!Caverns] Spawned " + spawned + " cave animals from biome " + map.Biome.defName + ".");
        }

        private PawnKindDef ChooseAnimalFromBiome(Map map)
        {
            return map.Biome.AllWildAnimals
                .Where(record =>
                    record != null
                    && record.race != null
                    && record.race.race != null
                    && record.race.race.Animal)
                .RandomElementByWeightWithFallback(record => map.Biome.CommonalityOfAnimal(record));
        }

        private bool TryFindSpawnCell(Map map, out IntVec3 cell)
        {
            return CellFinder.TryFindRandomCell(map, c =>
                c.InBounds(map)
                && c.Standable(map)
                && !c.Impassable(map)
                && (allowFoggedCells || !c.Fogged(map))
                && c.GetFirstPawn(map) == null,
                out cell);
        }
    }
}
