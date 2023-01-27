using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OuterRelics
{
    /// <summary>
    /// Information that holds data for all spawns and locations
    /// </summary>
    public class PlacementData
    {
        /// <summary>
        /// Scene that the placement data will activate
        /// </summary>
        public string system;
        /// <summary>
        /// Internal name of the celestial body that all spawnpoints in the file are on
        /// </summary>
        public string body;
        /// <summary>
        /// User-friendly version of the planet/object name (for example, "Ember Twin" instead of the default "CaveTwin_Body").
        /// This is used for the hint system. If left blank, will use the body name.
        /// </summary>
        public string bodyName;
        /// <summary>
        /// List of overarching areas that contain spawn points (for example, "Chert's Camp" or "Tower of Quantum Knowledge")
        /// </summary>
        public List<Location> locations;
    }
}
