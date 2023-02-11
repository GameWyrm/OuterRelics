using OWML.ModHelper;
using System.Collections.Generic;

namespace OuterRelics
{
    public class AddonManager
    {
        public List<ModBehaviour> activeMods;
        public Dictionary<string, List<ItemSpawnList>> addonFilesLoaded;
        public Dictionary<string, List<ItemSpawnList>> addonHintsLoaded;
        public Dictionary<string, List<string>> addonFileNames;
        public Dictionary<string, List<string>> addonHintNames;

        OuterRelics main => OuterRelics.Main;
        SaveManager save => main.saveManager;

        /// <summary>
        /// Adds an addon File
        /// </summary>
        /// <param name="modName">Name of the mod</param>
        /// <param name="listToAdd">Placement/Hint file</param>
        /// <param name="fileName">Name of the file (including .json)</param>
        /// <param name="isHint">Whether this is a hint</param>
        public void AddFile(string modName, ItemSpawnList listToAdd, string fileName, bool isHint)
        {
            if (isHint)
            {
                addonFilesLoaded[modName].Add(listToAdd);
                addonFileNames[modName].Add(fileName);
            }
            else
            {
                addonHintsLoaded[modName].Add(listToAdd);
                addonHintNames[modName].Add(fileName);
            }
        }

        public static ModBehaviour GetMod(string modName)
        {
            return (ModBehaviour)OuterRelics.Main.ModHelper.Interaction.TryGetMod(modName);
        }

        /// <summary>
        /// Returns list of addon placement files
        /// </summary>
        /// <returns></returns>
        public List<ItemSpawnList> GetSavedPlacements()
        {
            addonFileNames = new();
            List<ItemSpawnList> savedPlacements = new();

            foreach (string modName in save.GetAddonPlacementDict().Keys)
            {
                ModBehaviour mod = GetMod(modName);
                foreach (string fileName in save.GetAddonPlacementDict()[modName])
                {
                    savedPlacements.Add(mod.ModHelper.Storage.Load<ItemSpawnList>("Placement/" + fileName));
                }
            }
            /*
            foreach (ModBehaviour mod in activeMods)
            {
                string modName = mod.ModHelper.Manifest.UniqueName;
                foreach (string fileName in save.GetAddonPlacementFiles(modName))
                {
                    addonFileNames[modName].Add(fileName);
                }
                foreach (ItemSpawnList placementList in addonFilesLoaded[modName])
                {
                    if (addonFileNames[modName].Contains(placementList.modName))
                    {
                        savedPlacements.Add(placementList);
                    }
                }
            }*/

            return savedPlacements;
        }

        /// <summary>
        /// Returns list of addon hint files
        /// </summary>
        /// <returns></returns>
        public List<ItemSpawnList> GetSavedHints()
        {
            addonFileNames = new();
            List<ItemSpawnList> savedPlacements = new();

            foreach (string modName in save.GetAddonPlacementDict().Keys)
            {
                ModBehaviour mod = GetMod(modName);
                foreach (string fileName in save.GetAddonPlacementDict()[modName])
                {
                    savedPlacements.Add(mod.ModHelper.Storage.Load<ItemSpawnList>("Hints/" + fileName));
                }
            }
            /*
            foreach (ModBehaviour mod in activeMods)
            {
                string modName = mod.ModHelper.Manifest.UniqueName;
                foreach (string fileName in save.GetAddonHintFiles(modName))
                {
                    addonFileNames[modName].Add(fileName);
                }
                foreach (ItemSpawnList placementList in addonFilesLoaded[modName])
                {
                    if (addonFileNames[modName].Contains(placementList.modName))
                    {
                        savedPlacements.Add(placementList);
                    }
                }
            }*/

            return savedPlacements;
        }
    }
}