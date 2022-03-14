using AutoMapper;
using WebApiProject.Entities.DbSet;
using WebApiProject.Entities.Dtos.Incoming;
using WebApiProject.Entities.Dtos.Outgoing.Profile;

namespace WebApiProject.Api.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserDto, User>()
                .ForMember(
                    dest => dest.FirstName,
                    from => from.MapFrom(x => $"{x.FirstName}")
                )
                .ForMember(
                    dest => dest.LastName,
                    from => from.MapFrom(x => $"{x.LastName}")
                )
                .ForMember(
                    dest => dest.Email,
                    from => from.MapFrom(x => $"{x.Email}")
                )
                .ForMember(
                    dest => dest.PhoneNumber,
                    from => from.MapFrom(x => $"{x.PhoneNumber}")
                )
                .ForMember(
                    dest => dest.DateOfBirth,
                    from => from.MapFrom(x => Convert.ToDateTime(x.DateOfBirth))
                ).ForMember(
                    dest => dest.Country,
                    from => from.MapFrom(x => $"{x.Country}")
                ).ForMember(
                    dest => dest.Status,
                    from => from.MapFrom(x => 1)
                );

            CreateMap<User, ProfileDto>()
              .ForMember(
                  dest => dest.FirstName,
                  from => from.MapFrom(x => $"{x.FirstName}")
              )
              .ForMember(
                  dest => dest.LastName,
                  from => from.MapFrom(x => $"{x.LastName}")
              )
              .ForMember(
                  dest => dest.Email,
                  from => from.MapFrom(x => $"{x.Email}")
              )
              .ForMember(
                  dest => dest.PhoneNumber,
                  from => from.MapFrom(x => $"{x.PhoneNumber}")
              )
              .ForMember(
                  dest => dest.DateOfBirth,
                  from => from.MapFrom(x => $"{x.DateOfBirth.ToShortDateString()}")
              ).ForMember(
                  dest => dest.Country,
                  from => from.MapFrom(x => $"{x.Country}")
              );
        }
    }
}
