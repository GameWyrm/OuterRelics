using UnityEngine;
using OWML.ModHelper;

namespace OuterRelics
{
    /// <summary>
    /// Base class for collectable items
    /// </summary>
    public abstract class ItemCollectable : MonoBehaviour
    {
        /// <summary>
        /// Name of the item that will appear in the collected text
        /// </summary>
        public string itemName = "ITEM";

        /// <summary>
        /// Whether Echoes of the Eye is installed
        /// </summary>
        protected bool hasDLC;
        /// <summary>
        /// Parent of the key (to prevent issues with animations)
        /// </summary>
        protected GameObject holder;
        /// <summary>
        /// Notification for item acquired
        /// </summary>
        protected NotificationData itemGet;
        /// <summary>
        /// Animator attached to the item
        /// </summary>
        protected Animator animator;
        /// <summary>
        /// Audiosource attached to the item
        /// </summary>
        protected AudioSource audio;
        /// <summary>
        /// Main mod singleton
        /// </summary>
        protected OuterRelics main;
        /// <summary>
        /// Sibling visibility to compare against to determine item visibility
        /// </summary>
        protected MeshRenderer siblingRenderer;
        /// <summary>
        /// MeshRenderer attached to the item
        /// </summary>
        protected MeshRenderer myRenderer;
        

        private void Start()
        {
            main = OuterRelics.Main;
            hasDLC = OuterRelics.HasDLC;

            itemGet = new NotificationData(itemName + " ACQUIRED");
            holder = transform.parent.gameObject;
            holder.transform.position = holder.transform.position + holder.transform.TransformDirection(Vector3.up * 0.5f);
            //main.LogSuccess("Item " + gameObject.name + " has been created");
            itemGet.minDuration = 5;

            animator = GetComponent<Animator>();
            audio = GetComponent<AudioSource>();
            myRenderer = GetComponent<MeshRenderer>();

            if (hasDLC) DLCRendererCheck();
        }

        protected virtual void Update()
        {
            //Hide item if attached object is invisible
            if (hasDLC && siblingRenderer != null)
            {
                if (myRenderer.enabled != siblingRenderer.enabled)
                {
                    myRenderer.enabled = !myRenderer.enabled;
                    transform.GetChild(0).gameObject.SetActive(myRenderer.enabled);
                }
            }

            
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.CompareTag("Player"))
            {
                main.LogSuccess("A " + gameObject.name + " has been collected by the player");
                NotificationManager.s_instance.PostNotification(itemGet);
                Collect();
            }
        }

        /// <summary>
        /// Checks for DLC and adjusts renderer
        /// </summary>
        private void DLCRendererCheck()
        {
            if (transform.parent.transform.parent.transform.parent == null)
            {
                main.LogInfo("No higher parent found");
                return;
            }
            MeshRenderer[] renderers = transform.parent.transform.parent.transform.parent.gameObject.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer renderer in renderers)
            {
                //main.LogInfo("2: " + (renderer == null ? "NULL" : "NOT NULL"));
                if (renderer == myRenderer) continue;
                else
                {
                    siblingRenderer = renderer;
                }
                //main.LogMessage("A sibling was found for " + transform.parent.name);
                return;
            }

        }

        /// <summary>
        /// What does this item do when collected?
        /// </summary>
        protected abstract void Collect();
    }
}
