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
        OuterRelics main => OuterRelics.Main;

        private void Awake()
        {
            myText = GetComponent<Text>();
            myText.font = Resources.Load<Font>("fonts/english - latin/SpaceMono-Regular");
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
            NotificationData notif = new NotificationData(notification.ToUpper());
            NotificationManager.s_instance.PostNotification(notif);
            if (!myText.text.Contains(notification))
            {
                myText.text = notification + "\n" + myText.text;
            }
            timer = 5f;
            
        }
    }
}
