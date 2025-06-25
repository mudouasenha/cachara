using System.Globalization;

namespace Cachara.Playground;

public class ImplicitOperator
{
    public Task PracticeImplicitOperatorMapping()
    {
        var dto = new MeterReadingDto { Timestamp = "2025-06-15T12:30:00Z", Value = 123.45 };

        // Implicitly converts without casting
        MeterReading reading = dto;

        Console.WriteLine(reading.Date); // Output: 6/15/2025 12:30:00 PM +00:00
        Console.WriteLine(reading.Value);
        return Task.CompletedTask;
    }

    public class MeterReadingDto
    {
        public string Timestamp { get; set; } = "";
        public double Value { get; set; }

        // Implicit conversion to MeterReading
        public static implicit operator MeterReading(MeterReadingDto dto)
        {
            return new MeterReading { Date = DateTimeOffset.Parse(dto.Timestamp, CultureInfo.InvariantCulture), Value = dto.Value };
        }
    }

    public class MeterReading
    {
        public DateTimeOffset Date { get; set; }
        public double Value { get; set; }
    }
}
