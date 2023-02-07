using OWML.ModHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
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
        /// List of hints
        /// </summary>
        public List<string> hints;
        ///<summary>
        ///List of locations that an item has been placed in
        ///</summary>
        public List<RandomizedPlacement> itemPlacements;
        /// <summary>
        /// List of locations that a hint has been placed in
        /// </summary>
        public List<RandomizedPlacement> hintPlacements;
        /// <summary>
        /// Lists of all locations that can spawn items
        /// </summary>
        public List<ItemSpawnList> spawnLists;
        /// <summary>
        /// List of hint placement info
        /// </summary>
        public ItemSpawnList hintList;
        /// <summary>
        /// List of models available for hints
        /// </summary>
        public List<GameObject> hintModels;

        string seed => main.seed;
        string spoilerLog;

        OuterRelics main;
        List<string> items = new List<string>();
        Random rnd;

        public ItemManager()
        {
            itemPlacements = new List<RandomizedPlacement>();

            spawnLists = new List<ItemSpawnList>();
            hintList = new ItemSpawnList();

            main = OuterRelics.Main;

            rnd = new Random();
        }

        public void Randomize(bool SingleMode, out bool success)
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

            List<ItemSpawnLocation> allAvailableLocations = new();

            foreach (ItemSpawnList list in spawnLists)
            {
                foreach (ItemSpawnLocation location in list.spawnLocations)
                {
                    allAvailableLocations.Add(location);
                }
            }

            List<ItemSpawnLocation> availableLocations = new();

            for (int i = 0; i < allAvailableLocations.Count; i++)
            {
                ItemSpawnLocation thisLoc = allAvailableLocations[i];
                availableLocations.Add(new ItemSpawnLocation(thisLoc.system, thisLoc.body, thisLoc.locationName, new List<ItemSpawnPoint>()));
                for (int j = 0; j < allAvailableLocations[i].spawnPoints.Count; j++)
                {
                    ItemSpawnPoint itemSpawnPoint = allAvailableLocations[i].spawnPoints[j];
                    if (LogicTokenizer.TestConditions(itemSpawnPoint.logic))
                    {
                        availableLocations[i].spawnPoints.Add(itemSpawnPoint);
                    }
                }
            }
            allAvailableLocations = availableLocations;

            for (int i = allAvailableLocations.Count - 1; i >= 0; i--)
            {
                if (allAvailableLocations[i].spawnPoints.Count == 0)
                {
                    availableLocations.Remove(allAvailableLocations[i]);
                }
            }

            itemPlacements = new();

            for (int i = 0; i < 12; i++)
            {
                if (availableLocations.Count == 0)
                {
                    success = false;
                    return;
                }
                int itemIndex = rnd.Next(0, availableLocations.Count - 1);
                ItemSpawnLocation location = availableLocations[itemIndex];
                int spawnIndex = rnd.Next(0, location.spawnPoints.Count - 1);
                ItemSpawnPoint spawnPoint = location.spawnPoints[spawnIndex];

                itemPlacements.Add(new RandomizedPlacement(ItemType.Key, i, location.system, location.body, spawnPoint.parent, location.locationName, spawnPoint.spawnPointName, new Vector3(spawnPoint.position.x, spawnPoint.position.y, spawnPoint.position.z), new Vector3(spawnPoint.rotation.x, spawnPoint.rotation.y, spawnPoint.rotation.z)));
                spoilerLog += $"Key of {OuterRelics.KeyNames[i]} ({i}): {location.system}, {location.body}, {spawnPoint.spawnPointName}\n";

                if (SingleMode)
                {
                    availableLocations.RemoveAt(itemIndex);
                }
                else
                {
                    availableLocations[itemIndex].spawnPoints.RemoveAt(spawnIndex);
                    if (availableLocations[itemIndex].spawnPoints.Count <= 0) availableLocations.RemoveAt(itemIndex);
                }
            }

            GenerateHints();

            File.WriteAllText(main.ModHelper.Manifest.ModFolderPath + "/SpoilerLogs/" + seed + ".txt", spoilerLog);

            success = true;
        }

        public void GenerateHints()
        {
            rnd = new Random(seed.GetHashCode());

            hintPlacements = new();

            int index = 0;
            if (hintList == null || hintList.spawnLocations.Count == 0) return;
            foreach (ItemSpawnLocation location in hintList.spawnLocations)
            {
                if (location.locationName == "DLCHints" && !OuterRelics.HasDLC) break;
                //main.LogInfo($"Creating hint placement for {location.body}");
                foreach (ItemSpawnPoint spawnPoint in location.spawnPoints)
                {
                    hintPlacements.Add(new RandomizedPlacement(ItemType.Key, index, location.system, location.body, spawnPoint.parent, location.locationName, spawnPoint.spawnPointName, new Vector3(spawnPoint.position.x, spawnPoint.position.y, spawnPoint.position.z), new Vector3(spawnPoint.rotation.x, spawnPoint.rotation.y, spawnPoint.rotation.z)));
                    //main.LogInfo($"Registered new hint at {spawnPoint.parent}");
                    index++;
                }
            }

            HintManager hintManager = new();
            hints = hintManager.GenerateHints(seed, itemPlacements, hintPlacements);
        }

        public void SpawnItems()
        {
            foreach (RandomizedPlacement placement in itemPlacements)
            {
                switch (placement.type)
                {
                    case ItemType.Key:
                        CreateKey(placement);
                        break;
                    case ItemType.JetpackBooster:
                        break;
                    case ItemType.JetpackTank:
                        break;
                    case ItemType.HeartContainer:
                        break;
                    case ItemType.ShipBoost:
                        break;
                    case ItemType.JumpBoost:
                        break;
                    case ItemType.SpeedBoost:
                        break;
                    default:
                        main.LogError("Did not find a valid item type");
                        break;
                }
            }
            main.LogSuccess("Finished creating major items! Generating hints...");
            foreach (RandomizedPlacement hintSpawn in hintPlacements)
            {
                CreateHint(hintSpawn);
            }
        }

        /// <summary>
        /// Spawn key
        /// </summary>
        /// <param name="placement">Randomized data</param>
        public void CreateKey(RandomizedPlacement placement)
        {
            
            if (main.hasKey[placement.id]) return;

            if (SceneManager.GetActiveScene().name == placement.system)
            {
                GameObject keyParent = new GameObject();
                keyParent.transform.parent = GameObject.Find(placement.body).transform.Find(placement.parent);
                keyParent.transform.localPosition = placement.position;
                keyParent.transform.localEulerAngles = placement.rotation;
                keyParent.transform.position = keyParent.transform.position + keyParent.transform.TransformDirection(Vector3.up * 2);
                keyParent.name = "NOMAI KEY " + (placement.id + 1);
                GameObject key = GameObject.Instantiate(main.assets.LoadAsset<GameObject>("NK" + (placement.id + 1)), keyParent.transform);
                keyParent.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                KeyCollectable kc = key.AddComponent<KeyCollectable>();
                kc.itemName = "KEY OF " + OuterRelics.KeyNames[placement.id];
                kc.lockManager = main.lockManager;
                kc.keyID = placement.id;

                main.LogMessage("Created key " + placement.id + " on " + placement.body + " at " + placement.locationName + " " + (placement.spawnPointName != null ? placement.spawnPointName : ""));
            }
        }

        /// <summary>
        /// Creates a hint object with the given hint
        /// </summary>
        /// <param name="hintType"></param>
        public void CreateHint(RandomizedPlacement placement)
        {
            //if (OuterRelics.GetSystemName() != placement.system) return;
            GameObject hintParent = new GameObject();
            hintParent.transform.parent = GameObject.Find(placement.body).transform.Find(placement.parent);
            if (hintParent.transform.parent == null) main.LogError($"Unable to find {placement.body}/{placement.parent}");
            hintParent.transform.localPosition = placement.position;
            hintParent.transform.localEulerAngles = placement.rotation;
            hintParent.transform.position += hintParent.transform.TransformDirection(Vector3.up * 0.5f);

            int hintType;
            if (placement.body == "RingWorld_Body" || placement.body == "DreamWorld_Body")
            {
                hintType = 1;
            }
            else
            {
                hintType = 0;
            }

            GameObject hintObject = GameObject.Instantiate(hintModels[hintType], hintParent.transform);
            hintParent.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);

            HintCollectable hint = hintObject.AddComponent<HintCollectable>();

            hint.hint = hints[placement.id];
            hint.id = placement.id;

            main.LogMessage($"Created a hint at {placement.body}/{placement.parent}, ID {placement.id}");
        }

        private void LoadFiles()
        {
            foreach (string file in Directory.GetFiles(main.ModHelper.Manifest.ModFolderPath + "PlacementInfo"))
            {
                ItemSpawnList listToAdd = main.ModHelper.Storage.Load<ItemSpawnList>("PlacementInfo/" + Path.GetFileName(file));
                if (listToAdd != null)
                {
                    spawnLists.Add(listToAdd);
                }
                else
                {
                    main.LogError("Could not parse file " + file);
                    continue;
                }
                //TODO add addon support
            }

            main.LogInfo("Loaded " + spawnLists.Count + " placement files");

            hintList = main.ModHelper.Storage.Load<ItemSpawnList>("Hints/HintPlacements.json");
            if (hintList != null)
            {
                main.LogSuccess($"Loaded Hint Data with {hintList.spawnLocations.Count} possible locations");
            }
            else main.LogWarning("Could not find Hint Data");
        }

        

    }
}
