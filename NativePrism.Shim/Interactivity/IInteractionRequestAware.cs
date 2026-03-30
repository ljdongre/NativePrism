// Native replacement shim for Prism.Interactivity.InteractionRequest.IInteractionRequestAware.

using System;

namespace Prism.Interactivity.InteractionRequest
{
    /// <summary>
    /// Replacement for Prism.Interactivity.InteractionRequest.IInteractionRequestAware.
    /// Implemented by views that participate in interaction requests.
    /// </summary>
    public interface IInteractionRequestAware
    {
        /// <summary>
        /// Gets or sets the notification context.
        /// </summary>
        INotification Notification { get; set; }

        /// <summary>
        /// Gets or sets the action to complete the interaction.
        /// </summary>
        Action FinishInteraction { get; set; }
    }
}
