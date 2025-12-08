using CommunityToolkit.Mvvm.ComponentModel; // 修复 ObservableObject, RelayCommand
using CommunityToolkit.Mvvm.Input;          // 修复 RelayCommand
using SchoolFacilityReport.Models;          // 修复 Profile
using SchoolFacilityReport.Views;           // 修复 StudentDashboardPage
using Supabase.Gotrue;                      // 修复 User 相关
using System.Diagnostics;
using System.Globalization;
using SchoolFacilityReport.Resources.Strings; // 引用你刚才填的翻译文件
using CommunityToolkit.Maui.Views;

namespace SchoolFacilityReport.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly Supabase.Client _supabase;

    // 修复 Email 和 Password 报错
    [ObservableProperty]
    string email;

    [ObservableProperty]
    string password;

    public LoginViewModel(Supabase.Client client)
    {
        _supabase = client;
    }

    [RelayCommand]
    async Task SignIn()
    {
        // 简单校验
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            await Shell.Current.DisplayAlert("Error", "请输入邮箱和密码", "OK");
            return;
        }

        try
        {
            // 1. 登录
            var response = await _supabase.Auth.SignIn(Email, Password);

            if (response.User != null)
            {
                var userIdStr = response.User.Id;
                var userIdGuid = Guid.Parse(userIdStr);

                // 2. 查询 Profile
                var result = await _supabase.From<Profile>()
                                            .Where(x => x.Id == userIdGuid)
                                            .Get();

                var profile = result.Models.FirstOrDefault();

                if (profile == null)
                {
                    // A. 新用户 -> 去选角色
                    await Shell.Current.GoToAsync("RoleSelectionPage");
                }
                else
                {
                    // --- 替换开始 ---

                    // 1. 创建并显示自定义弹窗
                    var popup = new SuccessPopup(profile.Role);
                    await Shell.Current.CurrentPage.ShowPopupAsync(popup);

                    // 2. 弹窗关闭后，继续跳转逻辑
                    if (profile.Role?.Trim().Equals("Maintenance", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        if (Shell.Current is AppShell appShell)
                        {
                            await appShell.SwitchToAdminRole();
                        }
                    }
                    else
                    {
                        await Shell.Current.GoToAsync("//StudentTabs");
                    }

                    // --- 替换结束 ---
                }
            }
        }
        catch (Exception ex)
        {
            // 修复 Shell 报错
            await Shell.Current.DisplayAlert("Error", "登录失败: " + ex.Message, "OK");
        }
    }

    [RelayCommand]
    async Task SignUp()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            await Shell.Current.DisplayAlert("Error", "请输入邮箱和密码", "OK");
            return;
        }

        try
        {
            var response = await _supabase.Auth.SignUp(Email, Password);

            if (response.User != null)
            {
                await Shell.Current.DisplayAlert("Success", "注册成功! 请登录。", "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", "注册失败: " + ex.Message, "OK");
        }
    }

    [RelayCommand]
    async Task SwitchLanguage()
    {
        // 1. 弹窗让用户选择语言
        string action = await Shell.Current.DisplayActionSheet(
            AppResources.ChangeLang, "Cancel", null, "English", "中文", "Bahasa Melayu");

        string cultureCode = "en"; // 默认英文

        if (action == "中文") cultureCode = "zh-Hans";
        else if (action == "Bahasa Melayu") cultureCode = "ms";
        else if (action == "English") cultureCode = "en";
        else return; // 如果点了取消，什么都不做

        // 2. 设置 App 的语言环境
        CultureInfo customCulture = new CultureInfo(cultureCode);
        CultureInfo.DefaultThreadCurrentCulture = customCulture;
        CultureInfo.DefaultThreadCurrentUICulture = customCulture;

        // 3. 重新加载页面 (刷新 UI)
        Application.Current.MainPage = new AppShell();
    }
}