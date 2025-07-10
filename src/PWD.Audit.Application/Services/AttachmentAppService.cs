using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using PWD.Audit.DtoModels;
using PWD.Audit.Entities;
using PWD.Audit.Enum;
using PWD.Audit.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;


namespace PWD.Audit.Services
{
    public class AttachmentAppService : ApplicationService, IAttachmentAppService
    {
        private readonly IRepository<PWD.Audit.Models.Attachment, int> _repository;

        public AttachmentAppService(IRepository<PWD.Audit.Models.Attachment, int> repository)
        {
            _repository = repository;
        }

        public async Task<AttachmentDto> CreateAsync(AttachmentDto input)
        {
            var attachment = ObjectMapper.Map<AttachmentDto, PWD.Audit.Models.Attachment>(input);
            var insertedAttachment = await _repository.InsertAsync(attachment);
            return ObjectMapper.Map<PWD.Audit.Models.Attachment, AttachmentDto>(insertedAttachment);
        }

        public async Task DeleteAsync(int id) => await _repository.DeleteAsync(id);

        public async Task<AttachmentDto> GetByIdAsync(int id)
        {
            var attachment = await _repository.GetAsync(id);
            if (attachment == null)
            {
                throw new KeyNotFoundException($"Attachment with ID {id} not found.");
            }
            return ObjectMapper.Map<PWD.Audit.Models.Attachment, AttachmentDto>(attachment);
        }

        //private async Task<IQueryable<PWD.Audit.Models.Attachment>> GetListAsync()
        //{
        //    return await _repository.GetQueryableAsync();
        //    //var attachments = await _repository.GetListAsync();
        //    //return ObjectMapper.Map<List<PWD.Audit.Models.Attachment>, List<AttachmentDto>>(attachments);
        //}

        public async Task<List<AttachmentDto>> GetListResponseIdAsync(int id, AttachmentType type)
        {
            var attachments = await _repository.GetQueryableAsync();
            attachments = attachments.Where(a => a.ResponseId == id && a.AttachmentType == type);
            var attachmentsListDto = ObjectMapper.Map<IQueryable<PWD.Audit.Models.Attachment>, List<AttachmentDto>>(attachments);
            return attachmentsListDto;
        }

        public async Task<List<AttachmentDto>> GetListByObjectionIdAsync(int id, AttachmentType type)
        {
            var attachments = await _repository.GetQueryableAsync();
            attachments = attachments.Where(a => a.ObjectionId == id && a.AttachmentType == type);
            var attachmentsListDto = ObjectMapper.Map<IQueryable<PWD.Audit.Models.Attachment>, List<AttachmentDto>>(attachments);
            return attachmentsListDto;
        }

        public async Task<AttachmentDto> UpdateAsync(AttachmentDto input)
        {
            var attachment = await _repository.GetAsync(input.Id);
            if (attachment == null)
            {
                throw new KeyNotFoundException($"Attachment with ID {input.Id} not found.");
            }
            attachment.ObjectionId = input.ObjectionId;
            attachment.ResponseId = input.ResponseId;
            attachment.FileName = input.FileName;
            attachment.OriginalFileName = input.OriginalFileName;
            attachment.AttachmentType = input.AttachmentType;
            attachment.Path = input.Path;
            attachment.FileSize = input.FileSize;
            attachment.IsFileUploaded = input.IsFileUploaded;

            // Update the attachment in the repository
            var updatedAttachment = await _repository.UpdateAsync(attachment);
            return ObjectMapper.Map<PWD.Audit.Models.Attachment, AttachmentDto>(updatedAttachment);
        }

        public async Task InsertBulkAsync(IEnumerable<AttachmentDto> newAttachments)
        {
            var convertedAttachments = ObjectMapper.Map<IEnumerable<AttachmentDto>, IEnumerable<PWD.Audit.Models.Attachment>>(newAttachments);
            await _repository.InsertManyAsync(convertedAttachments, true);
        }
    }
}
