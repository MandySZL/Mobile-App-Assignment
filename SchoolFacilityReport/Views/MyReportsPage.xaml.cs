using SchoolFacilityReport.ViewModels;

namespace SchoolFacilityReport.Views;

public partial class MyReportsPage : ContentPage
{
    public MyReportsPage(MyReportsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    // ??????????
    protected override void OnAppearing()
    {
        base.OnAppearing();
        (BindingContext as MyReportsViewModel)?.LoadDataCommand.Execute(null);
    }
}