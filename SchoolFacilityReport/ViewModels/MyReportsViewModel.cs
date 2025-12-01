using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SchoolFacilityReport.Models;
using System.Collections.ObjectModel;
// 👇 修复点：使用 Supabase 下面的 Postgrest
using Supabase.Postgrest;

namespace SchoolFacilityReport.ViewModels;

public partial class MyReportsViewModel : ObservableObject
{
    private readonly Supabase.Client _supabase;

    [ObservableProperty]
    bool isRefreshing;

    public ObservableCollection<Report> ReportsList { get; } = new();

    public MyReportsViewModel(Supabase.Client client)
    {
        _supabase = client;
    }

    [RelayCommand]
    async Task LoadData()
    {
        try
        {
            IsRefreshing = true;

            // 安全检查：如果没登录，直接退出
            if (_supabase.Auth.CurrentUser == null) return;

            // 👇【漏掉的就是这一行】定义 userIdStr
            var userIdStr = _supabase.Auth.CurrentUser.Id;

            // 把 String 转成 Guid
            var userIdGuid = Guid.Parse(userIdStr);

            // 1. 查询数据库
            var response = await _supabase.From<Report>()
                .Where(x => x.UserId == userIdGuid)
                .Order("created_at", Supabase.Postgrest.Constants.Ordering.Descending)
                .Get();

            // 2. 刷新列表
            ReportsList.Clear();
            foreach (var item in response.Models)
            {
                ReportsList.Add(item);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", "加载失败: " + ex.Message, "OK");
        }
        finally
        {
            IsRefreshing = false;
        }
    }
}