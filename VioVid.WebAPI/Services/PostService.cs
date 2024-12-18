using Application.DTOs.Post;
using Application.DTOs.Post.Res;
using Application.Exceptions;
using Application.Models;
using Microsoft.EntityFrameworkCore;
using VioVid.Core.Common;
using VioVid.Core.Entities;
using VioVid.Infrastructure.DatabaseContext;
using VioVid.WebAPI.ServiceContracts;

namespace VioVid.WebAPI.Services;

public class PostService : IPostService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;


    public PostService(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<PostResponse> GetByIdAsync(Guid id)
    {
        var post = await _dbContext.Posts
            .Include(post => post.ApplicationUser)
            .ThenInclude(applicationUser => applicationUser.UserProfile)
            .Include(post => post.PostComments)
            .ThenInclude(comment => comment.ApplicationUser)
            .ThenInclude(applicationUser => applicationUser.UserProfile)
            .FirstOrDefaultAsync(post => post.Id == id);

        if (post == null) throw new NotFoundException($"Không tìm thấy Post có id {id}");

        return new PostResponse
        {
            Id = post.Id,
            OwnerName = post.ApplicationUser.UserProfile.Name,
            OwnerAvatar = post.ApplicationUser.UserProfile.Avatar,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt,
            Hashtags = post.Hashtags,
            Content = post.Content,
            ImageUrls = post.ImageUrls,
            Likes = post.Likes,
            Comments = post.PostComments.Select(comment => new PostCommentResponse
            {
                Id = comment.Id,
                CreatedAt = comment.CreatedAt,
                Content = comment.Content,
                UserName = comment.ApplicationUser.UserProfile.Name,
                UserAvatar = comment.ApplicationUser.UserProfile.Avatar
            }).ToList()
        };
    }

    public async Task<PaginationResponse<Post>> GetAllByChannelAsync(GetPagingPostRequest getPagingPostRequest)
    {
        var pageIndex = getPagingPostRequest.PageIndex;
        var pageSize = getPagingPostRequest.PageSize;
        var channelId = getPagingPostRequest.ChannelId;

        var query = _dbContext.Posts.AsQueryable();

        if (channelId != null)
            query = query.Where(p => p.ChannelId.Equals(channelId));

        // Tính tổng số lượng record
        var totalRecords = await query.CountAsync();

        // Lấy ra trang trong request cần
        var posts = await query
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginationResponse<Post>
        {
            TotalCount = totalRecords,
            Items = posts,
            PageIndex = pageIndex,
            PageSize = pageSize
        };
    }

    public async Task<Post> CreatePostAsync(CreatePostRequest createPostRequest)
    {
        try
        {
            var user = _httpContextAccessor.HttpContext?.User!;
            Console.WriteLine($"User: {user?.Identity?.Name}");
            var userIdClaim = user.FindFirst("UserId");
            var applicationUserId = Guid.Parse(userIdClaim!.Value);

            var applicationUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id.Equals(applicationUserId));

            Console.WriteLine($"ApplicationUserId: {applicationUserId}");

            var post = new Post
            {
                ChannelId = createPostRequest.ChannelId,
                Content = createPostRequest.Content,
                ApplicationUserId = applicationUserId,
                Hashtags = createPostRequest.Hashtags,
                ImageUrls = createPostRequest.ImageUrls,
                Likes = 0,
                CreatedAt = DateTime.UtcNow, 
                UpdatedAt = DateTime.UtcNow,
                ApplicationUser = applicationUser!
            };

            await _dbContext.Posts.AddAsync(post);


            await _dbContext.SaveChangesAsync();
            return post;
        }
        catch (Exception e)
        {
            throw new Exception("Cannot create post: ", e);
        }
    }

    public async Task<PaginationResponse<Post>> GetAllAsync(PaginationFilter getPagingPostRequest)
    {
        var pageIndex = getPagingPostRequest.PageIndex;
        var pageSize = getPagingPostRequest.PageSize;

        var query = _dbContext.Posts.AsQueryable();

        // Tính tổng số lượng record
        var totalRecords = await query.CountAsync();

        // Lấy ra trang trong request cần
        var posts = await query
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginationResponse<Post>
        {
            TotalCount = totalRecords,
            Items = posts,
            PageIndex = pageIndex,
            PageSize = pageSize
        };
    }

    //
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