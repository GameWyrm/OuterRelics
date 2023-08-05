using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace OuterRelics
{
    /// <summary>
    /// Controls display of obtained items on the pause menu
    /// </summary>
    public class ItemDisplayManager : MonoBehaviour
    {
        public Color unobtainedColor = Color.white;
        public Color obtainedColor = new(0.9960785f, 0.5372549f, 0.1921569f);

        private bool isPaused;
        private Text keyText;
        private Image[] keySprites;
        private GameObject keyList;
        private GameObject spriteHolder;
        private OuterRelics main => OuterRelics.Main;
        private SaveManager save => OuterRelics.Main.saveManager;
        private StatManager stats => OuterRelics.Main.statManager;

        private void Start()
        {
            keyText = transform.Find("KeyList").gameObject.GetComponent<Text>();
            keyList = keyText.gameObject;
            spriteHolder = transform.Find("KeyDisplayHolder").gameObject;
            keySprites = spriteHolder.GetComponentsInChildren<Image>();

            keyText.font = Resources.Load<Font>("fonts/english - latin/SpaceMono-Regular");

            ShowItemList(false);
        }

        private void Update()
        {
            if (isPaused != main.ModHelper.Menus.PauseMenu.IsOpen)
            {
                isPaused = !isPaused;
                ShowItemList(isPaused);
            }
        }

        public void UpdateList()
        {
            string keyString = "";
            int index = 0;
            foreach (string keyName in OuterRelics.KeyNames)
            {
                bool keyObtained = save.GetHasKey(index);
                if (keyObtained)
                {
                    keyString += keyName;
                }
                else
                {
                    keyString += $"<color={unobtainedColor}>{keyName}</color>";
                }
                keyString += "\n";
                index++;
            }
            keyString += $"\n{stats.TimerToStandardFormat()}\nHints: {stats.hintIDsObtained.Count}\nLoops: {stats.TotalLoops()}";
            keyText.text = keyString;
            index = 0;
            foreach (Image image in keySprites)
            {
                bool keyObtained = save.GetHasKey(index);
                image.color = keyObtained ? obtainedColor : unobtainedColor;
                index++;
            }
        }

        public void ShowItemList(bool show)
        {
            if(keyList != null) keyList.SetActive(show);
            if(spriteHolder != null) spriteHolder.SetActive(show);

            if (show) UpdateList();
        }
    }
}