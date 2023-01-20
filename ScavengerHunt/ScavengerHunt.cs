using OWML.Common;
using OWML.Common.Menus;
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
        /// Enables debug tools and placing indicators
        /// </summary>
        public bool debugMode = true;
        /// <summary>
        /// Orange glowing material
        /// </summary>
        public Material collectedMat;
        /// <summary>
        /// Purple glowing material
        /// </summary>
        public Material uncollectedMat;
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
        //Handles placing indicators
        PlacerAssist placer;
        Menu scavengerHuntMenu;
        PopupInputMenu groupSelector;
        //Menu Framework
        IMenuAPI menuAPI;
        //Is the player in a playable solar system?
        bool inGame;
        

        private void Awake()
        {
            // You won't be able to access OWML's mod helper in Awake.
            // So you probably don't want to do anything here.
            // Use Start() instead.
            //Initialize objects independant of OWML
            positionalIndicators = new List<GameObject>();
            itemManager = new ItemManager();
            placer = gameObject.AddComponent<PlacerAssist>();
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

            //Get materials
            collectedMat = assets.LoadAsset<Material>("Collected");
            uncollectedMat = assets.LoadAsset<Material>("Uncollected");

            //Menu Initialization
            menuAPI = ModHelper.Interaction.TryGetModApi<IMenuAPI>("_nebula.MenuFramework");
            LogInfo("Menu API: " + (menuAPI == null ? "NULL" : "FOUND"));

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
                //GrabObjectPosition();
            }

            //Manually unlocks ATP
            if (Keyboard.current[Key.Numpad9].wasPressedThisFrame)
            {
                LogInfo("ATP Unlock was manually triggerred");
                lockManager.UnlockATP();
            }
        }

        public void ToggleDebugMode()
        {
            debugMode = !debugMode;
            if (debugMode)
            {

            }
        }

        private void NewGroupButton()
        {
            //var myButton =
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

            //Menu stuff

            if (debugMode)
            {
                scavengerHuntMenu = menuAPI.PauseMenu_MakePauseListMenu("SCAVENGER HUNT");
                menuAPI.PauseMenu_MakeMenuOpenButton("SCAVENGER HUNT DEBUG", scavengerHuntMenu);
                groupSelector = menuAPI.MakeInputFieldPopup("Select group of locations to place spawn points in within current body. If not found, will create a new group.", "User-Friendly name, i.e. \"Chert's Camp\".", "Change Group", "Cancel");
                groupSelector.OnPopupConfirm += ConfirmGroup;
                menuAPI.PauseMenu_MakeMenuOpenButton("SELECT GROUP", groupSelector, scavengerHuntMenu);
            }
        }

        private void ConfirmGroup()
        {
            //grab player and camera
            Transform player = Locator.GetPlayerCamera().transform;
            Transform playerBody = Locator.GetPlayerBody().transform;

            //layers of valid collision for raycast
            LayerMask mask = LayerMask.GetMask("Default", "IgnoreSun", "IgnoreOrbRaycast");
            Physics.Raycast(player.position, player.TransformDirection(Vector3.down), out RaycastHit hit, 1000f, mask); //Ignore layer 8!

            Collider collider = hit.collider;
            if (collider == null)
            {
                Physics.Raycast(player.position, playerBody.TransformDirection(Vector3.down), out hit, 1000f, mask);
                collider = hit.collider;
                if (collider == null)
                {
                    NotificationData failure = new NotificationData("FAILURE: NO VALID BODY DETECTED");
                    NotificationManager.s_instance.PostNotification(failure);
                    LogError("Failed to create group, no valid body found below the player or in front of the camera");
                    return;
                }
            }
            placer.LoadBody(GetBody(collider.gameObject).name);
            placer.GetLocation(groupSelector.GetInputText());
        }

        /// <summary>
        /// Grabs the absolute parent of the given object
        /// </summary>
        /// <param name="baseObject"></param>
        /// <returns></returns>
        public static GameObject GetBody(GameObject baseObject)
        {
            Transform obj = baseObject.transform;
            while (obj.parent != null)
            {
                obj = obj.parent;
            }
            return obj.gameObject;
        }

        /// <summary>
        /// Grabs the full path of the object provided, except the topmost object. Use GetBody for that instead.
        /// </summary>
        /// <param name="baseObject"></param>
        /// <returns></returns>
        public static string GetObjectPath(GameObject baseObject)
        {
            Transform obj = baseObject.transform.parent;
            string path = "";
            while (obj.parent != null)
            {
                path = obj.name + path;
                obj = obj.parent;
            }
            return path;
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