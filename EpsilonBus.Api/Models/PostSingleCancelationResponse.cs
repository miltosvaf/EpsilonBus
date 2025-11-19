namespace EpsilonBus.Api.Models
{
    public class PostSingleCancelationResponse
    {
        public int IsSuccess { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMsg { get; set; }
    }
}