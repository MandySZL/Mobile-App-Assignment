using System.Globalization;

namespace SchoolFacilityReport.Converters;

public class CategoryIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var category = value as string ?? "";

        // 简单的关键词匹配 (根据你的数据库内容调整关键词)
        if (category.Contains("Electrical") || category.Contains("电力")) return "⚡";
        if (category.Contains("Plumbing") || category.Contains("水管")) return "🚰";
        if (category.Contains("Furniture") || category.Contains("桌椅")) return "🪑";
        if (category.Contains("AC") || category.Contains("Fan") || category.Contains("空调")) return "❄️";

        return "🛠️"; // 默认图标
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}