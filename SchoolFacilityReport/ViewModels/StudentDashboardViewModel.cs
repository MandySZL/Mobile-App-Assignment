using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SchoolFacilityReport.Models;
using SchoolFacilityReport.Resources.Strings;
using System.Collections.ObjectModel;

namespace SchoolFacilityReport.ViewModels;

public partial class StudentDashboardViewModel : ObservableObject
{
    private readonly Supabase.Client _supabase;

    [ObservableProperty]
    string description; // 故障描述

    [ObservableProperty]
    string selectedCategory; // 选中的分类

    [ObservableProperty]
    string selectedUrgency; // 选中的紧急程度

    [ObservableProperty]
    ImageSource photoPreview; // 用来在界面上显示刚才拍的照片

    private FileResult _photoFile; // 内存里实际的照片文件
    private Location _currentLocation; // 内存里的位置信息

    // 下拉菜单的数据源
    public ObservableCollection<string> Categories { get; } = new()
    {
        "Electrical (电力)", "Plumbing (水管)", "Furniture (桌椅)", "AC/Fan (空调风扇)", "Other (其他)"
    };

    public ObservableCollection<string> UrgencyLevels { get; } = new()
    {
        "Low (不急)", "Medium (普通)", "High (紧急)"
    };

    public StudentDashboardViewModel(Supabase.Client client)
    {
        _supabase = client;
    }

    // 📸 拍照功能
    [RelayCommand]
    async Task TakePhoto()
    {
        // 改动 1: 检查是否支持 (选图通常都支持)
        if (MediaPicker.Default.IsCaptureSupported)
        {
            // 改动 2: 把 CapturePhotoAsync (拍照) 改为 PickPhotoAsync (选图)
            _photoFile = await MediaPicker.Default.PickPhotoAsync();

            if (_photoFile != null)
            {
                // 显示预览
                var stream = await _photoFile.OpenReadAsync();
                PhotoPreview = ImageSource.FromStream(() => stream);
            }
        }
    }

    // 📍 获取位置功能
    [RelayCommand]
    async Task GetLocation()
    {
        try
        {
            // 先尝试获取最后一次已知位置（速度快）
            _currentLocation = await Geolocation.Default.GetLastKnownLocationAsync();

            // 如果没有，就重新请求定位（精度中等）
            if (_currentLocation == null)
            {
                _currentLocation = await Geolocation.Default.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Medium));
            }

            if (_currentLocation != null)
            {
                await Shell.Current.DisplayAlert("GPS", $"已获取位置: {_currentLocation.Latitude}, {_currentLocation.Longitude}", "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", "无法获取位置 (模拟器需手动设置虚拟位置): " + ex.Message, "OK");
        }
    }

    // 🚀 提交报修单
    [RelayCommand]
    async Task SubmitReport()
    {
        if (string.IsNullOrEmpty(Description) || string.IsNullOrEmpty(SelectedCategory))
        {
            await Shell.Current.DisplayAlert("提示", "请填写描述并选择分类", "OK");
            return;
        }

        try
        {
            string imageUrl = null;

            // 1. 如果拍了照，先上传照片
            if (_photoFile != null)
            {
                var fileName = $"{Guid.NewGuid()}.jpg";
                using var stream = await _photoFile.OpenReadAsync();

                // 【修改开始】：把 Stream 转换成 byte[]
                byte[] fileBytes;
                using (var memoryStream = new MemoryStream())
                {
                    await stream.CopyToAsync(memoryStream);
                    fileBytes = memoryStream.ToArray();
                }
                // 【修改结束】

                // 上传到 Supabase Storage (注意：这里传的是 fileBytes)
                await _supabase.Storage
                    .From("facility_photos")
                    .Upload(fileBytes, fileName);

                // 获取图片的公开访问链接
                imageUrl = _supabase.Storage.From("facility_photos").GetPublicUrl(fileName);
            }

            // 2. 转换紧急程度 (文本 -> 数字)
            int urgencyInt = 1;
            if (SelectedUrgency?.Contains("Medium") == true) urgencyInt = 2;
            if (SelectedUrgency?.Contains("High") == true) urgencyInt = 3;

            // 3. 准备数据对象
            var report = new Report
            {
                UserId = Guid.Parse(_supabase.Auth.CurrentUser.Id),
                Category = SelectedCategory,
                Description = Description,
                Urgency = urgencyInt,
                Status = "Pending",
                ImageUrl = imageUrl,
                Latitude = _currentLocation?.Latitude ?? 0,
                Longitude = _currentLocation?.Longitude ?? 0
            };

            // 4. 写入数据库
            await _supabase.From<Report>().Insert(report);

            // 需要引用 using SchoolFacilityReport.Resources.Strings;
            await Shell.Current.DisplayAlert(AppResources.SuccessTitle, "Report Submitted Successfully", "OK");

            // 5. 清空表单
            Description = "";
            PhotoPreview = null;
            _photoFile = null;
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", "提交失败: " + ex.Message, "OK");
        }
    }

    [RelayCommand]
    async Task GoBack()
    {
        await Shell.Current.GoToAsync("//LoginPage");
    }
}