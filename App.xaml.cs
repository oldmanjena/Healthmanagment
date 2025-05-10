using System.Configuration;
using System.Data;
using System.Globalization;
using System.Windows;
using ModernWpf;

namespace Healthmanagment;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
/// 
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Kultur global auf Deutsch (Deutschland) setzen
        CultureInfo culture = new CultureInfo("de-DE");
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
        Thread.CurrentThread.CurrentCulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;
        //Hintergrund einstellen
        ModernWpf.ThemeManager.Current.ApplicationTheme = ModernWpf.ApplicationTheme.Light;
        ModernWpf.ThemeManager.Current.AccentColor = System.Windows.Media.Colors.LightBlue;
    }

   
}


