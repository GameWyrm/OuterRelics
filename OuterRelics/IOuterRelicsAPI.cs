using OWML.ModHelper;
using System.Collections.Generic;
using UnityEngine.Events;

//Rename this namespace to whatever your mod's namespace is, or remove it entirely
namespace OuterRelics
{
    public interface IOuterRelicsAPI
    {
        /// <summary>
        /// Registers your placement files and hints placed in the mod's specific PlacementInfo and Hints folder in your mod's root directory
        /// </summary>
        /// <param name="modClass">Your mod's ModBehaviour</param>
        public void RegisterMod(ModBehaviour modClass);

        /// <summary>
        /// Removes all your placement files and hints
        /// </summary>
        /// <param name="modClass">Your mod's ModBehaviour</param>
        public void UnregisterMod(ModBehaviour modClass);

        /// <summary>
        /// Give a body a name
        /// </summary>
        /// <param name="internalName">The internal name of the body you made (i.e. "CaveTwin_Body")</param>
        /// <param name="properName">The proper name of the body you want to appear in hints</param>
        public void RegisterBody(string internalName, string properName);

        /// <summary>
        /// Registers a specific file. Must be placed within the PlacementInfo or Hints folder in your mod's root directory
        /// </summary>
        /// <param name="modClass">Your mod's ModBehaviour</param>
        /// <param name="fileName">Name of the file. You do not need to inclue the .json extension.</param>
        /// <param name="isHint">If true, the file being registered is a hint. If false, it is a placment info file.</param>
        /// <returns>If true, the file was successfully registered.</returns>
        public bool TryRegisterFile(ModBehaviour modClass, string fileName, bool isHint = false);

        /// <summary>
        /// Removes a specific placement or hint file
        /// </summary>
        /// <param name="modClass">Your mod's ModBehaviour</param>
        /// <param name="fileName">Name of the file. You do not need to include the .json extension.</param>
        /// <param name="isHint">If true, the file being removed is a hint.</param>
        /// <returns>If true, the file was successfully unregistered.</returns>
        public bool TryUnregisterFile(ModBehaviour modClass, string fileName, bool isHint = false);

        /// <summary>
        /// Registers a list of files. Must be placed within the PlacementInfo or Hints folder in your mod's root directory
        /// </summary>
        /// <param name="modClass">Your mod's ModBehaviour</param>
        /// <param name="fileNames">Names of the files. You do not need to inclue the .json extension.</param>
        /// <param name="isHint">If true, the files being registered are hints. If false, they are placment info files.</param>
        /// <returns>If true, all of the files were successfully registered.</returns>
        public bool TryRegisterFiles(ModBehaviour modClass, List<string> fileNames, bool isHint = false);

        /// <summary>
        /// Removes a list of placement or hint files
        /// </summary>
        /// <param name="modClass">Your mod's ModBehaviour</param>
        /// <param name="fileNames">Names of the files. You do not need to include the .json extension.</param>
        /// <param name="isHint">If true, the files being removed are hints.</param>
        /// <returns>If true, the files were successfully unregistered.</returns>
        public bool TryUnregisterFiles(ModBehaviour modClass, List<string> fileNames, bool isHint = false);

        /// <summary>
        /// Registers an array of files. Must be placed within the PlacementInfo or Hints folder in your mod's root directory
        /// </summary>
        /// <param name="modClass">Your mod's ModBehaviour</param>
        /// <param name="fileNames">Names of the files. You do not need to inclue the .json extension.</param>
        /// <param name="isHint">If true, the files being registered are hints. If false, they are placment info files.</param>
        /// <returns>If true, all of the files were successfully registered.</returns>
        public bool TryRegisterFiles(ModBehaviour modClass, string[] fileNames, bool isHint = false);

        /// <summary>
        /// Removes an array of placement or hint files
        /// </summary>
        /// <param name="modClass">Your mod's ModBehaviour</param>
        /// <param name="fileNames">Names of the files. You do not need to include the .json extension.</param>
        /// <param name="isHint">If true, the files being removed are hints.</param>
        /// <returns>If true, the files were successfully unregistered.</returns>
        public bool TryUnregisterFiles(ModBehaviour modClass, string[] fileNames, bool isHint = false);

        /// <summary>
        /// Called right before running randomization.
        /// </summary>
        /// <returns></returns>
        public UnityEvent PreRandomizeFilesEvent();

        /// <summary>
        /// Called right after running randomization.
        /// </summary>
        /// <returns></returns>
        public UnityEvent PostRandomizeFilesEvent();
    }
}
