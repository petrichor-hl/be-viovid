using System.Text.Json;
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
    private readonly ISupabaseService _supabaseService;


    public PostService(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor,
        ISupabaseService supabaseService)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
        _supabaseService = supabaseService;
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
            query = query.Where(p => p.ChannelId.Equals(channelId)).OrderByDescending(p => p.CreatedAt);

        // Tính tổng số lượng record
        var totalRecords = await query.CountAsync();

        // Lấy ra trang trong request cần
        var posts = await query
            .Include(post => post.ApplicationUser)
            .Include(p => p.PostComments)
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
            Console.WriteLine("Creating post...");

            // Retrieve the user from the current HTTP context
            var user = _httpContextAccessor.HttpContext?.User!;
            var userIdClaim = user.FindFirst("UserId");
            var applicationUserId = Guid.Parse(userIdClaim!.Value);

            // Retrieve the user from the database
            var applicationUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id.Equals(applicationUserId));

            // Prepare list to store image URLs
            var imageUrls = new List<string>();
            Console.WriteLine($"Post request: {JsonSerializer.Serialize(createPostRequest)}");


            // If images are provided in the request, upload them to Supabase and store the returned URLs
            if (createPostRequest.Images != null && createPostRequest.Images.Any())
                foreach (var image in createPostRequest.Images)
                    try
                    {
                        Console.WriteLine("Uploading image...");

                        var uniqueFileName = $"{Guid.NewGuid()}_{image.FileName}";

                        // Upload file to Supabase storage using the SupabaseService
                        var fileUrl =
                            await _supabaseService.UploadFileAsync(image.OpenReadStream(), uniqueFileName);

                        // Add the file URL to the list of image URLs
                        imageUrls.Add(fileUrl);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error uploading image: {ex.Message}");
                        // Optionally, handle individual image upload failure
                    }

            // Create a new post object
            var post = new Post
            {
                ChannelId = createPostRequest.ChannelId,
                Content = createPostRequest.Content,
                ApplicationUserId = applicationUserId,
                Hashtags = createPostRequest.Hashtags,
                ImageUrls = imageUrls.Any() ? imageUrls.ToArray() : null, // Only store URLs if any were uploaded
                Likes = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                ApplicationUser = applicationUser!
            };

            // Add the new post to the database
            await _dbContext.Posts.AddAsync(post);
            await _dbContext.SaveChangesAsync();

            return post;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error creating post: {e.Message}");
            throw new Exception("Cannot create post", e); // Re-throwing the exception to be handled by the caller
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

    public async Task<Post> LikePostAsync(Guid postId)
    {
        try
        {
            var post = await _dbContext.Posts.FirstOrDefaultAsync(p => p.Id == postId);
            if (post == null)
                throw new NotFoundException($"No post found with id {postId}");

            // Increment likes
            post.Likes++;
            _dbContext.Posts.Update(post);
            await _dbContext.SaveChangesAsync();
            return post;
        }
        catch (Exception e)
        {
            throw new Exception("Cannot like post: ", e);
        }
    }

    public async Task<Post> UnlikePostAsync(Guid postId)
    {
        try
        {
            var post = await _dbContext.Posts.FirstOrDefaultAsync(p => p.Id == postId);
            if (post == null)
                throw new NotFoundException($"No post found with id {postId}");

            // Decrement likes (ensure it doesn't go below 0)
            post.Likes = Math.Max(0, post.Likes - 1);
            _dbContext.Posts.Update(post);
            await _dbContext.SaveChangesAsync();
            return post;
        }
        catch (Exception e)
        {
            throw new Exception("Cannot unlike post: ", e);
        }
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