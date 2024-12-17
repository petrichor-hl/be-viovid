using Application.DTOs.PostComment;
using Application.Exceptions;
using Microsoft.EntityFrameworkCore;
using VioVid.Core.Common;
using VioVid.Core.Entities;
using VioVid.Infrastructure.DatabaseContext;
using VioVid.WebAPI.ServiceContracts;

namespace VioVid.WebAPI.Services;

public class PostCommentService : IPostCommentService
{
    private readonly ApplicationDbContext _dbContext;

    public PostCommentService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PaginationResponse<PostComment>> GetAllAsync(
        GetPagingPostCommentRequest getPagingPostCommentRequest)
    {
        var pageIndex = getPagingPostCommentRequest.PageIndex;
        var pageSize = getPagingPostCommentRequest.PageSize;
        var postId = getPagingPostCommentRequest.PostId;

        var query = _dbContext.PostComments.AsQueryable();

        if (postId != null)
            query = query.Where(p => p.PostId == postId);

        // Tính tổng số lượng record
        var totalRecords = await query.CountAsync();

        // Lấy ra trang trong request cần
        var postComments = await query
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginationResponse<PostComment>
        {
            TotalCount = totalRecords,
            Items = postComments,
            PageIndex = pageIndex,
            PageSize = pageSize
        };
    }

    public async Task<PostComment> GetByIdAsync(Guid id)
    {
        var postComment = await _dbContext.PostComments
            .FirstOrDefaultAsync(p => p.Id == id);
        if (postComment == null) throw new NotFoundException($"Không tìm thấy PostComment có id {id}");
        return postComment;
    }

    public async Task<PostComment> CreatePostCommentAsync(CreatePostCommentRequest createPostCommentRequest)
    {
        var postComment = new PostComment
        {
            PostId = createPostCommentRequest.PostId,
            ApplicationUserId = createPostCommentRequest.ApplicationUserId,
            Content = createPostCommentRequest.Content
        };

        await _dbContext.PostComments.AddAsync(postComment);
        await _dbContext.SaveChangesAsync();
        return postComment;
    }

    // public async Task<Person> UpdatePersonAsync(Guid id, UpdatePersonRequest updatePersonRequest)
    // {
    //     var person = await _dbContext.Persons.FindAsync(id);
    //     if (person == null) throw new NotFoundException($"Không tìm thấy Person có id {id}");
    //
    //     person.Name = updatePersonRequest.Name;
    //     person.Gender = updatePersonRequest.Gender;
    //     person.Popularity = updatePersonRequest.Popularity;
    //     person.ProfilePath = updatePersonRequest.ProfilePath;
    //     person.Biography = updatePersonRequest.Biography;
    //     person.KnownForDepartment = updatePersonRequest.KnownForDepartment;
    //     person.Dob = updatePersonRequest.Dob;
    //
    //     await _dbContext.SaveChangesAsync();
    //     return person;
    // }
    //
    // public async Task<Guid> DeletePersonAsync(Guid id)
    // {
    //     var person = await _dbContext.Persons.FindAsync(id);
    //     if (person == null) throw new NotFoundException($"Không tìm thấy Person có id {id}");
    //
    //     _dbContext.Persons.Remove(person);
    //     await _dbContext.SaveChangesAsync();
    //     return id;
    // }
}