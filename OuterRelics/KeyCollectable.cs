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

        protected override void Start()
        {
            base.Start();
            if (main.useQSB) main.OnObtainKey += OnCollectKey;
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
        private void CollectKey(bool isHost = true)
        {
            if (isHost) main.notifManager.AddNotification(itemGet);
            lockManager.CollectKey(keyID);
            GetComponent<Collider>().enabled = false;
            animator.SetTrigger("Collect");
            isCollected = true;

            audio.loop = false;
            audio.Stop();
            audio.clip = main.assets.LoadAsset<AudioClip>("CityLights_Off_01");
            audio.Play();

            if (isHost && main.useQSB)
            {
                main.qsb.SendMessage<int>("ORCollect", keyID);
            }
        }

        private void OnCollectKey(uint playerID, int keyID)
        {
            main.LogSuccess("A key has been collected!");
            if (keyID == this.keyID)
            {
                main.LogInfo($"Key {keyID} has been collected and is syncing.");
                main.notifManager.AddNotification($"{main.qsb.GetPlayerName(playerID)} COLLECTED THE KEY OF {OuterRelics.KeyNames[keyID]}");
                CollectKey(false);
            }
        }
    }
}