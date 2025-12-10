using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SchoolFacilityReport.Resources.Strings;
using System.Globalization;

namespace SchoolFacilityReport.ViewModels;

[QueryProperty(nameof(ShowBackButton), "ShowBack")]
public partial class SettingsViewModel : ObservableObject
{
    private readonly Supabase.Client _supabase;

    [ObservableProperty]
    bool showBackButton; // Default false

    [RelayCommand]
    async Task GoBack()
    {
        await Shell.Current.GoToAsync("..");
    }

    public SettingsViewModel(Supabase.Client client)
    {
        _supabase = client;
    }

    [RelayCommand]
    async Task ChangeLanguage()
    {
        // 1. 弹窗选择语言
        string action = await Shell.Current.DisplayActionSheet(
            AppResources.ChangeLang, "Cancel", null, "English", "中文", "Bahasa Melayu");

        string cultureCode = "en"; // 默认英文

        if (action == "中文") cultureCode = "zh-Hans";
        else if (action == "Bahasa Melayu") cultureCode = "ms";
        else if (action == "English") cultureCode = "en";
        else return; // 点了取消

        // 2. 设置语言
        CultureInfo customCulture = new CultureInfo(cultureCode);
        CultureInfo.DefaultThreadCurrentCulture = customCulture;
        CultureInfo.DefaultThreadCurrentUICulture = customCulture;

        // 3. 【关键一步】重启 AppShell
        // 这会重新加载整个 App 的界面，从而让所有文字（包括 TabBar）都变成新语言
        Application.Current.MainPage = new AppShell();

        // 注意：重启后会回到登录页，但因为 Supabase 记住了登录状态，用户只需点一下登录即可自动跳回
        // (如果要做的更完美，可以在登录页加个自动跳转，但目前这样是最稳妥的)
    }


    [RelayCommand]
    async Task SignOut()
    {
        bool confirm = await Shell.Current.DisplayAlert(AppResources.SettingsTitle, "Are you sure you want to log out?", "Yes", "No");
        if (!confirm) return;

        // 1. Supabase 登出
        await _supabase.Auth.SignOut();

        // 2. 清除本地可能的缓存 (可选)
        Preferences.Remove("LastOpened");

        // 3. 返回登录页
        await Shell.Current.GoToAsync("//LoginPage");
    }
}