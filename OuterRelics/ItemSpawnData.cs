using System;
using System.Collections.Generic;
using UnityEngine;

namespace OuterRelics
{
    public class ItemSpawnData
    {
        public Dictionary<string, string> bodies;

        public ItemSpawnData()
        {

            bodies = new Dictionary<string, string>();
            populateBodies();
        }

        private void populateBodies()
        {
            bodies.Add("BackerSatellite_Body", "Backer Satellite");
            bodies.Add("BrambleIsland_Body", "Giant's Deep");
            bodies.Add("BrittleHollow_Body", "Brittle Hollow");
            bodies.Add("CaveTwin_Body", "Ember Twin");
            bodies.Add("Comet_Body", "The Interloper");
            bodies.Add("ConstructionYardIsland_Body", "Giant's Deep");
            bodies.Add("DarkBramble_Body", "Dark Bramble");
            bodies.Add("DB_PioneerDimension", "Dark Bramble");
            bodies.Add("DB_VesselDimension_Body", "Dark Bramble");
            bodies.Add("DreamWorld_Body", "Dream World");
            bodies.Add("FeldsparShip_Body", "Dark Bramble");
            bodies.Add("GabbroIsland_Body", "Giant's Deep");
            bodies.Add("GabbroShip_Body", "Giant's Deep");
            bodies.Add("MiningRig_Body", "Timber Hearth");
            bodies.Add("Moon_Body", "The Attlerock");
            bodies.Add("NomaiProbe_Body", "Eye Seeker Probe");
            bodies.Add("OrbitalProbeCannon_Body", "Giant's Deep");
            bodies.Add("QuantumIsland_Body", "Giant's Deep");
            bodies.Add("QuantumMoon_Body", "Quantum Moon");
            bodies.Add("RingWorld_Body", "The Stranger");
            bodies.Add("Satellite_Body", "Timber Hearth");
            bodies.Add("Sector_EscapePodBody", "Dark Bramble");
            bodies.Add("StatueIsland_Body", "Giant's Deep");
            bodies.Add("SunStation_Body", "Sun Station");
            bodies.Add("TimberHearth_Body", "Timber Hearth");
            bodies.Add("TowerTwin_Body", "Ash Twin");
            bodies.Add("VolcanicMoon_Body", "Hollow's Lantern");
            bodies.Add("WhiteholeStation_Body", "White Hole Station");
        }

        
    }
    [Serializable]
    public struct SimpleVector3
    {
        public float x;
        public float y;
        public float z;

        public SimpleVector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }

    [Serializable]
    public struct ItemSpawnPoint
    {
        public string system;
        public string body;
        public string locationName;
        public string spawnPointName;
        public string parent;
        public SimpleVector3 position;
        public SimpleVector3 rotation;

        public ItemSpawnPoint(string system, string body, string locationName, string spawnPointName, string parent, SimpleVector3 position, SimpleVector3 rotation)
        {
            this.system = system;
            this.body = body;
            this.locationName = locationName;
            this.spawnPointName = spawnPointName;
            this.parent = parent;
            this.position = position;
            this.rotation = rotation;
        }
    }

    [Serializable]
    public class ItemSpawnList
    {
        public List<ItemSpawnPoint> SpawnPoints;

        public ItemSpawnList()
        {
            SpawnPoints = new List<ItemSpawnPoint>();
        }
    }
}