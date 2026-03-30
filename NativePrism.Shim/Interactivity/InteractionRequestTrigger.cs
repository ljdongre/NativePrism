// Native replacement shim for Prism.Interactivity.InteractionRequestTrigger.

using System;
using System.Windows;
using Prism.Interactivity.InteractionRequest;

namespace Prism.Interactivity
{
    /// <summary>
    /// Replacement for Prism.Interactivity.InteractionRequestTrigger.
    /// Monitors an IInteractionRequest source object and invokes its actions when raised.
    /// </summary>
    public class InteractionRequestTrigger : System.Windows.Interactivity.EventTrigger
    {
        /// <summary>
        /// Gets the event name to monitor on the source object.
        /// </summary>
        /// <returns>The event name "Raised".</returns>
        protected override string GetEventName()
        {
            return "Raised";
        }
    }
}
