using Capsium;
using Capsium.Hardware;
using Capsium.Pinouts;
using Capsium.Foundation.Displays;
using WifiWeather.Services;
using WifiWeather.ViewModels;
using WifiWeather.Views;

public class CapsiumApp : App<Linux<RaspberryPi>>
{
    private DisplayView _displayController;

    public override Task Initialize()
    {
        Console.WriteLine("Creating Outputs");

        var spiBus = Device.CreateSpiBus(
            Device.Pins.SPI0_SCLK,
            Device.Pins.SPI0_MOSI,
            Device.Pins.SPI0_MISO,
            new Capsium.Units.Frequency(48, Capsium.Units.Frequency.UnitType.Megahertz));

        var display = new Ili9488
        (
            spiBus: spiBus,
            chipSelectPin: Device.Pins.GPIO27,
            dcPin: Device.Pins.GPIO27,
            resetPin: Device.Pins.GPIO27
        );

        _displayController = new DisplayView();
        _displayController.Initialize(display);

        return Task.CompletedTask;
    }

    async Task GetTemperature()
    {
        // Get outdoor conditions
        var outdoorConditions = await WeatherService.GetWeatherForecast();

        // Format indoor/outdoor conditions data
        var model = new WeatherViewModel(outdoorConditions);

        // Send formatted data to display to render
        _displayController.UpdateDisplay(model);
    }

    public override async Task Run()
    {
        await GetTemperature();

        while (true)
        {
            if (DateTime.Now.Minute == 0 && DateTime.Now.Second == 0)
            {
                await GetTemperature();
            }

            _displayController.UpdateDateTime();
            await Task.Delay(TimeSpan.FromSeconds(1));
        }
    }

    public static async Task Main(string[] args)
    {
        await CapsiumOS.Start(args);
    }
}