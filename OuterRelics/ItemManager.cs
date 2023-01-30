using OWML.ModHelper;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;

namespace OuterRelics
{
    /// <summary>
    /// Class that spawns keys
    /// </summary>
    public class ItemManager
    {
        /// <summary>
        /// The seed for randomization. Two runs with the same seed and settings will be identical.
        /// </summary>
        //public string seed;
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
        /// <summary>
        /// List of placement info
        /// </summary>
        public List<PlacementData> placements;

        string seed => main.seed;
        string spoilerLog;

        OuterRelics main;
        List<string> items = new List<string>();
        Random rnd;

        public ItemManager()
        {
            loadedFiles = new List<string>();

            placements = new List<PlacementData>();

            main = OuterRelics.Main;

            SinglePerGroup = false;

            rnd = new Random();
        }

        public void Randomize()
        {
            if (seed != null && seed != "")
            {
                main.LogInfo("Randomizing with seed \"" + seed + "\"");
            }
            else
            {
                main.LogInfo("No seed provided, using random seed");
                main.seed = DateTime.Now.Ticks.ToString();
            }
            rnd = new Random(seed.GetHashCode());
            spoilerLog = $"Seed: {seed}\n";

            LoadFiles();

            List<PlacementData> unusedPlacements = new List<PlacementData>(placements);

            main.LogMessage("Starting Randomization and Placement");
            for (int i = 0; i < 12; i++)
            {
                PlacementData currentPlacement = WeightedPlacement(unusedPlacements);
                
                int locIndex = rnd.Next(0, currentPlacement.locations.Count - 1);
                //main.LogInfo("Location Index picked: " + locIndex + "(" + currentPlacement.locations[locIndex].locationName + "), Location Count: " + currentPlacement.locations.Count);

                SpawnPoint spawn;
                int spawnIndex = rnd.Next(0, currentPlacement.locations[locIndex].spawnPoints.Count - 1);
                //main.LogInfo("Spawn point index picked: " + spawnIndex + "(" + currentPlacement.locations[locIndex].spawnPoints[spawnIndex].spawnPointName + "), Spawn Count: " + currentPlacement.locations[locIndex].spawnPoints.Count);
                spawn = currentPlacement.locations[locIndex].spawnPoints[spawnIndex];

                CreateKey(i, currentPlacement, currentPlacement.locations[locIndex], spawn);

                currentPlacement.locations[locIndex].spawnPoints.RemoveAt(spawnIndex);
                if (currentPlacement.locations[locIndex].spawnPoints.Count <= 0 || SinglePerGroup)
                {
                    currentPlacement.locations.RemoveAt(locIndex);
                }
                if (currentPlacement.locations.Count <= 0)
                {
                    unusedPlacements.Remove(currentPlacement);
                }
            }

            File.WriteAllText(main.ModHelper.Manifest.ModFolderPath + "/SpoilerLogs/" + seed + ".txt", spoilerLog);
        }

        /// <summary>
        /// Spawn key
        /// </summary>
        /// <param name="keyID">ID of the key. Should be unique.</param>
        /// <param name="placement">Placement Data file that contains information for spawn points</param>
        /// <param name="loc">Location group that contains spawn points</param>
        /// <param name="spawn">Specific spawnpoint for the key</param>
        public void CreateKey(int keyID, PlacementData placement, Location loc, SpawnPoint spawn)
        {
            if (!OuterRelics.GetConfigBool("DryMode"))
            {
                if (main.hasKey[keyID]) return;

                if (SceneManager.GetActiveScene().name == placement.system)
                {
                    GameObject keyParent = new GameObject();
                    keyParent.transform.parent = GameObject.Find(placement.body).transform.Find(spawn.parent);
                    keyParent.transform.localPosition = new Vector3(spawn.posX, spawn.posY, spawn.posZ);
                    keyParent.transform.localEulerAngles = new Vector3(spawn.rotX, spawn.rotY, spawn.rotZ);
                    keyParent.transform.position = keyParent.transform.position + keyParent.transform.TransformDirection(Vector3.up * 2);
                    keyParent.name = "NOMAI KEY " + (keyID + 1);
                    GameObject key = GameObject.Instantiate(main.assets.LoadAsset<GameObject>("NK" + (keyID + 1)), keyParent.transform);
                    KeyCollectable kc = key.AddComponent<KeyCollectable>();
                    kc.itemName = "KEY OF " + OuterRelics.KeyNames[keyID];
                    kc.lockManager = main.lockManager;
                    kc.keyID = keyID;

                    main.LogMessage("Created key " + keyID + " on " + placement.body + " at " + loc.locationName + " " + (spawn.spawnPointName != null ? spawn.spawnPointName : ""));
                }
            }
            spoilerLog += $"KEY OF {OuterRelics.KeyNames[keyID]} ({keyID}): {placement.system}, {placement.body}, {loc.locationName}" + (spawn.spawnPointName != null ? (", " + spawn.spawnPointName) : "") + "\n";
        }

        private void LoadFiles()
        {
            placements = new List<PlacementData>();
            for (int i = 0; i < loadedFiles.Count; i++)
            {
                placements.Add(main.ModHelper.Storage.Load<PlacementData>(loadedFiles[i]));
                if (placements[i] == null)
                {
                    main.LogError("File " + loadedFiles[i] + " not found!");
                }
                /*int spawnpoints = 0;
                foreach (Location loc in placements[i].locations)
                {
                    spawnpoints += loc.spawnPoints.Count;
                }
                main.LogInfo($"{loadedFiles[i]}: Locations: {placements[i].locations.Count}, Spawnpoints: {spawnpoints}");*/
            }
            main.LogInfo("Loaded " + placements.Count + " placement files");
        }

        private PlacementData WeightedPlacement(List<PlacementData> unusedPlacements)
        {
            List<int> weights = new List<int>();
            //main.LogMessage("Unused Placements: " + unusedPlacements.Count);
            for (int i = 0; i < unusedPlacements.Count; i++)
            {
                weights.Add(unusedPlacements[i].locations.Count);
                if (i > 0) weights[i] += weights[i - 1];
            }
            //main.LogMessage("Established weights");
            int placementIndex = rnd.Next(0, weights[weights.Count - 1]);
            for (int i = 0; i < weights.Count; i++)
            {
                if (placementIndex < weights[i])
                {
                    return unusedPlacements[i];
                }
            }
            main.LogError("Couldn't properly get a weight!");
            return null;
        }
    }
}
