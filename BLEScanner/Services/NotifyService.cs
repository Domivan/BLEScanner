namespace BLEScanner.Services
{
    public class NotifyService
    {
        #region Bluetooth
        public delegate Task ConnectToScannerEventHandler(bool isConnected);
        public event ConnectToScannerEventHandler ConnectToScanner;
        public async Task OnScannerConnect(bool isConnected)
        {
            if (ConnectToScanner != null) await ConnectToScanner?.Invoke(isConnected);
        }

        public delegate Task ScannerReadEventHandler(string guid);
        public event ScannerReadEventHandler ScannerRead;
        public async Task OnScannerRead(string guid)
        {
            if (ScannerRead != null) await ScannerRead?.Invoke(guid);
        }
        #endregion
    }
}