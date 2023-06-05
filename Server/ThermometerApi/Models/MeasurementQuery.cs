using DhtApi.Models.Pagination;
using Models.Pagination;

namespace DhtApi.Models
{
    public enum MeasurementQueryGroups
    {
        None,
        Hour,
        Day
    }

    public class MeasurementQuery: Query
    {
        public Criteria? Date { get; set; }
        public Criteria? Hour { get; set; }
        public Criteria? Temperature { get; set; }
        public Criteria? Humidity { get; set; }
        public Criteria? DeviceName { get; set; }

        public string GroupBy { get; set; } = MeasurementQueryGroups.Hour.ToString();
    }
}
