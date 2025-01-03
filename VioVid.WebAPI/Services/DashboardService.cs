using Application.DTOs.Dashboard;
using Application.DTOs.Dashboard.Req;
using Microsoft.EntityFrameworkCore;
using VioVid.Infrastructure.DatabaseContext;
using VioVid.WebAPI.ServiceContracts;
namespace VioVid.WebAPI.Services;

public class DashboardService : IDashboardService
{
    private readonly ApplicationDbContext _dbContext;
    
    public DashboardService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<List<int>> GetRegistrationsPerMonthAsync(GetUserRegistrationStatsRequest getUserRegistrationStatsRequest)
    {
        var stats = await _dbContext.Users
            .Where(user => user.CreatedDate.Year == getUserRegistrationStatsRequest.Year)
            .GroupBy(user => user.CreatedDate.Month)
            .Select(group => new
            {
                Month = group.Key,
                Count = group.Count()
            })
            .OrderBy(group => group.Month)
            .ToListAsync();
        
        
        // Tạo một danh sách List<int> với 12 phần tử mặc định là 0
        var monthlyStats = Enumerable.Repeat(0, 12).ToList();
        // Điền vào các giá trị vào danh sách monthlyStats từ kết quả truy vấn
        foreach (var stat in stats)
        {
            monthlyStats[stat.Month - 1] = stat.Count; // Điều chỉnh vì tháng bắt đầu từ 1
        }
        return monthlyStats;
    }
}