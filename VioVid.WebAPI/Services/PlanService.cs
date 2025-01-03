using Application.DTOs.Plan;
using Application.Exceptions;
using Microsoft.EntityFrameworkCore;
using VioVid.Core.Entities;
using VioVid.Infrastructure.DatabaseContext;
using VioVid.WebAPI.ServiceContracts;

namespace VioVid.WebAPI.Services;

public class PlanService : IPlanService
{
    private readonly ApplicationDbContext _dbContext;
    
    public PlanService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<List<Plan>> GetAllAsync()
    {
        return await _dbContext.Plans.OrderBy(p => p.Price).ToListAsync();
    }

    public async Task<Plan> CreatePlanAsync(CreatePlanRequest createPlanRequest)
    {
        // Kiểm tra bảng có dữ liệu hay không
        // int maxOrder = -1;
        // if (await _dbContext.Plans.AnyAsync())
        // {
        //     maxOrder = await _dbContext.Plans.MaxAsync(p => p.Order);
        // }
        
        var newPlan = new Plan()
        {
            Name = createPlanRequest.Name,
            Price = createPlanRequest.Price,
            Duration = createPlanRequest.Duration,
            // Order = maxOrder + 1,
        };
        
        await _dbContext.Plans.AddAsync(newPlan);
        await _dbContext.SaveChangesAsync();
        return newPlan;
    }

    public async Task<Plan> UpdatePlanAsync(Guid id, UpdatePlanRequest updateGenreRequest)
    {
        var plan = await _dbContext.Plans.FindAsync(id);
        if (plan == null)
        {
            throw new NotFoundException($"Không tìm thấy Plan có id {id}");
        }
        
        plan.Name = updateGenreRequest.Name;
        plan.Price = updateGenreRequest.Price;
        plan.Duration = updateGenreRequest.Duration;
        // plan.Order = updateGenreRequest.Order;
        
        await _dbContext.SaveChangesAsync();
        return plan;
    }

    public async Task<Guid> DeletePlanAsync(Guid id)
    {
        var plan = await _dbContext.Plans.FindAsync(id);
        if (plan == null)
        {
            throw new NotFoundException($"Không tìm thấy Plan có id {id}");
        }
        
        _dbContext.Plans.Remove(plan);
        await _dbContext.SaveChangesAsync();
        return id;
    }
}