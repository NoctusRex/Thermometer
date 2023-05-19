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

        public void Set(decimal temperature, decimal humidity)
        {
            DatabaseRepository.ExecuteNonQuery("INSERT INTO measurements VALUES (@0, @1, @2)", DateTime.Now, temperature, humidity);
        }

        public IEnumerable<Measurement> Get(MeasurementQuery query)
        {
            var criterias = new List<Criteria?>
                {
                    query.Date,
                    query.Hour,
                    query.Temperature,
                    query.Humidity
                };

            var whereStatement = PaginationUtils.GetWhereStatementFromCriterias(criterias, new List<string>() { "DATE(time_stamp)", "HOUR(time_stamp)", "temperature", "humidity" }, 2);
            var sql = $"SELECT DATE(time_stamp) AS time_stamp_date, HOUR(time_stamp) AS time_stamp_hour, AVG(temperature) AS temperature, AVG(humidity) AS humidity FROM measurements {whereStatement.Item1} GROUP BY time_stamp_date, time_stamp_hour ORDER BY DATE(time_stamp) DESC, HOUR(time_stamp) ASC LIMIT @0 OFFSET @1";

            return DatabaseRepository.ExecuteReader(sql, new List<object>() { query.Limit, query.Offset }.Concat(whereStatement.Item2).ToArray()).
                   Rows.
                   Select(x => new Measurement()
                   {
                       Date = x.GetValue<DateTime>("time_stamp_date"),
                       Hour = x.GetValue<int>("time_stamp_hour"),
                       Temperature = x.GetValue<decimal>("temperature"),
                       Humidity = x.GetValue<decimal>("humidity")
                   });
        }

    }
}
