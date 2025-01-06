using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Application.DTOs.Film.Req;

namespace Application.DTOs.Film.Req;

public class CreateFilmRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    public string Overview { get; set; } = string.Empty;
    
    [Required]
    public string PosterPath { get; set; } = string.Empty;
    
    [Required]
    public string BackdropPath { get; set; } = string.Empty;
    
    [Required]
    public string ContentRating { get; set; } = string.Empty;
    
    [Required]
    public DateOnly? ReleaseDate { get; set; }
    
    [Required]
    public string TmdbId { get; set; } = string.Empty;
    
    [JsonPropertyName("seasons")]
    public IEnumerable<SeasonRequest> SeasonRequests { get; set; } = new List<SeasonRequest>();
    
    [JsonPropertyName("genreIds")]
    public IEnumerable<Guid> GenreIds { get; set; } = new List<Guid>();
    
    [JsonPropertyName("topicIds")]
    public IEnumerable<Guid> TopicIds { get; set; } = new List<Guid>();
    
    [JsonPropertyName("casts")]
    public IEnumerable<CastRequest> CastRequests { get; set; } = new List<CastRequest>();
    
    [JsonPropertyName("crews")]
    public IEnumerable<CrewRequest> CrewRequests { get; set; } = new List<CrewRequest>();
}

public class SeasonRequest
{
    public Guid? Id { get; set; }
    
    public int? Order { get; set; }
    
    // [Required]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("episodes")]
    public IEnumerable<EpisodeRequest> EpisodeRequests { get; set; } = new List<EpisodeRequest>();
}

public class EpisodeRequest
{
    public Guid? Id { get; set; }
    
    public int? Order { get; set; }
    
    [Required]
    public string Title { get; set; } = string.Empty;
    
    // [Required]
    public string Summary { get; set; } = string.Empty;
    
    [Required]
    public string Source { get; set; } = string.Empty;
    
    [Required]
    public int Duration { get; set; }
    
    // [Required]
    public string StillPath { get; set; } = string.Empty;
    
    [Required]
    public bool IsFree { get; set; }
}

public class CastRequest
{
    [Required]
    public Guid PersonId { get; set; }
    
    [Required]
    public string Character { get; set; } = null!;
}

public class CrewRequest
{
    [Required]
    public Guid PersonId { get; set; }
    
    [Required]
    public string Role { get; set; } = null!;
}
