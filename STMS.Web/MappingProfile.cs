using AutoMapper;
using STMS.DTOs;
using STMS.Entities;
using TaskEntity = STMS.Entities.Task;
using AttachmentEntity = STMS.Entities.Attachment;

namespace STMS.Web
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TaskEntity, TaskDTO>()
                .ForMember(dest => dest.CurrentStatus, opt => opt.MapFrom(src =>
                    src.TaskSteps.OrderByDescending(s => s.DateTime).FirstOrDefault() != null
                        ? src.TaskSteps.OrderByDescending(s => s.DateTime).First().Status
                        : "No Status"));

            CreateMap<TaskStep, TaskStepDTO>();
            CreateMap<Comment, CommentDTO>();
            CreateMap<AttachmentEntity, AttachmentDTO>();
        }
    }
}