using System;
using UnityEngine;

namespace OuterRelics
{
    /// <summary>
    /// Placement for hints
    /// </summary>
    [Serializable]
    public struct HintPlacement
    {
        public string systemName;
        public string bodyName;
        public string parent;
        public float posX;
        public float posY;
        public float posZ;
        public float rotX;
        public float rotY;
        public float rotZ;

        public HintPlacement(string systemName, string bodyName, string parent, float posX, float posY, float posZ, float rotX, float rotY, float rotZ)
        {
            this.systemName = systemName;
            this.bodyName = bodyName;
            this.parent = parent;
            this.posX = posX;
            this.posY = posY;
            this.posZ = posZ;
            this.rotX = posX;
            this.rotY = posY;
            this.rotZ = rotZ;
        }
    }
}