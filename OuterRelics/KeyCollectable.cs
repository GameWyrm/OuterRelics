using System.Collections;
using UnityEngine;

namespace OuterRelics
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

        //register key collection event for other objects
        private void Awake()
        {
            //if (OuterRelics.Main.useQSB) OuterRelics.Main.OnObtainKey += OnCollectKey;
        }

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
            CollectKey(true);
        }

        // The actual code that collects the key
        private void CollectKey(bool collectedByClient = true)
        {
            if (collectedByClient) OuterRelics.Main.notifManager.AddNotification(itemGet);
            lockManager.CollectKey(keyID);
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = false;
            }
            if (animator == null) { animator = GetComponent<Animator>(); }
            animator.SetTrigger("Collect");
            isCollected = true;

            if (audio == null) { audio = GetComponent<AudioSource>(); }
            audio.loop = false;
            audio.Stop();
            audio.clip = OuterRelics.Main.assets.LoadAsset<AudioClip>("CityLights_Off_01");
            audio.Play();

            if (collectedByClient && OuterRelics.Main.useQSB)
            {
                OuterRelics.Main.LogInfo("Key collected by client, sending message to connected");
                OuterRelics.Main.qsb.SendMessage<int>("ORCollect", keyID);
            }
        }

        public void OnCollectKey(uint playerID, int keyID)
        {
            OuterRelics.Main.LogInfo($"Received key info {keyID}");
            if (keyID == this.keyID)
            {
                OuterRelics.Main.LogInfo($"Key {keyID} has been collected and is syncing.");
                OuterRelics.Main.notifManager.AddNotification($"{OuterRelics.Main.qsb.GetPlayerName(playerID)} COLLECTED THE KEY OF {OuterRelics.KeyNames[keyID]}");
                CollectKey(false);
            }
        }
    }
}