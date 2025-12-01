using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SchoolFacilityReport.Models;
using SchoolFacilityReport.Views;

namespace SchoolFacilityReport.ViewModels;

public partial class RoleSelectionViewModel : ObservableObject
{
    private readonly Supabase.Client _supabase;

    public RoleSelectionViewModel(Supabase.Client client)
    {
        _supabase = client;
    }

    [RelayCommand]
    async Task SelectRole(string role)
    {
        try
        {
            var userId = _supabase.Auth.CurrentUser.Id;

            // 1. 创建新的 Profile 记录
            var newProfile = new Profile
            {
                Id = Guid.Parse(userId),
                Email = _supabase.Auth.CurrentUser.Email,
                Role = role
            };

            // 2. 存入 Supabase 数据库
            // Upsert = Update or Insert
            await _supabase.From<Profile>().Upsert(newProfile);

            await Shell.Current.DisplayAlert("Success", $"身份已确认为: {role}", "OK");

            // TODO: 这里下一步将跳转到主页 (Dashboard)
       
            // 如果是学生，跳转到报修填写页
            if (role == "Student" || role == "Staff")
            {
                // ///StudentDashboardPage 表示重置导航堆栈，防止用户按返回键退回登录页
                await Shell.Current.GoToAsync($"//{nameof(StudentDashboardPage)}");
            }

            else if (role == "Maintenance")
            {
                // 跳转到维修工主页
                await Shell.Current.GoToAsync($"//{nameof(AdminDashboardPage)}");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", "保存失败: " + ex.Message, "OK");
        }
    }
}