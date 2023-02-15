using Epic.OnlineServices;
using OWML.ModHelper;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;

namespace OuterRelics
{
    /// <summary>
    /// Class for saving and loading Scavenger Hunt-specific data
    /// </summary>
    public class SaveManager
    {
        public class OuterRelicsSaveData
        {
            public string seed;
            public string version;
            public bool[] enabledPools;
            public Dictionary<string, List<string>> addonFilesLoaded;
            public Dictionary<string, List<string>> addonHintsLoaded;
            public bool singlePerGroup;
            public HintDifficulty hints;
            public int uselessHintChance;
            public bool[] savedKeysObtained;
            public int totalSavedKeys;
            public float timer;
            public int startLoop;
            public List<int> hintIDsObtained;
        }

        /// <summary>
        /// Seed for randomization stored in the save file
        /// </summary>
        public string Seed;
        public List<string> Pools = new List<string>
        {
            "HourglassTwins",
            "TimberHearth",
            "BrittleHollow",
            "GiantsDeep",
            "DarkBramble",
            "QuantumMoon",
            "Interloper",
            "Stranger",
            "DreamWorld",
            "DreamWorldStealth",
            "HardMode",
            "Addons"
        };

        private OuterRelicsSaveData saveData;
        private OuterRelics main => OuterRelics.Main;
        string profile => StandaloneProfileManager.s_instance.currentProfile.profileName;
        StatManager stats => main.statManager;
        AddonManager addons => main.addonManager;

        public SaveManager()
        {
            LoadData();
        }

        public void LoadData()
        {
            saveData = main.ModHelper.Storage.Load<OuterRelicsSaveData>("SaveData/" + profile + "OuterRelicsSave.json");
            if (saveData == null)
            {
                saveData = new OuterRelicsSaveData();
                

                main.LogInfo("Save file did not exist, creating new save info");
            }
            else main.LogSuccess("Loaded save data for " + profile);
            main.LogInfo("TIMER FROM SAVE: " + saveData.timer);
        }

        /// <summary>
        /// Saves which items have been obtained
        /// </summary>
        /// <param name="keysObtained">List of keys obtained</param>
        public void SaveData(bool titleOverride, bool[] keysObtained = null)
        {
            if (!titleOverride && SceneManager.GetActiveScene().name != "SolarSystem")
            {
                main.LogInfo("Cannot save outside of the standard solar system scene, aborting save");
                return;
            }
            if (keysObtained == null)
            {
                keysObtained = main.hasKey;
            }

            saveData.totalSavedKeys = 0;
            for (int i = 0; i < keysObtained.Length; i++)
            {
                saveData.savedKeysObtained[i] = keysObtained[i];
                if (keysObtained[i] == true)
                {
                    saveData.totalSavedKeys++;
                }
            }

            saveData.timer = stats.timer;
            saveData.startLoop = stats.startingLoop;
            saveData.hintIDsObtained = stats.hintIDsObtained;

            main.ModHelper.Storage.Save<OuterRelicsSaveData>(saveData, $"SaveData/{profile}OuterRelicsSave.json");

            main.LogInfo($"Saved Outer Relics data for {profile}");
        }

        public void NewSave()
        {
            saveData = new OuterRelicsSaveData();

            saveData.seed = main.seed;
            saveData.version = main.ModHelper.Manifest.Version;
            saveData.enabledPools = new bool[]
            {
                    OuterRelics.GetConfigBool("HourglassTwins"),
                    OuterRelics.GetConfigBool("TimberHearth"),
                    OuterRelics.GetConfigBool("BrittleHollow"),
                    OuterRelics.GetConfigBool("GiantsDeep"),
                    OuterRelics.GetConfigBool("DarkBramble"),
                    OuterRelics.GetConfigBool("QuantumMoon"),
                    OuterRelics.GetConfigBool("Interloper"),
                    OuterRelics.GetConfigBool("Stranger"),
                    OuterRelics.GetConfigBool("DreamWorld"),
                    OuterRelics.GetConfigBool("DreamWorldStealth"),
                    OuterRelics.GetConfigBool("HardMode"),
                    OuterRelics.GetConfigBool("Addons")
            };
            if (saveData.enabledPools[11])
            {
                saveData.addonFilesLoaded = addons.addonFileNames;
                saveData.addonHintsLoaded = addons.addonHintNames;
            }
            else
            {
                saveData.addonFilesLoaded = new();
                saveData.addonHintsLoaded = new();
            }
            saveData.singlePerGroup = OuterRelics.GetConfigBool("SingleMode");
            saveData.hints = (HintDifficulty)Enum.Parse(typeof(HintDifficulty), OuterRelics.GetConfigString("Hints").Replace(" ", ""));
            saveData.uselessHintChance = OuterRelics.GetConfigInt("UselessHintChance");
            saveData.savedKeysObtained = new bool[12];
            saveData.totalSavedKeys = 0;
            saveData.timer = 0f;
            saveData.startLoop = PlayerData.LoadLoopCount();
            saveData.hintIDsObtained = new List<int>();

            main.LogSuccess("Created new file info");
        }

        /// <summary>
        /// Returns whether a specific key has been obtained
        /// </summary>
        /// <param name="keyID">The key to check</param>
        /// <returns>Whether the key has been obtained</returns>
        public bool GetHasKey(int keyID)
        {
            return saveData.savedKeysObtained[keyID] == true;
        }

        /// <summary>
        /// Returns whether only one major item can spawn per group
        /// </summary>
        /// <returns></returns>
        public bool GetSinglePerGroup()
        {
            return saveData.singlePerGroup;
        }

        /// <summary>
        /// Returns the string of the seed
        /// </summary>
        /// <returns>Seed in plain text</returns>
        public string GetSeed()
        {
            return saveData.seed;
        }

        /// <summary>
        /// Returns the version the save file was made in
        /// </summary>
        /// <returns></returns>
        public string GetVersion()
        {
            return saveData.version;
        }

        /// <summary>
        /// Returns list of pools used for confirming placement
        /// </summary>
        /// <returns></returns>
        public bool[] GetPools()
        {
            return saveData.enabledPools;
        }

        /// <summary>
        /// Returns the entire list of obtained keys
        /// </summary>
        /// <returns>Array of keys and whether they have been obtained or not</returns>
        public bool[] GetKeyList()
        {
            return saveData.savedKeysObtained;
        }

        /// <summary>
        /// Returns the total amount of obtained keys
        /// </summary>
        /// <returns>Total obtained keys</returns>
        public int GetKeyCount()
        {
            return saveData.totalSavedKeys;
        }

        /// <summary>
        /// Returns chance of useless hints
        /// </summary>
        /// <returns></returns>
        public int GetUselessHintChance()
        {
            return saveData.uselessHintChance;
        }

        public HintDifficulty GetHintDifficulty()
        {
            return saveData.hints;
        }

        /// <summary>
        /// Returns the total amount of time the run has gone on for
        /// </summary>
        /// <returns></returns>
        public float GetTimer()
        {
            return saveData.timer;
        }

        /// <summary>
        /// Returns the number of the loop that the run started on
        /// </summary>
        /// <returns></returns>
        public int GetStartLoop()
        {
            return saveData.startLoop;
        }

        /// <summary>
        /// Returns the number of unique hints found
        /// </summary>
        /// <returns></returns>
        public int GetHintCount()
        {
            return saveData.hintIDsObtained.Count;
        }

        /// <summary>
        /// Returns the list of hints found
        /// </summary>
        /// <returns></returns>
        public List<int> GetHintIDs()
        {
            return saveData.hintIDsObtained;
        }

        /// <summary>
        /// Gets a list of placement files provided by addons that were loaded by the save file
        /// </summary>
        /// <param name="modName">Unique name of the mod</param>
        /// <returns></returns>
        public List<string>  GetAddonPlacementFiles(string modName)
        {
            return saveData.addonFilesLoaded[modName];
        }

        /// <summary>
        /// Gets the full dictionary of placement files
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, List<string>> GetAddonPlacementDict()
        {
            return saveData.addonFilesLoaded;
        }

        /// <summary>
        /// Gets a list of hint files provided by addons that were loaded by the save file
        /// </summary>
        /// <param name="modName">Unique name of the mod</param>
        /// <returns></returns>
        public List<string> GetAddonHintFiles(string modName)
        {
            return saveData.addonHintsLoaded[modName];
        }

        /// <summary>
        /// Gets the full dictionary of saved hints
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, List<string>> GetAddonHintsDict()
        {
            return saveData.addonHintsLoaded;
        }

        /// <summary>
        /// Resets all save data related to Outer Relics for the current profile
        /// </summary>
        public void ClearSaveData()
        {
            try
            {
                File.Delete(main.ModHelper.Manifest.ModFolderPath + $"SaveData/{profile}OuterRelicsSave.json");
                saveData = new OuterRelicsSaveData();
                main.LogInfo("Deleted Outer Relics data");
            }
            catch (Exception e)
            {
                main.LogError($"EXCEPTION: {e.Message}\nSave Data was not deleted");
            }
        }

        /// <summary>
        /// Checks if the save data currently exists
        /// </summary>
        /// <returns></returns>
        public bool GetSaveDataExists()
        {
            return File.Exists(main.ModHelper.Manifest.ModFolderPath + $"SaveData/{profile}OuterRelicsSave.json");
        }

        
    }
}
