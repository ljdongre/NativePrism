// Native replacement shim for Prism Library region management types.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Prism.Regions
{
    /// <summary>
    /// Replacement for Prism.Regions.IViewsCollection.
    /// Represents a filtered, observable collection of views in a region.
    /// </summary>
    public interface IViewsCollection : IEnumerable<object>, INotifyCollectionChanged
    {
        /// <summary>
        /// Determines whether the collection contains a specific view.
        /// </summary>
        /// <param name="value">The view to locate.</param>
        /// <returns>True if the view is found.</returns>
        bool Contains(object value);
    }

    /// <summary>
    /// Replacement for Prism.Regions.NavigationParameters.
    /// Dictionary-based collection of navigation parameters.
    /// </summary>
    public class NavigationParameters : IEnumerable<KeyValuePair<string, object>>
    {
        private readonly Dictionary<string, object> _entries = new Dictionary<string, object>();

        /// <summary>
        /// Creates empty navigation parameters.
        /// </summary>
        public NavigationParameters() { }

        /// <summary>
        /// Creates navigation parameters from a query string.
        /// </summary>
        /// <param name="query">The query string (e.g. "?key=value").</param>
        public NavigationParameters(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return;
            var trimmed = query.TrimStart('?');
            foreach (var part in trimmed.Split('&'))
            {
                var kv = part.Split(new[] { '=' }, 2);
                if (kv.Length == 2)
                    _entries[Uri.UnescapeDataString(kv[0])] = Uri.UnescapeDataString(kv[1]);
                else if (kv.Length == 1)
                    _entries[Uri.UnescapeDataString(kv[0])] = null;
            }
        }

        /// <summary>
        /// Adds a parameter.
        /// </summary>
        /// <param name="key">The parameter key.</param>
        /// <param name="value">The parameter value.</param>
        public void Add(string key, object value) => _entries[key] = value;

        /// <summary>
        /// Gets a parameter value by key.
        /// </summary>
        /// <typeparam name="T">The expected value type.</typeparam>
        /// <param name="key">The parameter key.</param>
        /// <returns>The parameter value cast to T, or default.</returns>
        public T GetValue<T>(string key)
        {
            if (_entries.TryGetValue(key, out var value) && value is T typedValue)
                return typedValue;
            return default;
        }

        /// <summary>
        /// Checks whether a key exists.
        /// </summary>
        /// <param name="key">The parameter key.</param>
        /// <returns>True if the key exists.</returns>
        public bool ContainsKey(string key) => _entries.ContainsKey(key);

        /// <summary>
        /// Gets or sets a parameter value by key.
        /// </summary>
        /// <param name="key">The parameter key.</param>
        /// <returns>The parameter value.</returns>
        public object this[string key]
        {
            get => _entries.TryGetValue(key, out var v) ? v : null;
            set => _entries[key] = value;
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => _entries.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    /// <summary>
    /// Replacement for Prism.Regions.NavigationContext.
    /// Provides context about a navigation operation.
    /// </summary>
    public class NavigationContext
    {
        /// <summary>
        /// Creates a new NavigationContext.
        /// </summary>
        /// <param name="regionManager">The region manager.</param>
        /// <param name="uri">The target URI.</param>
        public NavigationContext(IRegionManager regionManager, Uri uri)
            : this(regionManager, uri, new NavigationParameters()) { }

        /// <summary>
        /// Creates a new NavigationContext with parameters.
        /// </summary>
        /// <param name="regionManager">The region manager.</param>
        /// <param name="uri">The target URI.</param>
        /// <param name="parameters">The navigation parameters.</param>
        public NavigationContext(IRegionManager regionManager, Uri uri, NavigationParameters parameters)
        {
            RegionManager = regionManager;
            Uri = uri;
            Parameters = parameters ?? new NavigationParameters();
        }

        /// <summary>
        /// Gets the region manager.
        /// </summary>
        public IRegionManager RegionManager { get; }

        /// <summary>
        /// Gets the target URI.
        /// </summary>
        public Uri Uri { get; }

        /// <summary>
        /// Gets the navigation parameters.
        /// </summary>
        public NavigationParameters Parameters { get; }
    }

    /// <summary>
    /// Replacement for Prism.Regions.NavigationResult.
    /// Represents the result of a navigation operation.
    /// </summary>
    public class NavigationResult
    {
        /// <summary>
        /// Creates a successful NavigationResult.
        /// </summary>
        /// <param name="context">The navigation context.</param>
        /// <param name="result">Whether navigation was successful.</param>
        public NavigationResult(NavigationContext context, bool? result)
        {
            Context = context;
            Result = result;
            Error = null;
        }

        /// <summary>
        /// Creates a NavigationResult with an error.
        /// </summary>
        /// <param name="context">The navigation context.</param>
        /// <param name="error">The error that occurred.</param>
        public NavigationResult(NavigationContext context, Exception error)
        {
            Context = context;
            Error = error;
            Result = false;
        }

        /// <summary>
        /// Gets the navigation context.
        /// </summary>
        public NavigationContext Context { get; }

        /// <summary>
        /// Gets whether the navigation was successful.
        /// </summary>
        public bool? Result { get; }

        /// <summary>
        /// Gets the error that occurred during navigation (if any).
        /// </summary>
        public Exception Error { get; }
    }

    /// <summary>
    /// Replacement for Prism.Regions.INavigationAware.
    /// Allows ViewModels to participate in navigation lifecycle.
    /// </summary>
    public interface INavigationAware
    {
        /// <summary>
        /// Called when the view is navigated to.
        /// </summary>
        /// <param name="navigationContext">The navigation context.</param>
        void OnNavigatedTo(NavigationContext navigationContext);

        /// <summary>
        /// Determines whether this instance can be the target of the navigation.
        /// </summary>
        /// <param name="navigationContext">The navigation context.</param>
        /// <returns>True if this instance can handle the navigation.</returns>
        bool IsNavigationTarget(NavigationContext navigationContext);

        /// <summary>
        /// Called when the view is navigated away from.
        /// </summary>
        /// <param name="navigationContext">The navigation context.</param>
        void OnNavigatedFrom(NavigationContext navigationContext);
    }

    /// <summary>
    /// Replacement for Prism.Regions.IConfirmNavigationRequest.
    /// Extends INavigationAware with the ability to confirm/cancel navigation.
    /// </summary>
    public interface IConfirmNavigationRequest : INavigationAware
    {
        /// <summary>
        /// Called to confirm or cancel a navigation request.
        /// </summary>
        /// <param name="navigationContext">The navigation context.</param>
        /// <param name="continuationCallback">Callback to invoke with true to continue, false to cancel.</param>
        void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback);
    }

    /// <summary>
    /// Represents a region that can hold views.
    /// </summary>
    public interface IRegion
    {
        /// <summary>
        /// Gets the name of the region.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Adds a view to the region.
        /// </summary>
        /// <param name="view">The view to add.</param>
        /// <returns>This region for fluent chaining.</returns>
        IRegion Add(object view);

        /// <summary>
        /// Removes a view from the region.
        /// </summary>
        /// <param name="view">The view to remove.</param>
        void Remove(object view);

        /// <summary>
        /// Gets all views in the region.
        /// </summary>
        IEnumerable<object> Views { get; }

        /// <summary>
        /// Gets the currently active views.
        /// </summary>
        IEnumerable<object> ActiveViews { get; }

        /// <summary>
        /// Activates a view in the region.
        /// </summary>
        /// <param name="view">The view to activate.</param>
        void Activate(object view);

        /// <summary>
        /// Deactivates a view in the region.
        /// </summary>
        /// <param name="view">The view to deactivate.</param>
        void Deactivate(object view);

        /// <summary>
        /// Navigates within this region.
        /// </summary>
        /// <param name="source">The target URI.</param>
        /// <param name="navigationCallback">Callback with the result.</param>
        void RequestNavigate(Uri source, Action<NavigationResult> navigationCallback);
    }

    /// <summary>
    /// Represents a collection of regions.
    /// </summary>
    public interface IRegionCollection : IEnumerable<IRegion>
    {
        /// <summary>
        /// Gets a region by name.
        /// </summary>
        /// <param name="regionName">The region name.</param>
        /// <returns>The region.</returns>
        IRegion this[string regionName] { get; }

        /// <summary>
        /// Checks whether a region with the specified name exists.
        /// </summary>
        /// <param name="regionName">The region name.</param>
        /// <returns>True if the region exists.</returns>
        bool ContainsRegionWithName(string regionName);

        /// <summary>
        /// Adds a region to the collection.
        /// </summary>
        /// <param name="region">The region to add.</param>
        void Add(IRegion region);

        /// <summary>
        /// Removes a region by name.
        /// </summary>
        /// <param name="regionName">The region name to remove.</param>
        /// <returns>True if the region was removed.</returns>
        bool Remove(string regionName);
    }

    /// <summary>
    /// Replacement for Prism.Regions.IRegionManager.
    /// Provides region registration, view injection, and navigation.
    /// </summary>
    public interface IRegionManager
    {
        /// <summary>
        /// Gets the collection of regions.
        /// </summary>
        IRegionCollection Regions { get; }

        /// <summary>
        /// Creates a scoped region manager.
        /// </summary>
        /// <returns>A new region manager.</returns>
        IRegionManager CreateRegionManager();

        /// <summary>
        /// Adds a view to a named region.
        /// </summary>
        /// <param name="regionName">The region name.</param>
        /// <param name="view">The view to add.</param>
        /// <returns>This region manager for fluent chaining.</returns>
        IRegionManager AddToRegion(string regionName, object view);

        /// <summary>
        /// Registers a view type with a region for automatic injection.
        /// </summary>
        /// <param name="regionName">The region name.</param>
        /// <param name="viewType">The view type.</param>
        /// <returns>This region manager for fluent chaining.</returns>
        IRegionManager RegisterViewWithRegion(string regionName, Type viewType);

        /// <summary>
        /// Registers a view factory with a region for automatic injection.
        /// </summary>
        /// <param name="regionName">The region name.</param>
        /// <param name="getContentDelegate">A factory delegate that returns the view.</param>
        /// <returns>This region manager for fluent chaining.</returns>
        IRegionManager RegisterViewWithRegion(string regionName, Func<object> getContentDelegate);

        /// <summary>
        /// Navigates to a URI within a region.
        /// </summary>
        /// <param name="regionName">The region name.</param>
        /// <param name="source">The target URI.</param>
        /// <param name="navigationCallback">Callback with the result.</param>
        void RequestNavigate(string regionName, Uri source, Action<NavigationResult> navigationCallback);

        /// <summary>
        /// Navigates to a URI within a region.
        /// </summary>
        /// <param name="regionName">The region name.</param>
        /// <param name="source">The target URI.</param>
        void RequestNavigate(string regionName, Uri source);

        /// <summary>
        /// Navigates to a view within a region by name.
        /// </summary>
        /// <param name="regionName">The region name.</param>
        /// <param name="source">The target view name.</param>
        /// <param name="navigationCallback">Callback with the result.</param>
        void RequestNavigate(string regionName, string source, Action<NavigationResult> navigationCallback);

        /// <summary>
        /// Navigates to a view within a region by name.
        /// </summary>
        /// <param name="regionName">The region name.</param>
        /// <param name="source">The target view name.</param>
        void RequestNavigate(string regionName, string source);

        /// <summary>
        /// Navigates to a URI within a region with parameters.
        /// </summary>
        /// <param name="regionName">The region name.</param>
        /// <param name="target">The target URI.</param>
        /// <param name="navigationCallback">Callback with the result.</param>
        /// <param name="navigationParameters">The navigation parameters.</param>
        void RequestNavigate(string regionName, Uri target, Action<NavigationResult> navigationCallback,
            NavigationParameters navigationParameters);

        /// <summary>
        /// Navigates to a view within a region by name with parameters.
        /// </summary>
        /// <param name="regionName">The region name.</param>
        /// <param name="target">The target view name.</param>
        /// <param name="navigationCallback">Callback with the result.</param>
        /// <param name="navigationParameters">The navigation parameters.</param>
        void RequestNavigate(string regionName, string target, Action<NavigationResult> navigationCallback,
            NavigationParameters navigationParameters);

        /// <summary>
        /// Navigates to a URI within a region with parameters.
        /// </summary>
        /// <param name="regionName">The region name.</param>
        /// <param name="target">The target URI.</param>
        /// <param name="navigationParameters">The navigation parameters.</param>
        void RequestNavigate(string regionName, Uri target, NavigationParameters navigationParameters);

        /// <summary>
        /// Navigates to a view within a region by name with parameters.
        /// </summary>
        /// <param name="regionName">The region name.</param>
        /// <param name="target">The target view name.</param>
        /// <param name="navigationParameters">The navigation parameters.</param>
        void RequestNavigate(string regionName, string target, NavigationParameters navigationParameters);
    }

    /// <summary>
    /// Default implementation of IRegion.
    /// Manages a collection of views with activation semantics.
    /// </summary>
    public class Region : IRegion
    {
        private readonly List<object> _views = new List<object>();
        private readonly List<object> _activeViews = new List<object>();

        /// <summary>
        /// Gets the name of the region.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Adds a view to the region.
        /// </summary>
        /// <param name="view">The view to add.</param>
        /// <returns>This region for fluent chaining.</returns>
        public IRegion Add(object view)
        {
            if (!_views.Contains(view))
                _views.Add(view);
            return this;
        }

        /// <summary>
        /// Removes a view from the region.
        /// </summary>
        /// <param name="view">The view to remove.</param>
        public void Remove(object view)
        {
            _views.Remove(view);
            _activeViews.Remove(view);
        }

        /// <summary>
        /// Gets all views in the region.
        /// </summary>
        public IEnumerable<object> Views => _views.AsReadOnly();

        /// <summary>
        /// Gets the currently active views.
        /// </summary>
        public IEnumerable<object> ActiveViews => _activeViews.AsReadOnly();

        /// <summary>
        /// Activates a view in the region.
        /// </summary>
        /// <param name="view">The view to activate.</param>
        public void Activate(object view)
        {
            if (_views.Contains(view) && !_activeViews.Contains(view))
                _activeViews.Add(view);
        }

        /// <summary>
        /// Deactivates a view in the region.
        /// </summary>
        /// <param name="view">The view to deactivate.</param>
        public void Deactivate(object view)
        {
            _activeViews.Remove(view);
        }

        /// <summary>
        /// Navigates within this region.
        /// </summary>
        /// <param name="source">The target URI.</param>
        /// <param name="navigationCallback">Callback with the result.</param>
        public void RequestNavigate(Uri source, Action<NavigationResult> navigationCallback)
        {
            var context = new NavigationContext(null, source);
            navigationCallback?.Invoke(new NavigationResult(context, true));
        }
    }

    /// <summary>
    /// Default implementation of IRegionCollection.
    /// </summary>
    public class RegionCollection : IRegionCollection
    {
        private readonly Dictionary<string, IRegion> _regions = new Dictionary<string, IRegion>();

        /// <summary>
        /// Gets a region by name.
        /// </summary>
        /// <param name="regionName">The region name.</param>
        /// <returns>The region.</returns>
        public IRegion this[string regionName]
        {
            get
            {
                if (_regions.TryGetValue(regionName, out var region))
                    return region;
                throw new KeyNotFoundException(string.Format("Region '{0}' not found.", regionName));
            }
        }

        /// <summary>
        /// Checks whether a region with the specified name exists.
        /// </summary>
        /// <param name="regionName">The region name.</param>
        /// <returns>True if the region exists.</returns>
        public bool ContainsRegionWithName(string regionName) => _regions.ContainsKey(regionName);

        /// <summary>
        /// Adds a region to the collection.
        /// </summary>
        /// <param name="region">The region to add.</param>
        public void Add(IRegion region) => _regions[region.Name] = region;

        /// <summary>
        /// Removes a region by name.
        /// </summary>
        /// <param name="regionName">The region name to remove.</param>
        /// <returns>True if the region was removed.</returns>
        public bool Remove(string regionName) => _regions.Remove(regionName);

        public IEnumerator<IRegion> GetEnumerator() => _regions.Values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    /// <summary>
    /// Default implementation of IRegionManager.
    /// </summary>
    public class RegionManager : IRegionManager
    {
        /// <summary>
        /// Identifies the RegionName attached property used in XAML to define regions.
        /// </summary>
        public static readonly DependencyProperty RegionNameProperty =
            DependencyProperty.RegisterAttached(
                "RegionName",
                typeof(string),
                typeof(RegionManager),
                new PropertyMetadata(null));

        /// <summary>
        /// Gets the region name for the specified element.
        /// </summary>
        /// <param name="target">The target element.</param>
        /// <returns>The region name.</returns>
        public static string GetRegionName(DependencyObject target)
        {
            return (string)target.GetValue(RegionNameProperty);
        }

        /// <summary>
        /// Sets the region name for the specified element.
        /// </summary>
        /// <param name="target">The target element.</param>
        /// <param name="value">The region name.</param>
        public static void SetRegionName(DependencyObject target, string value)
        {
            target.SetValue(RegionNameProperty, value);
        }

        /// <summary>
        /// Gets the collection of regions.
        /// </summary>
        public IRegionCollection Regions { get; } = new RegionCollection();

        /// <summary>
        /// Creates a scoped region manager.
        /// </summary>
        /// <returns>A new region manager.</returns>
        public IRegionManager CreateRegionManager() => new RegionManager();

        /// <summary>
        /// Adds a view to a named region.
        /// </summary>
        /// <param name="regionName">The region name.</param>
        /// <param name="view">The view to add.</param>
        /// <returns>This region manager for fluent chaining.</returns>
        public IRegionManager AddToRegion(string regionName, object view)
        {
            Regions[regionName].Add(view);
            return this;
        }

        /// <summary>
        /// Registers a view type with a region. This is a no-op in the shim;
        /// actual view injection must be done explicitly.
        /// </summary>
        /// <param name="regionName">The region name.</param>
        /// <param name="viewType">The view type.</param>
        /// <returns>This region manager for fluent chaining.</returns>
        public IRegionManager RegisterViewWithRegion(string regionName, Type viewType) => this;

        /// <summary>
        /// Registers a view factory with a region. This is a no-op in the shim;
        /// actual view injection must be done explicitly.
        /// </summary>
        /// <param name="regionName">The region name.</param>
        /// <param name="getContentDelegate">A factory delegate that returns the view.</param>
        /// <returns>This region manager for fluent chaining.</returns>
        public IRegionManager RegisterViewWithRegion(string regionName, Func<object> getContentDelegate) => this;

        /// <summary>
        /// Navigates to a URI within a region.
        /// </summary>
        public void RequestNavigate(string regionName, Uri source, Action<NavigationResult> navigationCallback)
        {
            RequestNavigate(regionName, source, navigationCallback, null);
        }

        /// <summary>
        /// Navigates to a URI within a region.
        /// </summary>
        public void RequestNavigate(string regionName, Uri source)
        {
            RequestNavigate(regionName, source, _ => { });
        }

        /// <summary>
        /// Navigates to a view within a region by name.
        /// </summary>
        public void RequestNavigate(string regionName, string source, Action<NavigationResult> navigationCallback)
        {
            RequestNavigate(regionName, new Uri(source, UriKind.RelativeOrAbsolute), navigationCallback);
        }

        /// <summary>
        /// Navigates to a view within a region by name.
        /// </summary>
        public void RequestNavigate(string regionName, string source)
        {
            RequestNavigate(regionName, source, _ => { });
        }

        /// <summary>
        /// Navigates to a URI within a region with parameters.
        /// </summary>
        public void RequestNavigate(string regionName, Uri target, Action<NavigationResult> navigationCallback,
            NavigationParameters navigationParameters)
        {
            var context = new NavigationContext(this, target, navigationParameters);
            try
            {
                if (Regions.ContainsRegionWithName(regionName))
                {
                    Regions[regionName].RequestNavigate(target, result =>
                        navigationCallback?.Invoke(result));
                }
                else
                {
                    navigationCallback?.Invoke(new NavigationResult(context, false));
                }
            }
            catch (Exception ex)
            {
                navigationCallback?.Invoke(new NavigationResult(context, ex));
            }
        }

        /// <summary>
        /// Navigates to a view within a region by name with parameters.
        /// </summary>
        public void RequestNavigate(string regionName, string target, Action<NavigationResult> navigationCallback,
            NavigationParameters navigationParameters)
        {
            RequestNavigate(regionName, new Uri(target, UriKind.RelativeOrAbsolute), navigationCallback, navigationParameters);
        }

        /// <summary>
        /// Navigates to a URI within a region with parameters.
        /// </summary>
        public void RequestNavigate(string regionName, Uri target, NavigationParameters navigationParameters)
        {
            RequestNavigate(regionName, target, _ => { }, navigationParameters);
        }

        /// <summary>
        /// Navigates to a view within a region by name with parameters.
        /// </summary>
        public void RequestNavigate(string regionName, string target, NavigationParameters navigationParameters)
        {
            RequestNavigate(regionName, new Uri(target, UriKind.RelativeOrAbsolute), _ => { }, navigationParameters);
        }
    }
}
