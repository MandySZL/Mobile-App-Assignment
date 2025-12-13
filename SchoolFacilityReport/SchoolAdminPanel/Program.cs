using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SchoolAdminPanel;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

var url = "https://qgzjpxrpnwdnelcxrxvc.supabase.co";
var key = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InFnempweHJwbndkbmVsY3hyeHZjIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NjM5Njk4NjcsImV4cCI6MjA3OTU0NTg2N30.tYnQXsUXiYx9Nzfkge1HzTHiV0mVtGL2yw5tlsU8bNY";
var options = new Supabase.SupabaseOptions
{
    AutoRefreshToken = true,
    AutoConnectRealtime = true,
};
builder.Services.AddSingleton(provider => new Supabase.Client(url, key, options));

await builder.Build().RunAsync();
