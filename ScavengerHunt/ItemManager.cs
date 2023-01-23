using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ScavengerHunt
{
    /// <summary>
    /// Class that spawns keys
    /// </summary>
    public class ItemManager
    {
        /// <summary>
        /// The seed for randomization. Two runs with the same seed and settings will be identical.
        /// </summary>
        public string seed;
        /// <summary>
        /// If true, only one spawn will be used per location. If false, any spawnpoint can be used.
        /// </summary>
        public bool SinglePerGroup;
        /// <summary>
        /// List of JSON files that should be loaded for locations
        /// </summary>
        public List<string> loadedFiles;
        /// <summary>
        /// List of locations that can spawn items
        /// </summary>
        public List<Location> locations;
        public List<Location> unusedLocations;

        

        ScavengerHunt main;
        List<string> items = new List<string>();

        public ItemManager()
        {
            loadedFiles = new List<string>
            {
                "TimberHearth_Body.json",
                "Satellite_Body.json",
                "Moon_Body.json"
            };

            main = ScavengerHunt.Main;

            seed = "Seed";

            SinglePerGroup = false;
        }

        public void Randomize()
        {
            if (seed != null && seed != "")
            {
                main.LogInfo("Randomizing with seed " + seed);
                Random.InitState(seed.GetHashCode());
            }
            else
            {
                main.LogInfo("No seed provided, using random seed");
                Random.InitState((int)DateTime.Now.Ticks);
            }
            
            locations = new List<Location>();
            foreach (string file in loadedFiles)
            {
                PlacementData data = main.ModHelper.Storage.Load<PlacementData>("PlacementInfo/" + file);
                foreach (Location loc in data.locations)
                {
                    locations.Add(loc);
                }
            }

            unusedLocations = new List<Location>(locations);

            for (int i = 0; i < 12; i++)
            {
                int locIndex = Random.Range(0, unusedLocations.Count);
                main.LogInfo("Location Index picked: " + locIndex + "(" + unusedLocations[locIndex].locationName + "), Location Count: " + unusedLocations.Count);

                SpawnPoint spawn;
                int spawnIndex = Random.Range(0, unusedLocations[locIndex].spawnPoints.Count);
                main.LogInfo("Spawnpoint Index picked: " + spawnIndex + "(" + unusedLocations[locIndex].spawnPoints[spawnIndex].spawnPointName + "), Spawn Count: " + unusedLocations[locIndex].spawnPoints.Count);
                spawn = unusedLocations[locIndex].spawnPoints[spawnIndex];

                CreateKey(i, unusedLocations[locIndex], spawn);


                if (SinglePerGroup)
                {
                    unusedLocations.RemoveAt(locIndex);
                }
                else
                {
                    unusedLocations[locIndex].spawnPoints.RemoveAt(spawnIndex);
                    if (unusedLocations[locIndex].spawnPoints.Count == 0) unusedLocations.RemoveAt(locIndex);
                }
            }
        }

        /// <summary>
        /// Spawn key
        /// </summary>
        /// <param name="keyID">ID of the key. Should be unique.</param>
        /// <param name="spawn">Specific spawnpoint for the key</param>
        public void CreateKey(int keyID, Location loc, SpawnPoint spawn)
        {
            if (main.hasKey[keyID]) return;

            GameObject keyParent = new GameObject();
            keyParent.transform.parent = GameObject.Find(loc.body).transform.Find(spawn.parent);
            keyParent.transform.localPosition = new Vector3(spawn.posX, spawn.posY, spawn.posZ);
            keyParent.transform.localEulerAngles = new Vector3(spawn.rotX, spawn.rotY, spawn.rotZ);
            keyParent.transform.position = keyParent.transform.position + keyParent.transform.TransformDirection(Vector3.up * 2);
            keyParent.name = "NOMAI KEY " + (keyID + 1);
            GameObject key = GameObject.Instantiate(main.assets.LoadAsset<GameObject>("NK" + (keyID + 1)), keyParent.transform);
            KeyCollectable kc = key.AddComponent<KeyCollectable>();
            kc.itemName = "KEY OF " + ScavengerHunt.KeyNames[keyID];
            kc.lockManager = main.lockManager;
            kc.keyID = keyID;

            main.LogMessage("Created key " + keyID + " on " + loc.body + " at " + loc.locationName + " " + (spawn.spawnPointName != null ? spawn.spawnPointName : ""));
        }

        private void LoadLocations()
        {

        }
    }
}
