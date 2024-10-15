using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace PWD.Audit.DtoModels
{
    public class PostingDto : EntityDto<int>
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
        public Guid? OrgUniId { get; set; }
        public Guid? UserId { get; set; }
        public string UserName { get; set; }

    }
    public class PostingConsumeDto
    {
        public Guid id { get; set; }
        public Guid orgUniId { get; set; }
        public int postingId { get; set; }
        public int employeeId { get; set; }
        public string name { get; set; }
        public string nameBn { get; set; }
        public string post { get; set; }
        public string designation { get; set; }
        public string designationBn { get; set; }
        public string office { get; set; }
        public string officeBn { get; set; }
        public string userName { get; set; }

    }
    public class OrganizationUnitDto 
    {
        public virtual Guid? id { get; set; }
        public virtual Guid? parentId { get; set; }
        public virtual Guid? userId { get; set; }
        public virtual string code { get; set; }
        public virtual string displayName { get; set; }
        public virtual string civilEm { get; set; }
        public virtual List<OrgRoleConsumeDto> roles { get; set; }
        public virtual List<string> roleNames { get; set; } = new List<string>();

    }
    public class ColleagueDto 
        //: EntityDto<Guid>
    {
        public Guid id { get; set; }
        public string userName { get; set; }
        public string name { get; set; }
        public string surName { get; set; }
    }
    public class RoleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsPublic { get; set; }
        public bool IsDefault { get; set; }
    }
    public class RoleConsumeDto
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public bool isPublic { get; set; }
        public bool isDefault { get; set; }
    }
    public class OrgRoleConsumeDto
    {
        public Guid roleId { get; set; }
        public Guid organizationUnitId { get; set; }
    }
 public class TransferParameter 
    {
        public int ProjectId { get; set; }
        public string FromUserName { get; set; }
        public string ToUserName { get; set; }
        public string FromOfficeCode { get; set; }
        public string ToOfficeCode { get; set; }
        public Guid FromUser { get; set; } = Guid.Empty;
        public Guid ToUser { get; set; } = Guid.Empty;
        public Guid FromOffice { get; set; } = Guid.Empty;
        public Guid ToOffice { get; set; } = Guid.Empty;
        public string Note { get; set; }
        public DateTime Date { get; set; }
        public int PostingId { get; set; }
    }

    public class UserInfo
    {
        public object tenantId { get; set; }
        public string userName { get; set; }
        public string name { get; set; }
        public object surname { get; set; }
        public string email { get; set; }
        public bool emailConfirmed { get; set; }
        public object phoneNumber { get; set; }
        public bool phoneNumberConfirmed { get; set; }
        //public bool lockoutEnabled { get; set; }
        //public object lockoutEnd { get; set; }
        //public string concurrencyStamp { get; set; }
        //public bool isDeleted { get; set; }
        //public object deleterId { get; set; }
        //public object deletionTime { get; set; }
        //public DateTime lastModificationTime { get; set; }
        //public object lastModifierId { get; set; }
        //public DateTime creationTime { get; set; }
        //public string creatorId { get; set; }
        public Guid id { get; set; }
        public Extraproperties extraProperties { get; set; }
    }

    public class Extraproperties
    {
    }

   

}
