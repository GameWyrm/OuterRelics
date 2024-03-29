﻿using OWML.ModHelper;
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

        public AddonManager()
        {
            activeMods = new();
            addonFilesLoaded = new();
            addonHintsLoaded = new();
            addonFileNames = new();
            addonHintNames = new();
        }

        /// <summary>
        /// Adds an addon File
        /// </summary>
        /// <param name="modName">Name of the mod</param>
        /// <param name="listToAdd">Placement/Hint file</param>
        /// <param name="fileName">Name of the file (including .json)</param>
        /// <param name="isHint">Whether this is a hint</param>
        public void AddFile(string modName, ItemSpawnList listToAdd, string fileName, bool isHint)
        {
            if (!addonFilesLoaded.ContainsKey(modName)) addonFilesLoaded.Add(modName, new List<ItemSpawnList>());
            if (!addonFileNames.ContainsKey(modName)) addonFileNames.Add(modName, new List<string>());
            if (!addonHintsLoaded.ContainsKey(modName)) addonHintsLoaded.Add(modName, new List<ItemSpawnList>());
            if (!addonHintNames.ContainsKey(modName)) addonHintNames.Add(modName, new List<string>());

            if ((isHint && addonHintNames[modName].Contains(fileName)) || (!isHint && addonFileNames[modName].Contains(fileName)))
            {
                main.LogWarning($"File {fileName} already registered, aborting");
                return;
            }

            if (!isHint)
            {
                addonFilesLoaded[modName].Add(listToAdd);
                addonFileNames[modName].Add(fileName);
            }
            else
            {
                addonHintsLoaded[modName].Add(listToAdd);
                addonHintNames[modName].Add(fileName);
            }
            main.LogInfo($"Added file {modName}.{fileName}. There are now {addonFilesLoaded[modName].Count} addon placement files loaded, and {addonHintsLoaded[modName].Count} addon hint files loaded.");
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
            //addonFileNames = new();
            List<ItemSpawnList> savedPlacements = new();

            foreach (string modName in save.GetAddonPlacementDict().Keys)
            {
                main.LogInfo("Loading mod " + modName);
                ModBehaviour mod = GetMod(modName);
                foreach (string fileName in save.GetAddonPlacementDict()[modName])
                {
                    ItemSpawnList list = mod.ModHelper.Storage.Load<ItemSpawnList>("PlacementInfo/" + fileName);
                    if (list == null)
                    {
                        main.LogError("Could not find the file specified! Make sure the file is in a folder named PlacementInfo");
                        continue;
                    }
                    savedPlacements.Add(list);
                    //main.LogInfo($"Loaded {fileName}");
                    //main.LogInfo($"{list.spawnLocations.Count} spawn locations");
                }
            }

            return savedPlacements;
        }

        /// <summary>
        /// Returns list of addon hint files
        /// </summary>
        /// <returns></returns>
        public List<ItemSpawnList> GetSavedHints()
        {
            if (main.useQSB)
            {
                main.LogWarning("Outer Relics does not support syncing addon data.\nHaving addons enabled in the Outer Relics config can result in locations between players being wildly different.\n(This is just a PSA, and does not mean that anything has broken.)");
                return null;
            }



            List<ItemSpawnList> savedPlacements = new();
            if (save.GetAddonHintsDict() != null && save.GetAddonHintsDict().Count > 0)
            {
                foreach (string modName in save.GetAddonHintsDict().Keys)
                {
                    ModBehaviour mod = GetMod(modName);
                    foreach (string fileName in save.GetAddonHintsDict()[modName])
                    {
                        savedPlacements.Add(mod.ModHelper.Storage.Load<ItemSpawnList>("Hints/" + fileName));
                    }
                }
            }

            return savedPlacements;
        }
    }
}