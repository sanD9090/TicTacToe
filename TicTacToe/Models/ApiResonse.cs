using System.Net;

namespace TicTacToe.Models
{
    public class ApiResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public object? Data { get; set; }
    }
}
