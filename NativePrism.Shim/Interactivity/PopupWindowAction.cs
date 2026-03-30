// Native replacement shim for Prism.Interactivity.PopupWindowAction.

using System;
using System.Windows;
using Prism.Interactivity.InteractionRequest;

namespace Prism.Interactivity
{
    /// <summary>
    /// Replacement for Prism.Interactivity.PopupWindowAction.
    /// A TriggerAction that opens a popup window when an InteractionRequest is raised.
    /// </summary>
    public class PopupWindowAction : System.Windows.Interactivity.TriggerAction<FrameworkElement>
    {
        /// <summary>
        /// Gets or sets the content of the window.
        /// </summary>
        public FrameworkElement WindowContent
        {
            get { return (FrameworkElement)GetValue(WindowContentProperty); }
            set { SetValue(WindowContentProperty, value); }
        }

        public static readonly DependencyProperty WindowContentProperty =
            DependencyProperty.Register("WindowContent", typeof(FrameworkElement),
                typeof(PopupWindowAction), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets whether the popup is modal.
        /// </summary>
        public bool IsModal
        {
            get { return (bool)GetValue(IsModalProperty); }
            set { SetValue(IsModalProperty, value); }
        }

        public static readonly DependencyProperty IsModalProperty =
            DependencyProperty.Register("IsModal", typeof(bool),
                typeof(PopupWindowAction), new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets whether the popup centers over the associated object.
        /// </summary>
        public bool CenterOverAssociatedObject
        {
            get { return (bool)GetValue(CenterOverAssociatedObjectProperty); }
            set { SetValue(CenterOverAssociatedObjectProperty, value); }
        }

        public static readonly DependencyProperty CenterOverAssociatedObjectProperty =
            DependencyProperty.Register("CenterOverAssociatedObject", typeof(bool),
                typeof(PopupWindowAction), new PropertyMetadata(false));

        protected override void Invoke(object parameter)
        {
            var args = parameter as InteractionRequestedEventArgs;
            if (args == null) return;

            var window = GetWindow(args.Context);
            var callback = args.Callback;

            window.Closed += (s, e) => callback?.Invoke();

            if (IsModal)
                window.ShowDialog();
            else
                window.Show();
        }

        /// <summary>
        /// Gets or creates the window for the interaction.
        /// Override to customize window creation.
        /// </summary>
        protected virtual Window GetWindow(INotification notification)
        {
            var window = CreateDefaultWindow(notification);
            return window;
        }

        /// <summary>
        /// Creates a default window for the notification.
        /// </summary>
        protected Window CreateDefaultWindow(INotification notification)
        {
            var window = new Window
            {
                Title = notification.Title,
                SizeToContent = SizeToContent.WidthAndHeight
            };

            if (WindowContent != null)
            {
                WindowContent.DataContext = notification;
                window.Content = WindowContent;
            }

            return window;
        }

        /// <summary>
        /// Prepares the content for the window, setting up IInteractionRequestAware if applicable.
        /// </summary>
        protected void PrepareContentForWindow(INotification notification, Window window)
        {
            if (WindowContent != null)
            {
                var aware = WindowContent as IInteractionRequestAware;
                if (aware != null)
                {
                    aware.Notification = notification;
                    aware.FinishInteraction = () => window.Close();
                }
                window.Content = WindowContent;
            }
        }
    }
}