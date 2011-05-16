namespace Encog.Neural.Prune
{
    /// <summary>
    /// Specify which network pattern to use.
    /// </summary>
    ///
    public enum NetworkPattern
    {
        /// <summary>
        /// Multilayer feedforward.
        /// </summary>
        ///
        MultiLayerFeedforward,

        /// <summary>
        /// Elman.
        /// </summary>
        ///
        Elman,

        /// <summary>
        /// Jordan.
        /// </summary>
        ///
        Jordan
    }
}