namespace OuterRelics
{
    /// <summary>
    /// The ratio of vague to precise hints
    /// </summary>
    public enum HintDifficulty
    {
        /// <summary>
        /// All hints are useless hints
        /// </summary>
        Disabled = -100,
        /// <summary>
        /// All hints are vague
        /// </summary>
        AllVague = 100,
        /// <summary>
        /// 75% vague, 25% precise
        /// </summary>
        MostlyVague = 75,
        /// <summary>
        /// 50% vague, 50% precise
        /// </summary>
        Balanced = 50,
        /// <summary>
        /// 25% vague, 75% precise
        /// </summary>
        MostlyPrecise = 25,
        /// <summary>
        /// All hints are precise
        /// </summary>
        AllPrecise = 0
    }
}