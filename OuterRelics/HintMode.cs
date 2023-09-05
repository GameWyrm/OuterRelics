using System;
using System.Collections.Generic;
using UnityEngine;

namespace OuterRelics
{
    public class HintMode : ShipLogMode
    {
        public ItemListWrapper Wrapper;
        public List<Tuple<string, bool, bool, bool>> HintItems => OuterRelics.Main.HintModeList;

        public override void Initialize(ScreenPromptList centerPromptList, ScreenPromptList upperRightPromptList, OWAudioSource oneShotSource)
        {
            OuterRelics.Main.LogInfo("Custom mode created");
            //runs when mode is created
        }
        
        //runs when mode is entered
        public override void EnterMode(string entryID = "", List<ShipLogFact> revealQueue = null)
        {
            Wrapper.SetItems(HintItems);
            Wrapper.SetSelectedIndex(0);
            Wrapper.Open();
            OuterRelics.Main.LogInfo("Opened the custom mode");
        }

        public override void ExitMode()
        {
            //runs when mode is exited
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
    }
}
