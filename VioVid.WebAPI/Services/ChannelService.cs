using Application.DTOs.Channel;
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

    public ChannelService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PaginationResponse<Channel>> GetAllAsync(GetPagingChannelRequest getPagingChannelRequest)
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
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginationResponse<Channel>
        {
            TotalCount = totalRecords,
            Items = channels,
            PageIndex = pageIndex,
            PageSize = pageSize
        };
    }

    public async Task<Channel> GetByIdAsync(Guid id)
    {
        var channel = await _dbContext.Channels
            .FirstOrDefaultAsync(p => p.Id == id);
        if (channel == null) throw new NotFoundException($"Không tìm thấy Channel có id {id}");
        return channel;
    }

    public async Task<Channel> CreateChannelAsync(CreateChannelRequest createChannelRequest)
    {
        var newChannel = new Channel
        {
            Name = createChannelRequest.Name,
            Description = createChannelRequest.Description,
            CreatedAt = DateTime.UtcNow
        };

        await _dbContext.Channels.AddAsync(newChannel);
        await _dbContext.SaveChangesAsync();
        return newChannel;
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