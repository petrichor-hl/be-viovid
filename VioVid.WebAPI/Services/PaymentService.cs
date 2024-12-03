using System.Text.Json;
using Application.DTOs.Genre;
using Application.DTOs.Payment;
using Application.Exceptions;
using Microsoft.EntityFrameworkCore;
using VioVid.Core.Entities;
using VioVid.Infrastructure.DatabaseContext;
using VioVid.WebAPI.ServiceContracts;

namespace VioVid.WebAPI.Services;

public class PaymentService : IPaymentService
{
    private readonly ApplicationDbContext _dbContext;

    public PaymentService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Payment> CreatePayment(CreatePaymentRequest createPaymentRequest)
    {
        var newPayment = new Payment
        {
            ApplicationUserId = createPaymentRequest.ApplicationUserId,
            PlanId = createPaymentRequest.PlanId,
            CreatedAt = DateTime.UtcNow,
            IsDone = false
        };
        await _dbContext.Payments.AddAsync(newPayment);
        await _dbContext.SaveChangesAsync();
        Console.WriteLine("New payment: " + JsonSerializer.Serialize(newPayment));

        return newPayment;
    }

    public async Task<Payment> UpdatePayment(Payment payment)
    {
        var oldPayment = await _dbContext.Payments.FindAsync(payment.Id);
        if (oldPayment == null) throw new NotFoundException($"Không tìm thấy Payment có id {payment.Id}");
        oldPayment = payment;
        await _dbContext.SaveChangesAsync();
        return oldPayment;
    }
    
    public async Task<List<Genre>> GetAllAsync()
    {
        return await _dbContext.Genres.ToListAsync();
    }

    public async Task<Genre> GetByIdAsync(Guid id)
    {
        var genre = await _dbContext.Genres.FindAsync(id);
        if (genre == null) throw new NotFoundException($"Không tìm thấy Thể loại có id {id}");
        return genre;
    }


    public async Task<Guid> DeleteGefdasnreAsync(Guid id)
    {
        var genre = await _dbContext.Genres.FindAsync(id);
        if (genre == null) throw new NotFoundException($"Không tìm thấy Genre có id {id}");
        _dbContext.Genres.Remove(genre);
        await _dbContext.SaveChangesAsync();
        return id;
    }

    public async Task<Genre> CreateGefdsafnreAsync(CreateGenreRequest createGenreRequest)
    {
        var newGenre = new Genre
        {
            Name = createGenreRequest.Name
        };
        await _dbContext.Genres.AddAsync(newGenre);
        await _dbContext.SaveChangesAsync();
        return newGenre;
    }

    public async Task<Genre> UpdateGenrfdsaeAsync(Guid id, UpdateGenreRequest updateGenreRequest)
    {
        var genre = await _dbContext.Genres.FindAsync(id);
        if (genre == null) throw new NotFoundException($"Không tìm thấy Genre có id {id}");
        genre.Name = updateGenreRequest.Name;
        await _dbContext.SaveChangesAsync();

        return genre;
    }


    public async Task<Guid> DeleteGenfdsafeAsync(Guid id)
    {
        var genre = await _dbContext.Genres.FindAsync(id);
        if (genre == null) throw new NotFoundException($"Không tìm thấy Genre có id {id}");
        _dbContext.Genres.Remove(genre);
        await _dbContext.SaveChangesAsync();
        return id;
    }

    public async Task<Genre> CreateGenrfdsafeAsync(CreateGenreRequest createGenreRequest)
    {
        var newGenre = new Genre
        {
            Name = createGenreRequest.Name
        };
        await _dbContext.Genres.AddAsync(newGenre);
        await _dbContext.SaveChangesAsync();
        return newGenre;
    }

    public async Task<Genre> UpdateGenfdsafdreAsync(Guid id, UpdateGenreRequest updateGenreRequest)
    {
        var genre = await _dbContext.Genres.FindAsync(id);
        if (genre == null) throw new NotFoundException($"Không tìm thấy Genre có id {id}");
        genre.Name = updateGenreRequest.Name;
        await _dbContext.SaveChangesAsync();

        return genre;
    }


    public async Task<Guid> DeleteGefdsafnreAsync(Guid id)
    {
        var genre = await _dbContext.Genres.FindAsync(id);
        if (genre == null) throw new NotFoundException($"Không tìm thấy Genre có id {id}");
        _dbContext.Genres.Remove(genre);
        await _dbContext.SaveChangesAsync();
        return id;
    }

    public async Task<Genre> CreateGenreAsync(CreateGenreRequest createGenreRequest)
    {
        var newGenre = new Genre
        {
            Name = createGenreRequest.Name
        };
        await _dbContext.Genres.AddAsync(newGenre);
        await _dbContext.SaveChangesAsync();
        return newGenre;
    }

    public async Task<Genre> UpdateGenreAsync(Guid id, UpdateGenreRequest updateGenreRequest)
    {
        var genre = await _dbContext.Genres.FindAsync(id);
        if (genre == null) throw new NotFoundException($"Không tìm thấy Genre có id {id}");
        genre.Name = updateGenreRequest.Name;
        await _dbContext.SaveChangesAsync();

        return genre;
    }


    public async Task<Guid> DeleteGenreAsync(Guid id)
    {
        var genre = await _dbContext.Genres.FindAsync(id);
        if (genre == null) throw new NotFoundException($"Không tìm thấy Genre có id {id}");
        _dbContext.Genres.Remove(genre);
        await _dbContext.SaveChangesAsync();
        return id;
    }
}