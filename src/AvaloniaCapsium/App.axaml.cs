using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AvaloniaCapsium.ViewModels;
using AvaloniaCapsium.Views;
using Capsium;
using Capsium.Hardware;
using Capsium.Pinouts;
using System.Threading.Tasks;

namespace AvaloniaCapsium
{
    public partial class App : AvaloniaCapsiumApplication<Linux<RaspberryPi>>
    {
        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);

            LoadCapsiumOS();
        }

        public override Task InitializeCapsium()
        {
            var r = Resolver.Services.Get<ICapsiumDevice>();

            if (r == null)
            {
                Resolver.Log.Info("ICapsiumDevice is null");
            }
            else
            {
                Resolver.Log.Info($"ICapsiumDevice is {r.GetType().Name}");
            }

            var led = Device.Pins.Pin40.CreateDigitalOutputPort(false);

            if (led == null)
            {
                Resolver.Log.Info("led is null");
            }
            else
            {
                Resolver.Log.Info($"led is {led.GetType().Name}");
                Resolver.Services.Add<IDigitalOutputPort>(led);
            }

            return Task.CompletedTask;
        }
    }
}