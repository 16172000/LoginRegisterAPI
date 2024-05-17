using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TicketProjectWEB.Models;

namespace TicketProjectWEB.Controllers
{
    //[SessionAuthorize]
    public class AdminController : Controller
    {
        private readonly CoreProject5DbContext _context;

        public AdminController(CoreProject5DbContext context)
        {
            _context = context;
        }

        // GET: Admin
        public async Task<IActionResult> Index()
        {
              return _context.Registers != null ? 
                          View(await _context.Registers.ToListAsync()) :
                          Problem("Entity set 'CoreProject5DbContext.Registers'  is null.");
        }

        public async Task<IActionResult> UserDetails()
        {
            return _context.Registers != null ?
                        View(await _context.Registers.ToListAsync()) :
                        Problem("Entity set 'CoreProject5DbContext.Registers'  is null.");
        }

        // GET: Admin/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Registers == null)
            {
                return NotFound();
            }

            var register = await _context.Registers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (register == null)
            {
                return NotFound();
            }

            return View(register);
        }

        // GET: Admin/Create
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserName,Email,Password,ConfirmPassword,Age,Dob,State,PhoneNumber,Salt")] Register register)
        {
            if (ModelState.IsValid)
            {
                _context.Add(register);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(register);
        }

        public IActionResult CreateEmployees()
        {
            return View();
        }


        public async Task<IActionResult> ProductDetail()
        {
            var data = await _context.Products.ToListAsync();
            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEmployees(Employee Employee)
        {
            if (ModelState.IsValid)
            {
                _context.Add(Employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(Employee);
        }

        // GET: Admin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Registers == null)
            {
                return NotFound();
            }

            var register = await _context.Registers.FindAsync(id);
            if (register == null)
            {
                return NotFound();
            }
            return View(register);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserName,Email,Password,ConfirmPassword,Age,Dob,State,PhoneNumber,Salt")] Register register)
        {
            if (id != register.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(register);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RegisterExists(register.Id))
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
            return View(register);
        }

        // GET: Admin/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Registers == null)
            {
                return NotFound();
            }

            var register = await _context.Registers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (register == null)
            {
                return NotFound();
            }

            return View(register);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Registers == null)
            {
                return Problem("Entity set 'CoreProject5DbContext.Registers'  is null.");
            }
            var register = await _context.Registers.FindAsync(id);
            if (register != null)
            {
                _context.Registers.Remove(register);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RegisterExists(int id)
        {
          return (_context.Registers?.Any(e => e.Id == id)).GetValueOrDefault();
        }


        public IActionResult ShowsTotalData()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetTotalData()
        {
            var result = _context.Ticketts
                .Where(m => m.Status == "Pending")
                .GroupBy(m => m.Category)
                .Select(g => new
                {
                    Category = g.Key,
                    TotalTicketsPending = g.Count()
                })
                .ToList();

            return Json(result);
        }


        public IActionResult ShowsCompletedData()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetCompletedData()
        {
            var result = _context.Ticketts
                .Where(m => m.Status == "Completed")
                .GroupBy(m => m.Category)
                .Select(g => new
                {
                    Category = g.Key,
                    TotalTicketsCompleted = g.Count()
                })
                .ToList();

            return Json(result);
        }


        public IActionResult ExportToExcel()
        {
            //Get the Employee data from the database
            var employees = _context.Employees.ToList();

            //Create an Instance of ExcelFileHandling
            ExcelFileHandling excelFileHandling = new ExcelFileHandling();
            //Call the CreateExcelFile method by passing the list of Employee
            var stream = excelFileHandling.CreateExcelFile(employees);

            //Give a Name to your Excel File
            string excelName = $"Employees-{Guid.NewGuid()}.xlsx";

            // 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' is the MIME type for Excel files
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }

        public IActionResult DownloadExcel()
        {
            return View();
        }


        //FOR HIGHCHART Graph
        public async Task<IActionResult> GetProductDetail_Graph()
        {
            //var data = await _context.Products
            //    .GroupBy(p => p.Brand)
            //    .Select( g => new {name = g.Key, count  = g.Sum(u => u.Price) }).ToListAsync();
            //return View(data);

            var data = await _context.Products
               .GroupBy(p => p.Brand)
               .Select(g => new
               { 
                   name = g.Key, 
                   count = g.Sum(u => u.Price),
                   stock = g.Sum(u => u.Stock)
               })
               .ToListAsync();

            return View(data);
        }

    }
}
