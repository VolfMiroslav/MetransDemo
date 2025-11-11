// Data/DataSeeder.cs (or similar location)

using MetransDemo.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public static class DataSeeder
{
    public static async Task SeedAllAsync(MetransDemoContext context)
    {
        await context.Database.EnsureCreatedAsync();

        if (await context.Orders.AnyAsync())
        {
            return; // Data already exists, do not re-seed
        }


        var statusList = new List<Status>
        {
            new Status { Name = "Processing", Code = "P" },
            new Status { Name = "Shipped", Code = "S" },
            new Status { Name = "Cancelled", Code = "C" }
        };
        await context.Statuses.AddRangeAsync(statusList);

        var customerList = new List<Customer>
        {
            new Customer { Name = "ABC s.r.o." },
            new Customer { Name = "XYZ a.s." },
            new Customer { Name = "Novák spol. s.r.o." }
        };
        await context.Customers.AddRangeAsync(customerList);

        await context.SaveChangesAsync();

        var abcCustomer = customerList.First(c => c.Name == "ABC s.r.o.");
        var xyzCustomer = customerList.First(c => c.Name == "XYZ a.s.");
        var novakCustomer = customerList.First(c => c.Name == "Novák spol. s.r.o.");

        var processingStatus = statusList.First(s => s.Code == "P");
        var shippedStatus = statusList.First(s => s.Code == "S");
        var cancelledStatus = statusList.First(s => s.Code == "C");


        var orders = new List<Order>
        {
            new Order
            {
                OrderDate = new DateTime(2025, 11, 8, 20, 6, 0),
                Amount = 12500.00m,
                CustomerId = abcCustomer.Id,
                StatusId = shippedStatus.Id 
            },
            new Order
            {
                OrderDate = new DateTime(2025, 11, 8, 17, 8, 0),
                Amount = 8000.00m,
                CustomerId = xyzCustomer.Id,
                StatusId = processingStatus.Id 
            },
            new Order
            {
                OrderDate = new DateTime(2025, 11, 8, 19, 56, 0),
                Amount = 11200.00m,
                CustomerId = novakCustomer.Id,
                StatusId = cancelledStatus.Id 
            },
            new Order
            {
                OrderDate = new DateTime(2025, 11, 9, 18, 46, 0),
                Amount = 13100.00m,
                CustomerId = abcCustomer.Id,
                StatusId = processingStatus.Id 
            }
        };

        await context.Orders.AddRangeAsync(orders);
        await context.SaveChangesAsync();
    }
}