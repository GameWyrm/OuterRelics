using OWML.Common;
using OWML.ModHelper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace ScavengerHunt
{
    /// <summary>
    /// Main mod class, holds important information and functions
    /// </summary>
    public class ScavengerHunt : ModBehaviour
    {
        /// <summary>
        /// Reference to main singleton
        /// </summary>
        public static ScavengerHunt Main
        {
            get 
            {
                GameObject instance = GameObject.Find("GameWyrm.ScavengerHunt");
                return instance.GetComponent<ScavengerHunt>(); 
            }
        }

        /// <summary>
        /// Names for every Nomai key
        /// </summary>
        public static List<string> KeyNames = new List<string>
        {
            "TRUTH",
            "CURIOSITY",
            "ELDER",
            "WILD",
            "LIFEGIVER",
            "EXPLORER",
            "NOMAI",
            "NATURE",
            "SUN",
            "WORLD",
            "SPIRIT",
            "NEWBORN"
        };

        /// <summary>
        /// Determines if Echoes of the Eye is installed
        /// </summary>
        public static bool HasDLC => EntitlementsManager.IsDlcOwned() == EntitlementsManager.AsyncOwnershipStatus.Owned;

        /// <summary>
        /// Total found keys
        /// </summary>
        public int keyCount;
        /// <summary>
        /// Specific keys found
        /// </summary>
        public bool[] hasKey;
        /// <summary>
        /// Models, materials, sounds, and other assets used by Scavenger Hunt
        /// </summary>
        public AssetBundle assets;
        /// <summary>
        /// The Lock Manager handles obtaining keys and restricting or opening access to the ATP Warp Core
        /// </summary>
        public LockManager lockManager;
        /// <summary>
        /// Use this to save or load key data
        /// </summary>
        public SaveManager saveManager;
        /// <summary>
        /// The orb that restricts access to the ATP Warp Core
        /// </summary>
        public GameObject orb;
        /// <summary>
        /// The visuals of the lock around the ATP orb
        /// </summary>
        public GameObject orbLock;
        /// <summary>
        /// Code responsible for locking and unlocking the ATP orb
        /// </summary>
        public NomaiInterfaceOrb orbInterface;

        //Objects for assisting with determining potential spawn points
        List<GameObject> positionalIndicators;
        //Handles spawning items in the world
        ItemManager itemManager;
        //Is the player in a playable solar system?
        bool inGame;
        //Used for indicator spawner
        int indicatorIndex = 0;
        

        private void Awake()
        {
            // You won't be able to access OWML's mod helper in Awake.
            // So you probably don't want to do anything here.
            // Use Start() instead.
            //Initialize objects independant of OWML
            positionalIndicators = new List<GameObject>();
            itemManager = new ItemManager();
        }

        private void Start()
        {
            saveManager = new SaveManager();

            //Load save data
            hasKey = saveManager.GetKeyList();
            keyCount = saveManager.GetKeyCount();
            LogInfo("Loaded save data with " + keyCount + " total found keys.");

            //Load game assets
            assets = ModHelper.Assets.LoadBundle("Models/scavengerhuntassets");

            //Register scene load event
            LoadManager.OnCompleteSceneLoad += (scene, loadScene) =>
            {
                if (loadScene != OWScene.SolarSystem)
                {
                    inGame = false;
                    return;
                }
                LogSuccess("Loaded into solar system!");
                StartCoroutine(LoadIn());
                inGame = true;
            };

            //Gamepad detection debug
            if (Gamepad.all.Count > 0)
            {
                LogInfo("Gamepad Detected");
            }
            else
            {
                LogWarning("Gamepad not found");
            }

            //DLC detection debug
            if (HasDLC)
            {
                LogInfo("DLC found! EOTE locations are available for item placement.");
            }
            else
            {
                LogWarning("DLC not found. EOTE locations will not be available for item placement.");
            }
        }

        private void Update()
        {
            //Code below this should only run if the player is in a playable solar system
            if (!inGame) return;

            //Triggers spawning placement indicators
            if (Keyboard.current[Key.J].wasPressedThisFrame || (Gamepad.all.Count > 0 && Gamepad.current[GamepadButton.DpadRight].wasPressedThisFrame))
            {
                GrabObjectPosition();
            }

            //Manually unlocks ATP
            if (Keyboard.current[Key.L].wasPressedThisFrame)
            {
                LogInfo("ATP Unlock was manually triggerred");
                lockManager.UnlockATP();
            }
        }

        /// <summary>
        /// Places an indicator at the position of the collider the player is looking at and returns the relative transform of indicator
        /// </summary>
        private void GrabObjectPosition()
        {
            //grab player and camera
            Transform player = Locator.GetPlayerCamera().transform;
            Transform playerBody = Locator.GetPlayerBody().transform;

            //layers of valid collision for raycast
            LayerMask mask = LayerMask.GetMask("Default", "IgnoreSun", "IgnoreOrbRaycast");
            Physics.Raycast(player.position, player.TransformDirection(Vector3.forward), out RaycastHit hit, 1000f, mask); //Ignore layer 8!

            Collider collider = hit.collider;
            if (collider == null)
            {
                LogWarning("Did not find any collider for the raycast!");
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
            if (positionalIndicators.Count < 5)
            {
                GameObject indicator;
                indicator = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                indicator.GetComponent<MeshRenderer>().material = assets.LoadAsset<Material>("Uncollected");
                indicator.GetComponent<CapsuleCollider>().enabled = false;
                indicator.transform.localScale = Vector3.one * 0.5f;
                positionalIndicators.Add(indicator);
                LogInfo("Created new indicator");
            }
            if (positionalIndicators[indicatorIndex] == null)
            {
                GameObject indicator;
                indicator = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                indicator.GetComponent<MeshRenderer>().material = assets.LoadAsset<Material>("Uncollected");
                indicator.GetComponent<CapsuleCollider>().enabled = false;
                indicator.transform.localScale = Vector3.one * 0.5f;
                positionalIndicators[indicatorIndex] = indicator;
                LogInfo("Created new indicator");

            }
            positionalIndicators[indicatorIndex].transform.position = hit.point;
            positionalIndicators[indicatorIndex].transform.rotation = playerBody.rotation;
            positionalIndicators[indicatorIndex].transform.parent = hit.collider.transform;

            LogMessage("Collider found: " + hitname + ", \nPosition: " + relativePos.ToString() + ", \nRotation: " + positionalIndicators[indicatorIndex].transform.localEulerAngles);

            indicatorIndex++;
            if (indicatorIndex >= 5) indicatorIndex = 0;
        }

        /// <summary>
        /// Initial set up that occurs on loading any scene TODO clean up so it doesn't require vanilla solar system
        /// </summary>
        /// <returns></returns>
        IEnumerator LoadIn()
        {
            GameObject atp = GameObject.Find("TimeLoopRing_Body").transform.Find("Interactibles_TimeLoopRing_Hidden/CoreContainmentInterface").gameObject;
            yield return new WaitUntil(() => atp.transform.childCount >= 5);
            LogInfo(atp == null ? "Could not find it yet" : "Located ATP: " + atp.name);

            orb = atp.transform.Find("Prefab_NOM_InterfaceOrb").gameObject;
            if (orb == null)
            {
                LogWarning("Could not find the orb!");
            }
            else
            {
                LogInfo("Found the orb! " + orb.name);
            }
            orbInterface = orb.GetComponent<NomaiInterfaceOrb>();
            orbInterface.AddLock();
            orbLock = Instantiate(assets.LoadAsset<GameObject>("Orb Lock"), orb.transform);
            orbLock.transform.position = orb.transform.position;
            orbLock.transform.localScale = Vector3.one * 0.55f;
            LogSuccess("Locked the orb!");

            GameObject mask = new GameObject();
            atp = GameObject.Find("TimeLoopRing_Body");
            mask.transform.parent = atp.transform.Find("Geo_TimeLoopRing/BatchedGroup/BatchedMeshColliders_0");
            mask.transform.localPosition = new Vector3(-23.4f, 11.4f, 0f);
            mask.transform.localEulerAngles = new Vector3(64f, 270f, 180f);
            lockManager = mask.AddComponent<LockManager>();

            for (int i = 0; i < 12; i++)
            {
                itemManager.CreateKey(i);
            }
        }

        #region Logging
        /// <summary>
        /// Logs an Info message to the OWML console. Will be blue.
        /// </summary>
        /// <param name="text">Text to print</param>
        public void LogInfo(string text)
        {
            ModHelper.Console.WriteLine(text, MessageType.Info);
        }

        /// <summary>
        /// Logs a Warning message to the OWML console. Will be yellow.
        /// </summary>
        /// <param name="text">Text to print</param>
        public void LogWarning(string text)
        {
            ModHelper.Console.WriteLine(text, MessageType.Warning);
        }

        /// <summary>
        /// Logs an Error message to the OWML console. Will be red.
        /// </summary>
        /// <param name="text">Text to print</param>
        public void LogError(string text)
        {
            ModHelper.Console.WriteLine(text, MessageType.Error);
        }

        /// <summary>
        /// Logs a Success message to the OWML console. Will be green.
        /// </summary>
        /// <param name="text">Text to print</param>
        public void LogSuccess(string text)
        {
            ModHelper.Console.WriteLine(text, MessageType.Success);
        }

        /// <summary>
        /// Logs a message to the OWML console. Will be white.
        /// </summary>
        /// <param name="text">Text to print</param>
        public void LogMessage(string text)
        {
            ModHelper.Console.WriteLine(text, MessageType.Message);
        }
        #endregion
    }
}