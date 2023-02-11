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
        /// Whether or not the item is in a Dream World location
        /// </summary>
        protected bool isDWLocation;
        /// <summary>
        /// Whether or not the item is in the Stranger
        /// </summary>
        protected bool isRingWorldLocation;
        /// <summary>
        /// Parent of the key (to prevent issues with animations)
        /// </summary>
        protected GameObject holder;
        /// <summary>
        /// Notification for item acquired
        /// </summary>
        protected string itemGet;
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
        /// MeshRenderer attached to the item
        /// </summary>
        protected MeshRenderer myRenderer;

        //Attempt to fix a rare bug where object appears at origin instead of initial spot and can be collected for a moment
        private int frameCounter;
        

        private void Start()
        {
            main = OuterRelics.Main;
            hasDLC = OuterRelics.HasDLC;

            itemGet = itemName + " ACQUIRED";
            holder = transform.parent.gameObject;
            holder.transform.position = holder.transform.position + holder.transform.TransformDirection(Vector3.up * 0.5f);
            //main.LogSuccess("Item " + gameObject.name + " has been created");

            animator = GetComponent<Animator>();
            audio = GetComponent<AudioSource>();
            myRenderer = GetComponent<MeshRenderer>();

            if (hasDLC)
            {
                isDWLocation = OuterRelics.GetBody(gameObject).name == "DreamWorld_Body";
                isRingWorldLocation = OuterRelics.GetBody(gameObject).name == "RingWorld_Body";
            }

            if (transform.parent.transform.parent.transform.parent == null)
            {
                main.LogInfo("No higher parent found");
                return;
            }
        }

        protected virtual void Update()
        {
            frameCounter++;
            if (isDWLocation)
            {
                myRenderer.enabled = PlayerState.InDreamWorld();
            }
            else if (isRingWorldLocation)
            {
                myRenderer.enabled = PlayerState.InCloakingField();
            }
            transform.GetChild(0).gameObject.SetActive(myRenderer.enabled);
            
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.CompareTag("Player") && frameCounter >= 180)
            {
                main.LogSuccess("A " + gameObject.name + " has been collected by the player");
                Collect();
            }
        }

        /// <summary>
        /// What does this item do when collected?
        /// </summary>
        protected abstract void Collect();
    }
}
