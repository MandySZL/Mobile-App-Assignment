using SchoolFacilityReport.ViewModels;

namespace SchoolFacilityReport.Views;

public partial class AdminDashboardPage : ContentPage
{
    public AdminDashboardPage(AdminDashboardViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        (BindingContext as AdminDashboardViewModel)?.LoadAllReportsCommand.Execute(null);
    }
}