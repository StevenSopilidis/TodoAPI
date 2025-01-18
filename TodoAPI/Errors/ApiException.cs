namespace TodoAPI.Errors
{
    public class ApiException
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string? StackTrace { get; set; }
        public ApiException(int statusCode, string message, string? stackTrace = null)
        {
            StatusCode=statusCode;
            Message=message;
            StackTrace=stackTrace;
        }
    }
}