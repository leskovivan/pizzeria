namespace pizzeria.Models
{
    public class QueryOptions
    {
        public string OrderPropertyName { get; set; } = "Id";
        public bool DescendingOrder { get; set; }
        public string? SearchPropertyName { get; set; }
        public string? SearchTerm { get; set; }
        public string? FilterPropertyName { get; set; }
        public string? FilterTerm { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 12;
    }
}
