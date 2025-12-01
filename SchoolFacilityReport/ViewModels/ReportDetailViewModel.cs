using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SchoolFacilityReport.Models;
using CommunityToolkit.Maui.Views; // 引用弹窗工具
using SchoolFacilityReport.Views;  // 引用弹窗页面

namespace SchoolFacilityReport.ViewModels;

// 接收从列表页传过来的 Report 对象
[QueryProperty(nameof(Report), "Report")]
public partial class ReportDetailViewModel : ObservableObject
{
    private readonly Supabase.Client _supabase;

    [ObservableProperty]
    Report report; // 当前正在查看的报修单

    [ObservableProperty]
    string adminNotes; // 管理员填写的备注

    [ObservableProperty]
    string selectedStatus; // 选中的新状态

    // 以前用的列表，现在用弹窗了，这个其实可以删掉，不删也没事
    public List<string> StatusOptions { get; } = new() { "Pending", "In Progress", "Resolved" };

    public ReportDetailViewModel(Supabase.Client client)
    {
        _supabase = client;
    }

    // 当 Report 数据传过来时，自动填充界面
    partial void OnReportChanged(Report value)
    {
        if (value != null)
        {
            SelectedStatus = value.Status;
            AdminNotes = "";
        }
    }

    // 👇👇👇 新增：打开状态选择弹窗 👇👇👇
    [RelayCommand]
    async Task OpenStatusPopup()
    {
        // 1. 创建弹窗
        var popup = new StatusSelectPopup();

        // 2. 显示弹窗并等待结果
        // ShowPopupAsync 是工具包的方法，返回 object
        var result = await Shell.Current.CurrentPage.ShowPopupAsync(popup);

        // 3. 如果用户选了东西 (result 不是 null 且是字符串)
        if (result is string newStatus)
        {
            SelectedStatus = newStatus;
        }
    }

    [RelayCommand]
    async Task UpdateReport()
    {
        try
        {
            // 1. 更新内存里的对象
            Report.Status = SelectedStatus;

            // 2. 更新数据库
            await _supabase.From<Report>()
                           .Where(x => x.Id == Report.Id)
                           .Set(x => x.Status, SelectedStatus)
                           .Update();

            await Shell.Current.DisplayAlert("Success", "状态已更新!", "OK");

            // 3. 返回上一页
            // 强制关闭当前页面，返回上一页
            await Shell.Current.Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", "更新失败: " + ex.Message, "OK");
        }
    }
}