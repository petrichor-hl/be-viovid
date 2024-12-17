using Application.DTOs.Channel;
using VioVid.Core.Common;
using VioVid.Core.Entities;

namespace VioVid.WebAPI.ServiceContracts;

public interface IChannelService
{
    Task<PaginationResponse<Channel>> GetAllAsync(GetPagingChannelRequest getPagingChannelRequest);

    Task<Channel> GetByIdAsync(Guid id);

    Task<Channel> CreateChannelAsync(CreateChannelRequest createChannelRequest);

    // Task<Channel> UpdatePersonAsync(Guid id, UpdatePersonRequest updatePersonRequest);
}