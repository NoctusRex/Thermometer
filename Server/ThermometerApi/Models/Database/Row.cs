namespace Models.Database
{
    public class Row
    {
        /// <summary>
        /// The columns of the row
        /// </summary>
        public List<Column> Columns { get; set; } = new List<Column>();

        /// <summary>
        /// Returns the value of a column
        /// </summary>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <param name="columnKey">The name of the column</param>
        /// <returns>The value of T</returns>
        public T GetValue<T>(string columnKey)
        {
            object value = Columns.First(x => x.Key == columnKey).Value;

#pragma warning disable CS8603 // Possible null reference return.
            return value is DBNull ? default : (T)value;
#pragma warning restore CS8603 // Possible null reference return.
        }
    }
}
