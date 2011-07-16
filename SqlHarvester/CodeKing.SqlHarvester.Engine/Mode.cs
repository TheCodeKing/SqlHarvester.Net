namespace CodeKing.SqlHarvester
{
    /// <summary>
    /// Defines the execution mode.
    /// </summary>
    public enum Mode
    {
        /// <summary>
        /// The mode is undefined.
        /// </summary>
        NotSet,
        /// <summary>
        /// Seeds content into a target database.
        /// </summary>
        Import,
        /// <summary>
        /// Scripts content from source database.
        /// </summary>
        Export
    }
}
