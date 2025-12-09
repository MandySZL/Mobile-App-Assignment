using SchoolFacilityReport.Services;
using SchoolFacilityReport.Views;

namespace SchoolFacilityReport
{
    public partial class App : Application
    {
        private readonly Supabase.Client _supabase;
        private readonly NotificationService _notificationService;

        public App(Supabase.Client supabase, NotificationService notificationService)
        {
            InitializeComponent();
            _supabase = supabase;
            _notificationService = notificationService;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = new Window(new AppShell());

            // 启动时检查登录状态
            Dispatcher.Dispatch(async () => await CheckUserSession());

            return window;
        }

        private async Task CheckUserSession()
        {
            try
            {
                // 1. 检查是否超过 30 天未打开
                var lastOpened = Preferences.Get("LastOpened", DateTime.MinValue);
                if (lastOpened != DateTime.MinValue && (DateTime.Now - lastOpened).TotalDays > 30)
                {
                    // 超过30天，强制登出
                    await _supabase.Auth.SignOut();
                    Preferences.Remove("LastOpened");
                    // 默认就是 LoginPage，所以不用跳转
                    return;
                }

                // 更新最后打开时间
                Preferences.Set("LastOpened", DateTime.Now);

                // 2. 尝试恢复会话
                await _supabase.InitializeAsync();
                var session = _supabase.Auth.CurrentSession;

                if (session != null && _supabase.Auth.CurrentUser != null)
                {
                    var userIdGuid = Guid.Parse(_supabase.Auth.CurrentUser.Id);

                    // 3. 获取用户角色
                    var result = await _supabase.From<SchoolFacilityReport.Models.Profile>()
                                                .Where(x => x.Id == userIdGuid)
                                                .Get();
                    
                    var profile = result.Models.FirstOrDefault();

                    if (profile != null)
                    {
                        // 启动通知服务
                        await _notificationService.StartListening(profile.Role, userIdGuid);

                        // 根据角色跳转
                        if (profile.Role?.Trim().Equals("Maintenance", StringComparison.OrdinalIgnoreCase) == true)
                        {
                            // 确保切到 AdminDashboard
                            // 使用 Shell 路由
                             var shell = Application.Current.MainPage as AppShell;
                             if (shell != null)
                             {
                                 await shell.SwitchToAdminRole();
                             }
                             else
                             {
                                 await Shell.Current.GoToAsync("//AdminDashboard");
                             }
                        }
                        else
                        {
                            await Shell.Current.GoToAsync("//StudentTabs");
                        }
                    }
                    else
                    {
                        // 也就是没角色，去选角色
                        await Shell.Current.GoToAsync(nameof(RoleSelectionPage));
                    }
                }
            }
            catch (Exception ex)
            {
                // 出错情况，保持在登录页
                System.Diagnostics.Debug.WriteLine($"Auto-login failed: {ex.Message}");
            }
        }
    }
}