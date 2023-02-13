﻿using OWML.Common;
using OWML.ModHelper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace OuterRelics
{
    /// <summary>
    /// Main mod class, holds important information and functions
    /// </summary>
    public class OuterRelics : ModBehaviour
    {
        /// <summary>
        /// Reference to main singleton
        /// </summary>
        public static OuterRelics Main
        {
            get 
            {
                if (main == null) main = FindObjectOfType<OuterRelics>();
                return main; 
            }
        }
        private static OuterRelics main;

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
        public int keyCount = 0; //TODO FIX
        /// <summary>
        /// % chance of a hint being useless
        /// </summary>
        public int uselessHints = 25;
        /// <summary>
        /// Specific keys found
        /// </summary>
        public bool[] hasKey = new bool[12]; //TODO FIX
        /// <summary>
        /// Turns true while in the Vessel at the eye scene. If you go to the main menu while this bool is true, the save file will be reset to the solar system.
        /// </summary>
        public bool prepareSaveReset = false;
        /// <summary>
        /// Enables debug tools and placing indicators
        /// </summary>
        public bool debugMode = true;
        /// <summary>
        /// Whether Quantum Space Buddies is installed, in which case notifications will use QSB's system instead
        /// </summary>
        public bool useQSB = false;
        /// <summary>
        /// Seed used for randomization
        /// </summary>
        public string seed;
        /// <summary>
        /// How precise hints are
        /// </summary>
        public HintDifficulty hintDifficulty = HintDifficulty.Balanced;
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
        /// Handles showing what you do and don't have in the pause menu
        /// </summary>
        public ItemDisplayManager itemDisplayManager;
        /// <summary>
        /// Handles placing items in the world
        /// </summary>
        public ItemManager itemManager;
        /// <summary>
        /// Data for items spawning
        /// </summary>
        public ItemSpawnData itemData;
        /// <summary>
        /// Handles pools
        /// </summary>
        public RegistrationManager registrationManager;
        /// <summary>
        /// Handles sending notifications, works when not in suit
        /// </summary>
        public FallBackNotificationManager notifManager;
        /// <summary>
        /// Handles stats
        /// </summary>
        public StatManager statManager;
        /// <summary>
        /// Handles loading of addon files
        /// </summary>
        public AddonManager addonManager;
        //Events
        /// <summary>
        /// Event called right before randomization
        /// </summary>
        public UnityEvent PreRandomize;
        /// <summary>
        /// Event called right after randomization
        /// </summary>
        public UnityEvent PostRandomize;

        //Handles placing indicators
        PlacerManager placer;
        //Pause menu button for selecting group
        PopupInputMenu groupSelector;
        //Object that holds the stats in the eye scene
        GameObject statsDisplay;

        //Menu Framework
        IMenuAPI menuAPI;
        //New Horizons API
        INewHorizons nhAPI;
        //Is the player starting a new Outer Relics file?
        bool newGame = false;
        //Time that the popup was opened
        float popupOpenTime;
        

        private void Awake()
        {
            // You won't be able to access OWML's mod helper in Awake.
            // So you probably don't want to do anything here.
            // Use Start() instead.
            //Initialize objects independant of OWML
            itemManager = new ItemManager();
            placer = gameObject.AddComponent<PlacerManager>();
            addonManager = new AddonManager();
            PreRandomize = new UnityEvent();
            PostRandomize = new UnityEvent();
            itemData = new ItemSpawnData();
        }

        private void Start()
        {

            //Load game assets
            assets = ModHelper.Assets.LoadBundle("Models/OuterRelicsassets");

            debugMode = GetConfigBool("Debug");
            LogInfo($"Debug Mode: {debugMode}");

            //Initialize stat manager
            statManager = gameObject.AddComponent<StatManager>();

            //Register scene load event
            LoadManager.OnCompleteSceneLoad += (scene, loadScene) =>
            {
                if (loadScene != OWScene.TitleScreen)
                {
                    if (saveManager == null) saveManager = new SaveManager();
                    if (saveManager.GetSaveDataExists()) saveManager.LoadData();
                    statManager.LoadStats();
                }
                if (loadScene == OWScene.SolarSystem)
                {
                    LogSuccess("Loaded into solar system!");
                    StartCoroutine(LoadIn(GetSystemName()));
                    statManager.runTimer = true;
                }
                else
                {
                    if (itemManager != null)
                    {
                        itemManager.itemPlacements = null;
                        itemManager.hintPlacements = null;
                    }
                    statManager.runTimer = false;
                }

                if (loadScene == OWScene.EyeOfTheUniverse)
                {
                    Transform vessel = GameObject.Find("Vessel_Body").transform.Find("Sector_VesselBridge/Geometry_VesselBridge/Structure_NOM_Vessel/body_collider");

                    statsDisplay = Instantiate(assets.LoadAsset<GameObject>("StatsCanvas"), vessel);
                    statsDisplay.transform.localPosition = new Vector3(0f, 27f, 163f);
                    statsDisplay.transform.localEulerAngles = new Vector3(0f, 180f, 0f);

                    statsDisplay.AddComponent<EndingSceneStats>();
                }

                if (loadScene == OWScene.TitleScreen)
                {
                    if (prepareSaveReset)
                    {
                        PlayerData.SaveEyeCompletion();
                    }
                }
            };
            if (nhAPI != null)
            {
                nhAPI.GetStarSystemLoadedEvent().AddListener(dumbRequiredString);
            }

            //Quit event, no cheating
            Application.quitting += () => { if (saveManager != null && SceneManager.GetActiveScene().name == "SolarSystem") saveManager.SaveData(false); };

            //Get materials
            collectedMat = assets.LoadAsset<Material>("Collected");
            uncollectedMat = assets.LoadAsset<Material>("Uncollected");

            //Menu Initialization
            menuAPI = ModHelper.Interaction.TryGetModApi<IMenuAPI>("_nebula.MenuFramework");
            LogInfo("Menu API: " + (menuAPI == null ? "NULL" : "FOUND"));

            ModHelper.Menus.MainMenu.OnInit += () => StartCoroutine(MainMenuButtons());
            ModHelper.Menus.PauseMenu.OnInit += () =>
            {
                if (debugMode)
                {
                    groupSelector = menuAPI.MakeInputFieldPopup("Select group of locations to place spawn points in within current body. If not found, will create a new group.", "User-Friendly name, i.e. \"Chert's Camp\".", "Change Group", "Cancel");
                    groupSelector.OnPopupConfirm += ConfirmGroup;
                    menuAPI.PauseMenu_MakeMenuOpenButton("OUTER RELICS: SELECT PLACEMENT GROUP", groupSelector);
                    PopupMenu confirmHintMode = menuAPI.MakeInfoPopup("Placement mode has been changed", "OK");
                    confirmHintMode.OnPopupConfirm += () =>
                    {
                        placer.placeHints = !placer.placeHints;
                        notifManager.AddNotification("PLACEMENT MODE SET TO " + (placer.placeHints ? "HINT MODE" : "INDICATOR MODE"));
                    };
                    menuAPI.PauseMenu_MakeMenuOpenButton("OUTER RELICS: TOGGLE PLACER MODE", confirmHintMode);
                }

                GameObject itemList = Instantiate(assets.LoadAsset<GameObject>("ItemListCanvas"));
                itemDisplayManager = itemList.AddComponent<ItemDisplayManager>();
            };

            //Get New Horizons API if possible
            nhAPI = ModHelper.Interaction.TryGetModApi<INewHorizons>("xen.NewHorizons");
            LogInfo("New Horizons API: " + (nhAPI == null ? "NULL" : "FOUND"));

            //DLC detection debug
            if (HasDLC)
            {
                LogInfo("DLC found! EOTE locations are available for item placement.");
            }
            else
            {
                LogWarning("DLC not found. EOTE locations will not be available for item placement.");
            }

            //Load key models
            itemManager.hintModels = new List<GameObject>();
            itemManager.hintModels.Add(assets.LoadAsset<GameObject>("Hint"));
            itemManager.hintModels.Add(assets.LoadAsset<GameObject>("Hint Stranger"));

            foreach (Material mat in Resources.FindObjectsOfTypeAll<Material>())
            {
                if (mat.name.Contains("Adobe"))
                {
                    LogInfo(mat.name);
                }
            }
        }

        private void Update()
        {
            //Manually unlocks ATP
            if (debugMode && GetSystemName() == "SolarSystem" && Keyboard.current[Key.Numpad9].wasPressedThisFrame)
            {
                LogInfo("ATP Unlock was manually triggerred");
                lockManager.UnlockATP();
            }
        }

        public override void Configure(IModConfig config)
        {
            debugMode = config.GetSettingsValue<bool>("Debug");
            if (statsDisplay != null)
            {
                bool allowMusic = config.GetSettingsValue<bool>("StatMusic");
                AudioSource aud = statsDisplay.GetComponent<AudioSource>();
                if(allowMusic) aud.Play(); 
                else aud.Stop();
            }

            base.Configure(config);
        }

        public override object GetApi()
        {
            return new OuterRelicsAPI();
        }

        private void dumbRequiredString(string sceneName)
        {
            StartCoroutine(LoadIn(sceneName));
            return;
        }

        /// <summary>
        /// Initial set up that occurs on loading any scene TODO clean up so it doesn't require vanilla solar system
        /// </summary>
        /// <returns></returns>
        IEnumerator LoadIn(string sceneName)
        {
            if (saveManager == null) saveManager = new SaveManager();
            if (!saveManager.GetSaveDataExists() && !newGame) yield break;

            saveManager.LoadData();

            if (seed == null || seed == "") seed = saveManager.GetSeed();
            hasKey = saveManager.GetKeyList();
            keyCount = saveManager.GetKeyCount();

            int frameCount = Time.frameCount;

            yield return new WaitUntil(() => Time.frameCount >= frameCount + 5);

            GameObject notifObject = Instantiate(assets.LoadAsset<GameObject>("FallbackCanvas"));
            notifManager = notifObject.transform.GetChild(0).gameObject.AddComponent<FallBackNotificationManager>();

            if (lockManager == null) lockManager = gameObject.AddComponent<LockManager>();
            if (GetSystemName() == "SolarSystem")
            {
                lockManager.LockATP();
            }

            if (itemManager.itemPlacements == null || itemManager.itemPlacements.Count <= 0)
            {
                itemManager.Randomize(out bool succeeded);
                if (!succeeded)
                {
                    LogError("Failed to randomize, ran out of spawn points! Have the placement files been edited?");
                }
            }

            itemManager.SpawnItems();

            newGame = false;

            statManager.LoadStats();

            saveManager.SaveData(false);

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
                    notifManager.AddNotification("FAILURE: NO VALID BODY DETECTED");
                    LogError("Failed to create group, no valid body found below the player or in front of the camera");
                    return;
                }
            }
            LogInfo("Accessing group " + groupSelector.GetInputText() + " on body " + GetBody(collider.gameObject).name);
            placer.LoadBody(GetBody(collider.gameObject).name);
            placer.currentGroup = groupSelector.GetInputText();
        }


        /// <summary>
        /// Grabs the highest parent of the given object
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
                path = obj.name + "/" + path;
                obj = obj.parent;
            }
            path = path.TrimEnd('/');
            return path;
        }

        /// <summary>
        /// Grabs a bool value from the mod config file
        /// </summary>
        /// <param name="key">Name of the value to retrieve</param>
        /// <returns></returns>
        public static bool GetConfigBool(string key)
        {
            return Main.ModHelper.Config.GetSettingsValue<bool>(key);
        }
        
        /// <summary>
        /// Grabs an int value from the mod config file
        /// </summary>
        /// <param name="key">Name of the value to retrieve</param>
        /// <returns></returns>
        public static int GetConfigInt(string key)
        {
            return Main.ModHelper.Config.GetSettingsValue<int>(key);
        }

        /// <summary>
        /// Grabs a string value from the config file
        /// </summary>
        /// <param name="key">Name of the value to retrieve</param>
        /// <returns></returns>
        public static string GetConfigString(string key)
        {
            return Main.ModHelper.Config.GetSettingsValue<string>(key);
        }

        IEnumerator MainMenuButtons()
        {
            yield return new WaitForEndOfFrame();

            PopupInputMenu seedMenu = menuAPI.MakeInputFieldPopup("Input a seed to generate a new run! Alpha-numeric characters only", "Leave blank for random seed", "Start new run", "Cancel");
            menuAPI.TitleScreen_MakeMenuOpenButton("NEW OUTER RELICS RUN", 1, seedMenu);

            seedMenu.CloseMenuOnOk(false);
            

            seedMenu.OnActivateMenu += () => popupOpenTime = Time.time;
            seedMenu.OnPopupConfirm += () =>
            {
                if (OWMath.ApproxEquals(Time.time, popupOpenTime)) return;

                seedMenu.EnableMenu(false);
                seed = seedMenu.GetInputText();

                NewFileRandomization();
            };
        }

        private void NewFileRandomization()
        {
            bool dryMode = GetConfigBool("DryMode");
            if (OWMath.ApproxEquals(Time.time, popupOpenTime)) return;

            if (saveManager == null) saveManager = new SaveManager();

            Regex reg = new Regex("^[a-zA-Z0-9 ]*$");

            if (!reg.IsMatch(seed))
            {
                ModHelper.Menus.PopupManager.CreateMessagePopup("ERROR: Seeds can only have alpha-numeric characters");
                return;
            }

            if (string.IsNullOrEmpty(seed))
            {
                seed = DateTime.Now.Ticks.ToString();
            }

            LogInfo($"Seed:{seed}");

            if (!dryMode)
            {
                if (saveManager == null) saveManager = new SaveManager();
                saveManager.ClearSaveData();
                saveManager.NewSave();
                itemManager.Randomize(out bool succeeded);
                if (!succeeded)
                {
                    ModHelper.Menus.PopupManager.CreateMessagePopup("RANDOMIZATION FAILED: Not enough spawn points available for placement! Enable more pools in the mod config");
                    return;
                }
                saveManager.SaveData(true);
                PlayerData.SaveEyeCompletion();
                if (PlayerData.LoadLoopCount() > 1)
                {
                    FindObjectOfType<TitleScreenManager>()._resumeGameAction.ConfirmSubmit();
                }
                else
                {
                    FindObjectOfType<TitleScreenManager>()._newGameAction.ConfirmSubmit();
                }

                newGame = true;
            }
            else
            {
                if (saveManager == null) saveManager = new SaveManager();
                saveManager.NewSave();
                itemManager.Randomize(out bool succeeded);
                if (!succeeded)
                {
                    ModHelper.Menus.PopupManager.CreateMessagePopup("RANDOMIZATION FAILED: Not enough spawn points available for placement! Enable more pools in the mod config");
                    return;
                }
                itemManager.itemPlacements = null;
                itemManager.hintPlacements = null;
                ModHelper.Menus.PopupManager.CreateMessagePopup($"Generated Dry Run with seed {seed}, check your Spoiler Log folder");
            }
        }

        public static string GetSystemName()
        {
            if (main.nhAPI == null)
            {
                return SceneManager.GetActiveScene().name;
            }
            else
            {
                return main.nhAPI.GetCurrentStarSystem();
            }
        }

        #region Logging
        /// <summary>
        /// Logs an Info message to the OWML console. Will be blue.
        /// </summary>
        /// <param name="text">Text to print</param>
        /// <param name="debug">Should text only get logged if in debug mode?</param>
        public void LogInfo(string text, bool debug = true)
        {
            if (!debug && !debugMode) return;
            ModHelper.Console.WriteLine(text, MessageType.Info);
        }

        /// <summary>
        /// Logs a Warning message to the OWML console. Will be yellow.
        /// </summary>
        /// <param name="text">Text to print</param>
        /// <param name="debug">Should text only get logged if in debug mode?</param>
        public void LogWarning(string text, bool debug = false)
        {
            if (!debug && !debugMode) return;
            ModHelper.Console.WriteLine(text, MessageType.Warning);
        }

        /// <summary>
        /// Logs an Error message to the OWML console. Will be red.
        /// </summary>
        /// <param name="text">Text to print</param>
        /// <param name="debug">Should text only get logged if in debug mode?</param>
        public void LogError(string text, bool debug = false)
        {
            if (!debug && !debugMode) return;
            ModHelper.Console.WriteLine(text, MessageType.Error);
        }

        /// <summary>
        /// Logs a Success message to the OWML console. Will be green.
        /// </summary>
        /// <param name="text">Text to print</param>
        /// <param name="debug">Should text only get logged if in debug mode?</param>
        public void LogSuccess(string text, bool debug = true)
        {
            if (!debug && !debugMode) return;
            ModHelper.Console.WriteLine(text, MessageType.Success);
        }

        /// <summary>
        /// Logs a message to the OWML console. Will be white.
        /// </summary>
        /// <param name="text">Text to print</param>
        /// <param name="debug">Should text only get logged if in debug mode?</param>
        public void LogMessage(string text, bool debug = true)
        {
            if (!debug && !debugMode) return;
            ModHelper.Console.WriteLine(text, MessageType.Message);
        }
        #endregion

    }
}