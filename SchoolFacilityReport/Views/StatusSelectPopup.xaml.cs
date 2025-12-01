using CommunityToolkit.Maui.Views;

namespace SchoolFacilityReport.Views;

public partial class StatusSelectPopup : Popup
{
    public StatusSelectPopup()
    {
        InitializeComponent();
    }

    // ????????????
    private void OnStatusClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        string status = button.CommandParameter.ToString();

        // ????????????(??Key)???
        Close(status);
    }

    private void OnCancelClicked(object sender, EventArgs e)
    {
        Close(null); // ?? null ????
    }
}