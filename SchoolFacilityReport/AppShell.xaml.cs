using SchoolFacilityReport.Views;

namespace SchoolFacilityReport;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // 注册路由
        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));

        // 👇👇👇 加上这一行！注册身份选择页 👇👇👇
        Routing.RegisterRoute(nameof(RoleSelectionPage), typeof(RoleSelectionPage));
        // 👆👆👆 加上这一行！👆👆👆

        Routing.RegisterRoute(nameof(StudentDashboardPage), typeof(StudentDashboardPage));
        Routing.RegisterRoute(nameof(MyReportsPage), typeof(MyReportsPage));
        Routing.RegisterRoute(nameof(AdminDashboardPage), typeof(AdminDashboardPage));
        Routing.RegisterRoute(nameof(ReportDetailPage), typeof(ReportDetailPage));
    }
}