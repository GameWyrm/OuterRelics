

using System.Collections.Generic;

namespace OuterRelics
{
    public class RegistrationManager
    {
        List<string> addonFiles;
        List<string> registeredFiles;

        List<string> RegisteredFiles
        {
            get 
            { 
                if (registeredFiles == null) registeredFiles = new List<string>(); 
                return registeredFiles; 
            }
            set
            {
                registeredFiles = value;
            }
        }
        List<string> AddonFiles
        {
            get
            {
                if (addonFiles == null) addonFiles = new List<string>();
                return addonFiles;
            }
            set { addonFiles = value; }
        }

        /// <summary>
        /// Adds or removes a file to/from the pool. Path and .json are appended for you.
        /// </summary>
        /// <param name="file">The name of the file to add/remove (exclude extension)</param>
        /// <param name="register">If true, adds; if false, removes</param>
        public void RegisterFile(string file, bool register)
        {
            if (register)
            {
                RegisteredFiles.Add(GetFile(file));
            }
            else if (RegisteredFiles.Contains(GetFile(file)))
            {
                RegisteredFiles.Remove(GetFile(file));
            }
        }

        /// <summary>
        /// Registers, or unregisters, an addon file
        /// </summary>
        /// <param name="path">Path to the file (exclude extension)</param>
        /// <param name="register">If true, adds; if false, removes</param>
        public void RegisterAddonFile(string path, bool register)
        {
            if (register)
            {
                AddonFiles.Add(path + ".json");
            }
            else if (AddonFiles.Contains(path + ".json"))
            {
                AddonFiles.Remove(path + ".json");
            }
        }

        /// <summary>
        /// Returns the list of all registered files, including addons
        /// </summary>
        /// <returns></returns>
        public List<string> GetRegisteredFiles()
        {
            List<string> loadedFiles = new List<string>();
            loadedFiles.AddRange(RegisteredFiles);
            loadedFiles.AddRange(AddonFiles);
            return loadedFiles;
        }

        private string GetFile(string file)
        {
            return "PlacementInfo/" + file + ".json";
        }
    }
}
