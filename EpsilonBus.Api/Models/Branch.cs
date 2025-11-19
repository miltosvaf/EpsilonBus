using System.Collections.Generic;

namespace EpsilonBus.Api.Models
{
    public class Branch
    {
        public int ID { get; set; }
        public int? CompanyID { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int? OperatesMonday { get; set; }
        public int? OperatesTuesday { get; set; }
        public int? OperatesWednesday { get; set; }
        public int? OperatesThursday { get; set; }
        public int? OperatesFriday { get; set; }
        public int? OperatesSaturday { get; set; }
        public int? OperatesSunday { get; set; }
        public Company Company { get; set; }
        public ICollection<Employee>? Employees { get; set; }
        public ICollection<Stop>? Stops { get; set; }
        public ICollection<BusAllocation>? BusAllocations { get; set; }
        public ICollection<NonWorkingDay>? NonWorkingDays { get; set; }
    }
}