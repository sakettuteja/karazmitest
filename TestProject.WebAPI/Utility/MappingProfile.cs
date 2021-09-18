using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestProject.WebAPI.Data;
using TestProject.WebAPI.SeedData;

namespace crud.Utility
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<UpdateUserForm, User>()
                .ForMember(u => u.UserId, opt => opt.MapFrom(x => x.Id))
                .ForMember(u=> u.PasswordHash, opt=> opt.MapFrom(x=>x.Password));

            CreateMap<Register, User>()
                 .ForMember(u => u.UserName, opt => opt.MapFrom(x => x.Email))
                .ForMember(u => u.PasswordHash, opt => opt.MapFrom(x => x.Password));
        }
    }
}
