using UnityEngine;

namespace OuterRelics
{
    public struct RandomizedPlacement
    {
        public ItemType type;
        public int id;
        public string system;
        public string body;
        public string parent;
        public string locationName;
        public string spawnPointName;
        public Vector3 position;
        public Vector3 rotation;

        public RandomizedPlacement(ItemType type, int id, string system, string body, string parent, string locationName, string spawnPointName, Vector3 position, Vector3 rotation)
        {
            this.type = type;
            this.id = id;
            this.system = system;
            this.body = body;
            this.parent = parent;
            this.locationName = locationName;
            this.spawnPointName = spawnPointName;
            this.position = position;
            this.rotation = rotation;
        }
    }
}