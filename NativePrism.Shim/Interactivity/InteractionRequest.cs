// Native replacement shim for Prism Library interactivity types.

using System;

namespace Prism.Interactivity.InteractionRequest
{
    /// <summary>
    /// Replacement for Prism.Interactivity.InteractionRequest.INotification.
    /// Represents a notification message.
    /// </summary>
    public interface INotification
    {
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Gets or sets the content/message.
        /// </summary>
        object Content { get; set; }
    }

    /// <summary>
    /// Replacement for Prism.Interactivity.InteractionRequest.Notification.
    /// Default implementation of INotification.
    /// </summary>
    public class Notification : INotification
    {
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the content/message.
        /// </summary>
        public object Content { get; set; }
    }

    /// <summary>
    /// Replacement for Prism.Interactivity.InteractionRequest.IConfirmation.
    /// Represents a confirmation dialog request.
    /// </summary>
    public interface IConfirmation : INotification
    {
        /// <summary>
        /// Gets or sets the user's confirmation result.
        /// </summary>
        bool Confirmed { get; set; }
    }

    /// <summary>
    /// Replacement for Prism.Interactivity.InteractionRequest.Confirmation.
    /// Default implementation of IConfirmation.
    /// </summary>
    public class Confirmation : Notification, IConfirmation
    {
        /// <summary>
        /// Gets or sets the user's confirmation result.
        /// </summary>
        public bool Confirmed { get; set; }
    }

    /// <summary>
    /// Replacement for Prism.Interactivity.InteractionRequest.IInteractionRequest.
    /// </summary>
    public interface IInteractionRequest
    {
        /// <summary>
        /// Raised when an interaction is requested.
        /// </summary>
        event EventHandler<InteractionRequestedEventArgs> Raised;
    }

    /// <summary>
    /// Replacement for Prism.Interactivity.InteractionRequest.InteractionRequestedEventArgs.
    /// </summary>
    public class InteractionRequestedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the notification context.
        /// </summary>
        public INotification Context { get; }

        /// <summary>
        /// Gets the callback to invoke when the interaction is complete.
        /// </summary>
        public Action Callback { get; }

        /// <summary>
        /// Creates a new InteractionRequestedEventArgs.
        /// </summary>
        /// <param name="context">The notification context.</param>
        /// <param name="callback">The completion callback.</param>
        public InteractionRequestedEventArgs(INotification context, Action callback)
        {
            Context = context;
            Callback = callback;
        }
    }

    /// <summary>
    /// Replacement for Prism.Interactivity.InteractionRequest.InteractionRequest&lt;T&gt;.
    /// Raises interaction requests from ViewModels to the View.
    /// </summary>
    /// <typeparam name="T">The notification type.</typeparam>
    public class InteractionRequest<T> : IInteractionRequest where T : INotification
    {
        /// <summary>
        /// Raised when an interaction is requested.
        /// </summary>
        public event EventHandler<InteractionRequestedEventArgs> Raised;

        /// <summary>
        /// Raises the interaction request with a notification and callback.
        /// </summary>
        /// <param name="context">The notification context.</param>
        /// <param name="callback">The callback to invoke when the interaction is complete.</param>
        public void Raise(T context, Action<T> callback)
        {
            Raised?.Invoke(this, new InteractionRequestedEventArgs(
                context,
                () => callback?.Invoke(context)));
        }

        /// <summary>
        /// Raises the interaction request with a notification and no callback.
        /// </summary>
        /// <param name="context">The notification context.</param>
        public void Raise(T context)
        {
            Raise(context, _ => { });
        }
    }
}
