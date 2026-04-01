using System;
using System.Collections.Generic;

namespace NativePrism.Shim
{
    public class ModuleCatalog
    {
        private readonly Dictionary<string, Type> _modules = new Dictionary<string, Type>();

        // Register a module
        public void RegisterModule<T>(string moduleName) where T : class
        {
            if (!_modules.ContainsKey(moduleName))
            {
                _modules[moduleName] = typeof(T);
            }
        }

        // Get a module by name
        public Type GetModule(string moduleName)
        {
            _modules.TryGetValue(moduleName, out var moduleType);
            return moduleType;
        }

        // Get all registered modules
        public IDictionary<string, Type> GetAllModules()
        {
            return _modules;
        }

        // Check if a module is registered
        public bool IsModuleRegistered(string moduleName)
        {
            return _modules.ContainsKey(moduleName);
        }

        // Clear all registered modules
        public void Clear()
        {
            _modules.Clear();
        }
    }
}