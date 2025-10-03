namespace MangoFusion_API.Models
{
    public class ApiResponse
    {
        public HttpClient StatusCode { get; set; }

        public bool IsSuccess { get; set; } = true;

        public List<string> ErrorMessage { get; set; } = [];
    
        public object? Result { get; set; }
    }
}
