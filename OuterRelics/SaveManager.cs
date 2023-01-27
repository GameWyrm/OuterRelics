using OWML.ModHelper;

namespace OuterRelics
{
    /// <summary>
    /// Class for saving and loading Scavenger Hunt-specific data
    /// </summary>
    public class SaveManager
    {
        public class OuterRelicsSaveData
        {
            public bool[] savedKeysObtained = new bool[12];
            public int totalSavedKeys = 0;
        }

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
        public void SaveData(bool[] keysObtained)
        {
            saveData.totalSavedKeys = 0;
            for (int i = 0; i < keysObtained.Length; i++)
            {
                saveData.savedKeysObtained[i] = keysObtained[i];
                if (keysObtained[i] == true)
                {
                    saveData.totalSavedKeys++;
                }
            }

            main.ModHelper.Storage.Save<OuterRelicsSaveData>(saveData, "SaveData/" + profile + "OuterRelicsSave.json");

            main.LogInfo("Saved Outer Relics data");
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
        /// Resets all save data related to Scavenger Hunt
        /// </summary>
        public void ClearSaveData()
        {
            OuterRelicsSaveData save = new OuterRelicsSaveData();
            main.ModHelper.Storage.Save<OuterRelicsSaveData>(saveData, "OuterRelicsSave.json");
            main.LogInfo("Deleted Scavenger Hunt data");
        }
    }
}
