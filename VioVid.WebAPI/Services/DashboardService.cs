using Application.DTOs.Dashboard.Res;
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
    
    public async Task<List<int>> GetRegistrationsPerMonthAsync(int year)
    {
        var stats = await _dbContext.Users
            .Where(user => user.CreatedDate.Year == year)
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

    public async Task<List<PaymentSummaryResponse>> GetPaymentSummaryPerMonthAsync(int year)
    {
        var result = await _dbContext.Payments
            .Where(p => p.CreatedAt.Year == year && p.IsDone)
            .GroupBy(p => new { p.CreatedAt.Month, p.MethodName })
            .Select(g => new
            {
                Month = g.Key.Month,
                MethodName = g.Key.MethodName,
                TotalAmount = g.Sum(p => p.Amount)
            })
            .ToListAsync();

        var summary = new List<PaymentSummaryResponse>();

        // Khởi tạo các tháng (1 - 12)
        for (var month = 1; month <= 12; month++)
        {
            var momo = result.FirstOrDefault(x => x.Month == month && x.MethodName == "MOMO")?.TotalAmount ?? 0;
            var vnPay = result.FirstOrDefault(x => x.Month == month && x.MethodName == "VNPAY")?.TotalAmount ?? 0;
            var stripe = result.FirstOrDefault(x => x.Month == month && x.MethodName == "STRIPE")?.TotalAmount ?? 0;

            summary.Add(new PaymentSummaryResponse()
            {
                Month = month,
                Momo = momo,
                VnPay = vnPay,
                Stripe = stripe
            });
        }

        return summary;
    }
}