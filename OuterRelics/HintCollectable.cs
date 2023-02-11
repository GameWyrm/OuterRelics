using System.Collections;
using UnityEngine;

namespace OuterRelics
{
    public class HintCollectable : ItemCollectable
    {
        public string hint;
        public string body;
        public string location;
        public string spawnPoint;
        public int id;

        private float cooldown;

        protected override void Update()
        {
            if (cooldown > 0) cooldown -= Time.deltaTime;
            base.Update();
        }

        protected override void Collect()
        {
            if (cooldown <= 0)
            {
                main.notifManager.AddNotification(hint);
                main.statManager.AddHint(id);
                main.saveManager.SaveData();
            }
        }
    }
}