// Native replacement shim for Prism.IActiveAware interface.

using System;

namespace Prism
{
    /// <summary>
    /// Replacement for Prism.IActiveAware.
    /// Allows objects to indicate whether they are currently active.
    /// Used by CompositeCommand and view lifecycle management.
    /// </summary>
    public interface IActiveAware
    {
        /// <summary>
        /// Gets or sets whether the object is active.
        /// </summary>
        bool IsActive { get; set; }

        /// <summary>
        /// Raised when the IsActive property changes.
        /// </summary>
        event EventHandler IsActiveChanged;
    }
}
