using Blazor.Bluetooth;
using System.Text;

namespace BLEScanner.Services
{
    public class ScannerService
    {
        private readonly NotifyService NotifyService;
        private readonly IBluetoothNavigator Bluetooth;
        private readonly List<byte> bytes = new();
        private string guid;

        public IDevice Scanner { get; set; }
        public bool IsConnected => Scanner != null && Scanner.Gatt.Connected;
        public ScannerService(IBluetoothNavigator bluetoothNavigator, NotifyService notifyService)
        {
            Bluetooth = bluetoothNavigator;
            NotifyService = notifyService;
        }

        public async Task<bool> IsAvailable()
        {
            try { return Bluetooth != null && await Bluetooth.GetAvailability(); }
            catch { return false; }
        }

        public async Task<bool> Connect()
        {
            try
            {
                var serviceId = "0000feea-0000-1000-8000-00805f9b34fb";
                var characteristicId = "00002aa1-0000-1000-8000-00805f9b34fb";

                var query = new RequestDeviceQuery
                {
                    AcceptAllDevices = false,
                    Filters = new List<Filter>() { new Filter { Name = "BarCode Scanner BLE" } },
                    OptionalServices = new List<string>() { serviceId }
                };

                Scanner = await Bluetooth.RequestDevice(query);
                await Scanner.Gatt.Connect();

                var service = await Scanner.Gatt.GetPrimaryService(serviceId);
                var characteristic = await service.GetCharacteristic(characteristicId);

                characteristic.OnRaiseCharacteristicValueChanged += OnRaiseCharacteristicValueChanged;
                await characteristic.StartNotifications();

                Scanner.OnGattServerDisconnected += Scanner_OnGattServerDisconnected;
                await NotifyService.OnScannerConnect(true);

                return true;
            }
            catch { return false; }
        }

        private async void OnRaiseCharacteristicValueChanged(object? sender, CharacteristicEventArgs e)
        {
            try
            {
                bytes.AddRange(e.Value);

                if (bytes.Last() == 13)
                {
                    guid = Encoding.Default.GetString(bytes.Take(bytes.Count - 1).ToArray());
                    bytes.Clear();

                    await NotifyService.OnScannerRead(guid);
                }
            }
            catch { }
        }

        private async void Scanner_OnGattServerDisconnected()
        {
            try
            {
                await Scanner.Gatt.Disonnect();
                await NotifyService.OnScannerConnect(false);
            }
            catch { }
        }
    }
}