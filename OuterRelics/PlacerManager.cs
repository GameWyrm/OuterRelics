﻿using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using OWML.Common;
using System.Collections.Generic;
using System.Linq;

namespace OuterRelics
{
    /// <summary>
    /// Class for indicator placement and saving locations
    /// </summary>
    public class PlacerManager : MonoBehaviour
    {
        /// <summary>
        /// While true, indicator placer inputs will be enabled
        /// </summary>
        public bool placerMode = true;
        /// <summary>
        /// If true, placements will spawn hint locations instead of item locations
        /// </summary>
        public bool placeHints = false;
        /// <summary>
        /// Name of the current body
        /// </summary>
        public string currentBody = "";
        /// <summary>
        /// The group of spawn points currently active
        /// </summary>
        public string currentGroup = "";
        /// <summary>
        /// List of locations that can spawn items
        /// </summary>
        public ItemSpawnList spawnList;
        /// <summary>
        /// List of locations that a hint can spawn in
        /// </summary>
        public ItemSpawnList hintSpawns;
        
        

        //Main mod singleton
        private OuterRelics main;
        //placement indicator that looks like a mask
        private GameObject maskIndicator;
        //List of indicators
        private List<GameObject> positionalIndicators;
        //Used for indicator spawner
        int indicatorIndex = -1;

        public GameObject GetIndicator()
        {
            if (indicatorIndex < 0|| indicatorIndex >= positionalIndicators.Count) return null;
            return positionalIndicators[indicatorIndex];
        }

        private void Awake()
        {
            main = GetComponent<OuterRelics>();
            positionalIndicators = new List<GameObject>();
        }

        private void Update()
        {
            bool hasGamepad = Gamepad.all.Count > 0;

            if (Keyboard.current[Key.Numpad7].wasPressedThisFrame || (hasGamepad && Gamepad.current[GamepadButton.DpadRight].wasPressedThisFrame))
            {
                GrabObjectPosition();
            }

            if (main.debugMode && (Keyboard.current[Key.Numpad8].wasPressedThisFrame || (hasGamepad && Gamepad.current[GamepadButton.DpadLeft].wasPressedThisFrame)))
            {
                if (currentGroup == string.Empty && !placeHints)
                {
                    main.notifManager.AddNotification("FAILURE: NO GROUP SELECTED");
                    main.LogError("No Group selected, select a group before saving data");
                    return;
                }
                if (positionalIndicators.Count <= 0 || positionalIndicators[indicatorIndex] == null)
                {
                    main.notifManager.AddNotification("FAILURE: PLACE INDICATOR FIRST");
                    main.LogError("No Indicators placed, place one first to determine spawn point");
                    return;
                }
                SaveSpawnLocation();
                SaveBody();
            }
        }

        public void LoadBody(string bodyName)
        {
            spawnList = main.ModHelper.Storage.Load<ItemSpawnList>($"PlacementInfo/{bodyName}.json");
            if (spawnList == null)
            {
                main.LogInfo($"Unable to find placement info for {bodyName}, creating new file");
                spawnList = new ItemSpawnList();
                spawnList.spawnLocations = new List<ItemSpawnLocation>();
            }

            currentBody = bodyName;
        }

        public void SaveBody()
        {
            if (!placeHints)
            {
                main.ModHelper.Storage.Save<ItemSpawnList>(spawnList, "PlacementInfo/" + currentBody + ".json");
            }
            else
            {
                main.ModHelper.Storage.Save<ItemSpawnList>(hintSpawns, "Hints/HintPlacements.json");
            }
        }

        public void LoadHints()
        {
            hintSpawns = main.ModHelper.Storage.Load<ItemSpawnList>("Hints/HintPlacements.json");
            if (hintSpawns == null)
            {
                main.LogInfo("Unable to locate hint placement data, creating new file");
                hintSpawns = new ItemSpawnList();
                hintSpawns.spawnLocations = new List<ItemSpawnLocation>();
            }
        }

        public ItemSpawnLocation GetSpawnLocation(string name)
        {
            if (placeHints)
            {
                if (PlayerState.InDreamWorld() || PlayerState.InCloakingField())
                {
                    name = "DLCHints";
                }
                else
                {
                    name = "NormalHints";
                }
            }
            ItemSpawnLocation spawnLocation = spawnList.spawnLocations.SingleOrDefault(loc => loc.locationName == name);
            if (spawnLocation.locationName == null)
            {
                main.LogInfo($"Created new location group {name}");
                spawnLocation = new ItemSpawnLocation(SceneManager.GetActiveScene().name, currentBody, name, new List<ItemSpawnPoint>());
                spawnList.spawnLocations.Add(spawnLocation);
            }
            return spawnLocation;
        }

        public void AddSpawnpoint(ItemSpawnLocation location, string parent, Vector3 position, Vector3 rotation)
        {
            SimpleVector3 pos = new SimpleVector3(position.x, position.y, position.z);
            SimpleVector3 rot = new SimpleVector3(rotation.x, rotation.y, rotation.z);
            ItemSpawnPoint spawnPoint = new ItemSpawnPoint("", parent, pos, rot, new List<LogicConditions>());
            main.LogInfo($"Attampting to create a spawnpoint in {location.locationName}");
            location.spawnPoints.Add(spawnPoint);
        }

        /// <summary>
        /// Places an indicator at the position of the collider the player is looking at and returns the relative transform of indicator
        /// </summary>
        private void GrabObjectPosition()
        {
            indicatorIndex++;
            if (indicatorIndex >= 5) indicatorIndex = 0;

            //grab player and camera
            Transform player = Locator.GetPlayerCamera().transform;
            Transform playerBody = Locator.GetPlayerBody().transform;

            //layers of valid collision for raycast
            LayerMask mask = LayerMask.GetMask("Default", "IgnoreSun", "IgnoreOrbRaycast", "Primitive");
            Physics.Raycast(player.position, player.TransformDirection(Vector3.forward), out RaycastHit hit, 1000f, mask); //Ignore layer 8!

            Collider collider = hit.collider;
            if (collider == null)
            {
                main.LogWarning("Did not find any collider for the raycast!");
                return;
            }

            //get raycast information
            Vector3 relativePos = collider.transform.InverseTransformPoint(player.position);

            string hitname = hit.collider.name;
            GameObject go = hit.collider.gameObject;
            while (go.transform.parent != null)
            {
                hitname = go.transform.parent.name + "/" + hitname;
                go = go.transform.parent.gameObject;
            }

            //create indicator objects
            if (positionalIndicators.Count < 5 || positionalIndicators[indicatorIndex] == null)
            {
                GameObject indicator = Indicator();
                if (positionalIndicators.Count < 5) positionalIndicators.Add(indicator);
                else if (positionalIndicators[indicatorIndex] == null) positionalIndicators[indicatorIndex] = indicator;
                main.LogInfo("Created new indicator");
            }

            positionalIndicators[indicatorIndex].transform.position = hit.point;
            positionalIndicators[indicatorIndex].transform.rotation = playerBody.rotation;
            positionalIndicators[indicatorIndex].transform.parent = hit.collider.transform;
            positionalIndicators[indicatorIndex].GetComponent<MeshRenderer>().material = main.uncollectedMat;
            main.LogInfo("Normal: " + hit.normal);

            main.LogMessage("Collider found: " + hitname + "\nPosition: " + relativePos.ToString() + "\nRotation: " + positionalIndicators[indicatorIndex].transform.localEulerAngles);

            if (maskIndicator == null) maskIndicator = Instantiate(main.assets.LoadAsset<GameObject>("Nomai Key Placement Indicator"));
            maskIndicator.transform.position = positionalIndicators[indicatorIndex].transform.position;
            maskIndicator.transform.rotation = positionalIndicators[indicatorIndex].transform.rotation;
            maskIndicator.transform.parent = positionalIndicators[indicatorIndex].transform;

        }

        /// <summary>
        /// Saves the location of the last indicator to the file
        /// </summary>
        private void SaveSpawnLocation()
        {

            GameObject indicator = positionalIndicators[indicatorIndex];
            LoadBody(currentBody);
            AddSpawnpoint(GetSpawnLocation(currentGroup), OuterRelics.GetObjectPath(indicator), indicator.transform.localPosition, indicator.transform.localEulerAngles);
            main.LogSuccess("Saved new spawn location for " + currentBody + " on " + currentGroup);
            main.notifManager.AddNotification("NEW " + (placeHints ? "HINT " : "") + "LOCATION SAVED");
            

            indicator.GetComponent<MeshRenderer>().material = main.collectedMat;

        }

        private GameObject Indicator()
        {
            GameObject indicator;
            indicator = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            indicator.GetComponent<MeshRenderer>().material = main.uncollectedMat;
            indicator.GetComponent<CapsuleCollider>().enabled = false;
            indicator.transform.localScale = Vector3.one * 0.5f;
            indicator.AddComponent<MatchPlayerRotation>();
            return indicator;
        }
    }
}
