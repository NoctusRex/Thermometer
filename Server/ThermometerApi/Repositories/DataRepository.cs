using DhtApi.Models;
using Models;
using Models.Pagination;
using Utils;

namespace Repositories
{
    public class DataRepository
    {
        private DatabaseRepository DatabaseRepository { get; set; }

        public DataRepository(DatabaseRepository databaseRepository)
        {
            DatabaseRepository = databaseRepository;
        }

        public void Set(decimal temperature, decimal humidity, string deviceName)
        {
            DatabaseRepository.ExecuteNonQuery("INSERT INTO measurements VALUES (@0, @1, @2, @3)", DateTime.Now, temperature, humidity, deviceName);
        }

        public IEnumerable<Measurement> Get(MeasurementQuery query)
        {
            var criterias = new List<Criteria?>
                {
                    query.Date,
                    query.Hour,
                    query.Temperature,
                    query.Humidity,
                    query.DeviceName
                };

            var whereStatement = PaginationUtils.GetWhereStatementFromCriterias(criterias, new List<string>() { "DATE(time_stamp)", "HOUR(time_stamp)", "temperature", "humidity", "device_name" }, 2);
            var sql = "SELECT DATE(time_stamp) AS time_stamp_date, HOUR(time_stamp) AS time_stamp_hour, MONTH(time_stamp) AS time_stamp_month, AVG(temperature) AS temperature, AVG(humidity) AS humidity, MIN(temperature) AS min_temperature, MIN(humidity) AS min_humidity, MAX(temperature) AS max_temperature, MAX(humidity) AS max_humidity, device_name, time_stamp ";

            if (query.GroupBy?.ToLower() == MeasurementQueryGroups.None.ToString().ToLower())
            {
                sql += $"FROM measurements {whereStatement.Item1} GROUP BY time_stamp, device_name ORDER BY time_stamp DESC ";
            }
            else if (query.GroupBy?.ToLower() == MeasurementQueryGroups.Hour.ToString().ToLower())
            {
                sql += $"FROM measurements {whereStatement.Item1} GROUP BY time_stamp_date, time_stamp_hour, device_name ORDER BY DATE(time_stamp) DESC, HOUR(time_stamp) ASC ";
            }
            else if (query.GroupBy?.ToLower() == MeasurementQueryGroups.Day.ToString().ToLower())
            {
                sql += $"FROM measurements {whereStatement.Item1} GROUP BY time_stamp_date, device_name ORDER BY DATE(time_stamp) DESC ";
            }
            else if (query.GroupBy?.ToLower() == MeasurementQueryGroups.Month.ToString().ToLower())
            {
                sql += $"FROM measurements {whereStatement.Item1} GROUP BY time_stamp_month, device_name ORDER BY DATE(time_stamp) DESC ";
            }
            else
            {
                throw new Exception("Invalid group type. (use none, hour, day, month)");
            }
            sql += "LIMIT @0 OFFSET @1";

            return DatabaseRepository.ExecuteReader(sql, new List<object>() { query.Limit, query.Offset }.Concat(whereStatement.Item2).ToArray()).
                   Rows.
                   Select(x => new Measurement()
                   {
                       TimeStamp = x.GetValue<DateTime>("time_stamp"),
                       Temperature = new()
                       {
                           Average = x.GetValue<decimal>("temperature"),
                           Max = x.GetValue<decimal>("max_temperature"),
                           Min = x.GetValue<decimal>("min_temperature"),
                       },
                       Humidity = new()
                       {
                           Average = x.GetValue<decimal>("humidity"),
                           Max = x.GetValue<decimal>("max_humidity"),
                           Min = x.GetValue<decimal>("min_humidity")
                       },
                       Device = x.GetValue<string>("device_name")
                   });
        }

        public IEnumerable<string> GetDeviceNames()
        {
            return DatabaseRepository.ExecuteReader("SELECT DISTINCT device_name FROM measurements").Rows.Select(x => x.GetValue<string>("device_name"));
        }
    }
}
