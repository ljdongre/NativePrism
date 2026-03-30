
// Native replacement shim for Prism Library event aggregation types.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Prism.Events
{
    /// <summary>
    /// Replacement for Prism.Events.IEventAggregator.
    /// Provides a loosely-coupled pub/sub messaging service.
    /// </summary>
    public interface IEventAggregator
    {
        /// <summary>
        /// Gets or creates an event instance of the given type.
        /// </summary>
        /// <typeparam name="TEventType">The type of event to get.</typeparam>
        /// <returns>The event instance.</returns>
        TEventType GetEvent<TEventType>() where TEventType : EventBase, new();
    }

    /// <summary>
    /// Replacement for Prism.Events.EventAggregator.
    /// Thread-safe event aggregator implementation.
    /// </summary>
    public class EventAggregator : IEventAggregator
    {
        private readonly Dictionary<Type, EventBase> _events = new Dictionary<Type, EventBase>();
        private readonly object _lock = new object();

        /// <summary>
        /// Gets or creates an event instance of the given type.
        /// </summary>
        /// <typeparam name="TEventType">The type of event to get.</typeparam>
        /// <returns>The event instance.</returns>
        public TEventType GetEvent<TEventType>() where TEventType : EventBase, new()
        {
            lock (_lock)
            {
                if (!_events.TryGetValue(typeof(TEventType), out var existingEvent))
                {
                    var newEvent = new TEventType();
                    _events[typeof(TEventType)] = newEvent;
                    return newEvent;
                }
                return (TEventType)existingEvent;
            }
        }
    }

    /// <summary>
    /// Base class for all events managed by the EventAggregator.
    /// </summary>
    public abstract class EventBase { }

    /// <summary>
    /// Defines the threading options for subscription callbacks.
    /// </summary>
    public enum ThreadOption
    {
        /// <summary>
        /// The callback is invoked on the same thread as the publisher.
        /// </summary>
        PublisherThread,

        /// <summary>
        /// The callback is invoked on the UI thread.
        /// </summary>
        UIThread,

        /// <summary>
        /// The callback is invoked on a background thread.
        /// </summary>
        BackgroundThread
    }

    /// <summary>
    /// Token returned by Subscribe that can be used to unsubscribe.
    /// </summary>
    public class SubscriptionToken : IDisposable
    {
        private Action<SubscriptionToken> _unsubscribeAction;

        /// <summary>
        /// Creates a new SubscriptionToken.
        /// </summary>
        /// <param name="unsubscribeAction">Action to invoke when disposing.</param>
        public SubscriptionToken(Action<SubscriptionToken> unsubscribeAction)
        {
            _unsubscribeAction = unsubscribeAction;
        }

        /// <summary>
        /// Unsubscribes the associated handler.
        /// </summary>
        public void Dispose()
        {
            var action = _unsubscribeAction;
            _unsubscribeAction = null;
            action?.Invoke(this);
        }
    }

    /// <summary>
    /// Replacement for Prism.Events.PubSubEvent&lt;TPayload&gt;.
    /// Provides a typed publish/subscribe event.
    /// </summary>
    /// <typeparam name="TPayload">The type of the event payload.</typeparam>
    public class PubSubEvent<TPayload> : EventBase
    {
        private readonly List<Subscription<TPayload>> _subscriptions = new List<Subscription<TPayload>>();
        private readonly object _lock = new object();

        /// <summary>
        /// Subscribes a handler to this event.
        /// </summary>
        /// <param name="action">The handler to invoke on publish.</param>
        /// <returns>A token that can be used to unsubscribe.</returns>
        public SubscriptionToken Subscribe(Action<TPayload> action)
        {
            return Subscribe(action, ThreadOption.PublisherThread, false, null);
        }

        /// <summary>
        /// Subscribes a handler to this event on the specified thread.
        /// </summary>
        /// <param name="action">The handler to invoke on publish.</param>
        /// <param name="threadOption">The thread on which to invoke the handler.</param>
        /// <returns>A token that can be used to unsubscribe.</returns>
        public SubscriptionToken Subscribe(Action<TPayload> action, ThreadOption threadOption)
        {
            return Subscribe(action, threadOption, false, null);
        }

        /// <summary>
        /// Subscribes a handler to this event with keep-alive and filter options.
        /// </summary>
        /// <param name="action">The handler to invoke on publish.</param>
        /// <param name="threadOption">The thread on which to invoke the handler.</param>
        /// <param name="keepSubscriberReferenceAlive">When true, holds a strong reference to the subscriber.</param>
        /// <returns>A token that can be used to unsubscribe.</returns>
        public SubscriptionToken Subscribe(Action<TPayload> action, ThreadOption threadOption,
            bool keepSubscriberReferenceAlive)
        {
            return Subscribe(action, threadOption, keepSubscriberReferenceAlive, null);
        }

        /// <summary>
        /// Subscribes a handler to this event with full options.
        /// </summary>
        /// <param name="action">The handler to invoke on publish.</param>
        /// <param name="threadOption">The thread on which to invoke the handler.</param>
        /// <param name="keepSubscriberReferenceAlive">When true, holds a strong reference to the subscriber.</param>
        /// <param name="filter">Optional filter predicate.</param>
        /// <returns>A token that can be used to unsubscribe.</returns>
        public SubscriptionToken Subscribe(Action<TPayload> action, ThreadOption threadOption,
            bool keepSubscriberReferenceAlive, Predicate<TPayload> filter)
        {
            var token = new SubscriptionToken(Unsubscribe);
            var subscription = new Subscription<TPayload>(action, filter, token, threadOption);
            lock (_lock)
            {
                _subscriptions.Add(subscription);
            }
            return token;
        }

        /// <summary>
        /// Publishes the event with the given payload.
        /// </summary>
        /// <param name="payload">The payload to send to subscribers.</param>
        public void Publish(TPayload payload)
        {
            List<Subscription<TPayload>> subscriptions;
            lock (_lock)
            {
                subscriptions = _subscriptions.ToList();
            }

            foreach (var sub in subscriptions)
            {
                if (sub.Filter == null || sub.Filter(payload))
                {
                    switch (sub.ThreadOption)
                    {
                        case ThreadOption.UIThread:
                            if (System.Windows.Application.Current?.Dispatcher != null)
                            {
                                System.Windows.Application.Current.Dispatcher.BeginInvoke(sub.Action, payload);
                            }
                            else
                            {
                                sub.Action(payload);
                            }
                            break;

                        case ThreadOption.BackgroundThread:
                            System.Threading.ThreadPool.QueueUserWorkItem(_ => sub.Action(payload));
                            break;

                        default:
                            sub.Action(payload);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Unsubscribes a handler by its token.
        /// </summary>
        /// <param name="token">The subscription token.</param>
        public void Unsubscribe(SubscriptionToken token)
        {
            lock (_lock)
            {
                _subscriptions.RemoveAll(s => s.Token == token);
            }
        }

        /// <summary>
        /// Unsubscribes a handler by its action delegate.
        /// </summary>
        /// <param name="action">The handler to remove.</param>
        public void Unsubscribe(Action<TPayload> action)
        {
            lock (_lock)
            {
                _subscriptions.RemoveAll(s => s.Action == action);
            }
        }
    }

    /// <summary>
    /// Non-generic PubSubEvent for events with no payload.
    /// </summary>
    public class PubSubEvent : EventBase
    {
        private readonly List<Subscription> _subscriptions = new List<Subscription>();
        private readonly object _lock = new object();

        /// <summary>
        /// Subscribes a handler to this event.
        /// </summary>
        /// <param name="action">The handler to invoke on publish.</param>
        /// <returns>A token that can be used to unsubscribe.</returns>
        public SubscriptionToken Subscribe(Action action)
        {
            var token = new SubscriptionToken(Unsubscribe);
            var subscription = new Subscription(action, token);
            lock (_lock)
            {
                _subscriptions.Add(subscription);
            }
            return token;
        }

        /// <summary>
        /// Publishes the event.
        /// </summary>
        public void Publish()
        {
            List<Subscription> subscriptions;
            lock (_lock)
            {
                subscriptions = _subscriptions.ToList();
            }

            foreach (var sub in subscriptions)
            {
                sub.Action();
            }
        }

        /// <summary>
        /// Unsubscribes a handler by its token.
        /// </summary>
        /// <param name="token">The subscription token.</param>
        public void Unsubscribe(SubscriptionToken token)
        {
            lock (_lock)
            {
                _subscriptions.RemoveAll(s => s.Token == token);
            }
        }

        /// <summary>
        /// Unsubscribes a handler by its action delegate.
        /// </summary>
        /// <param name="action">The handler to remove.</param>
        public void Unsubscribe(Action action)
        {
            lock (_lock)
            {
                _subscriptions.RemoveAll(s => s.Action == action);
            }
        }
    }

    internal class Subscription<TPayload>
    {
        public Action<TPayload> Action { get; }
        public Predicate<TPayload> Filter { get; }
        public SubscriptionToken Token { get; }
        public ThreadOption ThreadOption { get; }

        public Subscription(Action<TPayload> action, Predicate<TPayload> filter,
            SubscriptionToken token, ThreadOption threadOption)
        {
            Action = action;
            Filter = filter;
            Token = token;
            ThreadOption = threadOption;
        }
    }

    internal class Subscription
    {
        public Action Action { get; }
        public SubscriptionToken Token { get; }

        public Subscription(Action action, SubscriptionToken token)
        {
            Action = action;
            Token = token;
        }
    }
}
