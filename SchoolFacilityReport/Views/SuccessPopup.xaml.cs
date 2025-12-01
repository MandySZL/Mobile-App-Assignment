using CommunityToolkit.Maui.Views; // 👈 必须引用这个才能识别 Popup

namespace SchoolFacilityReport.Views;

public partial class SuccessPopup : Popup
{
    public SuccessPopup(string role)
    {
        InitializeComponent();
        // 设置文字
        MessageLabel.Text = $"Login successful. Role: {role}";
    }

    private void OnCloseClicked(object sender, EventArgs e)
    {
        Close(); // 关闭弹窗
    }
}