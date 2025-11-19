namespace EpsilonBus.Api.Models
{
    public class PostEmployeeCreateResponse
    {
        public int IsSuccess { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMsg { get; set; }
    }
}