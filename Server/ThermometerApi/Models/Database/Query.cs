namespace Models.Database
{
    public class Query
    {
        public string CommandText { get; set; } = string.Empty;
        public object[] Parameters { get; set; } = Array.Empty<object>();

        public Query() { }

        public Query(string commandText, params object[] parameters)
        {
            CommandText = commandText;
            Parameters = parameters;
        }
    }
}
