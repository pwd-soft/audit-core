using AutoMapper;
using AutoMapper.Mappers;
using PWD.Audit.DtoModels;
using PWD.Audit.DtoModels;
using PWD.Audit.Entities;
using PWD.Audit.Models;
using System.Collections.Generic;
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

            CreateMap<ResponseHistory, ResponseHistoryDto>();
            CreateMap<ResponseHistoryDto, ResponseHistory>();

            CreateMap<Associate, AssociateDto>();
            CreateMap<AssociateDto, Associate>();

            CreateMap<Summary, SummaryDto>();
            CreateMap<SummaryDto, Summary>();

            CreateMap<SummaryLine, SummaryLineDto>();
            CreateMap<SummaryLineDto, SummaryLine>();

            CreateMap<Posting, PostingDto>();
            CreateMap<PostingDto, Posting>();

            CreateMap<YearlyObjection, YearlyObjectionDto>();
            CreateMap<YearlyObjectionDto, YearlyObjection>();

            CreateMap<ResponseComment, ResponseCommentDto>();
            CreateMap<ResponseCommentDto, ResponseComment>();

            CreateMap<ResponseState, ResponseStateDto>();
            CreateMap<ResponseStateDto, ResponseState>();


            CreateMap<PostingConsumeDto, PostingDto>()
                .BeforeMap((s, d) =>
                {
                    d.Id = 0;
                    d.Office = s.office;
                    d.OfficeBn = s.officeBn;
                    d.NameBn = s.nameBn;
                    d.Name = s.name;
                    d.OrgUniId = s.orgUniId;
                    d.UserId = s.id;
                    d.DesignationBn = s.designationBn;
                    d.Designation = s.designation;
                    d.EmployeeId = s.employeeId;
                    d.Post = s.post;
                    d.PostingId = s.postingId;
                    d.UserName = s.userName;
                });

        }
    }
}
