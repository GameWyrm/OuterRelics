using OWML.ModHelper;

namespace ScavengerHunt
{
    /// <summary>
    /// Class for saving and loading Scavenger Hunt-specific data
    /// </summary>
    public class SaveManager
    {
        public class ScavengerHuntSaveData
        {
            public bool[] savedKeysObtained = new bool[12];
            public int totalSavedKeys = 0;
        }

        private ScavengerHuntSaveData saveData;
        private ScavengerHunt main;

        public SaveManager()
        {
            main = ScavengerHunt.Main;
            saveData = main.ModHelper.Storage.Load<ScavengerHuntSaveData>("ScavengerHuntSave.json");
            if (saveData == null)
            {
                saveData = new ScavengerHuntSaveData();
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

            main.ModHelper.Storage.Save<ScavengerHuntSaveData>(saveData, "ScavengerHuntSave.json");

            main.LogInfo("Saved Scavenger Hunt data");
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
            ScavengerHuntSaveData save = new ScavengerHuntSaveData();
            main.ModHelper.Storage.Save<ScavengerHuntSaveData>(saveData, "ScavengerHuntSave.json");
            main.LogInfo("Deleted Scavenger Hunt data");
        }
    }
}
