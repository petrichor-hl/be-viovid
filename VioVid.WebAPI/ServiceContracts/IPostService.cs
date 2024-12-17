using Application.DTOs.Post;
using Application.DTOs.Post.Res;
using VioVid.Core.Common;
using VioVid.Core.Entities;

namespace VioVid.WebAPI.ServiceContracts;

public interface IPostService
{
    // Task<PaginationResponse<Post>> GetAllAsync(GetPagingPostRequest getPagingPostRequest);

    Task<PostResponse> GetByIdAsync(Guid id);

    // Task<Post> CreatePostAsync(CreatePostRequest createPostRequest);

    // Task<Person> UpdatePersonAsync(Guid id, UpdatePersonRequest updatePersonRequest);
    //
    // Task<Guid> DeletePersonAsync(Guid id);
}