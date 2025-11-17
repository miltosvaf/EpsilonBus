namespace EpsilonBus.Api.Models
{
    public class Employee
    {
        public int ID { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public int CompanyID { get; set; }
        public int BranchID { get; set; }
        public int IsActive { get; set; }
        public Branch Branch { get; set; }
        public Company Company { get; set; }
    }
}