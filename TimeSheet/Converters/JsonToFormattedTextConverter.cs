// Converters/JsonToFormattedTextConverter.cs
using System;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Windows.Data;

namespace TimeSheet.Converters
{
    public class JsonToFormattedTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string json || string.IsNullOrWhiteSpace(json))
                return string.Empty;

            try
            {
                var sb = new StringBuilder();
                using var doc = JsonDocument.Parse(json);

                if (doc.RootElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var element in doc.RootElement.EnumerateArray())
                    {
                        sb.AppendLine(new string('-', 40));

                        foreach (var prop in element.EnumerateObject())
                        {
                            string val = prop.Value.ValueKind switch
                            {
                                JsonValueKind.String => prop.Value.GetString(),
                                JsonValueKind.Number => prop.Value.ToString(),
                                JsonValueKind.True => "Да",
                                JsonValueKind.False => "Нет",
                                JsonValueKind.Null => "—",
                                _ => prop.Value.ToString()
                            };

                            sb.AppendLine($"{prop.Name}: {val}");
                        }

                        sb.AppendLine(); // пустая строка между записями
                    }

                    sb.AppendLine(new string('-', 40));
                }
                else if (doc.RootElement.ValueKind == JsonValueKind.Object)
                {
                    sb.AppendLine(new string('-', 40));
                    foreach (var prop in doc.RootElement.EnumerateObject())
                        sb.AppendLine($"{prop.Name}: {prop.Value}");
                    sb.AppendLine(new string('-', 40));
                }
                else
                {
                    sb.AppendLine(json);
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                return $"Ошибка форматирования JSON:\n{ex.Message}";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}