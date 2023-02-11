using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OuterRelics
{
    /// <summary>
    /// Class that holds stats
    /// </summary>
    public class StatManager : MonoBehaviour
    {
        /// <summary>
        /// How long the run has been going
        /// </summary>
        public float timer;
        /// <summary>
        /// The loop number that the run started
        /// </summary>
        public int startingLoop;
        /// <summary>
        /// Whether or not the timer should be running or paused
        /// </summary>
        public bool runTimer;
        /// <summary>
        /// Every unique hint
        /// </summary>
        public List<int> hintIDsObtained = new();

        SaveManager save => OuterRelics.Main.saveManager;

        private void Update()
        {
            if (runTimer) timer += Time.deltaTime;
        }

        /// <summary>
        /// Loads stats from save file
        /// </summary>
        public void LoadStats()
        {
            timer = save.GetTimer();
            startingLoop = save.GetStartLoop();
            hintIDsObtained = save.GetHintIDs();

            OuterRelics.Main.LogInfo("TIMER: " + timer);
        }

        /// <summary>
        /// Adds a hint to the obtained hint list if it has not been obtained before
        /// </summary>
        /// <param name="id"></param>
        public void AddHint(int id)
        {
            if (!hintIDsObtained.Contains(id))
            {
                hintIDsObtained.Add(id);
            }
        }

        /// <summary>
        /// Gets the total number of loops the player has done
        /// </summary>
        /// <returns></returns>
        public int TotalLoops()
        {
            return (PlayerData.LoadLoopCount() - startingLoop) + 1;
        }

        /// <summary>
        /// Converts timer from number of seconds to standard xx:xx:xx.xx format
        /// </summary>
        /// <returns></returns>
        public string TimerToStandardFormat()
        {
            string standardTimer = "";
            float adjustTimer = Mathf.Floor(timer);
            float hundredths = timer - adjustTimer;
            hundredths = (float)Math.Round(hundredths, 2);
            float seconds = adjustTimer % 60;
            adjustTimer = Mathf.Floor(adjustTimer / 60);
            float minutes = adjustTimer % 60;
            float hours = Mathf.Floor(adjustTimer / 60);
            string hundredthsString = hundredths.ToString();
            hundredthsString = hundredthsString.Replace("0.", "");
            if (hundredths < 10) hundredthsString += "0";
            string secondsString = seconds.ToString();
            if (seconds < 10) secondsString = "0" + secondsString;
            string minutesString = minutes.ToString();
            if (minutes < 10) minutesString = "0" + minutesString;
            string hoursString = hours.ToString();

            standardTimer = $"{hoursString}:{minutesString}:{secondsString}.{hundredthsString}";

            return standardTimer;
        }
    }
}