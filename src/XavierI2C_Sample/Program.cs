using Capsium;
using Capsium.Foundation.Sensors.Atmospheric;
using Capsium.Foundation.Sensors.Motion;
using Capsium.Pinouts;
using Capsium.Units;
using System;
using System.Threading.Tasks;
using Capsium.Foundation;

namespace XavierI2C_Sample
{
    class CapsiumApp : App<Linux<JetsonXavierAGX>>
    {
        private Bno055 _bno055;
        private Ccs811 _ccs811;
        private Si70xx _si7021;
        private Adxl345 _adxl345;

        public static async Task Main(string[] args)
        {
            await CapsiumOS.Start(args);
        }

        public override Task Initialize()
        {
            // Note the Xavier uses bus 8 for pins 3 & 5
            var bus = Device.CreateI2cBus(8);

            _bno055 = new Bno055(bus);
            _bno055.EulerOrientationUpdated += OnEulerOrientationUpdated; 
            _bno055.StartUpdating(TimeSpan.FromSeconds(1));

            _ccs811 = new Ccs811(bus);
            _ccs811.Updated += AirConditionsUpdated;
            _ccs811.StartUpdating(TimeSpan.FromSeconds(5));

            _si7021 = new Si70xx(bus);
            _si7021.Updated += TempHumidityUpdated; 
            _si7021.StartUpdating(TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }

        private void TempHumidityUpdated(object sender, IChangeResult<(Temperature? Temperature, RelativeHumidity? Humidity)> e)
        {
            Console.WriteLine($"Temp:{e.New.Temperature.Value.Fahrenheit}F Humidity:{e.New.Humidity.Value.Percent}%");
        }

        private void AirConditionsUpdated(object sender, IChangeResult<(Concentration? Co2, Concentration? Voc)> e)
        {
            Console.WriteLine($"CO2:{e.New.Co2.Value.PartsPerMillion}PPM VOC:{e.New.Voc.Value.PartsPerMillion}PPM");
        }

        private void OnEulerOrientationUpdated(object sender, IChangeResult<Capsium.Foundation.Spatial.EulerAngles> e)
        {
            Console.WriteLine($"H:{e.New.Heading} P:{e.New.Pitch} R:{e.New.Roll}");
        }
    }
}
