using Application.DTOs.PostComment;
using VioVid.Core.Common;
using VioVid.Core.Entities;

namespace VioVid.WebAPI.ServiceContracts;

public interface IPostCommentService
{
    Task<PaginationResponse<PostComment>> GetAllAsync(GetPagingPostCommentRequest getPagingPostCommentRequest);

    Task<PostComment> GetByIdAsync(Guid id);

    Task<PostComment> CreatePostCommentAsync(CreatePostCommentRequest createPostCommentRequest);

    // Task<Person> UpdatePersonAsync(Guid id, UpdatePersonRequest updatePersonRequest);
    //
    // Task<Guid> DeletePersonAsync(Guid id);
}