using Application.DTOs.Channel;
using Application.DTOs.Channel.Res;
using VioVid.Core.Common;

namespace VioVid.WebAPI.ServiceContracts;

public interface IChannelService
{
    Task<PaginationResponse<ChannelResponse>> GetAllAsync(GetPagingChannelRequest getPagingChannelRequest);

    Task<PaginationResponse<ChannelResponse>> GetAllByUserAsync(GetPagingChannelRequest getPagingChannelRequest);


    Task<ChannelResponse> GetByIdAsync(Guid id);

    Task<ChannelResponse> CreateChannelAsync(CreateChannelRequest createChannelRequest);

    Task<bool> SubscribeAsync(SubscribeChannelRequest subscribeChannelRequest);

    Task<bool> UnsubscribeAsync(SubscribeChannelRequest subscribeChannelRequest);

    // HERE
    Task<PaginationResponse<SimplePostResponse>> GetListPost(Guid channelId);

    // Task<Channel> UpdatePersonAsync(Guid id, UpdatePersonRequest updatePersonRequest);
}