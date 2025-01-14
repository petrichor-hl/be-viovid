using Application.DTOs.Film.Res;
using Application.DTOs.Person.Req;
using Application.DTOs.Person.Res;
using Application.Exceptions;
using Microsoft.EntityFrameworkCore;
using VioVid.Core.Common;
using VioVid.Core.Entities;
using VioVid.Infrastructure.DatabaseContext;
using VioVid.WebAPI.ServiceContracts;

namespace VioVid.WebAPI.Services;

public class PersonService : IPersonService
{
    private readonly ApplicationDbContext _dbContext;
    
    public PersonService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<PaginationResponse<Person>> GetAllAsync(GetPagingPersonRequest getPagingPersonRequest)
    {
        var pageIndex = getPagingPersonRequest.PageIndex;
        var pageSize = getPagingPersonRequest.PageSize;
        var searchText = getPagingPersonRequest.SearchText?.ToLower();
        
        var query = _dbContext.Persons.AsQueryable();
        
        if (!string.IsNullOrEmpty(searchText))
        {
            query = query.Where(p => p.Id.ToString().ToLower().Contains(searchText) || 
                                     p.Name.ToLower().Contains(searchText));
        }
        
        // Tính tổng số lượng record
        var totalRecords = await query.CountAsync();
        
        // Lấy ra trang trong request cần
        var persons = await query
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return new PaginationResponse<Person>
        {
            TotalCount = totalRecords,
            Items = persons,
            PageIndex = pageIndex,
            PageSize = pageSize
        };
    }

    public async Task<PersonResponse> GetByIdAsync(Guid id)
    {
        var person = await _dbContext.Persons
            .Include(p => p.Casts)
            .ThenInclude(cast => cast.Film)
            .Include(p => p.Crews)
            .ThenInclude(crew => crew.Film)
            .FirstOrDefaultAsync(p => p.Id == id);
        if (person == null)
        {
            throw new NotFoundException($"Không tìm thấy Person có id {id}");
        }
        return new PersonResponse
        {
            Id = person.Id,
            Name = person.Name,
            Gender = person.Gender,
            Popularity = person.Popularity,
            ProfilePath = person.ProfilePath,
            Biography = person.Biography,
            KnownForDepartment = person.KnownForDepartment,
            Dob = person.Dob,
            Films = person.Casts.Select(cast => new SimpleFilmResponse
            {
                FilmId = cast.Film.Id,
                Name = cast.Film.Name,
                PosterPath = cast.Film.PosterPath,
            }).Concat(person.Crews.Select(crew => new SimpleFilmResponse
            {
                FilmId = crew.Film.Id,
                Name = crew.Film.Name,
                PosterPath = crew.Film.PosterPath,
            })).ToList(),
        };
    }

    public async Task<Person> CreatePersonAsync(CreatePersonRequest createPersonRequest)
    {
        var newPerson = new Person()
        {
            Name = createPersonRequest.Name,
            Gender = createPersonRequest.Gender,
            Popularity = createPersonRequest.Popularity,
            ProfilePath = createPersonRequest.ProfilePath,
            Biography = createPersonRequest.Biography,
            KnownForDepartment = createPersonRequest.KnownForDepartment,
            Dob = createPersonRequest.Dob,
        };
        
        await _dbContext.Persons.AddAsync(newPerson);
        await _dbContext.SaveChangesAsync();
        return newPerson;
    }

    public async Task<Person> UpdatePersonAsync(Guid id, UpdatePersonRequest updatePersonRequest)
    {
        var person = await _dbContext.Persons.FindAsync(id);
        if (person == null)
        {
            throw new NotFoundException($"Không tìm thấy Person có id {id}");
        }
        
        person.Name = updatePersonRequest.Name;
        person.Gender = updatePersonRequest.Gender;
        person.Popularity = updatePersonRequest.Popularity;
        person.ProfilePath = updatePersonRequest.ProfilePath;
        person.Biography = updatePersonRequest.Biography;
        person.KnownForDepartment = updatePersonRequest.KnownForDepartment;
        person.Dob = updatePersonRequest.Dob;
        
        await _dbContext.SaveChangesAsync();
        return person;
    }

    public async Task<Guid> DeletePersonAsync(Guid id)
    {
        var person = await _dbContext.Persons.FindAsync(id);
        if (person == null)
        {
            throw new NotFoundException($"Không tìm thấy Person có id {id}");
        }
        
        _dbContext.Persons.Remove(person);
        await _dbContext.SaveChangesAsync();
        return id;
    }
}