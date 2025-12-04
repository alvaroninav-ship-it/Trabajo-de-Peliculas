using AutoMapper;
using Movies.Core.Entities;
using Movies.Infrastructure.DTOs;

namespace Movies.Infrastructure.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<Movie, MovieDto>();
            CreateMap<MovieDto, Movie>();
            CreateMap<Actor, ActorDto>();
            CreateMap<ActorDto, Actor>();
            CreateMap<Comment, CommentDto>();
            CreateMap<CommentDto, Comment>();
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();
            CreateMap<Review, ReviewDto>();
            CreateMap<ReviewDto, Review>();
            CreateMap<Security, SecurityDto>().ReverseMap();


        }
    }
}
