using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SchoolFacilityReport.Models;
using CommunityToolkit.Maui.Views; // 引用弹窗工具
using SchoolFacilityReport.Views;  // 引用弹窗页面

namespace SchoolFacilityReport.ViewModels;

// 接收从列表页传过来的 Report 对象 和 IsAdmin 状态
[QueryProperty(nameof(Report), "Report")]
[QueryProperty(nameof(IsAdmin), "IsAdmin")]
public partial class ReportDetailViewModel : ObservableObject
{
    private readonly Supabase.Client _supabase;

    [ObservableProperty]
    Report report; // 当前正在查看的报修单

    [ObservableProperty]
    string adminNote; // Match XAML binding (Singular)

    [ObservableProperty]
    string selectedStatus; // 选中的新状态

    [ObservableProperty]
    bool isAdmin;

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
            AdminNote = "";
        }
    }

    // 👇👇👇 改名：OpenStatusPopup -> ChangeStatus 👇👇👇
    [RelayCommand]
    async Task ChangeStatus()
    {
        // 1. 创建弹窗
        var popup = new StatusSelectPopup();

        // 2. 显示弹窗并等待结果
        var result = await Shell.Current.CurrentPage.ShowPopupAsync(popup);

        // 3. 如果用户选了东西 (result 不是 null 且是字符串)
        if (result is string newStatus)
        {
            SelectedStatus = newStatus;
            // 立即更新 Report Model 以便 UI 上的 "Target Status" 按钮文字变化 (如果绑定了的话)
            // 但注意：XAML 里的按钮文字绑定的是 {Binding Report.Status}，这里我们只是改了 SelectedStatus
            // 如果想实时预览，可能需要界面调整。
            // 暂时：SelectedStatus 只用于保存中间状态，点击 Save Review 才提交。
            
            // 修正：让用户看到选了什么，这里我们可以直接更新 View Model 的 SelectedStatus
            // 然后 SaveReview 的时候提交这个 SelectedStatus
        }
    }

    // 👇👇👇 改名：UpdateReport -> SaveReview 👇👇👇
    [RelayCommand]
    async Task SaveReview()
    {
        try
        {
            // 1. 更新内存里的对象
            if (Report != null)
            {
               Report.Status = SelectedStatus;
               // Report.AdminNote = AdminNote; // Database doesn't have this column yet?
            }

            // 2. 更新数据库
            await _supabase.From<Report>()
                           .Where(x => x.Id == Report.Id)
                           .Set(x => x.Status, SelectedStatus)
                           .Update();

            await Shell.Current.DisplayAlert("Success", "状态已更新!", "OK");

            // 3. 返回上一页
            await Shell.Current.Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", "更新失败: " + ex.Message, "OK");
        }
    }
}