namespace Eslon
{
    /// <summary>
    /// Defines codes to identify read exceptions.
    /// </summary>
    public enum EslonReadExceptionCode
    {
        /// <summary>
        /// Indicates that the source depleted before reading could finish.
        /// </summary>
        Depletion,
        /// <summary>
        /// Indicates that the text caused a syntax error.
        /// </summary>
        SyntaxError,
        /// <summary>
        /// Indicates that the text contains an invalid character.
        /// </summary>
        CorruptText,
        /// <summary>
        /// Indicates that the recursion limit has been exceeded.
        /// </summary>
        LevelCapExceeded,
        /// <summary>
        /// Indicates that a duplicate key was encountered.
        /// </summary>
        KeyExists
    }
}
