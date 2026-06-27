using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace NorthSouthSystems.Entities;

internal sealed class DateTimeOffsetUtcDateTimeConverter()
    : ValueConverter<DateTimeOffset, DateTime>(
        dto => dto.UtcDateTime,
        dt => InferUtc(dt))
{
    private static DateTimeOffset InferUtc(DateTime dt) =>
        dt.Kind != DateTimeKind.Local
            ? new(dt, TimeSpan.Zero)
            : throw new NotSupportedException();
}