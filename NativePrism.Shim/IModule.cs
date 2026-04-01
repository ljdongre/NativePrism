public interface IModule
{
    string ModuleId { get; }
    void Initialize();
    void Register();
}