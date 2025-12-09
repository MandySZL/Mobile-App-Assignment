using SchoolFacilityReport.ViewModels;

namespace SchoolFacilityReport.Views;

public partial class SettingsPage : ContentPage
{
    public SettingsPage(SettingsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}