using System.Diagnostics;
using MetransDemo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MetransDemo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MetransDemoContext _context;

        private readonly DateTime _defaultYearStart;
        private readonly DateTime _defaultYearEnd;

        public HomeController(ILogger<HomeController> logger, MetransDemoContext context)
        {
            _logger = logger;
            _context = context;

            DateTime currentYear = DateTime.Today;
            _defaultYearStart = new DateTime(currentYear.Year, 1, 1);
            _defaultYearEnd = new DateTime(currentYear.Year, 12, 31);
        }

        public async Task<IActionResult> Index(
            DateTime? dailySumDate,
            DateTime? avgStartDate,
            DateTime? avgEndDate,
            DateTime? topCustomerStartDate,
            DateTime? topCustomerEndDate,
            DateTime? yearlySumStartDate, 
            DateTime? yearlySumEndDate)
        {
            await CalculateDailySumAsync(dailySumDate);

            await CalculateYearlySumAsync(yearlySumStartDate, yearlySumEndDate);

            await CalculateAverageAmountAsync(avgStartDate, avgEndDate);

            await CalculateTopCustomerAsync(topCustomerStartDate, topCustomerEndDate);

            return View();
        }

        private async Task CalculateDailySumAsync(DateTime? date)
        {
            DateTime filterDate = date?.Date ?? DateTime.Today;

            DateTime startOfDay = filterDate.Date;
            DateTime endOfNextDay = filterDate.Date.AddDays(1);

            decimal totalAmount = await _context.Orders
                .Where(o => o.OrderDate >= startOfDay && o.OrderDate < endOfNextDay)
                .SumAsync(o => o.Amount);

            ViewData["TotalAmount"] = totalAmount.ToString("N2");
            ViewData["DailySumDate"] = filterDate.Date.ToString("yyyy-MM-dd");
        }

        private async Task CalculateYearlySumAsync(DateTime? startDate, DateTime? endDate)
        {
            DateTime actualStart = startDate?.Date ?? _defaultYearStart;
            DateTime actualEnd = endDate?.Date ?? _defaultYearEnd;

            decimal totalAmount = await _context.Orders
                .Where(o => o.OrderDate >= actualStart && o.OrderDate < actualEnd)
                .SumAsync(o => o.Amount);

            ViewData["YearlyTotalAmount"] = totalAmount.ToString("N2");

            ViewData["YearlySumStartDate"] = actualStart.ToString("yyyy-MM-dd");
            ViewData["YearlySumEndDate"] = actualEnd.ToString("yyyy-MM-dd");
        }

        private async Task CalculateAverageAmountAsync(DateTime? avgStartDate, DateTime? avgEndDate)
        {
            DateTime avgStart = avgStartDate?.Date ?? _defaultYearStart;
            DateTime avgEnd = avgEndDate?.Date ?? _defaultYearEnd;

            var ordersInPeriod = _context.Orders
                .AsNoTracking()
                .Where(o => o.OrderDate >= avgStart && o.OrderDate < avgEnd);

            decimal averageAmount = 0;
            if (await ordersInPeriod.AnyAsync())
            {
                averageAmount = await ordersInPeriod.AverageAsync(o => o.Amount);
            }

            ViewData["AverageAmount"] = averageAmount.ToString("N2");
            ViewData["AvgStartDate"] = avgStart.ToString("yyyy-MM-dd");
            ViewData["AvgEndDate"] = avgEnd.ToString("yyyy-MM-dd");
        }

        private async Task CalculateTopCustomerAsync(DateTime? startDate, DateTime? endDate)
        {
            DateTime actualStart = startDate?.Date ?? _defaultYearStart;
            DateTime actualEnd = endDate?.Date ?? _defaultYearEnd;

            var topCustomer = await _context.Orders
                .Where(o => o.OrderDate >= actualStart && o.OrderDate < actualEnd)
                .GroupBy(o => o.CustomerId)
                .Select(g => new
                {
                    CustomerId = g.Key,
                    TotalAmount = g.Sum(o => o.Amount)
                })
                .OrderByDescending(x => x.TotalAmount)
                .Take(1)
                .FirstOrDefaultAsync();

            if (topCustomer != null)
            {
                var customer = await _context.Customers.FindAsync(topCustomer.CustomerId);
                ViewData["TopCustomerName"] = customer.Name;
                ViewData["TopCustomerAmount"] = topCustomer.TotalAmount.ToString("N2");
            }
            else
            {
                ViewData["TopCustomerName"] = "N/A";
                ViewData["TopCustomerAmount"] = "0.00";
            }

            ViewData["TopCustomerStartDate"] = actualStart.ToString("yyyy-MM-dd");
            ViewData["TopCustomerEndDate"] = actualEnd.ToString("yyyy-MM-dd");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
