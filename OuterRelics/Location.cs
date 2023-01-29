using System.Collections.Generic;
using System;
using UnityEngine;

namespace OuterRelics
{
    /// <summary>
    /// Overall locations that items can spawn, holds multiple SpawnPoints
    /// </summary>
    [Serializable]
    public struct Location
    {
        /// <summary>
        /// User-friendly name of the location that spawns are placed in. Used for hint system.
        /// </summary>
        public string locationName;
        /// <summary>
        /// List of specific locations that items can spawn
        /// </summary>
        public List<SpawnPoint> spawnPoints;

        public Location(string locationName, string body, List<SpawnPoint> spawnPoints)
        {
            this.locationName = locationName;
            this.spawnPoints = spawnPoints;
        }
    }
}