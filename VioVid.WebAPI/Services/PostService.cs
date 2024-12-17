using Application.DTOs.Post;
using Application.Exceptions;
using Microsoft.EntityFrameworkCore;
using VioVid.Core.Common;
using VioVid.Core.Entities;
using VioVid.Infrastructure.DatabaseContext;
using VioVid.WebAPI.ServiceContracts;

namespace VioVid.WebAPI.Services;

public class PostService : IPostService
{
    private readonly ApplicationDbContext _dbContext;

    public PostService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PaginationResponse<Post>> GetAllAsync(GetPagingPostRequest getPagingPostRequest)
    {
        var pageIndex = getPagingPostRequest.PageIndex;
        var pageSize = getPagingPostRequest.PageSize;
        var channelId = getPagingPostRequest.ChannelId;

        var query = _dbContext.Posts.AsQueryable();

        if (channelId != null)
            query = query.Where(p => p.ChannelId == channelId);

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

    public async Task<Post> GetByIdAsync(Guid id)
    {
        var post = await _dbContext.Posts
            .FirstOrDefaultAsync(p => p.Id == id);
        if (post == null) throw new NotFoundException($"Không tìm thấy Post có id {id}");
        return post;
    }

    public async Task<Post> CreatePostAsync(CreatePostRequest createPostRequest)
    {
        var post = new Post
        {
            ChannelId = createPostRequest.ChannelId,
            Content = createPostRequest.Content,
            ApplicationUserId = createPostRequest.ApplicationUserId,
            Hashtags = createPostRequest.Hashtags,
            ImageUrls = createPostRequest.ImageUrls
        };

        await _dbContext.Posts.AddAsync(post);
        await _dbContext.SaveChangesAsync();
        return post;
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