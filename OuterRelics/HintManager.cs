using System;
using System.Collections.Generic;
using Random = System.Random;

namespace OuterRelics
{
    public class HintManager
    {
        public List<string> hints;

        OuterRelics main => OuterRelics.Main;
        SaveManager save => main.saveManager;
        Random rnd = new Random();
        ItemSpawnData data = new ItemSpawnData();
        HintList hintList;

        public HintManager()
        {
            hintList = main.ModHelper.Storage.Load<HintList>("Hints/hints.json");
            if (hintList == null) main.LogWarning("Did not find hint list");
        }

        public List<string> GenerateHints(string seed, List<RandomizedPlacement> itemPlacements, List<RandomizedPlacement> hintPlacements)
        {
            hints = new();
            rnd = new Random(seed.GetHashCode());
            string newHint = "";
            for (int i = 0; i < hintPlacements.Count; i++)
            {
                RandomizedPlacement placement = itemPlacements[rnd.Next(0, itemPlacements.Count - 1)];
                if (rnd.Next(0, 100) > save.GetUselessHintChance() && save.GetHintDifficulty() != HintDifficulty.Disabled)
                {
                    if (rnd.Next(0, 100) < (int)save.GetHintDifficulty())
                    {
                        newHint = hintList.vagueHints[rnd.Next(0, hintList.vagueHints.Length - 1)];
                    }
                    else
                    {
                        newHint = hintList.preciseHints[0];
                    }
                }
                else
                {
                    if (placement.body == "RingWorld_Body" || placement.body == "DreamWorld_Body")
                    {
                        newHint = hintList.uselessHintsStranger[rnd.Next(0, hintList.uselessHintsStranger.Length - 1)];
                    }
                    else
                    {
                        newHint = hintList.uselessHints[rnd.Next(0, hintList.uselessHints.Length - 1)];
                    }
                }
                if (placement.type == ItemType.Key)
                {
                    newHint = newHint.Replace("%ITEM%", $"Key of {OuterRelics.KeyNames[placement.id]}");
                }
                else newHint = newHint.Replace("%ITEM%", placement.type.ToString());
                newHint = newHint.Replace("%SPAWN%", placement.spawnPointName);
                newHint = newHint.Replace("%LOCATION%", placement.locationName);
                newHint = newHint.Replace("%BODY%", data.bodies[placement.body]);
                hints.Add(newHint);
            }

            return hints;
        }

        public class HintList
        {
            public string[] preciseHints;
            public string[] vagueHints;
            public string vagueHintNothingLocation;
            public string vagueHintNothingSpecific;
            public string[] uselessHints;
            public string[] uselessHintsStranger;
        }
    }
}
