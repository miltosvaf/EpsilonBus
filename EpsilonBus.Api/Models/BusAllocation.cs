using System;

namespace EpsilonBus.Api.Models
{
    public class BusAllocation
    {
        public int ID { get; set; }
        public int BranchID { get; set; }
        public int TypeID { get; set; }
        public int AllocationNum { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Branch Branch { get; set; }
        public BusType BusType { get; set; }
    }
}