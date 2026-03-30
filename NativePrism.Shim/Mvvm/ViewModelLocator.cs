// Native replacement shim for Prism.Mvvm.ViewModelLocator attached property.

using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows;

namespace Prism.Mvvm
{
    /// <summary>
    /// Replacement for Prism.Mvvm.ViewModelLocator.
    /// Provides an attached property that automatically locates and sets the DataContext
    /// of a view to its corresponding ViewModel using naming conventions.
    /// </summary>
    public static class ViewModelLocator
    {
        /// <summary>
        /// Identifies the AutoWireViewModel attached property.
        /// </summary>
        public static readonly DependencyProperty AutoWireViewModelProperty =
            DependencyProperty.RegisterAttached(
                "AutoWireViewModel",
                typeof(bool),
                typeof(ViewModelLocator),
                new PropertyMetadata(false, OnAutoWireViewModelChanged));

        /// <summary>
        /// Gets the AutoWireViewModel property value.
        /// </summary>
        /// <param name="obj">The target dependency object.</param>
        /// <returns>True if auto-wiring is enabled.</returns>
        public static bool GetAutoWireViewModel(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoWireViewModelProperty);
        }

        /// <summary>
        /// Sets the AutoWireViewModel property value.
        /// </summary>
        /// <param name="obj">The target dependency object.</param>
        /// <param name="value">True to enable auto-wiring.</param>
        public static void SetAutoWireViewModel(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoWireViewModelProperty, value);
        }

        private static void OnAutoWireViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(d))
                return;

            if ((bool)e.NewValue)
            {
                var view = d as FrameworkElement;
                if (view != null)
                {
                    var viewModel = ResolveViewModel(view.GetType());
                    if (viewModel != null)
                    {
                        view.DataContext = viewModel;
                    }
                }
            }
        }

        private static object ResolveViewModel(Type viewType)
        {
            // Convention: Views.FooView -> ViewModels.FooViewModel
            var viewName = viewType.FullName;
            var viewModelName = viewName
                .Replace(".Views.", ".ViewModels.")
                .Replace("View", "ViewModel");

            var viewModelType = viewType.Assembly.GetType(viewModelName);
            if (viewModelType == null)
                return null;

            try
            {
                return Activator.CreateInstance(viewModelType);
            }
            catch
            {
                return null;
            }
        }
    }
}
