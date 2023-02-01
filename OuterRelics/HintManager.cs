using System;
using Random = System.Random;

namespace OuterRelics
{
    public class HintManager
    {
        OuterRelics main;
        Random rnd = new Random();
        HintList hints;

        public HintManager()
        {
            main = OuterRelics.Main;

            hints = main.ModHelper.Storage.Load<HintList>("hints.json");
        }

        public void GenerateHints()
        {

        }

        public class HintList
        {
            public string[] preciseHints;
            public string[] vagueHints;
            public string vagueHintNothingLocation;
            public string vagueHintNothingSpecific;
            public string[] uselessHints;
            public string[] uselessHintsStranger;
        }
    }
}
