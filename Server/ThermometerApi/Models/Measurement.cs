namespace Models
{
    public class Measurement
    {
        public DateTime TimeStamp { get; set; }
        public MeasurementRange Temperature { get; set; } = new();
        public MeasurementRange Humidity { get; set; } = new();
        public string Device { get; set; } = string.Empty;
    }

    public class MeasurementRange
    {
        public decimal Average { get; set; }
        public decimal Min { get; set; }
        public decimal Max { get; set; }
    }
}
