using Application.DTOs.PostComment;
using Application.Exceptions;
using Application.Models;
using Microsoft.EntityFrameworkCore;
using VioVid.Core.Common;
using VioVid.Core.Entities;
using VioVid.Infrastructure.DatabaseContext;
using VioVid.WebAPI.ServiceContracts;

namespace VioVid.WebAPI.Services;

public class PostCommentService : IPostCommentService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;


    public PostCommentService(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
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
        try
        {
            var user = _httpContextAccessor.HttpContext?.User!;
            var userIdClaim = user.FindFirst("UserId");
            var applicationUserId = Guid.Parse(userIdClaim!.Value);

            var applicationUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id.Equals(applicationUserId));

            // Create the new PostComment
            var postComment = new PostComment
            {
                Content = createPostCommentRequest.Content,
                ApplicationUserId = applicationUserId,
                PostId = createPostCommentRequest.PostId,
                CreatedAt = DateTime.UtcNow,
                ApplicationUser = applicationUser!
            };

            // Add the PostComment to the Post's PostComments collection
            var post = await _dbContext.Posts.Include(p => p.PostComments) // Ensure PostComments is loaded
                .FirstOrDefaultAsync(p => p.Id == createPostCommentRequest.PostId);

            if (post == null) throw new Exception("Post not found");

            // Add the comment to the Post's collection
            post.PostComments.Add(postComment);

            foreach (var comment in post.PostComments)
                Console.WriteLine($"Comment ID: {comment.Id}, Content: {comment.Content}");

            // Save changes to the database
            await _dbContext.SaveChangesAsync();

            return postComment;
        }
        catch (Exception e)
        {
            throw new Exception("Cannot create post comment: ", e);
        }
    }


    public async Task<PaginationResponse<PostComment>> GetAllAsync(
        PaginationFilter getPagingPostCommentRequest)
    {
        var pageIndex = getPagingPostCommentRequest.PageIndex;
        var pageSize = getPagingPostCommentRequest.PageSize;

        var query = _dbContext.PostComments.AsQueryable();


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

    public async Task<PaginationResponse<PostComment>> GetAllByPostAsync(
        GetPagingPostCommentRequest getPagingPostCommentRequest)
    {
        var pageIndex = getPagingPostCommentRequest.PageIndex;
        var pageSize = getPagingPostCommentRequest.PageSize;
        var postId = getPagingPostCommentRequest.PostId;

        var query = _dbContext.PostComments.AsQueryable();

        if (postId != null)
            query = query.Where(p => p.PostId.Equals(postId)).OrderByDescending(p => p.CreatedAt);

        // Tính tổng số lượng record
        var totalRecords = await query.CountAsync();

        // Lấy ra trang trong request cần
        var posts = await query
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginationResponse<PostComment>
        {
            TotalCount = totalRecords,
            Items = posts,
            PageIndex = pageIndex,
            PageSize = pageSize
        };
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