public interface INavigationService
{
    void Navigate(string destination);
    object GetCurrentContext();
    void RegisterNavigationHandler(INavigationHandler handler);
}