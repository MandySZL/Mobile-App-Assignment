using System.Globalization;
using SchoolFacilityReport.Resources.Strings;

namespace SchoolFacilityReport.Converters;

public class CategoryLocalizationConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var dbString = value as string;
        if (string.IsNullOrEmpty(dbString)) return "";

        // 这里做“映射”：把数据库里的旧名字 -> 对应到资源文件里的新变量
        return dbString switch
        {
            "Electrical (电力)" => AppResources.Cat_Elec,
            "Plumbing (水管)" => AppResources.Cat_Plumb,
            "Furniture (桌椅)" => AppResources.Cat_Furn,
            "AC/Fan (空调风扇)" => AppResources.Cat_AC,
            "Other (其他)" => AppResources.Cat_Other,
            _ => dbString // 如果遇到不认识的，就显示原样
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}