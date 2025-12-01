using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SchoolFacilityReport.Models;
using System.Collections.ObjectModel;
using Supabase.Postgrest; // 记得引用这个

namespace SchoolFacilityReport.ViewModels;

public partial class AdminDashboardViewModel : ObservableObject
{
    private readonly Supabase.Client _supabase;

    [ObservableProperty]
    bool isRefreshing;

    // 维修工看到的列表
    public ObservableCollection<Report> AllReports { get; } = new();

    public AdminDashboardViewModel(Supabase.Client client)
    {
        _supabase = client;
    }

    [RelayCommand]
    async Task LoadAllReports()
    {
        try
        {
            IsRefreshing = true;

            // 查询所有报修单
            // 逻辑：按 Urgency (紧急程度) 倒序排 -> 3(High) 会在最前面
            var response = await _supabase.From<Report>()
                .Order("urgency", Constants.Ordering.Descending)
                .Order("created_at", Constants.Ordering.Descending)
                .Get();

            AllReports.Clear();
            foreach (var item in response.Models)
            {
                AllReports.Add(item);
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

    [RelayCommand]
    async Task GoToDetails(Report report)
    {
        if (report == null) return;

        // 带着选中的 report 数据跳转到详情页
        var navigationParameter = new Dictionary<string, object>
        {
            { "Report", report }
        };

        await Shell.Current.GoToAsync(nameof(Views.ReportDetailPage), navigationParameter);
    }
}