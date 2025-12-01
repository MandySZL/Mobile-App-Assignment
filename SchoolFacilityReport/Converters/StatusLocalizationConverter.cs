using System.Globalization;
using SchoolFacilityReport.Resources.Strings;

namespace SchoolFacilityReport.Converters;

public class StatusLocalizationConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // 拿到数据库里的状态字符串
        var status = value as string;

        if (string.IsNullOrEmpty(status)) return "";

        // 根据英文单词，返回对应的多语言资源
        return status switch
        {
            "Pending" => AppResources.StatusPending,
            "In Progress" => AppResources.StatusInProgress,
            "Resolved" => AppResources.StatusResolved,
            _ => status // 如果是不认识的词，直接显示原文
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}