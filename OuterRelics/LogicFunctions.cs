namespace OuterRelics
{
    public class LogicFunctions
    {
        SaveManager save => OuterRelics.Main.saveManager;
        public bool CanDoHourglassTwins()
        {
            return save.GetPools()[0];
        }

        public bool CanDoTimberHearth()
        {
            return save.GetPools()[1];
        }

        public bool CanDoBrittleHollow()
        {
            return save.GetPools()[2];
        }

        public bool CanDoGiantsDeep()
        {
            return save.GetPools()[3];
        }

        public bool CanDoDarkBramble()
        {
            return save.GetPools()[4];
        }

        public bool CanDoQuantumMoon()
        {
            return save.GetPools()[5];
        }

        public bool CanDoTheInterloper()
        {
            return save.GetPools()[6];
        }

        public bool CanDoStranger()
        {
            return save.GetPools()[7] && OuterRelics.HasDLC;
        }

        public bool CanDoDreamWorld()
        {
            return save.GetPools()[8] && OuterRelics.HasDLC;
        }

        public bool CanDoDreamWorldStealth()
        {
            return save.GetPools()[9] && OuterRelics.HasDLC;
        }

        public bool CanDoHardMode()
        {
            return save.GetPools()[10];
        }

        public bool CanDoHardModeDLC()
        {
            return save.GetPools()[11] && OuterRelics.HasDLC;
        }

        public bool CanDoAddons()
        {
            return save.GetPools()[12];
        }
    }
}