// IngilizceProje/MAUIClient/Converters/StatusToColorConverter.cs
using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using IngilizceProje.Domain.Enums;

namespace IngilizceProjeMAUI.Converters
{
    public class StatusToColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is LearningStatusEnum status)
            {
                return status switch
                {
                    LearningStatusEnum.VeryImportant => Color.FromArgb("#EF4444"), // Premium Red
                    LearningStatusEnum.NeedsReview => Color.FromArgb("#F59E0B"),   // Gold/Yellow
                    LearningStatusEnum.Learned => Color.FromArgb("#10B981"),       // Sleek Green
                    LearningStatusEnum.NotReviewed => Color.FromArgb("#6B7280"),   // Neutral Gray
                    _ => Color.FromArgb("#9CA3AF")
                };
            }
            return Color.FromArgb("#9CA3AF");
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
