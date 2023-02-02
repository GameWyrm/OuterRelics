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

            bodies = new Dictionary<string, string>
            {
                {"BackerSatellite_Body", "Backer Satellite" },
                {"BrambleIsland_Body", "Giant's Deep"},
                {"BrittleHollow_Body", "Brittle Hollow"},
                {"CaveTwin_Body", "Ember Twin"},
                {"Comet_Body", "The Interloper"},
                {"ConstructionYardIsland_Body", "Giant's Deep"},
                {"DarkBramble_Body", "Dark Bramble"},
                {"DB_PioneerDimension_Body", "Dark Bramble"},
                {"DB_VesselDimension_Body", "Dark Bramble"},
                {"DreamWorld_Body", "Dream World"},
                {"FeldsparShip_Body", "Dark Bramble"},
                {"GabbroIsland_Body", "Giant's Deep"},
                {"GabbroShip_Body", "Giant's Deep"},
                {"GiantsDeep_Body", "Giant's Deep"},
                {"MiningRig_Body", "Timber Hearth"},
                {"Moon_Body", "The Attlerock"},
                {"NomaiProbe_Body", "Eye Seeker Probe"},
                {"OrbitalProbeCannon_Body", "Giant's Deep"},
                {"QuantumIsland_Body", "Giant's Deep"},
                {"QuantumMoon_Body", "Quantum Moon"},
                {"RingWorld_Body", "The Stranger"},
                {"Satellite_Body", "Timber Hearth"},
                {"Sector_EscapePodBody", "Dark Bramble"},
                {"StatueIsland_Body", "Giant's Deep"},
                {"SunStation_Body", "Sun Station"},
                {"TimberHearth_Body", "Timber Hearth"},
                {"TowerTwin_Body", "Ash Twin"},
                {"VolcanicMoon_Body", "Hollow's Lantern"},
                {"WhiteholeStation_Body", "White Hole Station"}
            };
        }

        public string GetSimpleBody(string bodyName)
        {
            bodyName = bodies[bodyName];
            bodyName = bodyName.Replace(" ", "");
            bodyName = bodyName.Replace("'", "");
            return bodyName;
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
    public struct LogicConditions
    {
        public List<string> condition;

        public LogicConditions(List<string> conditions)
        {
            this.condition = conditions;
        }
    }

    [Serializable]
    public struct ItemSpawnPoint
    {
        public string spawnPointName;
        public string parent;
        public SimpleVector3 position;
        public SimpleVector3 rotation;
        public List<LogicConditions> logic;

        public ItemSpawnPoint(string spawnPointName, string parent, SimpleVector3 position, SimpleVector3 rotation, List<LogicConditions> logic)
        {
            this.spawnPointName = spawnPointName;
            this.parent = parent;
            this.position = position;
            this.rotation = rotation;
            this.logic = logic;
        }
    }

    [Serializable]
    public struct ItemSpawnLocation
    {
        public string system;
        public string body;
        public string locationName;
        public List<ItemSpawnPoint> spawnPoints;

        public ItemSpawnLocation(string system, string body, string locationName, List<ItemSpawnPoint> spawnPoints)
        {
            this.system = system;
            this.body = body;
            this.locationName = locationName;
            this.spawnPoints = spawnPoints;
        }
    }

    [Serializable]
    public class ItemSpawnList
    {
        public List<ItemSpawnLocation> spawnLocations;

        public ItemSpawnList()
        {
            spawnLocations = new List<ItemSpawnLocation>();
        }
    }
}