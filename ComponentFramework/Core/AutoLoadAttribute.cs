using System;

namespace ComponentFramework.Core
{
    /// <summary>
    /// Flags a <see cref="Component" /> as auto-loading, so that the the <see cref="Core" /> will load and initialize it
    /// when the Run method is called.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class AutoLoadAttribute : Attribute
    {
        /// <summary>
        /// Default constructor, assumes auto-loading under every <see cref="RunMode"/>.
        /// </summary>
        public AutoLoadAttribute() : this(RunMode.Always) { }

        /// <summary>
        /// Parameterized constructor, only auto-loads if the <see cref="RunMode"/> matches the specified one.
        /// </summary>
        public AutoLoadAttribute(RunMode runModeRestriction)
        {
            RunModeRestriction = runModeRestriction;
        }

        /// <summary>
        /// The run mode restriction must match the current run mode for auto-loading to be performed.
        /// </summary>
        public RunMode RunModeRestriction { get; set; }
    }
}