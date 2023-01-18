using UnityEngine;
using OWML.ModHelper;

namespace ScavengerHunt
{
    public abstract class ItemCollectable : MonoBehaviour
    {
        public string itemName = "ITEM";

        protected GameObject holder;
        protected NotificationData itemGet;

        private void Start()
        {
            itemGet = new NotificationData(itemName + " AQUIRED");
            holder = transform.parent.gameObject;
            holder.transform.position = holder.transform.position + holder.transform.TransformDirection(Vector3.up * 0.5f);
            ScavengerHunt.Main.ModHelper.Console.WriteLine("Item " + gameObject.name + " has been created");
            itemGet.minDuration = 5;
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.CompareTag("Player"))
            {
                ScavengerHunt.Main.ModHelper.Console.WriteLine("A " + gameObject.name + " has been collected by the player");
                NotificationManager.s_instance.PostNotification(itemGet);
                Collect();
            }
        }

        protected abstract void Collect();
    }
}
