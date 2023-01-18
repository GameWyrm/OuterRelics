using OWML.Common;
using OWML.ModHelper;
using ScavengerHunt;
using UnityEngine;

namespace ScavengerHunt
{
    public class LockManager : MonoBehaviour
    {
        public ScavengerHunt main;

        GameObject[] keys;
        Material uncollectedMat;
        Material collectedMat;

        private void Awake()
        {
            transform.position = transform.position + transform.TransformDirection(Vector3.up * 0.5f);

            if (main == null)
            {
                Debug.Log("Main was not set!");
                main = Resources.FindObjectsOfTypeAll<ScavengerHunt>()[0];
            }

            uncollectedMat = main.assets.LoadAsset<Material>("Uncollected");
            collectedMat = main.assets.LoadAsset<Material>("Collected");

            keys = new GameObject[12];

            for (int i = 0; i < keys.Length; i++)
            {
                GameObject go = Instantiate(main.assets.LoadAsset<GameObject>("NK P" + (i + 1)), transform);
                keys[i] = go;
                go.transform.position = transform.position;
                go.transform.rotation = transform.rotation;
                go.GetComponent<MeshRenderer>().material = uncollectedMat;
            }
        }

        public void CollectKey(int keyID)
        {
            keys[keyID].GetComponent<MeshRenderer>().material = collectedMat;

            //implement key collection for unlocking ATP
        }
    }
}
