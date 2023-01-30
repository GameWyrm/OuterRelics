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
        Disabled,
        /// <summary>
        /// All hints are vague
        /// </summary>
        Vague,
        /// <summary>
        /// 75% vague, 25% precise
        /// </summary>
        MostlyVague,
        /// <summary>
        /// 50% vague, 50% precise
        /// </summary>
        Balanced,
        /// <summary>
        /// 25% vague, 75% precise
        /// </summary>
        MostlyPrecise,
        /// <summary>
        /// All hints are precise
        /// </summary>
        Precise
    }
}