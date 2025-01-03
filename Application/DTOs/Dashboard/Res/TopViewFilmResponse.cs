using Application.DTOs.Film.Res;

namespace Application.DTOs.Dashboard.Res;

public class TopViewFilmResponse : SimpleFilmResponse
{
    public int NumberOfViews { get; set; }
}