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
        /// If true, only one spawn will be used per location. If false, any spawnpoint can be used.
        /// </summary>
        public bool SinglePerGroup;
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
                ItemSpawnLocation location = availableLocations[rnd.Next(0, availableLocations.Count - 1)];
                ItemSpawnPoint spawnPoint = location.spawnPoints[rnd.Next(0, location.spawnPoints.Count - 1)];

                itemPlacements.Add(new RandomizedPlacement(ItemType.Key, i, location.system, location.body, spawnPoint.parent, location.locationName, spawnPoint.spawnPointName, new Vector3(spawnPoint.position.x, spawnPoint.position.y, spawnPoint.position.z), new Vector3(spawnPoint.rotation.x, spawnPoint.rotation.y, spawnPoint.rotation.z)));
                spoilerLog += $"Key of {OuterRelics.KeyNames[i]} ({i}): {location.system}, {location.body}, {spawnPoint.spawnPointName}\n";

                int filledLoc = availableLocations.IndexOf(location);
                availableLocations[filledLoc].spawnPoints.Remove(spawnPoint);
                if (availableLocations[filledLoc].spawnPoints.Count == 0) availableLocations.RemoveAt(filledLoc);
            }

            File.WriteAllText(main.ModHelper.Manifest.ModFolderPath + "/SpoilerLogs/" + seed + ".txt", spoilerLog);
        }

        public void GenerateHints()
        {
            rnd = new Random(seed.GetHashCode());

            hintPlacements = new();

            int index = 0;
            foreach (ItemSpawnLocation location in hintList.spawnLocations)
            {
                foreach (ItemSpawnPoint spawnPoint in location.spawnPoints)
                {
                    hintPlacements.Add(new RandomizedPlacement(ItemType.Key, index, location.system, location.body, spawnPoint.parent, location.locationName, spawnPoint.spawnPointName, new Vector3(spawnPoint.position.x, spawnPoint.position.y, spawnPoint.position.z), new Vector3(spawnPoint.rotation.x, spawnPoint.rotation.y, spawnPoint.rotation.z)));
                    index++;
                }
            }
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
                    case ItemType.Hint:
                        CreateHint(placement);
                        break;
                    case ItemType.JetpackBooster:
                        throw new NotImplementedException();
                        break;
                    case ItemType.JetpackTank:
                        throw new NotImplementedException();
                        break;
                    case ItemType.HeartContainer:
                        throw new NotImplementedException();
                        break;
                    case ItemType.ShipBoost:
                        throw new NotImplementedException();
                        break;
                    case ItemType.JumpBoost:
                        throw new NotImplementedException();
                        break;
                    case ItemType.SpeedBoost:
                        throw new NotImplementedException();
                        break;
                    default:
                        main.LogError("Did not find a valid item type");
                        break;
                }
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
            if (SceneManager.GetActiveScene().name != placement.system) return;
            GameObject hintParent = new GameObject();
            hintParent.transform.parent = GameObject.Find(placement.body).transform.Find(placement.parent);
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

            main.LogMessage("Created a hint");

            hint.hint = "This is a " + (hintType == 0 ? "Nomai Hint" : "Stranger Hint");
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
                main.LogInfo("Loaded Hint Data");
            }
            else main.LogWarning("Could not find Hint Data");
        }

        

    }
}
