using AutoMapper;
using AutoMapper.Mappers;
using PWD.Attendance_Swagger.DtoModels;
using PWD.Audit.Entities;
using Volo.Abp.AuditLogging;

namespace PWD.Audit
{
    public class AuditApplicationAutoMapperProfile : Profile
    {
        public AuditApplicationAutoMapperProfile()
        {
            /* You can configure your AutoMapper mapping configuration here.
             * Alternatively, you can split your mapping configurations
             * into multiple profile classes for a better organization. */

            CreateMap<Objection, ObjectionDto>();
            CreateMap<ObjectionDto, Objection>();
            CreateMap<Associate, AssociateDto>();
            CreateMap<AssociateDto, Associate>();
            CreateMap<OfficeUserDto, OfficeUser>();
            CreateMap<OfficeUser, OfficeUserDto>();
            CreateMap<Summary, SummaryDto>();
            CreateMap<SummaryDto, Summary>();
            CreateMap<SummaryLine, SummaryLineDto>();
            CreateMap<SummaryLineDto, SummaryLine>();

        }
    }
}
