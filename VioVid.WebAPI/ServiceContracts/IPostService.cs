using Application.DTOs.Post;
using VioVid.Core.Common;
using VioVid.Core.Entities;

namespace VioVid.WebAPI.ServiceContracts;

public interface IPostService
{
    // Task<PaginationResponse<Post>> GetAllAsync(GetPagingPostRequest getPagingPostRequest);

    Task<Post> GetByIdAsync(Guid id);

    // Task<Post> CreatePostAsync(CreatePostRequest createPostRequest);

    // Task<Person> UpdatePersonAsync(Guid id, UpdatePersonRequest updatePersonRequest);
    //
    // Task<Guid> DeletePersonAsync(Guid id);
}