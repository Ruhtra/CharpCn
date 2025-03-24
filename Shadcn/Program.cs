using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using shadcn.Components.Priv.Portal;
using Shadcn;
using TwMerge;



var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");



builder.Services.AddPortals();
builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<ToastService>();
builder.Services.AddScoped<AccordionService>();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();




var a = new TwsMerge().Merge("bg-red-500 bg-yellow-200");

Console.WriteLine(a);
