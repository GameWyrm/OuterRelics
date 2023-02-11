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

        /// <summary>
        /// Generates all hints based on seed
        /// </summary>
        /// <param name="seed">The seed of the run</param>
        /// <param name="itemPlacements">List of locations where items are spawning</param>
        /// <param name="hintPlacements">List of locations that a hint object will be created</param>
        /// <returns>List of hints for the run</returns>
        public List<string> GenerateHints(string seed, List<RandomizedPlacement> itemPlacements, List<RandomizedPlacement> hintPlacements, out List<string> bodies, out List<string> locations, out List<string> spawnPoints)
        {
            hints = new();
            rnd = new Random(seed.GetHashCode());
            string newHint = "";
            string body = null;
            string location = null;
            string spawnPoint = null;
            bodies = new();
            locations = new();
            spawnPoints = new();
            for (int i = 0; i < hintPlacements.Count; i++)
            {
                RandomizedPlacement placement = itemPlacements[rnd.Next(0, itemPlacements.Count - 1)];
                newHint = CreateHint(placement, true, out body, out location, out spawnPoint);
                hints.Add(newHint);
                bodies.Add(body);
                locations.Add(location);
                spawnPoints.Add(spawnPoint);
            }

            if (save.GetHintDifficulty() != HintDifficulty.Disabled)
            {
                List<int> hintsToReplace = new();

                for (int i = 0; i < 24; i++)
                {
                    //failsafe in case enough hint spots don't generate somehow
                    if (hints.Count < 24)
                    {
                        main.LogWarning("Did not have enough space for guaranteed hint replacement, aborting");
                        break;
                    }

                    int hintValue = rnd.Next(0, hints.Count - 1);
                    while (hintsToReplace.Contains(hintValue))
                    {
                        hintValue++;
                        if (hintValue >= hints.Count) hintValue = 0;
                    }
                    hintsToReplace.Add(hintValue);
                }

                int index = 0;
                
                foreach (int hint in hintsToReplace)
                {
                    RandomizedPlacement placement = itemPlacements[index % 12];
                    newHint = CreateHint(placement, false, out body, out location, out spawnPoint);
                    hints[hint] = newHint;
                    bodies[hint] = body;
                    locations[hint] = location;
                    spawnPoints[hint] = spawnPoint;
                    index++;
                }
            }

            return hints;
        }

        //creates a hint with neccessary data
        private string CreateHint(RandomizedPlacement placement, bool allowUseless, out string body, out string location, out string spawnpoint)
        {
            string newHint = "";
            if ((rnd.Next(0, 100) > save.GetUselessHintChance() || !allowUseless) && save.GetHintDifficulty() != HintDifficulty.Disabled)
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
                if ((placement.body == "RingWorld_Body" || placement.body == "DreamWorld_Body") && OuterRelics.HasDLC)
                {
                    newHint = hintList.uselessHintsStranger[rnd.Next(0, hintList.uselessHintsStranger.Length - 1)];
                }
                else
                {
                    newHint = hintList.uselessHints[rnd.Next(0, hintList.uselessHints.Length - 1)];
                }
            }

            newHint = ConvertFields(newHint, placement, out body, out location, out spawnpoint);
            return newHint;
        }

        //converts %VARIABLES% into readable values
        private string ConvertFields(string input, RandomizedPlacement placement, out string body, out string location, out string spawnPoint)
        {
            body = null;
            location = null;
            spawnPoint = null;
            if (placement.type == ItemType.Key)
            {
                input = input.Replace("%ITEM%", $"Key of {OuterRelics.KeyNames[placement.id]}");
            }
            else input = input.Replace("%ITEM%", placement.type.ToString());
            if (input.Contains("%SPAWN%"))
            {
                input = input.Replace("%SPAWN%", placement.spawnPointName);
                spawnPoint = placement.spawnPointName;
            }
            if (input.Contains("%LOCATION"))
            {
                input = input.Replace("%LOCATION%", placement.locationName);
                location = placement.locationName;
            }
            if (input.Contains("%BODY%"))
            {
                input = input.Replace("%BODY%", data.bodies[placement.body]);
                body = placement.body;
            }
            return input;
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
