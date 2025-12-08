using CommunityToolkit.Maui.Alerts; // 用于显示 Toast
using CommunityToolkit.Maui.Core;   // 用于 Toast 样式
using SchoolFacilityReport.Models;
using Supabase.Realtime;            // 用于实时监听

namespace SchoolFacilityReport.Services;

public class NotificationService
{
    private readonly Supabase.Client _supabase;
    private bool _isListening = false;

    public NotificationService(Supabase.Client client)
    {
        _supabase = client;
    }

    // 启动监听 (登录成功后调用)
    public async Task StartListening(string userRole, Guid currentUserId)
    {
        if (_isListening) return; // 防止重复订阅

        try
        {
            await _supabase.Realtime.ConnectAsync();

            // 订阅 reports 表的所有变动
            var channel = _supabase.From<Report>();

            // 🎯 场景 A: 维修工 (Admin) -> 监听 "INSERT" (新增报修)
            if (userRole == "Maintenance")
            {
                channel.On(Supabase.Realtime.PostgresChanges.PostgresChangesOptions.ListenType.Inserts, (sender, change) =>
                {
                    // change.Model<T>() 获取强类型对象
                    var newReport = change.Model<Report>();
                    ShowToast($"🔔 新报修任务: {newReport.Category}");
                });
            }

            // 🎯 场景 B: 学生 (Student) -> 监听 "UPDATE" (状态更新)
            if (userRole == "Student" || userRole == "Staff")
            {
                channel.On(Supabase.Realtime.PostgresChanges.PostgresChangesOptions.ListenType.Updates, (sender, change) =>
                {
                    var updatedReport = change.Model<Report>();

                    // 关键过滤：只提醒 "我自己" 提交的单子
                    if (updatedReport.UserId == currentUserId)
                    {
                        ShowToast($"🔔 状态更新: 你的 {updatedReport.Category} 报修单现在是 {updatedReport.Status}");
                    }
                });
            }

            _isListening = true;
            Console.WriteLine("✅ Notification Service Started!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Notification Error: {ex.Message}");
        }
    }

    // 显示漂亮的 Toast 提示
    private async void ShowToast(string message)
    {
        // 必须在主线程显示 UI
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            var toast = Toast.Make(message, ToastDuration.Long, 16);
            await toast.Show();
        });
    }
}