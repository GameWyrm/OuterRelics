using System.Collections;
using UnityEngine;

namespace ScavengerHunt
{
    public class KeyCollectable : ItemCollectable
    {
        public LockManager lockManager;
        public int keyID;

        protected override void Collect()
        {
            lockManager.CollectKey(keyID);
            transform.parent.gameObject.SetActive(false);
        }
    }
}