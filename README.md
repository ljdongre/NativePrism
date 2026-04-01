# NativePrism

A lightweight shim implementation of essential Prism functionality for .NET applications, designed to provide module management and navigation without requiring the full Prism framework.

## Overview

NativePrism provides a minimal, focused implementation of core Prism patterns:

- **Module Catalog**: Register and discover modules
- **Navigation Service**: Route between modules with typed parameters
- **Singleton Pattern**: Centralized access point via `PrismShim.Instance`

## Key Features

- ✓ Lightweight and focused - no unnecessary dependencies
- ✓ Module management and discovery
- ✓ Type-safe navigation
- ✓ Event-driven architecture for navigation
- ✓ Fully unit tested
- ✓ Example application included

## Getting Started

### Installation

```bash
# Clone the repository
git clone https://github.com/ljdongre/NativePrism.git
cd NativePrism
```

### Basic Usage

```csharp
using NativePrism.Shim;

// Get the singleton instance
var shim = PrismShim.Instance;

// Create a module
public class MyModule : IModule
{
    public string ModuleId => "MyModule";
    
    public void Initialize() 
    { 
        // Setup code here
    }
    
    public void Register() 
    { 
        // Registration code here
    }
}

// Register the module
shim.Initialize(new MyModule());

// Navigate between modules
shim.NavigationService.Navigate("SourceModule", "TargetModule", "ViewName", parameters);

// Listen to navigation events
shim.NavigationService.RegisterNavigationHandler(context => 
{
    Console.WriteLine($"Navigating from {context.SourceModuleId} to {context.TargetModuleId}");
});
```

## Project Structure

```
NativePrism/
├── NativePrism.Shim/           # Core shim implementation
│   ├── IModule.cs              # Module interface
│   ├── ModuleCatalog.cs        # Module registry
│   ├── INavigationService.cs   # Navigation contract
│   ├── NavigationService.cs    # Navigation implementation
│   ├── NavigationContext.cs    # Navigation state
│   └── PrismShim.cs            # Main entry point
├── NativePrism.Tests/          # Comprehensive unit tests
│   ├── ModuleCatalogTests.cs   # Catalog tests
│   ├── NavigationServiceTests.cs  # Navigation tests
│   └── PrismShimTests.cs       # Shim tests
├── NativePrism.Example/        # Example application
│   ├── Program.cs              # Demo application
│   └── README.md               # Example documentation
└── README.md                   # This file
```

## API Reference

### PrismShim

Main entry point providing access to module management and navigation.

```csharp
public static class PrismShim
{
    public static PrismShim Instance { get; }
    public ModuleCatalog ModuleCatalog { get; }
    public INavigationService NavigationService { get; }
    public void Initialize(IModule module);
    public static void Reset();
}
```

### ModuleCatalog

Manages module registration and discovery.

```csharp
public class ModuleCatalog
{
    public int ModuleCount { get; }
    public void RegisterModule(IModule module);
    public IModule GetModule(string moduleId);
    public IEnumerable<IModule> GetAllModules();
    public bool IsModuleRegistered(string moduleId);
    public void Clear();
}
```

### INavigationService

Provides navigation capabilities.

```csharp
public interface INavigationService
{
    void Navigate(string sourceModuleId, string targetModuleId, string viewName, object parameters = null);
    NavigationContext GetCurrentContext();
    void RegisterNavigationHandler(Action<NavigationContext> handler);
}
```

### IModule

Module contract that implementations must fulfill.

```csharp
public interface IModule
{
    string ModuleId { get; }
    void Initialize();
    void Register();
}
```

## Running Tests

```bash
# Run all tests
dotnet test

# Run specific test file
dotnet test NativePrism.Tests/ModuleCatalogTests.cs

# Run with verbose output
dotnet test --verbosity=detailed
```

## Running the Example

```bash
cd NativePrism.Example
dotnet run
```

The example application demonstrates:
- Module registration and discovery
- Navigation between modules
- Event handling for navigation
- Module catalog queries

## Design Principles

1. **Minimalism**: Only essential Prism functionality
2. **Testability**: Comprehensive test coverage
3. **Extensibility**: Clean interfaces for customization
4. **Type Safety**: Strongly-typed navigation parameters
5. **Simplicity**: Easy to understand and maintain

## Scope and Limitations

NativePrism is designed as a lightweight shim and does **NOT** include:

- Dependency injection container
- Event aggregation system
- Full UI framework integration
- Region/view composition
- Command handling
- Dialog services

For full Prism functionality, use the [official Prism library](https://github.com/PrismLibrary/Prism).

## License

This project is provided as-is for reference and educational purposes.

## Contributing

Contributions are welcome! Please feel free to submit pull requests or open issues for bugs and feature requests.

## Support

For issues and questions:
1. Check the [example application](NativePrism.Example/README.md)
2. Review the [unit tests](NativePrism.Tests/) for usage patterns
3. Open an issue on the GitHub repository