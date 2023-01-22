using System.Collections.Generic;
using UnityEngine;

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
        public bool SinglePerGroup = true;
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
        }

        public void Randomize()
        {
            if (seed != null && seed == "")
            {
                Random.State oldState = Random.state;
                Random.InitState(seed.GetHashCode());
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
                Location loc;
                int locIndex = Random.Range(0, unusedLocations.Count - 1);
                loc = unusedLocations[locIndex];
                unusedLocations.RemoveAt(locIndex);

                SpawnPoint spawn;
                int spawnIndex = Random.Range(0, loc.spawnPoints.Count - 1);
                spawn = loc.spawnPoints[spawnIndex];

                CreateKey(i, loc, spawn);
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
