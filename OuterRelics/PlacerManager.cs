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
        /// Name of the current body
        /// </summary>
        public string currentBody = "";
        /// <summary>
        /// The group of spawn points currently active
        /// </summary>
        public string currentGroup = "";
        /// <summary>
        /// Currently json for editing
        /// </summary>
        public PlacementData placements;

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

            if (Keyboard.current[Key.Numpad8].wasPressedThisFrame || (hasGamepad && Gamepad.current[GamepadButton.DpadLeft].wasPressedThisFrame))
            {
                if (currentGroup == string.Empty)
                {
                    NotificationData notif = new NotificationData("FAILURE: NO GROUP SELECTED");
                    NotificationManager.s_instance.PostNotification(notif);
                    main.LogError("No Group selected, select a group before saving data");
                    return;
                }
                if (positionalIndicators.Count <= 0 || positionalIndicators[indicatorIndex] == null)
                {
                    NotificationData notif = new NotificationData("FAILURE: PLACE INDICATOR FIRST");
                    NotificationManager.s_instance.PostNotification(notif);
                    main.LogError("No Indicators placed, place one first to determine spawn point");
                    return;
                }
                SaveSpawnLocation();
                SaveBody();
            }
        }

        public void LoadBody(string bodyName)
        {
            placements = main.ModHelper.Storage.Load<PlacementData>("PlacementInfo/" + bodyName + ".json");
            if (placements == null)
            {
                main.LogInfo("Unable to find placement info for " + bodyName + ", creating new file");
                placements = new PlacementData();
                placements.system = SceneManager.GetActiveScene().name;
                placements.body = bodyName;
                placements.bodyName = bodyName + " RENAME THIS IN THE JSON FILE";
                placements.locations = new List<Location>();
            }
            currentBody = bodyName;
        }

        public void SaveBody()
        {
            main.ModHelper.Storage.Save<PlacementData>(placements, "PlacementInfo/" + currentBody + ".json");
        }

        public Location GetLocation(string name)
        {
            Location location = placements.locations.SingleOrDefault(loc => loc.locationName == name);
            if (location.locationName == null)
            {
                main.LogInfo("Created new location group");
                location = new Location(name, currentBody, new List<SpawnPoint>());
                placements.locations.Add(location);
            }
            return location;
        }

        public void AddSpawnpoint(Location location, string parent, Vector3 position, Vector3 rotation)
        {
            SpawnPoint spawn = new SpawnPoint();
            spawn.parent = parent;
            spawn.posX = position.x;
            spawn.posY = position.y;
            spawn.posZ = position.z;
            spawn.rotX = rotation.x;
            spawn.rotY = rotation.y;
            spawn.rotZ = rotation.z;
            main.LogInfo("Attempting to create a spawnpoint");
            location.spawnPoints.Add(spawn);
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
            LoadBody(currentBody);

            GameObject indicator = positionalIndicators[indicatorIndex];
            AddSpawnpoint(GetLocation(currentGroup), OuterRelics.GetObjectPath(indicator), indicator.transform.localPosition, indicator.transform.localEulerAngles);
            indicator.GetComponent<MeshRenderer>().material = main.collectedMat;

            NotificationData notif = new NotificationData("NEW LOCATION SAVED");
            NotificationManager.s_instance.PostNotification(notif);
            main.LogSuccess("Saved new spawn location for " + currentBody + " on " + currentGroup);
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