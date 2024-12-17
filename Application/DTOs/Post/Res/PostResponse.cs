using Application.DTOs.Channel.Res;

namespace Application.DTOs.Post.Res;

public class PostResponse : SimplePostResponse
{
    public List<PostCommentResponse> Comments { get; set; }
}

public class PostCommentResponse {
    
    public Guid Id { get; set; }
    public string UserName { get; set; } = null!;
    public string UserAvatar { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public string Content { get; set; } = null!;
}