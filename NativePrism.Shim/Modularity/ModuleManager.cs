// Native replacement shim for Prism Library modularity types.

using System;

namespace Prism.Modularity
{
    /// <summary>
    /// Replacement for Prism.Modularity.IModule.
    /// Represents a module that can be initialized by the application.
    /// </summary>
    public interface IModule
    {
        /// <summary>
        /// Called when the module is loaded. Register types and initialize module services here.
        /// </summary>
        void Initialize();
    }

    /// <summary>
    /// Replacement for Prism.Modularity.IModuleManager.
    /// Provides module loading and initialization services.
    /// </summary>
    public interface IModuleManager
    {
        /// <summary>
        /// Loads and initializes the module with the given name.
        /// </summary>
        /// <param name="moduleName">The name of the module to load.</param>
        void LoadModule(string moduleName);

        /// <summary>
        /// Raised when a module has completed loading.
        /// </summary>
        event EventHandler<ModuleCompletedEventArgs> LoadModuleCompleted;

        /// <summary>
        /// Runs the module manager to load all registered modules.
        /// </summary>
        void Run();
    }

    /// <summary>
    /// Replacement for Prism.Modularity.IModuleCatalog.
    /// A catalog of module metadata used for discovery and loading.
    /// </summary>
    public interface IModuleCatalog
    {
        /// <summary>
        /// Gets the collection of module info entries.
        /// </summary>
        System.Collections.Generic.IEnumerable<ModuleInfo> Modules { get; }

        /// <summary>
        /// Adds a module to the catalog.
        /// </summary>
        /// <param name="moduleInfo">The module info entry to add.</param>
        void AddModule(ModuleInfo moduleInfo);

        /// <summary>
        /// Initializes the catalog (e.g. loads from configuration or XAML).
        /// </summary>
        void Initialize();
    }

    /// <summary>
    /// Replacement for Prism.Modularity.ModuleInfo.
    /// Describes a module entry in the module catalog.
    /// </summary>
    public class ModuleInfo
    {
        /// <summary>
        /// Gets or sets the name of the module.
        /// </summary>
        public string ModuleName { get; set; }

        /// <summary>
        /// Gets or sets the type name of the module.
        /// </summary>
        public string ModuleType { get; set; }

        /// <summary>
        /// Gets or sets the initialization mode.
        /// </summary>
        public InitializationMode InitializationMode { get; set; }

        /// <summary>
        /// Creates a new ModuleInfo.
        /// </summary>
        public ModuleInfo() { }

        /// <summary>
        /// Creates a new ModuleInfo with the specified name and type.
        /// </summary>
        /// <param name="name">The module name.</param>
        /// <param name="type">The module type name.</param>
        public ModuleInfo(string name, string type)
        {
            ModuleName = name;
            ModuleType = type;
        }
    }

    /// <summary>
    /// Replacement for Prism.Modularity.InitializationMode.
    /// </summary>
    public enum InitializationMode
    {
        /// <summary>
        /// Module is loaded when available.
        /// </summary>
        WhenAvailable,

        /// <summary>
        /// Module is loaded on demand.
        /// </summary>
        OnDemand
    }

    /// <summary>
    /// Event args for module load completion.
    /// </summary>
    public class ModuleCompletedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the module info of the loaded module.
        /// </summary>
        public ModuleInfo ModuleInfo { get; }

        /// <summary>
        /// Gets any error that occurred during loading.
        /// </summary>
        public Exception Error { get; }

        /// <summary>
        /// Gets whether the load was successful.
        /// </summary>
        public bool IsErrorHandled { get; set; }

        /// <summary>
        /// Creates a new ModuleCompletedEventArgs.
        /// </summary>
        /// <param name="moduleInfo">The module info.</param>
        /// <param name="error">Optional error.</param>
        public ModuleCompletedEventArgs(ModuleInfo moduleInfo, Exception error)
        {
            ModuleInfo = moduleInfo;
            Error = error;
        }
    }
}
