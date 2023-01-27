using System;
using UnityEngine;

namespace OuterRelics
{
    /// <summary>
    /// Locations that items can spawn
    /// </summary>
    [Serializable]
    public struct SpawnPoint
    {
        /// <summary>
        /// Additional information about the spawn for hint system. Optional.
        /// </summary>
        public string spawnPointName;
        /// <summary>
        /// GameObject that item gets parented to
        /// </summary>
        public string parent;
        /// <summary>
        /// Transform of the spawn point
        /// </summary>
        public float posX;
        public float posY;
        public float posZ;
        public float rotX;
        public float rotY;
        public float rotZ;

        public SpawnPoint(string spawnPointName, string parent, float posX, float posY, float posZ, float rotX, float rotY, float rotZ)
        {
            this.spawnPointName = spawnPointName;
            this.parent = parent;
            this.posX = posX;
            this.posY = posY;
            this.posZ = posZ;
            this.rotX = rotX;
            this.rotY = rotY;
            this.rotZ = rotZ;
        }
    }
}