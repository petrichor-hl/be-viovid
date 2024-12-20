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
            .Include(channel => channel.UserChannels)
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .Select(channel => new ChannelResponse
            {
                Id = channel.Id,
                Name = channel.Name,
                Description = channel.Description,
                CreatedAt = channel.CreatedAt,
                UserChannels = channel.UserChannels
                    .Select(userChannel => new UserChannelResponse
                    {
                        ApplicationUserId = userChannel.ApplicationUserId,
                        ChannelId = userChannel.ChannelId
                    })
                    .ToList()
            })
            .ToListAsync();

        Console.WriteLine($"Total Records: {totalRecords}");
        Console.WriteLine($"Detail: {JsonSerializer.Serialize(channels)}");
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
        try
        {
            Console.WriteLine($"Id: {id}");
            var channel = await _dbContext.Channels
                .Include(c => c.UserChannels)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (channel == null) throw new NotFoundException($"Không tìm thấy Channel có id {id}");

            return new ChannelResponse
            {
                Id = channel.Id,
                Name = channel.Name,
                Description = channel.Description,
                CreatedAt = channel.CreatedAt,
                UserChannels = channel.UserChannels.Select(userChannel => new UserChannelResponse
                {
                    ApplicationUserId = userChannel.ApplicationUserId,
                    ChannelId = userChannel.ChannelId
                }).ToList()
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
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

        var userChannel = await _dbContext.UserChannels.AddAsync(new UserChannel
        {
            ChannelId = newChannel.Id,
            ApplicationUserId = applicationUserId
        });

        newChannel.UserChannels.Add(userChannel.Entity);

        await _dbContext.SaveChangesAsync();

        return new ChannelResponse
        {
            Id = newChannel.Id,
            Name = newChannel.Name,
            Description = newChannel.Description,
            CreatedAt = newChannel.CreatedAt,
            UserChannels = newChannel.UserChannels
                .Select(userChannel => new UserChannelResponse
                {
                    ApplicationUserId = userChannel.ApplicationUserId,
                    ChannelId = userChannel.ChannelId
                })
                .ToList()
        };
    }

    public async Task<bool> SubscribeAsync(SubscribeChannelRequest subscribeChannelRequest)
    {
        try
        {
            var user = _httpContextAccessor.HttpContext?.User!;
            var userIdClaim = user.FindFirst("UserId");

            if (userIdClaim == null) throw new UnauthorizedAccessException("User is not authenticated.");

            var applicationUserId = Guid.Parse(userIdClaim.Value);

            var channelId = subscribeChannelRequest.ChannelId;
            var existingSubscription = await _dbContext.UserChannels
                .FirstOrDefaultAsync(uc => uc.ChannelId == channelId && uc.ApplicationUserId == applicationUserId);

            if (existingSubscription != null)
                throw new InvalidOperationException("User is already subscribed to this channel.");

            var newSubscription = new UserChannel
            {
                ChannelId = channelId,
                ApplicationUserId = applicationUserId
            };

            await _dbContext.UserChannels.AddAsync(newSubscription);
            await _dbContext.SaveChangesAsync();

            Console.WriteLine($"User {applicationUserId} subscribed to channel {channelId}.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return false;
        }
    }

    public async Task<bool> UnsubscribeAsync(SubscribeChannelRequest subscribeChannelRequest)
    {
        try
        {
            var user = _httpContextAccessor.HttpContext?.User!;
            var userIdClaim = user.FindFirst("UserId");

            if (userIdClaim == null) throw new UnauthorizedAccessException("User is not authenticated.");

            var applicationUserId = Guid.Parse(userIdClaim.Value);

            var channelId = subscribeChannelRequest.ChannelId;

            var subscription = await _dbContext.UserChannels
                .FirstOrDefaultAsync(uc => uc.ChannelId == channelId && uc.ApplicationUserId == applicationUserId);

            if (subscription == null) throw new InvalidOperationException("User is not subscribed to this channel.");

            _dbContext.UserChannels.Remove(subscription);
            await _dbContext.SaveChangesAsync();

            Console.WriteLine($"User {applicationUserId} unsubscribed from channel {channelId}.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return false;
        }
    }

    public async Task<PaginationResponse<SimplePostResponse>> GetListPost(Guid channelId)
    {
        var channel = await _dbContext.Channels
            .Include(channel => channel.Posts)
            .FirstOrDefaultAsync(channel => channel.Id == channelId);

        // Implement Paging ...

        throw new NotImplementedException();
    }

    public async Task<PaginationResponse<ChannelResponse>> GetAllByUserAsync(
        GetPagingChannelRequest getPagingChannelRequest)
    {
        try
        {
            Console.WriteLine("Get all channels by user");
            var pageIndex = getPagingChannelRequest.PageIndex;
            var pageSize = getPagingChannelRequest.PageSize;
            var searchText = getPagingChannelRequest.SearchText?.ToLower();


            var user = _httpContextAccessor.HttpContext?.User!;
            Console.WriteLine($"User: {user?.Identity?.Name}");
            var userIdClaim = user.FindFirst("UserId");
            var applicationUserId = Guid.Parse(userIdClaim!.Value);

            var query = _dbContext.Channels.AsQueryable();

            if (!string.IsNullOrEmpty(searchText))
                query = query.Where(p => p.Id.ToString().ToLower().Contains(searchText) ||
                                         p.Name.ToLower().Contains(searchText));

            // Filter only channels where UserChannels match the ApplicationUserId
            query = query.Where(p => p.UserChannels.Any(u => u.ApplicationUserId == applicationUserId));
            // Tính tổng số lượng record
            var totalRecords = await query.CountAsync();

            // Lấy ra trang trong request cần
            var channels = await query
                .Include(channel => channel.UserChannels)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .Select(channel => new ChannelResponse
                {
                    Id = channel.Id,
                    Name = channel.Name,
                    Description = channel.Description,
                    CreatedAt = channel.CreatedAt,
                    UserChannels = channel.UserChannels
                        .Select(userChannel => new UserChannelResponse
                        {
                            ApplicationUserId = userChannel.ApplicationUserId,
                            ChannelId = userChannel.ChannelId
                        })
                        .ToList()
                })
                .ToListAsync();

            Console.WriteLine($"Total Records: {totalRecords}");
            Console.WriteLine($"Detail: {JsonSerializer.Serialize(channels)}");
            return new PaginationResponse<ChannelResponse>
            {
                TotalCount = totalRecords,
                Items = channels,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return null;
        }
    }
}