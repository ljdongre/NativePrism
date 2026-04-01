using System;

namespace NativePrism.Shim
{
    public class NavigationContext
    {
        public string SourceModuleId { get; set; }
        public string TargetModuleId { get; set; }
        public string ViewName { get; set; }
        public object Parameters { get; set; }
        public DateTime NavigatedAt { get; set; }

        public NavigationContext()
        {
            NavigatedAt = DateTime.UtcNow;
        }
    }
}