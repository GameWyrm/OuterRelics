using UnityEngine.SceneManagement;

namespace OuterRelics
{
    public class LogicFunctions
    {
        SaveManager save => OuterRelics.Main.saveManager;
        bool isTitle => SceneManager.GetActiveScene().name == "TitleScreen";

        public bool CanDoHourglassTwins()
        {
            if (isTitle)
            {
                return OuterRelics.GetConfigBool("HourglassTwins");
            }
            else return save.GetPools()[0];
        }

        public bool CanDoTimberHearth()
        {
            if (isTitle)
            {
                return OuterRelics.GetConfigBool("TimberHearth");
            }
            else return save.GetPools()[1];
        }

        public bool CanDoBrittleHollow()
        {
            if (isTitle)
            {
                return OuterRelics.GetConfigBool("BrittleHollow");
            }
            else return save.GetPools()[2];
        }

        public bool CanDoGiantsDeep()
        {
            if (isTitle)
            {
                return OuterRelics.GetConfigBool("GiantsDeep");
            }
            else return save.GetPools()[3];
        }

        public bool CanDoDarkBramble()
        {
            if (isTitle)
            {
                return OuterRelics.GetConfigBool("DarkBramble");
            }
            else return save.GetPools()[4];
        }

        public bool CanDoQuantumMoon()
        {
            if (isTitle)
            {
                return OuterRelics.GetConfigBool("QuantumMoon");
            }
            else return save.GetPools()[5];
        }

        public bool CanDoTheInterloper()
        {
            if (isTitle)
            {
                return OuterRelics.GetConfigBool("Interloper");
            }
            else return save.GetPools()[6];
        }

        public bool CanDoStranger()
        {
            if (isTitle)
            {
                return OuterRelics.GetConfigBool("Stranger");
            }
            else return save.GetPools()[7] && OuterRelics.HasDLC;
        }

        public bool CanDoDreamWorld()
        {
            if (isTitle)
            {
                return OuterRelics.GetConfigBool("DreamWorld");
            }
            else return save.GetPools()[8] && OuterRelics.HasDLC;
        }

        public bool CanDoDreamWorldStealth()
        {
            if (isTitle)
            {
                return OuterRelics.GetConfigBool("DreamWorldStealth");
            }
            else return save.GetPools()[9] && OuterRelics.HasDLC;
        }

        public bool CanDoHardMode()
        {
            if (isTitle)
            {
                return OuterRelics.GetConfigBool("HardMode");
            }
            else return save.GetPools()[10];
        }

        public bool CanDoHardModeDLC()
        {
            if (isTitle)
            {
                return OuterRelics.GetConfigBool("HardMode") && OuterRelics.HasDLC;
            }
            else return save.GetPools()[10] && OuterRelics.HasDLC;
        }

        public bool CanDoAddons()
        {
            if (isTitle)
            {
                return OuterRelics.GetConfigBool("HourglassTwins");
            }
            else return save.GetPools()[11];
        }
    }
}