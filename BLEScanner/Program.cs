using Blazor.Bluetooth;

using BLEScanner;
using BLEScanner.Services;

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddBluetoothNavigator();

builder.Services.AddScoped<NotifyService>();
builder.Services.AddScoped<ScannerService>();

await builder.Build().RunAsync();
