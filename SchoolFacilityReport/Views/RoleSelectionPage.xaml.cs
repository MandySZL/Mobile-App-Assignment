using SchoolFacilityReport.ViewModels;

namespace SchoolFacilityReport.Views;

public partial class RoleSelectionPage : ContentPage
{
    public RoleSelectionPage(RoleSelectionViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}