namespace ComponentFramework.Core
{
    /// <summary>
    /// Provides an alternative to pre-processor instructions to differentiate Debug code from Release code.
    /// </summary>
    /// <see cref="ComponentFramework.Core.AutoLoadAttribute"/>
    public enum RunMode
    {
        /// <summary>
        /// Load always, disregarding the DEBUG pre-processor flag
        /// </summary>
        Always,
        /// <summary>
        /// Load only in Debug mode (if the DEBUG pre-processor flag is set)
        /// </summary>
        Debug,
        /// <summary>
        /// Load only in Release mode (if DEBUG pre-processor flag is NOT set)
        /// </summary>
        Release
    }
}