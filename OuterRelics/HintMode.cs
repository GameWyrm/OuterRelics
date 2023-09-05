using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OuterRelics
{
    public class HintMode : ShipLogMode
    {
        public ItemListWrapper Wrapper;
        public List<Tuple<string, bool, bool, bool>> HintItems => OuterRelics.Main.HintModeList;
        public GameObject RootObject;
        public SaveManager Save => OuterRelics.Main.saveManager;
        public ItemManager Items => OuterRelics.Main.itemManager;

        GameObject keyCanvas;
        Image[] keySprites;
        Text systemText;
        Text bodyText;
        Text areaText;
        Text locationText;
        int selectedItem;
        Color dimWhite = new Color(1, 1, 1, 0.3f);
        Color dimOrange = new Color(0.996f, 0.5372f, 0.1921f, 0.3f);
        Color orange = new Color(0.996f, 0.5372f, 0.1921f);

        public override void Initialize(ScreenPromptList centerPromptList, ScreenPromptList upperRightPromptList, OWAudioSource oneShotSource)
        {
            OuterRelics.Main.LogInfo("Custom mode created");
            //runs when mode is created
        }
        
        //runs when mode is entered
        public override void EnterMode(string entryID = "", List<ShipLogFact> revealQueue = null)
        {
            CreateCanvasStuff();
            Wrapper.Open();
            Wrapper.SetName("Hints");
            Wrapper.SetItems(HintItems);
            Wrapper.SetSelectedIndex(0);
            Wrapper.UpdateList();
            selectedItem = 0;
            RootObject.name = "OuterRelicsHintList";
            OuterRelics.Main.LogInfo("Opened the custom mode");
        }

        public override void ExitMode()
        {
            //runs when mode is exited
            Wrapper.Close();
        }

        public override void OnEnterComputer()
        {
            //runs when player enters computer, update info that changes between computer sessions. Runs after EnterMode
        }

        public override void OnExitComputer()
        {
            //runs when the player leaves computer, after ExitMode
        }

        public override void UpdateMode()
        {
            //runs every frame this mode is active
            int changeIndex = Wrapper.UpdateList();

            if (changeIndex != 0)
            {
                keySprites[selectedItem].color = Save.GetHasKey(selectedItem) ? dimOrange : dimWhite;

                selectedItem += changeIndex;

                if (selectedItem < 0) selectedItem = HintItems.Count - 1;
                if (selectedItem >= HintItems.Count) selectedItem = 0;

                keySprites[selectedItem].color = Save.GetHasKey(selectedItem) ? orange : Color.white;
                ChangeHintText(selectedItem);
            }
        }

        public override bool AllowModeSwap()
        {
            //allows swapping modes in this mode
            return true;
        }

        public override bool AllowCancelInput()
        {
            //allows leaving the computer in this mode
            return true;
        }

        public override string GetFocusedEntryID()
        {
            //returns the ID of the selected ship log entry, but I don't use those
            return "";
        }

        private void CreateCanvasStuff()
        {
            if (keyCanvas == null)
            {
                keyCanvas = Instantiate(OuterRelics.Main.assets.LoadAsset<GameObject>("HintListCanvas"), RootObject.transform);
                keyCanvas.transform.localPosition = Vector3.zero;
                keyCanvas.transform.localRotation = Quaternion.identity;
                keyCanvas.transform.localScale = Vector3.one;

                keySprites = keyCanvas.transform.GetComponentsInChildren<Image>();

                systemText = keyCanvas.GetComponentsInChildren<Text>()[0];
                bodyText = keyCanvas.GetComponentsInChildren<Text>()[1];
                areaText = keyCanvas.GetComponentsInChildren<Text>()[2];
                locationText = keyCanvas.GetComponentsInChildren<Text>()[3];
            }

            for (int i = 0; i < 12; i++)
            {
                if (Save.GetHasKey(i))
                {
                    keySprites[i].color = dimOrange;
                }
                else
                {
                    keySprites[i].color = dimWhite;
                }
            }

            keySprites[0].color = Save.GetHasKey(0) ? orange : Color.white;
            ChangeHintText(0);
        }

        private void ChangeHintText(int id)
        {
            RandomizedPlacement placement = Items.itemPlacements[id];
            if (Save.GetHasKey(id))
            {
                systemText.text = "SYSTEM: " + placement.system;
                bodyText.text = "BODY: " + FixedBodyText(placement.body);
                areaText.text = "AREA: " + placement.locationName;
                locationText.text = "LOCATION: " + placement.spawnPointName;
            }
            else
            {
                systemText.text = "SYSTEM: ???";
                bodyText.text = "BODY: ???";
                areaText.text = "AREA: ???";
                locationText.text = "LOCATION: ???";
            }
        }

        private string FixedBodyText(string text)
        {
            ItemSpawnData spawnData = OuterRelics.Main.itemData;
            string bodyName;
            if (spawnData.bodies.ContainsKey(text))
            {
                text = spawnData.bodies[text];
            }
            else
            {
                bodyName = text;
                bodyName = bodyName.Replace("_Body", "");
                bodyName = System.Text.RegularExpressions.Regex.Replace(bodyName, "([A-Z])", " $1", System.Text.RegularExpressions.RegexOptions.Compiled).Trim();
                text = bodyName;
            }
            return text;
        }
    }
}
