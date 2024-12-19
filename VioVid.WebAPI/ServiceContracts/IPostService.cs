using Application.DTOs.Post;
using Application.DTOs.Post.Res;
using Application.Models;
using VioVid.Core.Common;
using VioVid.Core.Entities;

namespace VioVid.WebAPI.ServiceContracts;

public interface IPostService
{
    Task<PaginationResponse<Post>> GetAllAsync(PaginationFilter filter);

    Task<PaginationResponse<Post>> GetAllByChannelAsync(GetPagingPostRequest getPagingPostRequest);


    Task<PostResponse> GetByIdAsync(Guid id);

    Task<Post> CreatePostAsync(CreatePostRequest createPostRequest);

    Task<Post> LikePostAsync(Guid postId);

    Task<Post> UnlikePostAsync(Guid postId);

    // Task<Person> UpdatePersonAsync(Guid id, UpdatePersonRequest updatePersonRequest);
    //
    // Task<Guid> DeletePersonAsync(Guid id);
}