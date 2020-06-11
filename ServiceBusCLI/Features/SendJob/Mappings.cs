﻿using AutoMapper;
using Contracts;
using ServiceBusCLI.Features.SendMessage;

namespace ServiceBusCLI.Features.SendJob
{
    public class Mappings : Profile  
    {
        public Mappings()
        {
            CreateMap<Commands.SendJobCommand, SendMessage<Job>.Request>();
        }
    }
}
