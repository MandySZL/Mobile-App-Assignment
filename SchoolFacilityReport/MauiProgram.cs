using Microsoft.Extensions.Logging;
using Supabase;
using SchoolFacilityReport.Views;
using SchoolFacilityReport.ViewModels;
using CommunityToolkit.Maui; // 👈 引用工具包

namespace SchoolFacilityReport;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit() // 👈 关键：启动工具包！
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Supabase 配置
        var url = "https://qgzjpxrpnwdnelcxrxvc.supabase.co";
        var key = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InFnempweHJwbndkbmVsY3hyeHZjIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NjM5Njk4NjcsImV4cCI6MjA3OTU0NTg2N30.tYnQXsUXiYx9Nzfkge1HzTHiV0mVtGL2yw5tlsU8bNY";

        var options = new SupabaseOptions
        {
            AutoRefreshToken = true,
            AutoConnectRealtime = true,
        };

        builder.Services.AddSingleton(provider => new Supabase.Client(url, key, options));

        // 注册页面和逻辑
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<LoginViewModel>();

        builder.Services.AddTransient<RoleSelectionPage>();
        builder.Services.AddTransient<RoleSelectionViewModel>();

        builder.Services.AddTransient<StudentDashboardPage>();
        builder.Services.AddTransient<StudentDashboardViewModel>();

        builder.Services.AddTransient<MyReportsPage>();
        builder.Services.AddTransient<MyReportsViewModel>();

        builder.Services.AddTransient<AdminDashboardPage>();
        builder.Services.AddTransient<AdminDashboardViewModel>();

        builder.Services.AddTransient<ReportDetailPage>();
        builder.Services.AddTransient<ReportDetailViewModel>();

        builder.Services.AddTransient<SettingsPage>();
        builder.Services.AddTransient<SettingsViewModel>();
        builder.Services.AddSingleton<SchoolFacilityReport.Services.NotificationService>();

#if DEBUG
        builder.Logging.AddDebug();
#endif
        return builder.Build();
    }
}