using System;
using System.Linq;
using Verse;

namespace DeepAndDeeperPatch_BiomesCaverns
{
    public class GenStep_CaveAnimals : GenStep
    {
        public int minCount;
        public int maxCount;
        public bool allowFoggedCells = false;

        public override int SeedPart => 9343982;

        public override void Generate(Map map, GenStepParams parms)
        {
            int cellCount = map.cellIndices.SizeX * map.cellIndices.SizeZ;
            if (minCount == null) minCount = cellCount / 1000;
            if (maxCount == null) maxCount = Math.Max(minCount * 2, 1);
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
                PawnKindDef kind = ChooseAnimalFromBiome(map);

                if (kind == null)
                {
                    Log.Warning("[DeepAndDeeperPatch - Biomes!Caverns] Could not choose animal from biome " + map.Biome.defName + ".");
                    return;
                }

                if (!TryFindSpawnCell(map, out IntVec3 cell))
                {
                    Log.Warning("[DeepAndDeeperPatch - Biomes!Caverns] Could not find valid cave animal spawn cell.");
                    return;
                }

                Pawn pawn = PawnGenerator.GeneratePawn(kind, null);
                GenSpawn.Spawn(pawn, cell, map);
                spawned++;
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
