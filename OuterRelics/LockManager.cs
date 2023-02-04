using OWML.Common;
using OWML.ModHelper;
using OuterRelics;
using UnityEngine;
using UnityEngine.Assertions;

namespace OuterRelics
{
    /// <summary>
    /// Class that handles the ATP orb lock and collecting keys
    /// </summary>
    public class LockManager : MonoBehaviour
    {
        /// <summary>
        /// Obtained keys
        /// </summary>
        public bool[] hasKey;
        /// <summary>
        /// Amount of obtained keys
        /// </summary>
        public int keyCount;

        //Singleton instance
        OuterRelics main;
        //List of key objects
        GameObject[] keys;
        GameObject orbLock;
        NomaiInterfaceOrb orbInterface;
        Material uncollectedMat;
        Material collectedMat;

        private void Awake()
        {
            main = OuterRelics.Main;

            
        }

        /// <summary>
        /// Collect a key and save value to a file
        /// </summary>
        /// <param name="keyID">ID of the key</param>
        public void CollectKey(int keyID)
        {
            hasKey[keyID] = true;
            main.hasKey[keyID] = true;
            keys[keyID].GetComponent<MeshRenderer>().material = collectedMat;

            keyCount++;
            main.keyCount++;

            //Open ATP if all 12 keys are found
            if (keyCount >= 12)
            {
                UnlockATP();
            }

            main.saveManager.SaveData(main.hasKey);
        }

        /// <summary>
        /// Code for removing the lock on the ARP core
        /// </summary>
        public void UnlockATP()
        {
            main.notifManager.AddNotification("ALL KEYS OBTAINED");
            NotificationData pinnedCompletion = new NotificationData("ASH TWIN PROJECT UNLOCKED");
            pinnedCompletion.minDuration = 60;
            NotificationManager.s_instance.PostNotification(pinnedCompletion);
            orbLock.SetActive(false);
            orbInterface.RemoveLock();
        }

        public void LockATP()
        {
            GameObject atp = GameObject.Find("TimeLoopRing_Body").transform.Find("Interactibles_TimeLoopRing_Hidden/CoreContainmentInterface").gameObject;
            main.LogInfo(atp == null ? "Could not find it yet" : "Located ATP: " + atp.name);

            if (main.saveManager.GetKeyCount() < 12)
            {
                GameObject orb = atp.transform.Find("Prefab_NOM_InterfaceOrb").gameObject;
                if (orb == null)
                {
                    main.LogWarning("Could not find the orb!");
                }
                else
                {
                    main.LogInfo("Found the orb! " + orb.name);
                }
                orbInterface = orb.GetComponent<NomaiInterfaceOrb>();
                orbInterface.AddLock();
                orbLock = Instantiate(main.assets.LoadAsset<GameObject>("Orb Lock"), orb.transform);
                orbLock.transform.position = orb.transform.position;
                orbLock.transform.localScale = Vector3.one * 0.55f;
                main.LogSuccess("Locked the orb!");
            }

            GameObject mask = new GameObject();
            atp = GameObject.Find("TimeLoopRing_Body");
            mask.transform.parent = atp.transform.Find("Geo_TimeLoopRing/BatchedGroup/BatchedMeshColliders_0");
            mask.transform.localPosition = new Vector3(-23.4f, 11.4f, 0f);
            mask.transform.localEulerAngles = new Vector3(64f, 270f, 180f);
              
            hasKey = new bool[12];
            keyCount = main.keyCount;
            mask.transform.position = mask.transform.position + transform.TransformDirection(Vector3.up * 0.5f);

            uncollectedMat = main.assets.LoadAsset<Material>("Uncollected");
            collectedMat = main.assets.LoadAsset<Material>("Collected");

            //create key displays
            keys = new GameObject[12];

            for (int i = 0; i < keys.Length; i++)
            {
                GameObject go = Instantiate(main.assets.LoadAsset<GameObject>("NK P" + (i + 1)), mask.transform);
                keys[i] = go;
                go.transform.position = mask.transform.position;
                go.transform.rotation = mask.transform.rotation;
                if (!main.hasKey[i]) go.GetComponent<MeshRenderer>().material = uncollectedMat;

                hasKey[i] = main.hasKey[i];
            }
        }
    }
}
