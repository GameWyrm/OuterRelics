using OWML.Common;
using OWML.ModHelper;
using OuterRelics;
using UnityEngine;

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
        Material uncollectedMat;
        Material collectedMat;

        private void Awake()
        {
            main = OuterRelics.Main;

            hasKey = new bool[12];
            keyCount = main.keyCount;
            transform.position = transform.position + transform.TransformDirection(Vector3.up * 0.5f);

            uncollectedMat = main.assets.LoadAsset<Material>("Uncollected");
            collectedMat = main.assets.LoadAsset<Material>("Collected");

            //create keys
            keys = new GameObject[12];

            for (int i = 0; i < keys.Length; i++)
            {
                GameObject go = Instantiate(main.assets.LoadAsset<GameObject>("NK P" + (i + 1)), transform);
                keys[i] = go;
                go.transform.position = transform.position;
                go.transform.rotation = transform.rotation;
                if (!main.hasKey[i]) go.GetComponent<MeshRenderer>().material = uncollectedMat;

                hasKey[i] = main.hasKey[i];
            }
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
            main.notifManager.AddNotification("ALL KEYS OBTAINED, ASH TWIN PROJECT UNLOCKED");
            main.orbLock.SetActive(false);
            main.orbInterface.RemoveLock();
        }
    }
}
