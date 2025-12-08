using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SchoolFacilityReport.Models;
using SchoolFacilityReport.Views;
using SchoolFacilityReport.Services;
using SchoolFacilityReport.Resources.Strings;
using CommunityToolkit.Maui.Views; // 用于弹窗
using System.Globalization;

namespace SchoolFacilityReport.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly Supabase.Client _supabase;
    private readonly NotificationService _notificationService; // 通知服务

    [ObservableProperty]
    string email;

    [ObservableProperty]
    string password;

    // 构造函数注入 Supabase 和 NotificationService
    public LoginViewModel(Supabase.Client client, NotificationService notificationService)
    {
        _supabase = client;
        _notificationService = notificationService;
    }

    [RelayCommand]
    async Task SignIn()
    {
        // 1. 简单校验
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            await Shell.Current.DisplayAlert(AppResources.ErrorTitle, "Please enter email and password", "OK");
            return;
        }

        try
        {
            // 2. 执行登录
            var response = await _supabase.Auth.SignIn(Email, Password);

            if (response.User != null)
            {
                var userIdStr = response.User.Id;
                var userIdGuid = Guid.Parse(userIdStr);

                // 3. 查询用户角色 (Profile)
                var result = await _supabase.From<Profile>()
                                            .Where(x => x.Id == userIdGuid)
                                            .Get();

                var profile = result.Models.FirstOrDefault();

                if (profile == null)
                {
                    // 新用户：还没选角色 -> 跳转到选择页
                    // 注意：RoleSelectionPage 是手动注册的，用普通跳转
                    await Shell.Current.GoToAsync(nameof(RoleSelectionPage));
                }
                else
                {
                    // 4. 启动通知监听服务
                    // (传入角色和ID，这样服务就知道该监听什么消息)
                    await _notificationService.StartListening(profile.Role, userIdGuid);

                    // 5. 显示漂亮的成功弹窗
                    // (注意：ShowPopupAsync 是 CommunityToolkit 提供的方法)
                    var popup = new SuccessPopup(profile.Role);
                    await Shell.Current.CurrentPage.ShowPopupAsync(popup);

                    // 6. 根据角色跳转 (弹窗关闭后执行)
                    // 使用 Trim() 和 IgnoreCase 防止数据库里的空格或大小写问题
                    if (profile.Role?.Trim().Equals("Maintenance", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        // 维修工 -> 普通推入跳转 (因为 AdminDashboardPage 是手动注册的)
                        await Shell.Current.GoToAsync(nameof(AdminDashboardPage));
                    }
                    else
                    {
                        // 学生 -> 切换 Shell 根路径 (因为 StudentTabs 在 AppShell.xaml 里)
                        await Shell.Current.GoToAsync("//StudentTabs");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(AppResources.ErrorTitle, "Login Failed: " + ex.Message, "OK");
        }
    }

    [RelayCommand]
    async Task SignUp()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            await Shell.Current.DisplayAlert(AppResources.ErrorTitle, "Please enter email and password", "OK");
            return;
        }

        try
        {
            // 注册新用户
            var response = await _supabase.Auth.SignUp(Email, Password);

            if (response.User != null)
            {
                await Shell.Current.DisplayAlert(AppResources.SuccessTitle, "Registration Successful! Please login.", "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(AppResources.ErrorTitle, "Registration Failed: " + ex.Message, "OK");
        }
    }

    // 切换语言功能
    [RelayCommand]
    async Task SwitchLanguage()
    {
        // 弹窗让用户选
        string action = await Shell.Current.DisplayActionSheet(
            AppResources.ChangeLang, "Cancel", null, "English", "中文", "Bahasa Melayu");

        string cultureCode = "en"; // 默认

        if (action == "中文") cultureCode = "zh-Hans";
        else if (action == "Bahasa Melayu") cultureCode = "ms";
        else if (action == "English") cultureCode = "en";
        else return; // 点了取消

        // 设置语言
        CultureInfo customCulture = new CultureInfo(cultureCode);
        CultureInfo.DefaultThreadCurrentCulture = customCulture;
        CultureInfo.DefaultThreadCurrentUICulture = customCulture;

        // 重启 AppShell 以刷新所有界面的语言
        Application.Current.MainPage = new AppShell();
    }
}