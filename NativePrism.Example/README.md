# NativePrism Example Application

This example application demonstrates the key features of the NativePrism Shim.

## Features Demonstrated

1. **Module Initialization**: Creating and initializing modules
2. **Module Catalog**: Registering and discovering modules
3. **Navigation**: Navigating between modules with parameters
4. **Event Handling**: Responding to navigation events
5. **Module Queries**: Checking module registration status

## Running the Example

```bash
cd NativePrism.Example
dotnet run
```

## Expected Output

The application will display:

1. Module initialization messages
2. Module catalog contents (3 modules registered)
3. Navigation events as modules are navigated
4. Module registration verification
5. Final navigation context information

## What's Happening

The example:
- Creates three modules: Dashboard, Settings, and Reports
- Initializes each module through the PrismShim
- Registers a navigation event handler
- Performs navigation operations between modules
- Displays the state of the system at various points

## Key Concepts

### Module Registration
Modules are registered through the `PrismShim.Initialize()` method, which calls the module's `Initialize()` and `Register()` methods in sequence.

### Navigation
Navigation is performed using `NavigationService.Navigate()`, which accepts:
- Source module ID
- Target module ID
- View name
- Optional parameters object

### Event Handling
Navigation handlers are registered using `RegisterNavigationHandler()` and are called whenever navigation occurs.

### Module Queries
You can check if a module is registered using `IsModuleRegistered()` or retrieve modules using `GetModule()`.