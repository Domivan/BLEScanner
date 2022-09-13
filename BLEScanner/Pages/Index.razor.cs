using BLEScanner.Services;

using Microsoft.AspNetCore.Components;

namespace BLEScanner.Pages
{
    public partial class Index : IDisposable
    {
        [Inject]
        protected ScannerService ScannerService { get; set; }

        [Inject]
        protected NotifyService NotifyService { get; set; }

        private bool isBluetoothAvailable;

        private bool ScannerConnected => ScannerService.IsConnected;
        private string resultString;

        protected override async Task OnInitializedAsync()
        {
            // return false with v 1.0.5.3
            isBluetoothAvailable = await ScannerService.IsAvailable();

            NotifyService.ConnectToScanner += NotifyService_ConnectToScanner;
            NotifyService.ScannerRead += NotifyService_ScannerRead;
        }

        private async Task NotifyService_ConnectToScanner(bool isConnected)
        {
            if (isConnected) resultString = "Scanner is connected";
            else resultString = "Scanner disconnected";

            await InvokeAsync(StateHasChanged);
        }

        private async void ConnectToScanner()
        {
            if (ScannerConnected)
            {
                resultString = "Scanner already connected";
                await InvokeAsync(StateHasChanged);
                return;
            }

            await ScannerService.Connect();
        }

        private async Task NotifyService_ScannerRead(string args)
        {
            resultString = args;
            await InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                NotifyService.ConnectToScanner -= NotifyService_ConnectToScanner;
                NotifyService.ScannerRead -= NotifyService_ScannerRead;
            }
        }
    }
}