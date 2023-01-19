using System.Collections.Generic;
using UnityEngine;

namespace ScavengerHunt
{
    /// <summary>
    /// Class that spawns keys
    /// </summary>
    public class ItemManager
    {
        public List<string> bodies = new List<string>();
        public List<string> parents = new List<string>();
        public List<Vector3> positions = new List<Vector3>();
        public List<Vector3> rotations = new List<Vector3>();

        ScavengerHunt main;
        List<string> items = new List<string>();

        public ItemManager()
        {
            bodies = new List<string>
            {
                "TimberHearth_Body",
                "CaveTwin_Body",
                "BrittleHollow_Body",
                "QuantumMoon_Body",
                "Moon_Body",
                "BrambleIsland_Body",
                "StatueIsland_Body",
                "ConstructionYardIsland_Body",
                "GabbroIsland_Body",
                "RingWorld_Body",
                "DreamWorld_Body",
                "Comet_Body"
            };
            parents = new List<string>
            {
                "Sector_TH/Sector_Village/Geometry_Village/BatchedGroup/BatchedMeshColliders_5",
                "Sector_CaveTwin/Geometry_CaveTwin/BatchedGroup/BatchedMeshColliders_1",
                "Sector_BH/Sector_QuantumFragment/Geometry_QuantumFragment/ControlledByProxy_Architecture/Architecture_Quantum_Fragment/BatchedGroup/BatchedMeshColliders_0",
                "Sector_QuantumMoon/State_EYE/Geometry_EYEState/BatchedGroup/BatchedMeshColliders_0",
                "Sector_THM/Geo_THM/BatchedGroup/BatchedMeshColliders_0",
                "Sector_BrambleIsland/Geo_BrambleIsland/BatchedGroup/BatchedMeshColliders_0",
                "Sector_StatueIsland/Geometry_StatueIsland/BatchedGroup/BatchedMeshColliders_3",
                "Sector_ConstructionYard/Geometry_ConstructionYard/BatchedGroup/BatchedMeshColliders_1",
                "Sector_GabbroIsland/Geo_GabbroIsland/BatchedGroup/BatchedMeshColliders_0",
                "Sector_RingWorld/Sector_SecretEntrance/Structures_SecretEntrance/BuildingKit/BatchedGroup/BatchedMeshColliders_0",
                "Sector_DreamWorld/Sector_DreamZone_3/Sector_Hotel/Geo_Hotel/BuildingKit_LongHall_NEW/BatchedGroup/BatchedMeshColliders_0",
                "Sector_CO/Geometry_CO/General_Batched/BatchedGroup/BatchedMeshColliders_0"
            };
            positions = new List<Vector3>
            {
                new Vector3(52.2f, 5.7f, -12.3f),
                new Vector3(29.4f, 120.6f, -115.0f),
                new Vector3(-3.1f, 264.2f, 11.0f),
                new Vector3(-5.1f, -69.5f, 16.3f),
                new Vector3(0.4f, -75.4f, 12.0f),
                new Vector3(54.0f, 12.1f, -7.8f),
                new Vector3(-30.0f, -7.5f, -104.7f),
                new Vector3(0.6f, 10.4f, 38.4f),
                new Vector3(-11.6f, 2.0f, 39.5f),
                new Vector3(5.7f, 1.8f, -3.9f),
                new Vector3(-5.6f, 1.8f, 10.1f),
                new Vector3(-73.2f, -46.4f, -7.6f)
            };
            rotations = new List<Vector3>
            {
                new Vector3(348.5f, 238.3f, 11.6f),
                new Vector3(315.4f, 347.4f, 358.7f),
                new Vector3(357.5f, 162.1f, 0.1f),
                new Vector3(13.4f, 178.9f, 183.8f),
                new Vector3(9.2f, 181.4f, 179.9f),
                new Vector3(356.1f, 227.8f, 4.7f),
                new Vector3(350.3f, 47.4f, 354.2f),
                new Vector3(355.7f, 181.5f, 360.0f),
                new Vector3(4.1f, 10.9f, 2.2f),
                new Vector3(360.0f, 90.8f, 0.0f),
                new Vector3(0.0f, 89.2f, 0.0f),
                new Vector3(327.6f, 197.8f, 230.8f),
            };

            main = ScavengerHunt.Main;
        }

        public void PopulateLists()
        {

        }

        /// <summary>
        /// Spawn key
        /// </summary>
        /// <param name="keyID">ID of the key. Should be unique.</param>
        public void CreateKey(int keyID)
        {
            if (main.hasKey[keyID]) return;

            GameObject keyParent = new GameObject();
            keyParent.transform.parent = GameObject.Find(bodies[keyID]).transform.Find(parents[keyID]);
            keyParent.transform.localPosition = positions[keyID];
            keyParent.transform.localEulerAngles = rotations[keyID];
            keyParent.name = "NOMAI KEY " + (keyID + 1);
            GameObject key = GameObject.Instantiate(main.assets.LoadAsset<GameObject>("NK" + (keyID + 1)), keyParent.transform);
            KeyCollectable kc = key.AddComponent<KeyCollectable>();
            kc.itemName = "KEY OF " + ScavengerHunt.KeyNames[keyID];
            kc.lockManager = main.lockManager;
            kc.keyID = keyID;

            main.LogMessage("Created key " + keyID);
        }
    }
}
