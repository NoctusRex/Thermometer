using System.Data;

namespace Models.Database
{
    public class Table
    {
        /// <summary>
        /// The rows of the table
        /// </summary>
        public List<Row> Rows { get; set; } = new List<Row>();

        /// <summary>
        /// Original data table of the sql reader
        /// </summary>
        public DataTable DataTable { get; set; } = new();

        /// <summary>
        /// Returns the value of a row and its column
        /// </summary>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <param name="index">The row index</param>
        /// <param name="columnKey">The name of the column</param>
        /// <returns>The value of T</returns>
        public T GetValue<T>(int index, string columnKey) => (T)Rows[index].Columns.First(x => x.Key == columnKey).Value;
    }
}
