
using System.Windows.Markup;

// Map the Prism XAML namespace URI to our shim CLR namespaces
// so that xmlns:prism="http://prismlibrary.com/" resolves to these types.
[assembly: XmlnsDefinition("http://prismlibrary.com/", "Prism.Mvvm")]
[assembly: XmlnsDefinition("http://prismlibrary.com/", "Prism.Regions")]
[assembly: XmlnsDefinition("http://prismlibrary.com/", "Prism.Interactivity")]

// Also map the old codeplex URI used by some XAML files.
[assembly: XmlnsDefinition("http://www.codeplex.com/prism", "Prism.Mvvm")]
[assembly: XmlnsDefinition("http://www.codeplex.com/prism", "Prism.Regions")]
[assembly: XmlnsDefinition("http://www.codeplex.com/prism", "Prism.Interactivity")]
