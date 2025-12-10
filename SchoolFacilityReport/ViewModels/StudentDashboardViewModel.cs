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
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    bool isBusy;

    public bool IsNotBusy => !IsBusy;

    [ObservableProperty]
    string description; 

    [ObservableProperty]
    string selectedCategory;

    // Urgency Selection Logic
    [ObservableProperty]
    double urgencyOpacity1 = 1.0;
    [ObservableProperty]
    double urgencyOpacity2 = 0.5;
    [ObservableProperty]
    double urgencyOpacity3 = 0.5;

    private int _selectedUrgencyLevel = 1; // Default to Low

    [ObservableProperty]
    ImageSource photoData; // Renamed from PhotoPreview to match XAML

    [ObservableProperty]
    string locationDisplay;

    private FileResult _photoFile; 
    private Location _currentLocation; 

    public ObservableCollection<string> Categories { get; } = new()
    {
        "Electrical", "Plumbing", "Furniture", "AC/Fan", "Other"
    };

    public StudentDashboardViewModel(Supabase.Client client)
    {
        _supabase = client;
        SetUrgency("1"); // Init default
    }

    [RelayCommand]
    void SetUrgency(string levelStr)
    {
        if (int.TryParse(levelStr, out int level))
        {
            _selectedUrgencyLevel = level;
            // Update Opacities to show selection
            UrgencyOpacity1 = level == 1 ? 1.0 : 0.5;
            UrgencyOpacity2 = level == 2 ? 1.0 : 0.5;
            UrgencyOpacity3 = level == 3 ? 1.0 : 0.5;
        }
    }

    [RelayCommand]
    async Task TakePhoto()
    {
        if (MediaPicker.Default.IsCaptureSupported)
        {
            try 
            {
                _photoFile = await MediaPicker.Default.PickPhotoAsync(); // Or CapturePhotoAsync
                if (_photoFile != null)
                {
                    var stream = await _photoFile.OpenReadAsync();
                    PhotoData = ImageSource.FromStream(() => stream);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", "Photo failed: " + ex.Message, "OK");
            }
        }
    }

    [RelayCommand]
    async Task GetLocation()
    {
        IsBusy = true;
        try
        {
            _currentLocation = await Geolocation.Default.GetLastKnownLocationAsync();

            if (_currentLocation == null)
            {
                _currentLocation = await Geolocation.Default.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Medium));
            }

            if (_currentLocation != null)
            {
                LocationDisplay = $"📍 {_currentLocation.Latitude:F4}, {_currentLocation.Longitude:F4}";
            }
        }
        catch (Exception ex)
        {
             LocationDisplay = "📍 Location Error";
             await Shell.Current.DisplayAlert("Error", "Location failed: " + ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    async Task SubmitReport()
    {
        if (IsBusy) return;

        var missingFields = new List<string>();

        if (string.IsNullOrEmpty(SelectedCategory)) missingFields.Add("Category");
        if (string.IsNullOrEmpty(Description)) missingFields.Add("Description");
        if (_photoFile == null) missingFields.Add("Photo");
        if (_currentLocation == null) missingFields.Add("Location");

        if (missingFields.Any())
        {
            var message = "Please fill in the following information:\n- " + string.Join("\n- ", missingFields);
            await Shell.Current.DisplayAlert("Incomplete Report", message, "OK");
            return;
        }

        IsBusy = true;
        try
        {
            string imageUrl = null;

            if (_photoFile != null)
            {
                var fileName = $"{Guid.NewGuid()}.jpg";
                using var stream = await _photoFile.OpenReadAsync();
                
                byte[] fileBytes;
                using (var memoryStream = new MemoryStream())
                {
                    await stream.CopyToAsync(memoryStream);
                    fileBytes = memoryStream.ToArray();
                }

                await _supabase.Storage
                    .From("facility_photos")
                    .Upload(fileBytes, fileName);

                imageUrl = _supabase.Storage.From("facility_photos").GetPublicUrl(fileName);
            }

            var report = new Report
            {
                UserId = Guid.Parse(_supabase.Auth.CurrentUser.Id),
                Category = SelectedCategory,
                Description = Description,
                Urgency = _selectedUrgencyLevel,
                Status = "Pending",
                ImageUrl = imageUrl,
                Latitude = _currentLocation?.Latitude ?? 0,
                Longitude = _currentLocation?.Longitude ?? 0,
                CreatedAt = DateTime.UtcNow // Fixed property name
            };

            await _supabase.From<Report>().Insert(report);

            await Shell.Current.DisplayAlert("Success", "Report Submitted Successfully", "OK");

            // Reset UI
            Description = "";
            PhotoData = null;
            _photoFile = null;
            LocationDisplay = null;
            _currentLocation = null;
            SelectedCategory = null;
            SetUrgency("1");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", "Submission failed: " + ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    async Task Logout()
    {
        await _supabase.Auth.SignOut();
        // Navigate back to Login
        await Shell.Current.GoToAsync("//LoginPage");
    }
}