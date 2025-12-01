using SchoolFacilityReport.ViewModels;

namespace SchoolFacilityReport.Views;

public partial class ReportDetailPage : ContentPage
{
    public ReportDetailPage(ReportDetailViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}