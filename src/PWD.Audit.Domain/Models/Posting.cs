using System;
using Volo.Abp.Domain.Entities;

namespace PWD.Audit.Models
{
    public class Posting : Entity<int>
    {
        public int PostingId { get; set; }
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public string NameBn { get; set; }
        public string Post { get; set; }
        public string Designation { get; set; }
        public string DesignationBn { get; set; }
        public string Office { get; set; }
        public string OfficeBn { get; set; }
        public Guid OrgUniId { get; set; }
        public Guid UserId { get; set; }
        public string UserName{ get; set; }

    }
    

}
