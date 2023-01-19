using System.Collections;
using UnityEngine;

namespace ScavengerHunt
{
    /// <summary>
    /// Nomai Key class
    /// </summary>
    public class KeyCollectable : ItemCollectable
    {
        public LockManager lockManager;
        public int keyID;

        bool isCollected;
        float collectionTimer;

        protected override void Update()
        {
            if (isCollected)
            {
                collectionTimer += Time.deltaTime;
                if (collectionTimer > 5f)
                {
                    transform.parent.gameObject.SetActive(false);
                    isCollected = false;
                }
            }

            base.Update();
        }

        /// <summary>
        /// Sets key as obtained and plays collected sound
        /// </summary>
        protected override void Collect()
        {
            lockManager.CollectKey(keyID);
            GetComponent<Collider>().enabled = false;
            animator.SetTrigger("Collect");
            isCollected = true;

            audio.loop = false;
            audio.Stop();
            audio.clip = main.assets.LoadAsset<AudioClip>("CityLights_Off_01");
            audio.Play();
        }
    }
}