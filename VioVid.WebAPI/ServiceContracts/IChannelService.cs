using Application.DTOs.Channel;
using Application.DTOs.Channel.Res;
using VioVid.Core.Common;
using VioVid.Core.Entities;

namespace VioVid.WebAPI.ServiceContracts;

public interface IChannelService
{
    Task<PaginationResponse<ChannelResponse>> GetAllAsync(GetPagingChannelRequest getPagingChannelRequest);

    Task<ChannelResponse> GetByIdAsync(Guid id);

    Task<ChannelResponse> CreateChannelAsync(CreateChannelRequest createChannelRequest);
    
    // HERE
    Task<PaginationResponse<SimplePostResponse>> GetListPost(Guid channelId);

    // Task<Channel> UpdatePersonAsync(Guid id, UpdatePersonRequest updatePersonRequest);
}