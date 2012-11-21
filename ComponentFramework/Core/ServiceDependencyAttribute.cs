using System;

namespace ComponentFramework.Core
{
    /// <summary>
    /// Flags a property that it should be injected by its declaring service interface type.
    /// Properties with this attribute must have a setter, though it may be private/internal.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ServiceDependencyAttribute : Attribute
    {
        /// <summary>
        /// Defines that this dependency is optional.
        /// Optional dependencies can be null if the required service cannot be found.
        /// Otherwise, an exception will be raised while trying to inject the service dependency.
        /// </summary>
        public bool Optional { get; set; }
    }

    /// <summary>
    /// Assertion-like exception that will be thrown if a non-optional dependency cannot be resolved.
    /// </summary>
    public class MissingServiceException : Exception
    {
        const string messageFormat = "The service dependency for {0} in {1} could not be resolved.";
        /// <summary>Parameterized constructor</summary>
        public MissingServiceException(Type requiringType, Type requiredType) : base(string.Format(messageFormat, requiredType, requiringType)) { }
    }

    /// <summary>
    /// Assertion-like exception that will be thrown if a service dependency cannot be injected.
    /// </summary>
    public class MissingSetterException : Exception
    {
        const string messageFormat = "The service dependency for {0} in {1} could not be injected because a setter could not be found.";
        /// <summary>Parameterized constructor</summary>
        public MissingSetterException(Type requiringType, Type requiredType) : base(string.Format(messageFormat, requiredType, requiringType)) { }
    }
}