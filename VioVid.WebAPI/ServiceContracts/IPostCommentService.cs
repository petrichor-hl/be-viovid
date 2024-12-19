using Application.DTOs.PostComment;
using Application.Models;
using VioVid.Core.Common;
using VioVid.Core.Entities;

namespace VioVid.WebAPI.ServiceContracts;

public interface IPostCommentService
{
    Task<PaginationResponse<PostComment>> GetAllAsync(PaginationFilter filter);

    Task<PaginationResponse<PostComment>> GetAllByPostAsync(GetPagingPostCommentRequest request);


    Task<PostComment> GetByIdAsync(Guid id);

    Task<PostComment> CreatePostCommentAsync(CreatePostCommentRequest createPostCommentRequest);

    // Task<Person> UpdatePersonAsync(Guid id, UpdatePersonRequest updatePersonRequest);
    //
    // Task<Guid> DeletePersonAsync(Guid id);
}