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
    AutoConnectRealtime = false,
};
try 
{
    var supabase = new Supabase.Client(url, key, options);
    await supabase.InitializeAsync();
    builder.Services.AddSingleton(supabase);
}
catch (Exception ex)
{
    Console.WriteLine($"Supabase Initialization Failed: {ex.Message}");
    // Fallback? Or just rethrow? If supabase fails, app is useless.
    // Registering a dummy client might avoid DI crash but app won't work.
    // Let's rely on the console log for debug.
    throw; 
}

builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<Microsoft.AspNetCore.Components.Authorization.AuthenticationStateProvider, SchoolAdminPanel.Providers.SupabaseAuthenticationStateProvider>();

await builder.Build().RunAsync();
