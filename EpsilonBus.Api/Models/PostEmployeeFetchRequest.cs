namespace EpsilonBus.Api.Models
{
    public class PostEmployeeFetchRequest
    {
        public int CompanyID { get; set; }
        public int BranchID { get; set; }
        public int LanguageID { get; set; } = 1;
    }
}