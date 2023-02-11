using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace OuterRelics
{
    public class EndingSceneStats : MonoBehaviour
    {
        public Text timeText;
        public Text hintsText;
        public Text loopText;

        StatManager stats => OuterRelics.Main.statManager;

        private void Start()
        {
            OuterRelics.Main.LogInfo("Stats display loaded!");
            OuterRelics.Main.prepareSaveReset = true;

            StartCoroutine(StartMusic());
        }

        private void Update()
        {
            if (!(Locator.GetEyeStateManager().GetState() == EyeState.AboardVessel))
            {
                OuterRelics.Main.prepareSaveReset = false;
                gameObject.SetActive(false);
            }
        }

        IEnumerator StartMusic()
        {
            float startTime = Time.frameCount;
            yield return new WaitUntil(() => Time.frameCount >= startTime + 15f);
            
            Font menuFont = null;
            Material menuMaterial = null;
            foreach (Font font in Resources.FindObjectsOfTypeAll<Font>())
            {
                if (font.name == "Adobe - SerifGothicStd-ExtraBold") menuFont = font;
            }
            menuMaterial = GameObject.Find("PauseMenu").GetComponentInChildren<Text>(true).material;

            OuterRelics.Main.LogInfo("FONT: " + menuFont.name);

            foreach (Text text in GetComponentsInChildren<Text>())
            {
                if (menuFont != null) text.font = menuFont;
                if (menuMaterial != null) text.material = menuMaterial;
            }

            timeText = GameObject.Find("TimeStat").GetComponent<Text>();
            hintsText = GameObject.Find("HintsStat").GetComponent<Text>();
            loopText = GameObject.Find("LoopsStat").GetComponent<Text>();

            OuterRelics.Main.LogInfo($"text: {timeText != null}, hints: {hintsText != null}, loop: {loopText != null}, mat: {menuMaterial.name}");

            if (timeText != null) timeText.text = stats.TimerToStandardFormat();
            if (hintsText != null) hintsText.text = stats.hintIDsObtained.Count.ToString();
            if (loopText != null) loopText.text = stats.TotalLoops().ToString();

            if (OuterRelics.GetConfigBool("StatMusic")) GetComponent<AudioSource>().Play();
        }
    }
}