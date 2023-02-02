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
            hintList = main.ModHelper.Storage.Load<HintList>("hints.json");
        }

        public void GenerateHints(string seed, List<RandomizedPlacement> placements)
        {
            hints = new();
            rnd = new Random(seed.GetHashCode());
            string newHint = "";
            for (int i = 0; i < placements.Count; i++)
            {
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
                    if (placements[i].body == "RingWorld_Body" || placements[i].body == "DreamWorld_Body")
                    {
                        newHint = hintList.uselessHintsStranger[rnd.Next(0, hintList.uselessHintsStranger.Length - 1)];
                    }
                    else
                    {
                        newHint = hintList.uselessHints[rnd.Next(0, hintList.uselessHints.Length - 1)];
                    }
                }
                if (placements[i].type == ItemType.Key)
                {
                    newHint = newHint.Replace("%ITEM%", $"Key of {OuterRelics.KeyNames[placements[i].id]}");
                }
                else newHint = newHint.Replace("%ITEM%", placements[i].type.ToString());
                newHint = newHint.Replace("%SPAWN%", placements[i].spawnPointName);
                newHint = newHint.Replace("%LOCATION", placements[i].locationName);
                newHint = newHint.Replace("%BODY%", data.bodies[placements[i].body]);
                hints.Add(newHint);
            }
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
