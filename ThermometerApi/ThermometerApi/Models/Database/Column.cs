namespace Models.Database
{
    public class Column
    {
        /// <summary>
        /// The table column title of the reader result
        /// </summary>
        public string Key { get; set; } = string.Empty;
        /// <summary>
        /// The value of the column
        /// </summary>
        public object Value { get; set; } = new();

        /// <summary>
        /// Returns the value as T
        /// </summary>
        /// <typeparam name="T">Datatype to return</typeparam>
        /// <returns>The value as T</returns>
        public T GetValue<T>() => (T)Value;

        public Column(string key, object value)
        {
            Key = key;
            Value = value;
        }

        public Column() { }
    }
}
