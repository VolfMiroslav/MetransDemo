using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MetransDemo.Models;

namespace MetransDemo.Controllers
{
    public class OrdersController : Controller
    {
        private readonly MetransDemoContext _context;

        public OrdersController(MetransDemoContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index(
            int? customerFilterId,
            int? statusFilterId,
            DateTime? fromDate,
            DateTime? toDate
            )
        {
            List<SelectListItem> allCustomers = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = "0",
                    Text = " ",
                    Selected = !customerFilterId.HasValue || customerFilterId.Value == 0
                }
            };
            
           allCustomers.AddRange(await _context.Customers
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name,
                    Selected = customerFilterId.HasValue && c.Id == customerFilterId.Value
                })
                .OrderBy(c => c.Text)
                .ToListAsync());

            List<SelectListItem> allStatuses = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = "0",
                    Text = " ",
                    Selected = !statusFilterId.HasValue || statusFilterId.Value == 0
                }
            };
            allStatuses.AddRange(await _context.Statuses
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Code,
                    Selected = statusFilterId.HasValue && s.Id == statusFilterId.Value
                })
                .OrderBy(s => s.Text)
                .ToListAsync());        

            if (fromDate.HasValue)
            {
                ViewBag.CurrentFromDate = fromDate.Value.ToString("yyyy-MM-ddThh:mm");
            }

            if (toDate.HasValue)
            {
                ViewBag.CurrentToDate = toDate.Value.ToString("yyyy-MM-ddThh:mm");
            }

            ViewBag.CustomerList = allCustomers;
            ViewBag.StatusList = allStatuses;

            var ordersQuery = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Status)
                .AsQueryable();

            if (customerFilterId.HasValue && customerFilterId.Value != 0)
            {
                ordersQuery = ordersQuery.Where(o => o.CustomerId == customerFilterId.Value);
            }

            if (statusFilterId.HasValue && statusFilterId.Value != 0)
            {
                ordersQuery = ordersQuery.Where(o => o.StatusId == statusFilterId.Value);
            }

            if (fromDate.HasValue)
            {
                ordersQuery = ordersQuery.Where(o => o.OrderDate >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                ordersQuery = ordersQuery.Where(o => o.OrderDate < toDate.Value);
            }

            ordersQuery = ordersQuery.OrderByDescending(o => o.OrderDate);

            return View(await ordersQuery.ToListAsync());
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Status)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Name");
            ViewData["StatusId"] = new SelectList(_context.Statuses, "Id", "Code");
            return View();
        }

        // GET: Orders/CreateApi
        public IActionResult CreateApi()
        {
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,OrderDate,Amount,CustomerId,StatusId")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Name", order.CustomerId);
            ViewData["StatusId"] = new SelectList(_context.Statuses, "Id", "Code", order.StatusId);
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Name", order.CustomerId);
            ViewData["StatusId"] = new SelectList(_context.Statuses, "Id", "Code", order.StatusId);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OrderDate,Amount,CustomerId,StatusId")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Name", order.CustomerId);
            ViewData["StatusId"] = new SelectList(_context.Statuses, "Id", "Code", order.StatusId);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Status)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}
