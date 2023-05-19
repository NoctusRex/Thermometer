namespace Models.Pagination
{
    public class Criteria {
        public object? Min { get; set; }
        public object? Max { get; set; }
        public bool Negate { get; set; }
        public bool Or { get; set; }
    }
}
