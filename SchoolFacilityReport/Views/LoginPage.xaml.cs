using SchoolFacilityReport.ViewModels;

namespace SchoolFacilityReport.Views;

public partial class LoginPage : ContentPage
{
    private bool _isAnimating = false;

    public LoginPage(LoginViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // 开启 Logo 悬浮动画
        _isAnimating = true;
        await StartFloatingAnimation();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        // 离开页面时停止动画，防止后台占用资源
        _isAnimating = false;
        // 这里的 CancelAnimations 是扩展方法，如果报错可以去掉
        LogoImage.CancelAnimations();
    }

    private async Task StartFloatingAnimation()
    {
        while (_isAnimating)
        {
            // 向上浮动 10px，耗时 2秒
            await LogoImage.TranslateTo(0, -10, 2000, Easing.SinInOut);

            if (!_isAnimating) break;

            // 向下浮动回原位，耗时 2秒
            await LogoImage.TranslateTo(0, 0, 2000, Easing.SinInOut);
        }
    }
}