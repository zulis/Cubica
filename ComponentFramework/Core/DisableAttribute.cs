using System;

namespace ComponentFramework.Core
{
    /// <summary>
    /// Disables a <see cref="IService"/> from auto-loading.
    /// If omitted, all services found in the calling assembly will auto-load.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface)]
    public class DisableAttribute : Attribute
    {
    }
}