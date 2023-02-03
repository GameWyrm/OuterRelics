using UnityEngine;
using UnityEngine.UI;

namespace OuterRelics
{
    /// <summary>
    /// Class for notifications when suit is not on
    /// </summary>
    public class FallBackNotificationManager : MonoBehaviour
    {
        private float timer;
        private bool isInSuit => PlayerState.IsWearingSuit();
        private Text myText;
        private OuterRelics main;

        private void Start()
        {
            main = OuterRelics.Main;
            myText = GetComponent<Text>();
            foreach (Font font in Resources.FindObjectsOfTypeAll<Font>())
            {
                if (font.name == "SpaceMono-Regular")
                {
                    myText.font = font;
                }
            }
            myText.text = "";
            main.LogInfo("Custom Notification system created");
        }
        
        private void Update()
        {
            if (timer > 0f) timer -= Time.deltaTime;
            if (timer <= 0f || isInSuit)
            {
                myText.text = "";
            }
        }

        public void AddNotification(string notification)
        {
            if (!main.useQSB)
            {
                NotificationData notif = new NotificationData(notification);
                NotificationManager.s_instance.PostNotification(notif);
                if (!main.useQSB)
                {
                    myText.text = notification;
                    timer = 5f;
                }
            }
            
        }
    }
}
