using OWML.ModHelper;
using System;
using System.Collections.Generic;
using System.IO;

namespace OuterRelics
{
    /// <summary>
    /// Class for saving and loading Scavenger Hunt-specific data
    /// </summary>
    public class SaveManager
    {
        public class OuterRelicsSaveData
        {
            public string seed = "";
            public string version = "";
            public List<string> loadedFiles;
            public bool singlePerGroup = true;
            public HintDifficulty hints = HintDifficulty.Balanced;
            public int uselessHintChance = 25;
            public bool[] savedKeysObtained = new bool[12];
            public int totalSavedKeys = 0;
        }

        /// <summary>
        /// Seed for randomization stored in the save file
        /// </summary>
        public string Seed;

        private OuterRelicsSaveData saveData;
        private OuterRelics main;
        string profile => StandaloneProfileManager.s_instance.currentProfile.profileName;

        public SaveManager()
        {
            main = OuterRelics.Main;
            saveData = main.ModHelper.Storage.Load<OuterRelicsSaveData>("SaveData/" + profile + "OuterRelicsSave.json");
            if (saveData == null)
            {
                saveData = new OuterRelicsSaveData();
                main.LogInfo("Save file did not exist, creating new save info");
            }
        }

        /// <summary>
        /// Saves which items have been obtained
        /// </summary>
        /// <param name="keysObtained">List of keys obtained</param>
        public void SaveData(bool[] keysObtained = null)
        {
            if (keysObtained == null)
            {
                keysObtained = main.hasKey;
            }

            saveData.seed = main.seed;

            saveData.totalSavedKeys = 0;
            for (int i = 0; i < keysObtained.Length; i++)
            {
                saveData.savedKeysObtained[i] = keysObtained[i];
                if (keysObtained[i] == true)
                {
                    saveData.totalSavedKeys++;
                }
            }

            saveData.version = main.ModHelper.Manifest.Version;
            saveData.loadedFiles = main.itemManager.loadedFiles;
            saveData.singlePerGroup = main.itemManager.SinglePerGroup;
            saveData.hints = main.hintDifficulty;
            saveData.uselessHintChance = main.uselessHints;

            main.ModHelper.Storage.Save<OuterRelicsSaveData>(saveData, $"SaveData/{profile}OuterRelicsSave.json");

            main.LogInfo($"Saved Outer Relics data for {profile}");
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
        /// Returns list of files used for confirming placement
        /// </summary>
        /// <returns></returns>
        public List<string> GetFiles()
        {
            return saveData.loadedFiles;
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
