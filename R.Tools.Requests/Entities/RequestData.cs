namespace R.Tools.Requests.Entities
{
    public class RequestData(string verb, string query, string path, DateTime startTime)
    {
        public DateTime StartTime { get; set; } = startTime;
        public DateTime EndTime { get; set; }
        public string Verb { get; set; } = verb;
        public string Path { get; set; } = path;
        public string Query { get; set; } = query;
        public string? IpAddress { get; set; }
        public int ResponseStatus { get; set; }
        public string? FailureText { get; set; }
        public string? UserId { get; set; }
        public object? ContentObj { get; set; }
        public string? ServiceName { get; set; }
    }
}
