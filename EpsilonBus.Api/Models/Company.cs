using System.Collections.Generic;

namespace EpsilonBus.Api.Models
{
    public class Company
    {
        public int ID { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public int IsActive { get; set; }
        public ICollection<Branch>? Branches { get; set; }
    }
}