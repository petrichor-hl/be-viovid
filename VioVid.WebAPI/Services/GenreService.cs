using Application.DTOs.Film.Res;
using Application.DTOs.Genre;
using Application.DTOs.Genre.Res;
using Application.Exceptions;
using Microsoft.EntityFrameworkCore;
using VioVid.Core.Entities;
using VioVid.Infrastructure.DatabaseContext;
using VioVid.WebAPI.ServiceContracts;

namespace VioVid.WebAPI.Services;

public class GenreService : IGenreService
{
    private readonly ApplicationDbContext _dbContext;
    
    public GenreService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<List<Genre>> GetAllAsync()
    {
        return await _dbContext.Genres.ToListAsync();
    }

    public async Task<GenreResponse> GetByIdAsync(Guid id)
    {
        var genre = await _dbContext.Genres
            .Include(genre => genre.GenreFilms)
                .ThenInclude(genreFilm => genreFilm.Film)
            .FirstOrDefaultAsync(genre => genre.Id == id);
        if (genre == null)
        {
            throw new NotFoundException($"Không tìm thấy Genre có id {id}");
        }
        return new GenreResponse
        {
            Id = genre.Id,
            Name = genre.Name,
            Films = genre.GenreFilms.Select(genreFilm => new SimpleFilmResponse
            {
                FilmId = genreFilm.FilmId,
                Name = genreFilm.Film.Name,
                PosterPath = genreFilm.Film.PosterPath,
            }).ToList()
        };
    }

    public async Task<Genre> CreateGenreAsync(CreateGenreRequest createGenreRequest)
    {
        var newGenre = new Genre()
        {
            Name = createGenreRequest.Name,
        };
        await _dbContext.Genres.AddAsync(newGenre);
        await _dbContext.SaveChangesAsync();
        return newGenre;
    }

    public async Task<Genre> UpdateGenreAsync(Guid id, UpdateGenreRequest updateGenreRequest)
    {
        var genre = await _dbContext.Genres.FindAsync(id);
        if (genre == null)
        {
            throw new NotFoundException($"Không tìm thấy Genre có id {id}");
        }
        genre.Name = updateGenreRequest.Name;
        await _dbContext.SaveChangesAsync();

        return genre;
    }


    public async Task<Guid> DeleteGenreAsync(Guid id)
    {
        var genre = await _dbContext.Genres.FindAsync(id);
        if (genre == null)
        {
            throw new NotFoundException($"Không tìm thấy Genre có id {id}");
        }
        _dbContext.Genres.Remove(genre);
        await _dbContext.SaveChangesAsync();
        return id;
    }
}