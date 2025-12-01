using SchoolFacilityReport.ViewModels;

namespace SchoolFacilityReport.Views;

public partial class StudentDashboardPage : ContentPage
{
    public StudentDashboardPage(StudentDashboardViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}