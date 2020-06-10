using AutoMapper;

namespace ServiceBusCLI.Features.SendJob
{
    public class Mappings : Profile
    {
        public Mappings()
        {
            CreateMap<Commands.SendJobCommand, SendJob.Request>();
        }
    }
}
