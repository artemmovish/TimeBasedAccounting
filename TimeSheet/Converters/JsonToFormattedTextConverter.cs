// Converters/JsonToFormattedTextConverter.cs
using System;
using System.Globalization;
using System.Text.Json;
using System.Windows.Data;

namespace TimeSheet.Converters
{
    public class JsonToFormattedTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string jsonString && !string.IsNullOrEmpty(jsonString))
            {
                try
                {
                    // Пытаемся десериализовать JSON для красивого форматирования
                    using JsonDocument document = JsonDocument.Parse(jsonString);
                    return JsonSerializer.Serialize(document.RootElement, new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping // Это исправит Unicode
                    });
                }
                catch
                {
                    // Если не JSON, возвращаем как есть
                    return jsonString;
                }
            }
            return "Нет данных для отображения";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}