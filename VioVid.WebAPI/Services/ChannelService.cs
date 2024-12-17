using System.Text.Json;
using Application.DTOs.Channel;
using Application.DTOs.Channel.Res;
using Application.Exceptions;
using Microsoft.EntityFrameworkCore;
using VioVid.Core.Common;
using VioVid.Core.Entities;
using VioVid.Infrastructure.DatabaseContext;
using VioVid.WebAPI.ServiceContracts;

namespace VioVid.WebAPI.Services;

public class ChannelService : IChannelService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;


    public ChannelService(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<PaginationResponse<ChannelResponse>> GetAllAsync(GetPagingChannelRequest getPagingChannelRequest)
    {
        Console.WriteLine("Total Records: ");

        var pageIndex = getPagingChannelRequest.PageIndex;
        var pageSize = getPagingChannelRequest.PageSize;
        var searchText = getPagingChannelRequest.SearchText?.ToLower();

        var query = _dbContext.Channels.AsQueryable();

        if (!string.IsNullOrEmpty(searchText))
            query = query.Where(p => p.Id.ToString().ToLower().Contains(searchText) ||
                                     p.Name.ToLower().Contains(searchText));

        // Tính tổng số lượng record
        var totalRecords = await query.CountAsync();

        // Lấy ra trang trong request cần
        var channels = await query
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .Select(channel => new ChannelResponse
            {
                Id = channel.Id,
                Name = channel.Name,
                Description = channel.Description,
                CreatedAt = channel.CreatedAt,
            })
            .ToListAsync();
        
        Console.WriteLine($"Total Records: {totalRecords}");
        return new PaginationResponse<ChannelResponse>
        {
            TotalCount = totalRecords,
            Items = channels,
            PageIndex = pageIndex,
            PageSize = pageSize
        };
    }

    public async Task<ChannelResponse> GetByIdAsync(Guid id)
    {
        var channel = await _dbContext.Channels.FindAsync(id);
        if (channel == null)
        {
            throw new NotFoundException($"Không tìm thấy Channel có id {id}");
        }
        return new ChannelResponse
        {
            Id = channel.Id,
            Name = channel.Name,
            Description = channel.Description,
            CreatedAt = channel.CreatedAt,
        };
    }

    public async Task<ChannelResponse> CreateChannelAsync(CreateChannelRequest createChannelRequest)
    {
        var user = _httpContextAccessor.HttpContext?.User!;
        Console.WriteLine($"User: {user?.Identity?.Name}");
        var userIdClaim = user.FindFirst("UserId");
        var applicationUserId = Guid.Parse(userIdClaim!.Value);

        Console.WriteLine($"ApplicationUserId: {applicationUserId}");
        
        var newChannel = new Channel
        {
            Id = Guid.NewGuid(),
            Name = createChannelRequest.Name,
            Description = createChannelRequest.Description,
            CreatedAt = DateTime.UtcNow
        };
        
        await _dbContext.Channels.AddAsync(newChannel);
        await _dbContext.UserChannels.AddAsync(new UserChannel
        {
            ChannelId = newChannel.Id,
            ApplicationUserId = applicationUserId
        });
        
        await _dbContext.SaveChangesAsync();
        return new ChannelResponse
        {
            Id = newChannel.Id,
            Name = newChannel.Name,
            Description = newChannel.Description,
            CreatedAt = newChannel.CreatedAt
        };
    }

    public async Task<PaginationResponse<SimplePostResponse>> GetListPost(Guid channelId)
    {
        var channel = await _dbContext.Channels
            .Include(channel => channel.Posts)
            .FirstOrDefaultAsync(channel => channel.Id == channelId);
        
        // Implement Paging ...
        
        throw new NotImplementedException();
    }
}