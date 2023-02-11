using OWML.ModHelper;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Events;

namespace OuterRelics
{
    public class OuterRelicsAPI : IOuterRelicsAPI
    {
        OuterRelics main => OuterRelics.Main;
        AddonManager addons => main.addonManager;

        /// <summary>
        /// Registers your placement files and hints placed in the mod's specific PlacementInfo and Hints folder in your mod's root directory
        /// </summary>
        /// <param name="modClass">Your mod's ModBehavior</param>
        public void RegisterMod(ModBehaviour modClass) 
        {
            string modName = modClass.ModHelper.Manifest.UniqueName;
            if (!addons.activeMods.Contains(modClass))
            {
                addons.activeMods.Add(modClass);
                addons.addonFilesLoaded.Add(modName, new List<ItemSpawnList>());
            }
            foreach (string file in Directory.GetFiles(modClass.ModHelper.Manifest.ModFolderPath + "PlacementInfo"))
            {
                ItemSpawnList listToAdd = modClass.ModHelper.Storage.Load<ItemSpawnList>("PlacementInfo/" + Path.GetFileName(file));
                if (listToAdd != null)
                {
                    addons.AddFile(modName, listToAdd, Path.GetFileName(file), false);
                    //addons.addonFilesLoaded[modName].Add(listToAdd);
                }
                else
                {
                    main.LogError("Could not parse file " + file);
                    continue;
                }
            }
            foreach (string file in Directory.GetFiles(modClass.ModHelper.Manifest.ModFolderPath + "Hints"))
            {
                ItemSpawnList listToAdd = modClass.ModHelper.Storage.Load<ItemSpawnList>("Hints/" + Path.GetFileName(file));
                if (listToAdd != null)
                {
                    addons.AddFile(modName, listToAdd, Path.GetFileName(file), true);
                    //addons.addonHintsLoaded[modName].Add(listToAdd);
                }
                else
                {
                    main.LogError("Could not parse file " + file);
                    continue;
                }
            }
        }

        /// <summary>
        /// Removes all your placement files and hints
        /// </summary>
        /// <param name="modClass">Your mod's ModBehavior</param>
        public void UnregisterMod(ModBehaviour modClass)
        {
            string modName = modClass.ModHelper.Manifest.UniqueName;
            if (addons.activeMods.Contains(modClass))
            {
                addons.activeMods.Remove(modClass);
                addons.addonFilesLoaded.Remove(modName);
                addons.addonHintsLoaded.Remove(modName);
            }
            else main.LogError($"Mod {modClass.ModHelper.Manifest.UniqueName} not registered, cannot unregister");
        }

        /// <summary>
        /// Registers a specific file. Must be placed within the PlacementInfo or Hints folder in your mod's root directory
        /// </summary>
        /// <param name="modClass">Your mod's ModBehavior</param>
        /// <param name="fileName">Name of the file. You do not need to inclue the .json extension.</param>
        /// <param name="isHint">If true, the file being registered is a hint. If false, it is a placment info file.</param>
        /// <returns>If true, the file was successfully registered.</returns>
        public bool TryRegisterFile(ModBehaviour modClass, string fileName, bool isHint = false)
        {
            if (!fileName.Contains(".json")) fileName += ".json";
            bool success = File.Exists(modClass.ModHelper.Manifest.ModFolderPath + (isHint ? "Hints/" : "PlacementInfo/") + fileName);

            if (!success)
            {
                main.LogError($"File {fileName} not found");
                return false;
            }

            string modName = modClass.ModHelper.Manifest.UniqueName;
            if (!addons.activeMods.Contains(modClass))
            {
                addons.activeMods.Add(modClass);
                addons.addonFilesLoaded.Add(modName, new List<ItemSpawnList>());
            }

            ItemSpawnList listToAdd = modClass.ModHelper.Storage.Load<ItemSpawnList>((isHint ? "Hints/" : "PlacementInfo/") + fileName);
            if (listToAdd != null)
            {
                addons.AddFile(modName, listToAdd, fileName, isHint);
                //addons.addonFilesLoaded[modName].Add(listToAdd);
            }
            else
            {
                main.LogError("Could not parse file " + fileName);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Removes a specific placement or hint file
        /// </summary>
        /// <param name="modClass">Your mod's ModBehavior</param>
        /// <param name="fileName">Name of the file. You do not need to include the .json extension.</param>
        /// <param name="isHint">If true, the file being removed is a hint.</param>
        /// <returns>If true, the file was successfully unregistered.</returns>
        public bool TryUnregisterFile(ModBehaviour modClass, string fileName, bool isHint = false)
        {
            string modName = modClass.ModHelper.Manifest.UniqueName;
            if (!fileName.Contains(".json")) fileName += ".json";
            bool success = File.Exists(modClass.ModHelper.Manifest.ModFolderPath + (isHint ? "Hints/" : "PlacementInfo/") + fileName);

            if (!success)
            {
                main.LogError($"File {fileName} not found");
                return false;
            }

            if (!addons.activeMods.Contains(modClass))
            {
                main.LogError($"Mod {modName} not registered, cannot remove file {fileName}");
                return false;
            }

            ItemSpawnList listToRemove = modClass.ModHelper.Storage.Load<ItemSpawnList>((isHint ? "Hints/" : "PlacementInfo/") + fileName);
            if (listToRemove != null)
            {
                addons.addonFilesLoaded[modName].Remove(listToRemove);
            }
            else
            {
                main.LogError("Could not parse file " + fileName);
                return false;
            }
            if (addons.addonFilesLoaded[modName].Count <= 0 && addons.addonHintsLoaded[modName].Count <= 0)
            {
                addons.activeMods.Remove(modClass);
                main.LogInfo($"No more files registered to {modName}, unregistering mod");
            }
            return true;
        }

        /// <summary>
        /// Registers a list of files. Must be placed within the PlacementInfo or Hints folder in your mod's root directory
        /// </summary>
        /// <param name="modClass">Your mod's ModBehavior</param>
        /// <param name="fileNames">Names of the files. You do not need to inclue the .json extension.</param>
        /// <param name="isHint">If true, the files being registered are hints. If false, they are placment info files.</param>
        /// <returns>If true, all of the files were successfully registered.</returns>
        public bool TryRegisterFiles(ModBehaviour modClass, List<string> fileNames, bool isHint = false)
        {
            string modName = modClass.ModHelper.Manifest.UniqueName;
            if (!addons.activeMods.Contains(modClass))
            {
                addons.activeMods.Add(modClass);
                addons.addonFilesLoaded.Add(modName, new List<ItemSpawnList>());
            }

            List<ItemSpawnList> fileList = new();
            foreach (string file in fileNames)
            {
                string fileName = file;
                if (!fileName.Contains(".json")) fileName += ".json";
                ItemSpawnList listToAdd = modClass.ModHelper.Storage.Load<ItemSpawnList>((isHint ? "Hints/" : "PlacementInfo/") + fileName);
                if (listToAdd != null)
                {
                    addons.AddFile(modName, listToAdd, fileName, isHint);
                    //fileList.Add(listToAdd);
                }
                else
                {
                    main.LogError("Could not parse file " + file);
                    return false;
                }
            }

            if (isHint)
            {
                addons.addonHintsLoaded[modName].AddRange(fileList);
            }
            else
            {
                addons.addonFilesLoaded[modName].AddRange(fileList);
             }
            return true;
        }

        /// <summary>
        /// Removes a list of placement or hint files
        /// </summary>
        /// <param name="modClass">Your mod's ModBehavior</param>
        /// <param name="fileNames">Names of the files. You do not need to include the .json extension.</param>
        /// <param name="isHint">If true, the files being removed are hints.</param>
        /// <returns>If true, the files were successfully unregistered.</returns>
        public bool TryUnregisterFiles(ModBehaviour modClass, List<string> fileNames, bool isHint = false)
        {
            string modName = modClass.ModHelper.Manifest.UniqueName;
            if (!addons.activeMods.Contains(modClass))
            {
                main.LogError($"Mod {modName} not registered, cannot remove files");
                return false;
            }

            List<ItemSpawnList> fileList = new();
            foreach (string file in fileNames)
            {
                string fileName = file;
                if (!file.Contains(".json")) fileName += ".json";
                ItemSpawnList listToAdd = modClass.ModHelper.Storage.Load<ItemSpawnList>((isHint ? "Hints/" : "PlacementInfo/") + fileName);
                if (listToAdd != null)
                {
                    fileList.Add(listToAdd);
                }
                else
                {
                    main.LogError("Could not parse file " + file);
                    return false;
                }
            }

            if (isHint)
            {
                foreach (ItemSpawnList list in fileList)
                {
                    addons.addonHintsLoaded[modName].Remove(list);
                }
            }
            else
            {
                foreach (ItemSpawnList list in fileList)
                {
                    addons.addonFilesLoaded[modName].Remove(list);
                }
            }
            if (addons.addonFilesLoaded[modName].Count <= 0 && addons.addonHintsLoaded[modName].Count <= 0)
            {
                addons.activeMods.Remove(modClass);
                main.LogInfo($"No more files registered to {modName}, unregistering mod");
            }
            return true;
        }

        /// <summary>
        /// Registers an array of files. Must be placed within the PlacementInfo or Hints folder in your mod's root directory
        /// </summary>
        /// <param name="modClass">Your mod's ModBehavior</param>
        /// <param name="fileNames">Names of the files. You do not need to inclue the .json extension.</param>
        /// <param name="isHint">If true, the files being registered are hints. If false, they are placment info files.</param>
        /// <returns>If true, all of the files were successfully registered.</returns>
        public bool TryRegisterFiles(ModBehaviour modClass, string[] fileNames, bool isHint = false)
        {
            bool success = TryRegisterFiles(modClass, new List<string>(fileNames), isHint);
            return success;
        }

        /// <summary>
        /// Removes an array of placement or hint files
        /// </summary>
        /// <param name="modClass">Your mod's ModBehavior</param>
        /// <param name="fileNames">Names of the files. You do not need to include the .json extension.</param>
        /// <param name="isHint">If true, the files being removed are hints.</param>
        /// <returns>If true, the files were successfully unregistered.</returns>
        public bool TryUnregisterFiles(ModBehaviour modClass, string[] fileNames, bool isHint = false)
        {
            bool success = TryUnregisterFiles(modClass, new List<string>(fileNames), isHint);
            return success;
        }

        /// <summary>
        /// Called right before running randomization. If you decide to create custom pool options, you should register every file in your mod at this point to ensure files are properly loaded from the save file
        /// </summary>
        /// <returns></returns>
        public UnityEvent PreRandomizeFilesEvent() => main.PreRandomize;

        /// <summary>
        /// Called right after running randomization. If you decide to create custom pool options, you can unregister all your files at this point and then re-register based on your pool options
        /// </summary>
        /// <returns></returns>
        public UnityEvent PostRandomizeFilesEvent() => main.PostRandomize;
    }
}