using NativePrism.Shim;

namespace NativePrism.Example
{
    // Example module implementations
    public class DashboardModule : IModule
    {
        public string ModuleId => "Dashboard";

        public void Initialize()
        {
            Console.WriteLine("[Dashboard] Initializing...");
        }

        public void Register()
        {
            Console.WriteLine("[Dashboard] Registering views and services...");
        }
    }

    public class SettingsModule : IModule
    {
        public string ModuleId => "Settings";

        public void Initialize()
        {
            Console.WriteLine("[Settings] Initializing...");
        }

        public void Register()
        {
            Console.WriteLine("[Settings] Registering views and services...");
        }
    }

    public class ReportsModule : IModule
    {
        public string ModuleId => "Reports";

        public void Initialize()
        {
            Console.WriteLine("[Reports] Initializing...");
        }

        public void Register()
        {
            Console.WriteLine("[Reports] Registering views and services...");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== NativePrism Example Application ===\n");

            // Get the singleton instance
            var shim = PrismShim.Instance;
            Console.WriteLine("✓ PrismShim instance created\n");

            // Initialize modules
            Console.WriteLine("--- Initializing Modules ---");
            shim.Initialize(new DashboardModule());
            shim.Initialize(new SettingsModule());
            shim.Initialize(new ReportsModule());
            Console.WriteLine();

            // Display module catalog
            Console.WriteLine("--- Module Catalog ---");
            Console.WriteLine($"Total modules registered: {shim.ModuleCatalog.ModuleCount}");
            foreach (var module in shim.ModuleCatalog.GetAllModules())
            {
                Console.WriteLine($"  • {{module.ModuleId}}");
            }
            Console.WriteLine();

            // Register navigation event handler
            Console.WriteLine("--- Registering Navigation Handler ---");
            shim.NavigationService.RegisterNavigationHandler(context =>
            {
                Console.WriteLine($"  ➜ Navigation event: {{context.SourceModuleId}} → {{context.TargetModuleId}}");
                Console.WriteLine($"     View: {{context.ViewName}}");
                if (context.Parameters != null)
                {
                    Console.WriteLine($"     Parameters: {{context.Parameters}}");
                }
            });
            Console.WriteLine("✓ Navigation handler registered\n");

            // Perform navigation
            Console.WriteLine("--- Performing Navigation ---");
            shim.NavigationService.Navigate("Dashboard", "Settings", "SettingsView", new { UserId = 42 });
            Console.WriteLine();

            shim.NavigationService.Navigate("Settings", "Reports", "ReportsView");
            Console.WriteLine();

            // Verify module registration
            Console.WriteLine("--- Verification ---");
            Console.WriteLine($"✓ Dashboard is registered: {{shim.ModuleCatalog.IsModuleRegistered("Dashboard")}}");
            Console.WriteLine($"✓ Settings is registered: {{shim.ModuleCatalog.IsModuleRegistered("Settings")}}");
            Console.WriteLine($"✓ Reports is registered: {{shim.ModuleCatalog.IsModuleRegistered("Reports")}}");
            Console.WriteLine($"✓ NonExistent is registered: {{shim.ModuleCatalog.IsModuleRegistered("NonExistent")}}");
            Console.WriteLine();

            // Display final navigation context
            var lastContext = shim.NavigationService.GetCurrentContext();
            Console.WriteLine("--- Last Navigation Context ---");
            Console.WriteLine($"From: {{lastContext.SourceModuleId}});
            Console.WriteLine($"To: {{lastContext.TargetModuleId}});
            Console.WriteLine($"View: {{lastContext.ViewName}});
            Console.WriteLine($"Time: {{lastContext.NavigatedAt:yyyy-MM-dd HH:mm:ss.fff}});
            Console.WriteLine();

            Console.WriteLine("=== Example Complete ===");
        }
    }
}