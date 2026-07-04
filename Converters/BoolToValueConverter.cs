// IngilizceProje/MAUIClient/Converters/BoolToValueConverter.cs
using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace IngilizceProjeMAUI.Converters
{
    public class BoolToValueConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool flag && parameter is string paramString)
            {
                var parts = paramString.Split('|');
                if (parts.Length >= 2)
                {
                    var resultStr = flag ? parts[0] : parts[1];

                    // Handle boolean target type mapping
                    if (targetType == typeof(bool) || targetType == typeof(bool?))
                    {
                        if (bool.TryParse(resultStr, out bool boolVal))
                        {
                            return boolVal;
                        }
                    }
                    // Handle Color target type mapping (e.g. Hex colors)
                    else if (targetType == typeof(Color))
                    {
                        try
                        {
                            return Color.FromArgb(resultStr);
                        }
                        catch
                        {
                            // Fallback to original string if parsing fails
                        }
                    }

                    return resultStr;
                }
            }
            // For visibility converter (true -> return true, false -> return false)
            if (value is string text && parameter is string visibilityCheck && visibilityCheck == "True|False")
            {
                return !string.IsNullOrEmpty(text);
            }
            if (value is double doubleValue && parameter is string scaleParam && scaleParam == "DivideBy100")
            {
                return doubleValue / 100.0;
            }
            return value;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
