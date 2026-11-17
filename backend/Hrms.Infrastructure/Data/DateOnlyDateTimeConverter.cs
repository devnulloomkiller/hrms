using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Hrms.Infrastructure.Data;

/// <summary>
/// MySql.Data читает DATE как DateTime и не поддерживает GetFieldValue&lt;DateOnly&gt;,
/// поэтому конвертация выполняется на уровне EF, а в БД колонка остаётся date.
/// </summary>
public sealed class DateOnlyDateTimeConverter : ValueConverter<DateOnly, DateTime>
{
    public DateOnlyDateTimeConverter()
        : base(
            dateOnly => dateOnly.ToDateTime(TimeOnly.MinValue),
            dateTime => DateOnly.FromDateTime(dateTime))
    {
    }
}
